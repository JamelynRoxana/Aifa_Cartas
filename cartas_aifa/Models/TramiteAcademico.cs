using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cartas_aifa.Models;

[Table("TramitesAcademicos")]
public class TramiteAcademico
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    [StringLength(50)]
    public string Folio { get; set; } = string.Empty;

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public int IdEstudiante { get; set; }

    [ForeignKey("IdEstudiante")]
    public Estudiante? Estudiante { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public int IdDetalleEtapa { get; set; }

    [ForeignKey("IdDetalleEtapa")]
    public DetalleEtapa? DetalleEtapa { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public int IdAut { get; set; }

    [ForeignKey("IdAut")]
    public AutoridadCarta? Autoridad { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public int IdTipoCarta { get; set; }

    [ForeignKey("IdTipoCarta")]
    public DetallesTipoCarta? TipoCarta { get; set; }

    [Column("EtapaAifa_Id")]
    public int? EtapaAifaId { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public DateTime? FechaInicioPeriodo { get; set; }

    public DateTime? FechaFinPeriodo { get; set; }

    /// <summary>Total de horas que debe cumplir el estudiante</summary>
    public int? TotalHoras { get; set; }
}
