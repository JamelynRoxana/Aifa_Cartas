using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cartas_aifa.Data;
using cartas_aifa.Models;

namespace cartas_aifa.Controllers;

public class AutoridadCartasController : Controller
{
    private readonly AppDbContext _db;
    public AutoridadCartasController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index() => View(await _db.AutoridadesAifa.ToListAsync());

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.AutoridadesAifa.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AutoridadCarta item)
    {
        if (ModelState.IsValid) { _db.AutoridadesAifa.Add(item); await _db.SaveChangesAsync(); TempData["Exito"] = "Registro creado exitosamente."; return RedirectToAction(nameof(Index)); }
        return View(item);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.AutoridadesAifa.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, AutoridadCarta item)
    {
        if (id != item.Id) return NotFound();
        if (ModelState.IsValid) { _db.Update(item); await _db.SaveChangesAsync(); TempData["Exito"] = "Registro actualizado exitosamente."; return RedirectToAction(nameof(Index)); }
        return View(item);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.AutoridadesAifa.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _db.AutoridadesAifa.FindAsync(id);
        if (item != null) { _db.AutoridadesAifa.Remove(item); await _db.SaveChangesAsync(); }
        TempData["Exito"] = "Registro eliminado exitosamente.";
        return RedirectToAction(nameof(Index));
    }
}
