using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Cartas_Aifa_Api.Models
{
    [Table("ImagenesCatalogo")]
    public class ImagenCatalogo
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre descriptivo es obligatorio")]
        [Display(Name = "Nombre del Logo")]
        public string NombreArchivo { get; set; }

        // IMPORTANTE: Quitamos el [Required] de aquí para que el ModelState 
        // no falle cuando el usuario sube el archivo. 
        // La URL la asignaremos manualmente en el Controlador.
        public string UrlImagen { get; set; }

        [Display(Name = "Fecha de Registro")]
        public DateTime FechaCarga { get; set; } = DateTime.Now;
    }
}