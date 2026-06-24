using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cartas_aifa.Data;

namespace cartas_aifa.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;

    public HomeController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var tramites = await _db.TramitesAcademicos
            .Include(t => t.Estudiante).ThenInclude(e => e!.Facultad).ThenInclude(f => f!.Universidad)
            .Include(t => t.DetalleEtapa)
            .Include(t => t.TipoCarta)
            .Include(t => t.Autoridad)
            .ToListAsync();

        ViewBag.Universidades = await _db.Universidades.OrderBy(u => u.NombreU).ToListAsync();

        return View(tramites);
    }

    public async Task<IActionResult> Estudiante()
    {
        var direcciones = await _db.DireccionesAifa.ToListAsync();
        var subdirecciones = await _db.SubdireccionesAifa.Include(s => s.Direccion).ToListAsync();
        ViewBag.Direcciones = direcciones;
        ViewBag.Subdirecciones = subdirecciones;

        // Buscar registro del estudiante si existe
        ViewBag.Registro = null;
        var token = HttpContext.Request.Query["userId"].ToString();
        if (int.TryParse(token, out int uid))
        {
            var registro = await _db.Estudiantes
                .Include(e => e.Facultad).ThenInclude(f => f!.Universidad)
                .Include(e => e.Direccion)
                .Include(e => e.Subdireccion)
                .Include(e => e.DetalleEtapa)
                .FirstOrDefaultAsync(e => e.IdUsuario == uid);
            ViewBag.Registro = registro;
        }

        return View("IndexEstudiante");
    }
}
