using Cartas_Aifa_Api.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using BCrypt.Net;

namespace Cartas_Aifa_Api.Controllers.api
{
    [RoutePrefix("api/auth")]
    public class AuthController : ApiController
    {
        private Model1 _db = new Model1();

        // Clase auxiliar para recibir las credenciales desde el body
        public class LoginDto
        {
            public string Correo { get; set; }
            public string Contrasena { get; set; }
        }

        [HttpPost]
        [Route("Registrar")]
        public async Task<IHttpActionResult> Registrar([FromBody] Usuario usuario)
        {
            try
            {
                if (!ModelState.IsValid || usuario == null) return BadRequest("Datos inválidos solicitados.");

                var existe = await _db.Usuarios.AnyAsync(u => u.Correo == usuario.Correo);
                if (existe) return BadRequest("El correo ya está registrado.");

                usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(usuario.Contrasena);

                _db.Usuarios.Add(usuario);
                await _db.SaveChangesAsync();

                return Ok(new { message = "Usuario registrado exitosamente." });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, $"Error interno del servidor {ex}");
            }
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IHttpActionResult> Login([FromBody] LoginDto loginData)
        {
            try
            {
                if (loginData == null || string.IsNullOrEmpty(loginData.Correo) || string.IsNullOrEmpty(loginData.Contrasena))
                    return BadRequest("Debe ingresar correo y contraseña.");

                var usuarioEncontrado = await _db.Usuarios.FirstOrDefaultAsync(u => u.Correo == loginData.Correo);

                if (usuarioEncontrado == null || !BCrypt.Net.BCrypt.Verify(loginData.Contrasena, usuarioEncontrado.Contrasena))
                {
                    return Content(HttpStatusCode.Unauthorized, "Correo o contraseña incorrectos.");
                }

                var token = CrearToken(usuarioEncontrado);

                return Ok(new
                {
                    token = token
                });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, $"Error interno del servidor: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPost]
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            try
            {
                return Ok(new { message = "Logout successful. Por favor elimine el token en el cliente." });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, "Error interno del servidor");
            }
        }

        private string CrearToken(Usuario usuario)
        {
            var secretKey = ConfigurationManager.AppSettings["JwtSecret"];
            if (string.IsNullOrEmpty(secretKey)) throw new Exception("Falta configurar JwtSecret en Web.config");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString() ?? ""),
                new Claim(ClaimTypes.Email, usuario.Correo),
            };

            var token = new JwtSecurityToken(
                issuer: "CartasAifaApi",
                audience: "CartasAifaApiUsers",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _db != null)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}