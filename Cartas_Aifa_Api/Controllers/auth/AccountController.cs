using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cartas_Aifa_Api.Controllers.auth
{
    public class AccountController : Controller
    {

        [ActionName("login")]
        [HttpGet]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View("~/Views/auth/Login.cshtml");
        }

        [ActionName("Login")]
        [HttpPost]
        public ActionResult Login(string returnUrl)
        {
            HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties
            {
                RedirectUri = returnUrl ?? Url.Action("Index", "Home")
            },
                "Auth0");
            return new HttpUnauthorizedResult();
        }



        [ActionName("Logout")]
        [HttpPost]
        [Authorize]
        public void Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            HttpContext.GetOwinContext().Authentication.SignOut("Auth0");
        }
    }
}