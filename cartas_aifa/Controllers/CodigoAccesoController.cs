using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cartas_aifa.Data;
using cartas_aifa.Models;

namespace cartas_aifa.Controllers;

public class CodigoAccesoController : Controller
{
    private readonly AppDbContext _db;
    public CodigoAccesoController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? buscar, string? estado)
    {
        var query = _db.CodigosAcceso.AsQueryable();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var term = buscar.ToLower();
            query = query.Where(c =>
                c.Codigo.ToLower().Contains(term) ||
                (c.UsadoPor != null && c.UsadoPor.ToLower().Contains(term))
            );
        }

        if (!string.IsNullOrWhiteSpace(estado))
        {
            if (estado == "usado")
                query = query.Where(c => c.Usado);
            else if (estado == "disponible")
                query = query.Where(c => !c.Usado && (c.FechaExpiracion == null || c.FechaExpiracion > DateTime.UtcNow));
            else if (estado == "expirado")
                query = query.Where(c => !c.Usado && c.FechaExpiracion != null && c.FechaExpiracion <= DateTime.UtcNow);
        }

        ViewBag.BuscarActual = buscar;
        ViewBag.EstadoActual = estado;

        var codigos = await query.OrderByDescending(c => c.FechaCreacion).ToListAsync();
        return View(codigos);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Generar(int cantidad = 1)
    {
        if (cantidad < 1) cantidad = 1;
        if (cantidad > 50) cantidad = 50;

        for (int i = 0; i < cantidad; i++)
        {
            var codigo = new CodigoAcceso
            {
                Codigo = GenerarCodigo(),
                FechaCreacion = DateTime.UtcNow,
                FechaExpiracion = DateTime.UtcNow.AddDays(7),
                Usado = false
            };
            _db.CodigosAcceso.Add(codigo);
        }

        await _db.SaveChangesAsync();
        TempData["Exito"] = "Registro creado exitosamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _db.CodigosAcceso.FindAsync(id);
        if (item != null)
        {
            _db.CodigosAcceso.Remove(item);
            await _db.SaveChangesAsync();
        }
        TempData["Exito"] = "Registro eliminado exitosamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ToggleAsignado(int id, string? asignadoA)
    {
        var item = await _db.CodigosAcceso.FindAsync(id);
        if (item == null) return NotFound();

        item.Asignado = !item.Asignado;
        item.AsignadoA = item.Asignado ? asignadoA : null;
        await _db.SaveChangesAsync();

        return Json(new { asignado = item.Asignado, asignadoA = item.AsignadoA });
    }

    private static string GenerarCodigo()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var random = new Random();
        return new string(Enumerable.Range(0, 8)
            .Select(_ => chars[random.Next(chars.Length)]).ToArray());
    }
}
