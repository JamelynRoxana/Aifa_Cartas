using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cartas_Aifa_Api.Models
{
    [Table("TramitesAcademicos")]
    public class TramiteAcademico
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Index(IsUnique = true)]
        [StringLength(50)]
        public string Folio { get; set; } // El folio que ingresas manualmente

        // 1. Selección del Estudiante
        [Required]
        public int IdEstudiante { get; set; }

        [ForeignKey("IdEstudiante")]
        public virtual Estudiante Estudiante { get; set; }

        // 2. Selección Directa del Catálogo (Estancia I, Servicio, etc.)
        [Required]
        public int IdDetalleEtapa { get; set; }

        [ForeignKey("IdDetalleEtapa")]
        public virtual DetalleEtapa DetalleEtapa { get; set; }

        // 3. Selección de la Autoridad que firma
        [Required]
        public int IdAut { get; set; }

        [ForeignKey("IdAut")]
        public virtual AutoridadAifa Autoridad { get; set; }

        // 4. Selección del tipo de carta (Aceptación o Terminación) 
        // Filtrado por el IdDetalleEtapa seleccionado arriba
        [Required]
        public int IdTipoCarta { get; set; }

        [ForeignKey("IdTipoCarta")]
        public virtual DetallesTipoCarta TipoCarta { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}