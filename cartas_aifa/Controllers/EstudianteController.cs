using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using cartas_aifa.Data;
using cartas_aifa.Models;

namespace cartas_aifa.Controllers;

public class EstudianteController : Controller
{
    private readonly AppDbContext _db;
    public EstudianteController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? buscar)
    {
        var query = _db.Estudiantes.Include(e => e.Facultad).ThenInclude(f => f!.Universidad).AsQueryable();

        if (!string.IsNullOrWhiteSpace(buscar))
        {
            var term = buscar.ToLower();
            query = query.Where(e =>
                e.NombreCompleto.ToLower().Contains(term) ||
                e.Matricula.ToLower().Contains(term) ||
                (e.Carrera != null && e.Carrera.ToLower().Contains(term)) ||
                (e.Facultad != null && e.Facultad.NombreF.ToLower().Contains(term)) ||
                (e.Facultad != null && e.Facultad.Universidad != null && e.Facultad.Universidad.NombreU.ToLower().Contains(term))
            );
        }

        ViewBag.BuscarActual = buscar;
        return View(await query.ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.Estudiantes.Include(e => e.Facultad).ThenInclude(f => f!.Universidad).FirstOrDefaultAsync(e => e.Id == id);
        return item == null ? NotFound() : View(item);
    }

    public async Task<IActionResult> Create(int? userId)
    {
        // Si es estudiante, verificar si ya tiene registro
        if (userId.HasValue)
        {
            var registro = await _db.Estudiantes
                .Include(e => e.Facultad).ThenInclude(f => f!.Universidad)
                .Include(e => e.Direccion)
                .Include(e => e.Subdireccion)
                .Include(e => e.DetalleEtapa)
                .FirstOrDefaultAsync(e => e.IdUsuario == userId.Value);
            if (registro != null)
            {
                return View("MiRegistro", registro);
            }
        }
        else
        {
            // Intentar desde el claim del JWT
            var userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userIdStr, out int uid))
            {
                var registro = await _db.Estudiantes
                    .Include(e => e.Facultad).ThenInclude(f => f!.Universidad)
                    .Include(e => e.Direccion)
                    .Include(e => e.Subdireccion)
                    .Include(e => e.DetalleEtapa)
                    .FirstOrDefaultAsync(e => e.IdUsuario == uid);
                if (registro != null)
                {
                    return View("MiRegistro", registro);
                }
            }
        }

        ViewBag.Universidades = new SelectList(await _db.Universidades.ToListAsync(), "Id", "NombreU");
        ViewBag.IdF = new SelectList(Enumerable.Empty<Facultad>(), "Id", "NombreF");
        ViewBag.Direcciones = new SelectList(await _db.DireccionesAifa.ToListAsync(), "Id", "NombreDir");
        ViewBag.Subdirecciones = new SelectList(Enumerable.Empty<SubdireccionAifa>(), "Id", "NombreSub");
        ViewBag.DetallesEtapa = new SelectList(await _db.DetallesEtapas.ToListAsync(), "Id", "TipoEtapa");
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Estudiante item)
    {
        // Verificar si ya tiene registro
        if (item.IdUsuario.HasValue)
        {
            var yaRegistrado = await _db.Estudiantes.AnyAsync(e => e.IdUsuario == item.IdUsuario.Value);
            if (yaRegistrado)
                return View("YaRegistrado");
        }

        if (ModelState.IsValid)
        {
            _db.Estudiantes.Add(item);
            await _db.SaveChangesAsync();
            TempData["Exito"] = "Estudiante registrado exitosamente.";
            // Si tiene IdUsuario es estudiante registrándose solo, sino es admin
            if (item.IdUsuario.HasValue && item.IdUsuario.Value > 0)
                return View("YaRegistrado");
            return RedirectToAction(nameof(Index));
        }
        var facultad = await _db.Facultades.Include(f => f.Universidad).FirstOrDefaultAsync(f => f.Id == item.IdF);
        ViewBag.Universidades = new SelectList(await _db.Universidades.ToListAsync(), "Id", "NombreU", facultad?.IdU);
        ViewBag.IdF = new SelectList(await _db.Facultades.Where(f => f.IdU == (facultad != null ? facultad.IdU : 0)).ToListAsync(), "Id", "NombreF");
        ViewBag.Direcciones = new SelectList(await _db.DireccionesAifa.ToListAsync(), "Id", "NombreDir", item.IdDireccion);
        ViewBag.Subdirecciones = new SelectList(await _db.SubdireccionesAifa.Where(s => s.IdDir == (item.IdDireccion ?? 0)).ToListAsync(), "Id", "NombreSub", item.IdSubdireccion);
        ViewBag.DetallesEtapa = new SelectList(await _db.DetallesEtapas.ToListAsync(), "Id", "TipoEtapa", item.IdDetalleEtapa);
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

    [HttpGet]
    public async Task<IActionResult> GetCarrerasPorFacultad(int idFacultad)
    {
        var carreras = await _db.Carreras
            .Where(c => c.IdF == idFacultad)
            .Select(c => new { c.Id, c.NombreCarrera })
            .ToListAsync();
        return Json(carreras);
    }

    [HttpGet]
    public async Task<IActionResult> GetSubdireccionesPorDireccion(int idDireccion)
    {
        var subs = await _db.SubdireccionesAifa
            .Where(s => s.IdDir == idDireccion)
            .Select(s => new { s.Id, s.NombreSub })
            .ToListAsync();
        return Json(subs);
    }

    [HttpGet]
    public async Task<IActionResult> VerificarRegistro(int userId)
    {
        var registro = await _db.Estudiantes.AnyAsync(e => e.IdUsuario == userId);
        return Json(new { registrado = registro });
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.Estudiantes.Include(e => e.Facultad).FirstOrDefaultAsync(e => e.Id == id);
        if (item == null) return NotFound();
        var idUniversidad = item.Facultad?.IdU ?? 0;
        ViewBag.Universidades = new SelectList(await _db.Universidades.ToListAsync(), "Id", "NombreU", idUniversidad);
        ViewBag.IdF = new SelectList(await _db.Facultades.Where(f => f.IdU == idUniversidad).ToListAsync(), "Id", "NombreF");
        ViewBag.Direcciones = new SelectList(await _db.DireccionesAifa.ToListAsync(), "Id", "NombreDir", item.IdDireccion);
        ViewBag.Subdirecciones = new SelectList(await _db.SubdireccionesAifa.Where(s => s.IdDir == (item.IdDireccion ?? 0)).ToListAsync(), "Id", "NombreSub", item.IdSubdireccion);
        ViewBag.DetallesEtapa = new SelectList(await _db.DetallesEtapas.ToListAsync(), "Id", "TipoEtapa", item.IdDetalleEtapa);
        return View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Estudiante item)
    {
        if (id != item.Id) return NotFound();
        if (ModelState.IsValid) { _db.Update(item); await _db.SaveChangesAsync(); TempData["Exito"] = "Registro actualizado exitosamente."; return RedirectToAction(nameof(Index)); }
        var facultad = await _db.Facultades.Include(f => f.Universidad).FirstOrDefaultAsync(f => f.Id == item.IdF);
        var idUniversidad = facultad?.IdU ?? 0;
        ViewBag.Universidades = new SelectList(await _db.Universidades.ToListAsync(), "Id", "NombreU", idUniversidad);
        ViewBag.IdF = new SelectList(await _db.Facultades.Where(f => f.IdU == idUniversidad).ToListAsync(), "Id", "NombreF");
        ViewBag.Direcciones = new SelectList(await _db.DireccionesAifa.ToListAsync(), "Id", "NombreDir", item.IdDireccion);
        ViewBag.Subdirecciones = new SelectList(await _db.SubdireccionesAifa.Where(s => s.IdDir == (item.IdDireccion ?? 0)).ToListAsync(), "Id", "NombreSub", item.IdSubdireccion);
        ViewBag.DetallesEtapa = new SelectList(await _db.DetallesEtapas.ToListAsync(), "Id", "TipoEtapa", item.IdDetalleEtapa);
        return View(item);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();
        var item = await _db.Estudiantes.Include(e => e.Facultad).FirstOrDefaultAsync(e => e.Id == id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _db.Estudiantes.FindAsync(id);
        if (item != null)
        {
            // Eliminar registros dependientes
            var etapas = _db.EtapasAifa.Where(e => e.IdEstudiante == id);
            _db.EtapasAifa.RemoveRange(etapas);

            var tramites = _db.TramitesAcademicos.Where(t => t.IdEstudiante == id);
            _db.TramitesAcademicos.RemoveRange(tramites);

            _db.Estudiantes.Remove(item);
            await _db.SaveChangesAsync();
        }
        TempData["Exito"] = "Registro eliminado exitosamente.";
        return RedirectToAction(nameof(Index));
    }
}
