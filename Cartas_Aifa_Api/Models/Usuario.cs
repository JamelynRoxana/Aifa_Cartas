using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Cartas_Aifa_Api.Models

{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;

        public string Contrasena { get; set; } = string.Empty;

        // Define el rol: "Administrativo", "Recursos Humanos" o "Estudiante"
        public string Rol { get; set; } = string.Empty;

        // Propiedad calculada opcional para lógica rápida en la Vista
        public bool EsAdmin => Rol.Equals("Administrativo", StringComparison.OrdinalIgnoreCase);
    }
}