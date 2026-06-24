using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cartas_aifa.Data;
using cartas_aifa.Models;

namespace cartas_aifa.Controllers;

public class UniversidadController : Controller
{
    private readonly AppDbContext _db;
    public UniversidadController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index() => View(await _db.Universidades.ToListAsync());

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.Universidades.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Universidad item)
    {
        if (ModelState.IsValid) { _db.Universidades.Add(item); await _db.SaveChangesAsync(); TempData["Exito"] = "Registro creado exitosamente."; return RedirectToAction(nameof(Index)); }
        return View(item);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.Universidades.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Universidad item)
    {
        if (id != item.Id) return NotFound();
        if (ModelState.IsValid) { _db.Update(item); await _db.SaveChangesAsync(); TempData["Exito"] = "Registro actualizado exitosamente."; return RedirectToAction(nameof(Index)); }
        return View(item);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.Universidades.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _db.Universidades.FindAsync(id);
        if (item != null) { _db.Universidades.Remove(item); await _db.SaveChangesAsync(); }
        TempData["Exito"] = "Registro eliminado exitosamente.";
        return RedirectToAction(nameof(Index));
    }
}
