using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cartas_aifa.Models;

[Table("CodigosAcceso")]
public class CodigoAcceso
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public string Codigo { get; set; } = string.Empty;

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    public DateTime? FechaExpiracion { get; set; }

    public bool Usado { get; set; } = false;

    public DateTime? FechaUso { get; set; }

    public string? CreadoPor { get; set; }

    public string? UsadoPor { get; set; }

    public bool Asignado { get; set; } = false;

    public string? AsignadoA { get; set; }

    /// <summary>Fecha y hora límite para que el estudiante complete su registro de datos</summary>
    public DateTime? FechaLimiteRegistro { get; set; }
}
