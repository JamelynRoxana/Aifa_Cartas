using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cartas_aifa.Data;
using cartas_aifa.Models;

namespace cartas_aifa.Controllers;

public class ImagenCatalogoController : Controller
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public ImagenCatalogoController(AppDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    public async Task<IActionResult> Index() => View(await _db.ImagenesCatalogo.ToListAsync());

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.ImagenesCatalogo.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ImagenCatalogo item, IFormFile archivoImagen)
    {
        if (archivoImagen == null || archivoImagen.Length == 0)
        {
            ModelState.AddModelError("", "Por favor seleccione una imagen.");
            return View(item);
        }

        if (ModelState.IsValid)
        {
            var carpeta = Path.Combine(_env.WebRootPath, "Uploads", "Logos");
            if (!Directory.Exists(carpeta))
                Directory.CreateDirectory(carpeta);

            var extension = Path.GetExtension(archivoImagen.FileName);
            var nombreUnico = Guid.NewGuid().ToString() + extension;
            var rutaCompleta = Path.Combine(carpeta, nombreUnico);

            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await archivoImagen.CopyToAsync(stream);
            }

            item.UrlImagen = "/Uploads/Logos/" + nombreUnico;
            item.FechaCarga = DateTime.Now;

            _db.ImagenesCatalogo.Add(item);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(item);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.ImagenesCatalogo.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ImagenCatalogo item, IFormFile? archivoImagen)
    {
        if (id != item.Id) return NotFound();

        if (ModelState.IsValid)
        {
            if (archivoImagen != null && archivoImagen.Length > 0)
            {
                var carpeta = Path.Combine(_env.WebRootPath, "Uploads", "Logos");
                if (!Directory.Exists(carpeta))
                    Directory.CreateDirectory(carpeta);

                // Borrar archivo viejo
                if (!string.IsNullOrEmpty(item.UrlImagen))
                {
                    var rutaVieja = Path.Combine(_env.WebRootPath, item.UrlImagen.TrimStart('/'));
                    if (System.IO.File.Exists(rutaVieja))
                        System.IO.File.Delete(rutaVieja);
                }

                var extension = Path.GetExtension(archivoImagen.FileName);
                var nombreUnico = Guid.NewGuid().ToString() + extension;
                var rutaCompleta = Path.Combine(carpeta, nombreUnico);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await archivoImagen.CopyToAsync(stream);
                }

                item.UrlImagen = "/Uploads/Logos/" + nombreUnico;
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
        var item = await _db.ImagenesCatalogo.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _db.ImagenesCatalogo.FindAsync(id);
        if (item != null)
        {
            // Borrar archivo físico
            if (!string.IsNullOrEmpty(item.UrlImagen))
            {
                var rutaFisica = Path.Combine(_env.WebRootPath, item.UrlImagen.TrimStart('/'));
                if (System.IO.File.Exists(rutaFisica))
                    System.IO.File.Delete(rutaFisica);
            }

            _db.ImagenesCatalogo.Remove(item);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
