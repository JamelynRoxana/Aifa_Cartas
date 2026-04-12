using System.Linq;
using System.Net;
using System.Web.Http;
using Cartas_Aifa_Api.Models; 

namespace Cartas_Aifa_Api.Controllers.api
{
    // 1. Usamos ApiController para APIs (JSON)
    [RoutePrefix("api/usuario")]
    public class UsuarioApiController : ApiController
    {
        private Model1 db = new Model1();


        [HttpGet]
        [Route("getUsuarios")]
        public IHttpActionResult GetUsuarios()
        {
            var usuarios = db.Usuarios
                .Select(u => new
                {
                    u.Id,
                    u.Nombre,
                    u.Correo,
                    u.Rol
                })
                .ToList();

            return Ok(usuarios);
        }
    }
}