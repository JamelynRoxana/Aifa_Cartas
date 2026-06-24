using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using cartas_aifa.Data;
using cartas_aifa.Models;

namespace cartas_aifa.Controllers;

public class DetallesTipoCartaController : Controller
{
    private readonly AppDbContext _db;
    public DetallesTipoCartaController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index() => View(await _db.DetallesTipoCartas.Include(d => d.DetalleEtapa).ToListAsync());

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.DetallesTipoCartas.Include(d => d.DetalleEtapa).FirstOrDefaultAsync(d => d.Id == id);
        return item == null ? NotFound() : View(item);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.IdEtapa = new SelectList(await _db.DetallesEtapas.ToListAsync(), "Id", "TipoEtapa");
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(DetallesTipoCarta item)
    {
        if (ModelState.IsValid) { _db.DetallesTipoCartas.Add(item); await _db.SaveChangesAsync(); TempData["Exito"] = "Registro creado exitosamente."; return RedirectToAction(nameof(Index)); }
        ViewBag.IdEtapa = new SelectList(await _db.DetallesEtapas.ToListAsync(), "Id", "TipoEtapa");
        return View(item);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.DetallesTipoCartas.FindAsync(id);
        if (item == null) return NotFound();
        ViewBag.IdEtapa = new SelectList(await _db.DetallesEtapas.ToListAsync(), "Id", "TipoEtapa");
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, DetallesTipoCarta item)
    {
        if (id != item.Id) return NotFound();
        if (ModelState.IsValid) { _db.Update(item); await _db.SaveChangesAsync(); TempData["Exito"] = "Registro actualizado exitosamente."; return RedirectToAction(nameof(Index)); }
        ViewBag.IdEtapa = new SelectList(await _db.DetallesEtapas.ToListAsync(), "Id", "TipoEtapa");
        return View(item);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.DetallesTipoCartas.Include(d => d.DetalleEtapa).FirstOrDefaultAsync(d => d.Id == id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _db.DetallesTipoCartas.FindAsync(id);
        if (item != null) { _db.DetallesTipoCartas.Remove(item); await _db.SaveChangesAsync(); }
        TempData["Exito"] = "Registro eliminado exitosamente.";
        return RedirectToAction(nameof(Index));
    }
}
