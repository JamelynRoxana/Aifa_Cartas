using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using cartas_aifa.Data;
using cartas_aifa.Models;

namespace cartas_aifa.Controllers;

public class FacultadsController : Controller
{
    private readonly AppDbContext _db;
    public FacultadsController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? buscar)
    {
        var query = _db.Facultades.Include(f => f.Universidad).AsQueryable();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var term = buscar.ToLower();
            query = query.Where(f =>
                f.NombreF.ToLower().Contains(term) ||
                (f.Universidad != null && f.Universidad.NombreU.ToLower().Contains(term))
            );
        }

        ViewBag.BuscarActual = buscar;
        return View(await query.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.Facultades.Include(f => f.Universidad).FirstOrDefaultAsync(f => f.Id == id);
        return item == null ? NotFound() : View(item);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.IdU = new SelectList(await _db.Universidades.ToListAsync(), "Id", "NombreU");
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Facultad item)
    {
        if (ModelState.IsValid) { _db.Facultades.Add(item); await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
        ViewBag.IdU = new SelectList(await _db.Universidades.ToListAsync(), "Id", "NombreU");
        return View(item);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.Facultades.FindAsync(id);
        if (item == null) return NotFound();
        ViewBag.IdU = new SelectList(await _db.Universidades.ToListAsync(), "Id", "NombreU");
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Facultad item)
    {
        if (id != item.Id) return NotFound();
        if (ModelState.IsValid) { _db.Update(item); await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
        ViewBag.IdU = new SelectList(await _db.Universidades.ToListAsync(), "Id", "NombreU");
        return View(item);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.Facultades.Include(f => f.Universidad).FirstOrDefaultAsync(f => f.Id == id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _db.Facultades.FindAsync(id);
        if (item != null) { _db.Facultades.Remove(item); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }
}
