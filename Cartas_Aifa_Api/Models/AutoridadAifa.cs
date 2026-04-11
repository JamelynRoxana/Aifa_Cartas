using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cartas_Aifa_Api.Models
{
    [Table("AutoridadesAifa")]
    public class AutoridadAifa
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }
        public string Rango { get; set; }
        public string Puesto { get; set; }
    }
}
//