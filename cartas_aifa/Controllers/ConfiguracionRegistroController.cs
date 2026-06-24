using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cartas_aifa.Data;
using cartas_aifa.Models;

namespace cartas_aifa.Controllers;

public class ConfiguracionRegistroController : Controller
{
    private readonly AppDbContext _db;
    public ConfiguracionRegistroController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var config = await _db.ConfiguracionesRegistro.OrderByDescending(c => c.Id).FirstOrDefaultAsync();
        return View(config);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Guardar(DateTime fechaInicio, DateTime fechaFin)
    {
        // Desactivar configuraciones anteriores
        var anteriores = await _db.ConfiguracionesRegistro.Where(c => c.Activo).ToListAsync();
        foreach (var a in anteriores) a.Activo = false;

        var config = new ConfiguracionRegistro
        {
            FechaInicio = fechaInicio,
            FechaFin = fechaFin,
            Activo = true,
            FechaCreacion = DateTime.UtcNow
        };

        _db.ConfiguracionesRegistro.Add(config);

        // Actualizar fecha límite en códigos no usados
        var codigosDisponibles = await _db.CodigosAcceso.Where(c => !c.Usado).ToListAsync();
        foreach (var codigo in codigosDisponibles)
        {
            codigo.FechaLimiteRegistro = fechaFin;
        }

        await _db.SaveChangesAsync();

        TempData["Exito"] = "Registro creado exitosamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Estado()
    {
        var config = await _db.ConfiguracionesRegistro
            .Where(c => c.Activo)
            .OrderByDescending(c => c.Id)
            .FirstOrDefaultAsync();

        if (config == null)
            return Json(new { habilitado = false, mensaje = "El registro no está disponible en este momento." });

        var ahora = DateTime.UtcNow;
        var habilitado = ahora >= config.FechaInicio && ahora <= config.FechaFin;

        return Json(new
        {
            habilitado,
            fechaInicio = config.FechaInicio.ToLocalTime().ToString("dd/MM/yyyy HH:mm"),
            fechaFin = config.FechaFin.ToLocalTime().ToString("dd/MM/yyyy HH:mm"),
            mensaje = habilitado
                ? $"Registro abierto hasta el {config.FechaFin.ToLocalTime():dd/MM/yyyy} a las {config.FechaFin.ToLocalTime():HH:mm}"
                : $"El registro estará disponible del {config.FechaInicio.ToLocalTime():dd/MM/yyyy HH:mm} al {config.FechaFin.ToLocalTime():dd/MM/yyyy HH:mm}"
        });
    }
}
