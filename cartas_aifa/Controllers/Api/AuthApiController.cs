using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using cartas_aifa.Data;
using cartas_aifa.Models;

namespace cartas_aifa.Controllers.Api;

[ApiController]
[Route("api/auth")]
public class AuthApiController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthApiController(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginData)
    {
        if (loginData == null || string.IsNullOrEmpty(loginData.Correo) || string.IsNullOrEmpty(loginData.Contrasena))
            return BadRequest(new { Message = "Debe ingresar correo y contraseña." });

        var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.Correo == loginData.Correo);

        if (usuario == null || string.IsNullOrEmpty(usuario.Contrasena))
            return Unauthorized(new { Message = "Correo o contraseña incorrectos." });

        // Verificar contraseña normal: primero BCrypt, si falla intentar texto plano
        bool passwordValid = false;
        try
        {
            passwordValid = BCrypt.Net.BCrypt.Verify(loginData.Contrasena, usuario.Contrasena);
        }
        catch
        {
            // Si falla BCrypt (no es un hash válido), comparar como texto plano
            passwordValid = usuario.Contrasena == loginData.Contrasena;
            if (passwordValid)
            {
                // Re-hashear la contraseña para migrarla a BCrypt
                usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(loginData.Contrasena);
                await _db.SaveChangesAsync();
            }
        }

        // Si la contraseña normal no es válida, intentar con la temporal
        if (!passwordValid && !string.IsNullOrEmpty(usuario.ContrasenaTemporal))
        {
            try
            {
                var tempValid = BCrypt.Net.BCrypt.Verify(loginData.Contrasena, usuario.ContrasenaTemporal);
                if (tempValid)
                {
                    // Verificar que no haya expirado (10 minutos)
                    if (usuario.ContrasenaTemporalExpira == null || DateTime.UtcNow > usuario.ContrasenaTemporalExpira)
                        return Unauthorized(new { Message = "La contraseña temporal ha expirado. Solicita una nueva." });

                    // Verificar que no se haya usado antes
                    if (usuario.ContrasenaTempUsada)
                        return Unauthorized(new { Message = "La contraseña temporal ya fue utilizada. Solicita una nueva." });

                    // Marcar como usada, invalidar temporal
                    usuario.ContrasenaTempUsada = true;
                    usuario.ContrasenaTemporal = null;
                    usuario.ContrasenaTemporalExpira = null;
                    await _db.SaveChangesAsync();

                    // Devolver flag para redirigir a cambio de contraseña
                    var tempToken = CrearToken(usuario);
                    return Ok(new { token = tempToken, nombre = usuario.Nombre, requiereCambio = true });
                }
            }
            catch { /* Si falla BCrypt en la temporal, ignorar */ }
        }

        if (!passwordValid)
            return Unauthorized(new { Message = "Correo o contraseña incorrectos." });

        var token = CrearToken(usuario);
        return Ok(new { token, nombre = usuario.Nombre });
    }

    [HttpPost("Registrar")]
    public async Task<IActionResult> Registrar([FromBody] Usuario usuario)
    {
        if (!ModelState.IsValid || usuario == null)
            return BadRequest(new { Message = "Datos inválidos." });

        var existe = await _db.Usuarios.AnyAsync(u => u.Correo == usuario.Correo);
        if (existe)
            return BadRequest(new { Message = "El correo ya está registrado." });

        usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(usuario.Contrasena);
        _db.Usuarios.Add(usuario);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Usuario registrado exitosamente." });
    }

    [HttpPost("Logout")]
    public IActionResult Logout()
    {
        return Ok(new { message = "Logout successful. Por favor elimine el token en el cliente." });
    }

    [HttpPost("CambiarContrasena")]
    public async Task<IActionResult> CambiarContrasena([FromBody] CambiarContrasenaDto data)
    {
        if (data == null || string.IsNullOrEmpty(data.NuevaContrasena))
            return BadRequest(new { message = "Debe ingresar una nueva contraseña." });

        if (data.NuevaContrasena.Length < 6)
            return BadRequest(new { message = "La contraseña debe tener al menos 6 caracteres." });

        // Obtener usuario del token JWT
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized(new { message = "Sesión no válida." });

        var userId = int.Parse(userIdClaim.Value);
        var usuario = await _db.Usuarios.FindAsync(userId);
        if (usuario == null)
            return NotFound(new { message = "Usuario no encontrado." });

        usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(data.NuevaContrasena);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Contraseña actualizada exitosamente." });
    }

    // Endpoint temporal para resetear contraseñas corruptas - ELIMINAR después de usar
    [HttpPost("FixPassword")]
    public async Task<IActionResult> FixPassword([FromBody] LoginDto data)
    {
        if (data == null || string.IsNullOrEmpty(data.Correo) || string.IsNullOrEmpty(data.Contrasena))
            return BadRequest(new { message = "Correo y contraseña requeridos." });
        var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.Correo == data.Correo);
        if (usuario == null) return NotFound(new { message = "No encontrado." });
        usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(data.Contrasena);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Contraseña restablecida." });
    }

    [HttpPost("ForgotPassword")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto data)
    {
        if (data == null || string.IsNullOrEmpty(data.Correo))
            return BadRequest(new { message = "Debe ingresar un correo." });

        var usuario = await _db.Usuarios.FirstOrDefaultAsync(u => u.Correo == data.Correo);
        if (usuario == null)
            return BadRequest(new { message = "No se encontró una cuenta con ese correo." });

        // Generar contraseña temporal con caducidad de 10 minutos
        var nuevaPass = GenerarPasswordTemporal();
        usuario.ContrasenaTemporal = BCrypt.Net.BCrypt.HashPassword(nuevaPass);
        usuario.ContrasenaTemporalExpira = DateTime.UtcNow.AddMinutes(10);
        usuario.ContrasenaTempUsada = false;
        await _db.SaveChangesAsync();

        // Enviar correo
        try
        {
            await EnviarCorreo(data.Correo, usuario.Nombre ?? "Usuario", nuevaPass);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error al enviar el correo: " + ex.Message });
        }

        return Ok(new { message = "Se envió una nueva contraseña a tu correo. Tienes 10 minutos para usarla." });
    }

    private string GenerarPasswordTemporal()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghjkmnpqrstuvwxyz23456789";
        var random = new Random();
        return new string(Enumerable.Range(0, 10).Select(_ => chars[random.Next(chars.Length)]).ToArray());
    }

    private async Task EnviarCorreo(string destinatario, string nombre, string nuevaPass)
    {
        var host = _config["Smtp:Host"] ?? "smtp.gmail.com";
        var port = int.Parse(_config["Smtp:Port"] ?? "587");
        var user = _config["Smtp:User"] ?? "";
        var pass = _config["Smtp:Password"] ?? "";
        var fromName = _config["Smtp:FromName"] ?? "AIFA";

        using var client = new System.Net.Mail.SmtpClient(host, port)
        {
            Credentials = new System.Net.NetworkCredential(user, pass),
            EnableSsl = true
        };

        var msg = new System.Net.Mail.MailMessage
        {
            From = new System.Net.Mail.MailAddress(user, fromName),
            Subject = "Tu nueva contraseña - Expediente Aero AIFA",
            IsBodyHtml = true,
            Body = $@"
                <div style='font-family:Inter,sans-serif;max-width:500px;margin:0 auto;padding:2rem;'>
                    <h2 style='color:#001f3f;'>Hola, {nombre}</h2>
                    <p style='color:#6b7c93;'>Se generó una nueva contraseña temporal para tu cuenta:</p>
                    <div style='background:#f0f4f8;border-radius:12px;padding:1rem;text-align:center;margin:1.5rem 0;'>
                        <code style='font-size:1.3rem;font-weight:700;color:#001f3f;letter-spacing:2px;'>{nuevaPass}</code>
                    </div>
                    <p style='color:#6b7c93;font-size:0.85rem;'>Te recomendamos cambiarla una vez que inicies sesión.</p>
                    <hr style='border:none;border-top:1px solid #edf2f7;margin:1.5rem 0;'/>
                    <p style='color:#a0aec0;font-size:0.75rem;'>AIFA - Sistema de Gestión Expediente Aero</p>
                </div>"
        };
        msg.To.Add(destinatario);
        await client.SendMailAsync(msg);
    }

    private string CrearToken(Usuario usuario)
    {
        var secretKey = _config["Jwt:Secret"]
            ?? throw new InvalidOperationException("Falta Jwt:Secret");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new Claim(ClaimTypes.Email, usuario.Correo ?? ""),
            new Claim(ClaimTypes.Role, usuario.Rol ?? "")
        };

        var token = new JwtSecurityToken(
            issuer: "CartasAifaApi",
            audience: "CartasAifaApiUsers",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
