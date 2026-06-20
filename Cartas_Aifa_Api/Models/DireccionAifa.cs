using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cartas_Aifa_Api.Models
{
    [Table("DireccionesAifa")]
    public class DireccionAifa
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string NombreDir { get; set; }


    }
}
//