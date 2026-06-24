using Microsoft.AspNetCore.Mvc;

namespace cartas_aifa.Controllers;

public class AuthController : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }

    [HttpGet]
    public IActionResult CambiarContrasena()
    {
        return View();
    }

    [HttpGet]
    public IActionResult LoginCodigo()
    {
        return View();
    }

    [HttpGet]
    public IActionResult RegistroEstudiante()
    {
        return View();
    }

    [HttpGet]
    public IActionResult MiPerfil()
    {
        return View();
    }
}
