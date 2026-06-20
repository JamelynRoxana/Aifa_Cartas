using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using cartas_aifa.Data;
using cartas_aifa.Models;

namespace cartas_aifa.Controllers;

public class CarreraController : Controller
{
    private readonly AppDbContext _db;
    public CarreraController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? buscar)
    {
        var query = _db.Carreras.Include(c => c.Facultad).ThenInclude(f => f!.Universidad).AsQueryable();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var term = buscar.ToLower();
            query = query.Where(c =>
                c.NombreCarrera.ToLower().Contains(term) ||
                (c.Facultad != null && c.Facultad.NombreF.ToLower().Contains(term)) ||
                (c.Facultad != null && c.Facultad.Universidad != null && c.Facultad.Universidad.NombreU.ToLower().Contains(term))
            );
        }

        ViewBag.BuscarActual = buscar;
        return View(await query.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.Carreras.Include(c => c.Facultad).ThenInclude(f => f!.Universidad).FirstOrDefaultAsync(c => c.Id == id);
        return item == null ? NotFound() : View(item);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Universidades = new SelectList(await _db.Universidades.ToListAsync(), "Id", "NombreU");
        ViewBag.IdF = new SelectList(Enumerable.Empty<Facultad>(), "Id", "NombreF");
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Carrera item)
    {
        if (ModelState.IsValid) { _db.Carreras.Add(item); await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
        var facultad = await _db.Facultades.Include(f => f.Universidad).FirstOrDefaultAsync(f => f.Id == item.IdF);
        ViewBag.Universidades = new SelectList(await _db.Universidades.ToListAsync(), "Id", "NombreU", facultad?.IdU);
        ViewBag.IdF = new SelectList(await _db.Facultades.Where(f => f.IdU == (facultad != null ? facultad.IdU : 0)).ToListAsync(), "Id", "NombreF");
        return View(item);
    }

    [HttpGet]
    public async Task<IActionResult> GetFacultadesPorUniversidad(int idUniversidad)
    {
        var facultades = await _db.Facultades
            .Where(f => f.IdU == idUniversidad)
            .Select(f => new { f.Id, f.NombreF })
            .ToListAsync();
        return Json(facultades);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.Carreras.Include(c => c.Facultad).FirstOrDefaultAsync(c => c.Id == id);
        if (item == null) return NotFound();
        var idUniversidad = item.Facultad?.IdU ?? 0;
        ViewBag.Universidades = new SelectList(await _db.Universidades.ToListAsync(), "Id", "NombreU", idUniversidad);
        ViewBag.IdF = new SelectList(await _db.Facultades.Where(f => f.IdU == idUniversidad).ToListAsync(), "Id", "NombreF");
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Carrera item)
    {
        if (id != item.Id) return NotFound();
        if (ModelState.IsValid) { _db.Update(item); await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
        var facultad = await _db.Facultades.Include(f => f.Universidad).FirstOrDefaultAsync(f => f.Id == item.IdF);
        var idUniversidad = facultad?.IdU ?? 0;
        ViewBag.Universidades = new SelectList(await _db.Universidades.ToListAsync(), "Id", "NombreU", idUniversidad);
        ViewBag.IdF = new SelectList(await _db.Facultades.Where(f => f.IdU == idUniversidad).ToListAsync(), "Id", "NombreF");
        return View(item);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.Carreras.Include(c => c.Facultad).ThenInclude(f => f!.Universidad).FirstOrDefaultAsync(c => c.Id == id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _db.Carreras.FindAsync(id);
        if (item != null) { _db.Carreras.Remove(item); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }
}
