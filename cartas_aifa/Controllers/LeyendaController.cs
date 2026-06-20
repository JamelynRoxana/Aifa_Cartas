using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cartas_aifa.Data;
using cartas_aifa.Models;

namespace cartas_aifa.Controllers;

public class LeyendaController : Controller
{
    private readonly AppDbContext _db;
    public LeyendaController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? buscar, string? mostrarEn)
    {
        var query = _db.Leyendas.AsQueryable();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var term = buscar.ToLower();
            query = query.Where(l => l.Texto.ToLower().Contains(term));
        }

        if (!string.IsNullOrWhiteSpace(mostrarEn))
        {
            query = query.Where(l => l.MostrarEn == mostrarEn);
        }

        ViewBag.BuscarActual = buscar;
        ViewBag.MostrarEnActual = mostrarEn;
        return View(await query.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.Leyendas.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Leyenda item)
    {
        if (ModelState.IsValid) { _db.Leyendas.Add(item); await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
        return View(item);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.Leyendas.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Leyenda item)
    {
        if (id != item.Id) return NotFound();
        if (ModelState.IsValid) { _db.Update(item); await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
        return View(item);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.Leyendas.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _db.Leyendas.FindAsync(id);
        if (item != null) { _db.Leyendas.Remove(item); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }
}
