using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cartas_aifa.Models;

[Table("TramitesAcademicos")]
public class TramiteAcademico
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Folio { get; set; } = string.Empty;

    [Required]
    public int IdEstudiante { get; set; }

    [ForeignKey("IdEstudiante")]
    public Estudiante? Estudiante { get; set; }

    [Required]
    public int IdDetalleEtapa { get; set; }

    [ForeignKey("IdDetalleEtapa")]
    public DetalleEtapa? DetalleEtapa { get; set; }

    [Required]
    public int IdAut { get; set; }

    [ForeignKey("IdAut")]
    public AutoridadCarta? Autoridad { get; set; }

    [Required]
    public int IdTipoCarta { get; set; }

    [ForeignKey("IdTipoCarta")]
    public DetallesTipoCarta? TipoCarta { get; set; }

    [Column("EtapaAifa_Id")]
    public int? EtapaAifaId { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}
