using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cartas_Aifa_Api.Models
{
    [Table("Universidades")]
    public class Universidad
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string NombreU { get; set; }

        public string DireccionU { get; set; }
    }
}
//