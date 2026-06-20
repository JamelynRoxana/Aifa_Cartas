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
            .Include(t => t.Estudiante)
            .Include(t => t.DetalleEtapa)
            .Include(t => t.TipoCarta)
            .Include(t => t.Autoridad)
            .ToListAsync();

        return View(tramites);
    }
}
