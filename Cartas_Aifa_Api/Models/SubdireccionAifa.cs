using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cartas_Aifa_Api.Models
{
    [Table("SubdireccionesAifa")]
    public class SubdireccionAifa
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdDir { get; set; } // FK

        [ForeignKey("IdDir")]
        public virtual DireccionAifa Direccion { get; set; }

        [Required]
        public string NombreSub { get; set; }
    }
}
//