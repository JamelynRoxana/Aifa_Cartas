using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cartas_aifa.Models;

[Table("Estudiantes")]
public class Estudiante
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public string Nombre { get; set; } = string.Empty;

    public string? ApellidoPaterno { get; set; }

    public string? ApellidoMaterno { get; set; }

    [NotMapped]
    public string NombreCompleto => $"{Nombre} {ApellidoPaterno} {ApellidoMaterno}".Trim();

    public DateTime? FechaNacimiento { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public string Matricula { get; set; } = string.Empty;

    public string? Carrera { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public int IdF { get; set; }

    [ForeignKey("IdF")]
    public Facultad? Facultad { get; set; }

    public int? IdDireccion { get; set; }

    [ForeignKey("IdDireccion")]
    public DireccionAifa? Direccion { get; set; }

    public int? IdSubdireccion { get; set; }

    [ForeignKey("IdSubdireccion")]
    public SubdireccionAifa? Subdireccion { get; set; }

    public int? IdDetalleEtapa { get; set; }

    [ForeignKey("IdDetalleEtapa")]
    public DetalleEtapa? DetalleEtapa { get; set; }

    public int? IdUsuario { get; set; }

    [ForeignKey("IdUsuario")]
    public Usuario? Usuario { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    /// <summary>Plan de estudios: Bimestral, Cuatrimestral, Semestral, Anual</summary>
    public string? PlanEstudios { get; set; }

    /// <summary>Periodo actual en el que se encuentra (1, 2, 3...)</summary>
    public int? PeriodoActual { get; set; }

    public int? IdCodigoAcceso { get; set; }

    [ForeignKey("IdCodigoAcceso")]
    public CodigoAcceso? CodigoAcceso { get; set; }
}
