using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cartas_Aifa_Api.Models
{
    [Table("Facultades")]
    public class Facultad
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int IdU { get; set; } // FK

        [ForeignKey("IdU")]
        public virtual Universidad Universidad { get; set; }

        [Required]
        public string NombreF { get; set; }
    }
}
//