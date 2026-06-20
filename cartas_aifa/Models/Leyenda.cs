using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cartas_aifa.Models;

[Table("Leyendas")]
public class Leyenda
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Texto { get; set; } = string.Empty;

    /// <summary>Centrado, Izquierda, Derecha</summary>
    public string? Alineacion { get; set; }

    /// <summary>Porcentaje X (0-100)</summary>
    [Required]
    public decimal CoordX { get; set; }

    /// <summary>Porcentaje Y (0-100)</summary>
    [Required]
    public decimal CoordY { get; set; }

    /// <summary>Tamaño de fuente en puntos</summary>
    public int TamanoFuente { get; set; } = 10;

    /// <summary>Normal, Bold, Italic</summary>
    public string? EstiloFuente { get; set; }

    /// <summary>Aceptacion, Termino, Ambas</summary>
    public string? MostrarEn { get; set; }

    [Required]
    public DateTime FechaInicio { get; set; }

    [Required]
    public DateTime FechaFin { get; set; }
}
