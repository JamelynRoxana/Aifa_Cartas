using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using cartas_aifa.Data;
using cartas_aifa.Models;

namespace cartas_aifa.Controllers;

public class EtapaAifaController : Controller
{
    private readonly AppDbContext _db;
    public EtapaAifaController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index() =>
        View(await _db.EtapasAifa.Include(e => e.Estudiante).Include(e => e.DetalleEtapa).Include(e => e.Direccion).Include(e => e.Subdireccion).ToListAsync());

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.EtapasAifa.Include(e => e.Estudiante).Include(e => e.DetalleEtapa).Include(e => e.Direccion).Include(e => e.Subdireccion).FirstOrDefaultAsync(e => e.Id == id);
        return item == null ? NotFound() : View(item);
    }

    public async Task<IActionResult> Create()
    {
        await CargarViewBags();
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EtapaAifa item)
    {
        if (ModelState.IsValid) { _db.EtapasAifa.Add(item); await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
        await CargarViewBags();
        return View(item);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.EtapasAifa.FindAsync(id);
        if (item == null) return NotFound();
        await CargarViewBags();
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EtapaAifa item)
    {
        if (id != item.Id) return NotFound();
        if (ModelState.IsValid) { _db.Update(item); await _db.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
        await CargarViewBags();
        return View(item);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.EtapasAifa.Include(e => e.Estudiante).Include(e => e.DetalleEtapa).FirstOrDefaultAsync(e => e.Id == id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _db.EtapasAifa.FindAsync(id);
        if (item != null) { _db.EtapasAifa.Remove(item); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarViewBags()
    {
        ViewBag.IdEstudiante = new SelectList(await _db.Estudiantes.ToListAsync(), "Id", "NombreCompleto");
        ViewBag.IdDetalleEtapa = new SelectList(await _db.DetallesEtapas.ToListAsync(), "Id", "TipoEtapa");
        ViewBag.IdDireccion = new SelectList(await _db.DireccionesAifa.ToListAsync(), "Id", "NombreDir");
        ViewBag.IdSubdireccion = new SelectList(await _db.SubdireccionesAifa.ToListAsync(), "Id", "NombreSub");
    }
}
