using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using cartas_aifa.Data;
using cartas_aifa.Models;

namespace cartas_aifa.Controllers;

public class EstudianteController : Controller
{
    private readonly AppDbContext _db;
    public EstudianteController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? buscar)
    {
        var query = _db.Estudiantes.Include(e => e.Facultad).ThenInclude(f => f!.Universidad).AsQueryable();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var term = buscar.ToLower();
            query = query.Where(e =>
                e.NombreCompleto.ToLower().Contains(term) ||
                e.Matricula.ToLower().Contains(term) ||
                (e.Carrera != null && e.Carrera.ToLower().Contains(term)) ||
                (e.Facultad != null && e.Facultad.NombreF.ToLower().Contains(term)) ||
                (e.Facultad != null && e.Facultad.Universidad != null && e.Facultad.Universidad.NombreU.ToLower().Contains(term))
            );
        }

        ViewBag.BuscarActual = buscar;
        return View(await query.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.Estudiantes.Include(e => e.Facultad).ThenInclude(f => f!.Universidad).FirstOrDefaultAsync(e => e.Id == id);
        return item == null ? NotFound() : View(item);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Universidades = new SelectList(await _db.Universidades.ToListAsync(), "Id", "NombreU");
        ViewBag.IdF = new SelectList(Enumerable.Empty<Facultad>(), "Id", "NombreF");
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Estudiante item)
    {
        if (ModelState.IsValid) { _db.Estudiantes.Add(item); await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
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

    [HttpGet]
    public async Task<IActionResult> GetCarrerasPorFacultad(int idFacultad)
    {
        var carreras = await _db.Carreras
            .Where(c => c.IdF == idFacultad)
            .Select(c => new { c.Id, c.NombreCarrera })
            .ToListAsync();
        return Json(carreras);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.Estudiantes.Include(e => e.Facultad).FirstOrDefaultAsync(e => e.Id == id);
        if (item == null) return NotFound();
        var idUniversidad = item.Facultad?.IdU ?? 0;
        ViewBag.Universidades = new SelectList(await _db.Universidades.ToListAsync(), "Id", "NombreU", idUniversidad);
        ViewBag.IdF = new SelectList(await _db.Facultades.Where(f => f.IdU == idUniversidad).ToListAsync(), "Id", "NombreF");
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Estudiante item)
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
        var item = await _db.Estudiantes.Include(e => e.Facultad).FirstOrDefaultAsync(e => e.Id == id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _db.Estudiantes.FindAsync(id);
        if (item != null) { _db.Estudiantes.Remove(item); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }
}
