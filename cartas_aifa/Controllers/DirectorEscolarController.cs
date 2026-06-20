using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using cartas_aifa.Data;
using cartas_aifa.Models;

namespace cartas_aifa.Controllers;

public class DirectorEscolarController : Controller
{
    private readonly AppDbContext _db;
    public DirectorEscolarController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? buscar)
    {
        var query = _db.DirectoresEscolares.Include(d => d.Facultad).ThenInclude(f => f!.Universidad).AsQueryable();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var term = buscar.ToLower();
            query = query.Where(d =>
                d.Nombre.ToLower().Contains(term) ||
                (d.Facultad != null && d.Facultad.NombreF.ToLower().Contains(term)) ||
                (d.Facultad != null && d.Facultad.Universidad != null && d.Facultad.Universidad.NombreU.ToLower().Contains(term))
            );
        }

        ViewBag.BuscarActual = buscar;
        return View(await query.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.DirectoresEscolares.Include(d => d.Facultad).FirstOrDefaultAsync(d => d.Id == id);
        return item == null ? NotFound() : View(item);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Universidades = new SelectList(await _db.Universidades.ToListAsync(), "Id", "NombreU");
        ViewBag.IdF = new SelectList(Enumerable.Empty<Facultad>(), "Id", "NombreF");
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DirectorEscolar item)
    {
        if (ModelState.IsValid) { _db.DirectoresEscolares.Add(item); await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
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
        var item = await _db.DirectoresEscolares.Include(d => d.Facultad).FirstOrDefaultAsync(d => d.Id == id);
        if (item == null) return NotFound();
        var idUniversidad = item.Facultad?.IdU ?? 0;
        ViewBag.Universidades = new SelectList(await _db.Universidades.ToListAsync(), "Id", "NombreU", idUniversidad);
        ViewBag.IdF = new SelectList(await _db.Facultades.Where(f => f.IdU == idUniversidad).ToListAsync(), "Id", "NombreF");
        ViewBag.IdUniversidadActual = idUniversidad;
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DirectorEscolar item)
    {
        if (id != item.Id) return NotFound();
        if (ModelState.IsValid) { _db.Update(item); await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
        var facultad = await _db.Facultades.Include(f => f.Universidad).FirstOrDefaultAsync(f => f.Id == item.IdF);
        var idUniversidad = facultad?.IdU ?? 0;
        ViewBag.Universidades = new SelectList(await _db.Universidades.ToListAsync(), "Id", "NombreU", idUniversidad);
        ViewBag.IdF = new SelectList(await _db.Facultades.Where(f => f.IdU == idUniversidad).ToListAsync(), "Id", "NombreF");
        ViewBag.IdUniversidadActual = idUniversidad;
        return View(item);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.DirectoresEscolares.Include(d => d.Facultad).FirstOrDefaultAsync(d => d.Id == id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _db.DirectoresEscolares.FindAsync(id);
        if (item != null) { _db.DirectoresEscolares.Remove(item); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }
}
