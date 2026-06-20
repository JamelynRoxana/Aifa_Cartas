using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using cartas_aifa.Data;
using cartas_aifa.Models;

namespace cartas_aifa.Controllers;

public class SubdireccionAifaController : Controller
{
    private readonly AppDbContext _db;
    public SubdireccionAifaController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? buscar)
    {
        var query = _db.SubdireccionesAifa.Include(s => s.Direccion).AsQueryable();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var term = buscar.ToLower();
            query = query.Where(s =>
                s.NombreSub.ToLower().Contains(term) ||
                (s.Direccion != null && s.Direccion.NombreDir.ToLower().Contains(term))
            );
        }

        ViewBag.BuscarActual = buscar;
        return View(await query.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.SubdireccionesAifa.Include(s => s.Direccion).FirstOrDefaultAsync(s => s.Id == id);
        return item == null ? NotFound() : View(item);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.IdDir = new SelectList(await _db.DireccionesAifa.ToListAsync(), "Id", "NombreDir");
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SubdireccionAifa item)
    {
        if (ModelState.IsValid) { _db.SubdireccionesAifa.Add(item); await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
        ViewBag.IdDir = new SelectList(await _db.DireccionesAifa.ToListAsync(), "Id", "NombreDir");
        return View(item);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.SubdireccionesAifa.FindAsync(id);
        if (item == null) return NotFound();
        ViewBag.IdDir = new SelectList(await _db.DireccionesAifa.ToListAsync(), "Id", "NombreDir");
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SubdireccionAifa item)
    {
        if (id != item.Id) return NotFound();
        if (ModelState.IsValid) { _db.Update(item); await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
        ViewBag.IdDir = new SelectList(await _db.DireccionesAifa.ToListAsync(), "Id", "NombreDir");
        return View(item);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.SubdireccionesAifa.Include(s => s.Direccion).FirstOrDefaultAsync(s => s.Id == id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _db.SubdireccionesAifa.FindAsync(id);
        if (item != null) { _db.SubdireccionesAifa.Remove(item); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }
}
