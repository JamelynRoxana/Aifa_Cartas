using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security.DataHandler.Encoder;
using System.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Web.Http;

[assembly: OwinStartup(typeof(Cartas_Aifa_Api.Startup))]

namespace Cartas_Aifa_Api
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureOAuth(app);

            HttpConfiguration config = new HttpConfiguration();
            WebApiConfig.Register(config);
            app.UseWebApi(config);
        }

        public void ConfigureOAuth(IAppBuilder app)
        {
            var secretKey = ConfigurationManager.AppSettings["JwtSecret"];
            if (string.IsNullOrEmpty(secretKey)) throw new System.Exception("Falta configurar JwtSecret en Web.config");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            app.UseJwtBearerAuthentication(
                new JwtBearerAuthenticationOptions
                {
                    AuthenticationMode = Microsoft.Owin.Security.AuthenticationMode.Active,
                    TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "CartasAifaApi",
                        ValidAudience = "CartasAifaApiUsers",
                        IssuerSigningKey = securityKey
                    }
                });
        }
    }
}