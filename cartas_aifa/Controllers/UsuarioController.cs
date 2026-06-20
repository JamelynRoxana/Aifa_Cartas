using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cartas_aifa.Data;
using cartas_aifa.Models;

namespace cartas_aifa.Controllers;

public class UsuarioController : Controller
{
    private readonly AppDbContext _db;
    public UsuarioController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? buscar)
    {
        var query = _db.Usuarios.AsQueryable();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var term = buscar.ToLower();
            query = query.Where(u =>
                (u.Nombre != null && u.Nombre.ToLower().Contains(term)) ||
                (u.Correo != null && u.Correo.ToLower().Contains(term)) ||
                (u.Rol != null && u.Rol.ToLower().Contains(term))
            );
        }

        ViewBag.BuscarActual = buscar;
        return View(await query.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.Usuarios.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Usuario item)
    {
        if (ModelState.IsValid)
        {
            item.Contrasena = BCrypt.Net.BCrypt.HashPassword(item.Contrasena);
            _db.Usuarios.Add(item);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(item);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.Usuarios.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Usuario item)
    {
        if (id != item.Id) return NotFound();
        if (ModelState.IsValid)
        {
            var existente = await _db.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            if (existente == null) return NotFound();

            // Si no se envió contraseña nueva, conservar la actual
            if (string.IsNullOrEmpty(item.Contrasena))
            {
                item.Contrasena = existente.Contrasena;
            }
            else if (item.Contrasena != existente.Contrasena)
            {
                // Solo hashear si es una contraseña nueva (diferente al hash guardado)
                item.Contrasena = BCrypt.Net.BCrypt.HashPassword(item.Contrasena);
            }

            _db.Update(item);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(item);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.Usuarios.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _db.Usuarios.FindAsync(id);
        if (item != null) { _db.Usuarios.Remove(item); await _db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }
}
