using System;
using System.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System.Linq; // Agregamos System.Linq para FirstOrDefault()
using System.Security.Claims; // Agregamos System.Security.Claims para los nuevos Claims

[assembly: OwinStartup(typeof(Cartas_Aifa_Api.Startup))]

namespace Cartas_Aifa_Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // 1. CARGA DE CONFIGURACIÓN
            string auth0Domain = ConfigurationManager.AppSettings["auth0:Domain"];
            string auth0ClientId = ConfigurationManager.AppSettings["auth0:ClientId"];
            string auth0RedirectUri = ConfigurationManager.AppSettings["auth0:RedirectUri"];
            string auth0PostLogoutRedirectUri = ConfigurationManager.AppSettings["auth0:PostLogoutRedirectUri"];
            string auth0Audience = ConfigurationManager.AppSettings["auth0:Audience"]; // http://api.aifacards.com

            // El issuer de Auth0 SIEMPRE debe terminar en diagonal /
            string issuer = $"https://{auth0Domain}/";

            // Seguridad de Protocolo para descargar llaves
            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;

            // 2. DESCARGA DE CONFIGURACIÓN DE LLAVES (OpenID Connect Discovery)
            var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                $"{issuer}.well-known/openid-configuration",
                new OpenIdConnectConfigurationRetriever());
            var openIdConfig = configManager.GetConfigurationAsync().ConfigureAwait(false).GetAwaiter().GetResult();

            // --- INICIO DE MIDDLEWARES ---

            // A. MIDDLEWARE DE COOKIES (Para navegación Web)
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = CookieAuthenticationDefaults.AuthenticationType,
                LoginPath = new PathString("/Account/Login"),
                CookieManager = new Microsoft.Owin.Host.SystemWeb.SystemWebCookieManager()
            });

            // B. MIDDLEWARE DE OPENID CONNECT (Interfaz de Login Auth0)
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                AuthenticationType = "Auth0",
                // ESTO ES CLAVE: Lo ponemos en modo pasivo para que no auto-redirija a Auth0.
                // Así, un 401 caerá en el Cookie Middleware y te llevará a /Account/login (tu vista).
                AuthenticationMode = AuthenticationMode.Passive, 
                Authority = issuer,
                ClientId = auth0ClientId,
                RedirectUri = auth0RedirectUri,
                PostLogoutRedirectUri = auth0PostLogoutRedirectUri,
                Scope = "openid profile email",
                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "https://aifacards.com/roles" // ¡Aquí está tu recordatorio! Enlaza el Claim directamente
                },
                CookieManager = new Microsoft.Owin.Host.SystemWeb.SystemWebCookieManager(),
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    SecurityTokenValidated = notification =>
                    {
                        // 1. Obtenemos los Claims (datos que mandó Auth0)
                        var claimsIdentity = notification.AuthenticationTicket.Identity;
                        
                        var auth0UserId = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                        var email = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value ?? claimsIdentity.FindFirst("email")?.Value;
                        var name = claimsIdentity.FindFirst("name")?.Value ?? email;

                        // 1.1 Intentamos obtener el rol desde el namespace personalizado configurado en tu Auth0 Action
                        var roleClaim = claimsIdentity.FindFirst("https://aifacards.com/roles");
                        var roleName = roleClaim != null ? roleClaim.Value : "Usuario";

                        // Auth0 manda la colección de roles como un arreglo JSON (por ejemplo: ["Administrador"]). 
                        // Limpiamos el texto por si viene con corchetes y comillas.
                        roleName = roleName.Trim(' ', '[', ']', '"');

                        if (!string.IsNullOrEmpty(email))
                        {
                            // 2. Conectamos a la Base de Datos Local
                            using (var db = new Cartas_Aifa_Api.Models.Model1())
                            {
                                // 3. Buscamos al usuario por su Auth0_Id o por Correo
                                var localUser = db.Usuarios.FirstOrDefault(u => u.Auth0_Id == auth0UserId || u.Correo == email);

                                if (localUser == null)
                                {
                                    // 4. Si el usuario no existe en la BD local, ¡lo creamos!
                                    localUser = new Cartas_Aifa_Api.Models.Usuario
                                    {
                                        Nombre = name,
                                        Correo = email,
                                        Rol = roleName, 
                                        Auth0_Id = auth0UserId
                                    };
                                    db.Usuarios.Add(localUser);
                                    db.SaveChanges();
                                }
                                else if (string.IsNullOrEmpty(localUser.Auth0_Id))
                                {
                                    // Si existe por correo pero no tiene el ID de Auth0, lo actualizamos
                                    localUser.Auth0_Id = auth0UserId;
                                    db.SaveChanges();
                                }

                                // 5. Agregamos el Rol y el ID local a la sesión de tu aplicación 
                                //    para que [Authorize(Roles="Admin")] de MVC funcione y 
                                //    para saber su ID interno sin tener que ir a la BD de nuevo.
                                claimsIdentity.AddClaim(new Claim("LocalUserId", localUser.Id.ToString()));
                                if (!string.IsNullOrEmpty(localUser.Rol))
                                {
                                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, localUser.Rol));
                                }
                            }
                        }

                        return Task.FromResult(0);
                    },

                    RedirectToIdentityProvider = notification =>
                    {
                        // Lógica de Logout
                        if (notification.ProtocolMessage.RequestType == OpenIdConnectRequestType.Logout)
                        {
                            var logoutUri = $"https://{auth0Domain}/v2/logout?client_id={auth0ClientId}";
                            var postLogoutUri = notification.ProtocolMessage.PostLogoutRedirectUri;
                            if (!string.IsNullOrEmpty(postLogoutUri))
                            {
                                if (postLogoutUri.StartsWith("/"))
                                {
                                    var request = notification.Request;
                                    postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                                }
                                logoutUri += $"&returnTo={Uri.EscapeDataString(postLogoutUri)}";
                            }
                            notification.Response.Redirect(logoutUri);
                            notification.HandleResponse();
                        }
                        return Task.FromResult(0);
                    }
                }
            });
        }
    }
}