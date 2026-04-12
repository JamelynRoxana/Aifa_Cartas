using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cartas_Aifa_Api.Models
{
    [Table("Estudiantes")]
    public class Estudiante
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string NombreCompleto { get; set; }

        [Required]
        public string Matricula { get; set; }

        public string Carrera { get; set; }

        // RELACIÓN ÚNICA: Solo necesitamos la Facultad
        [Required]
        public int IdF { get; set; }
        [ForeignKey("IdF")]
        public virtual Facultad Facultad { get; set; }

        // La Universidad se obtiene así: estudiante.Facultad.Universidad
    }
}