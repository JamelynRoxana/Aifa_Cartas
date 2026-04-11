using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Cartas_Aifa_Api.Models
{
    [Table("ConfiguracionesVisuales")]
    public class ConfiguracionVisual
    {
        [Key]
        public int Id { get; set; }

        // Relación con el catálogo
        public int ImagenId { get; set; }
        [ForeignKey("ImagenId")]
        public virtual ImagenCatalogo Imagen { get; set; }

        [Required]
        public decimal CoordX { get; set; }
        [Required]
        public decimal CoordY { get; set; }

        public decimal Ancho { get; set; }
        public decimal Alto { get; set; }

        // Control de tiempo y contexto
        public int AnioAplicacion { get; set; } // Para filtrar por año
        public string MostrarEn { get; set; } // "Aceptacion", "Termino", etc.

        [Required]
        public DateTime FechaInicio { get; set; }
        [Required]
        public DateTime FechaFin { get; set; }
    }
}