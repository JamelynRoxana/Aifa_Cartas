using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cartas_aifa.Models;

[Table("EtapasAifa")]
public class EtapaAifa
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public int IdEstudiante { get; set; }

    [ForeignKey("IdEstudiante")]
    public Estudiante? Estudiante { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public int IdDetalleEtapa { get; set; }

    [ForeignKey("IdDetalleEtapa")]
    public DetalleEtapa? DetalleEtapa { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public int IdDireccion { get; set; }

    [ForeignKey("IdDireccion")]
    public DireccionAifa? Direccion { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public int IdSubdireccion { get; set; }

    [ForeignKey("IdSubdireccion")]
    public SubdireccionAifa? Subdireccion { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public DateTime FechaInicio { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public DateTime FechaFin { get; set; }

    public bool Activa { get; set; } = true;

    public ICollection<TramiteAcademico>? Tramites { get; set; }
}
