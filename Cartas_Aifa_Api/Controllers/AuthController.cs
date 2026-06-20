using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cartas_Aifa_Api.Controllers
{
    public class AuthController : Controller
    {

        [HttpGet]
        [ActionName("Login")]
        public ActionResult Login()
        {
            return View("~/Views/Auth/Login.cshtml");
        }

    }
}