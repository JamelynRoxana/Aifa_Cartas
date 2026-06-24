using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cartas_aifa.Models;

[Table("PiesDePagina")]
public class PiePagina
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public string Texto { get; set; } = string.Empty;

    /// <summary>Si tiene línea superior decorativa</summary>
    public bool TieneLinea { get; set; } = true;

    /// <summary>Color de la línea/borde en hex (ej: #0c2f57)</summary>
    public string? ColorLinea { get; set; } = "#0c2f57";

    /// <summary>Color de fondo del recuadro en hex</summary>
    public string? ColorFondo { get; set; } = "#ffffff";

    /// <summary>Color de la letra en hex</summary>
    public string? ColorLetra { get; set; } = "#000000";

    /// <summary>Grosor de la línea en puntos</summary>
    public float GrosorLinea { get; set; } = 1.5f;

    /// <summary>Centrado, Izquierda, Derecha</summary>
    public string? Alineacion { get; set; } = "Centrado";

    /// <summary>Tamaño de fuente</summary>
    public int TamanoFuente { get; set; } = 9;

    /// <summary>Aceptacion, Termino, Ambas</summary>
    public string? MostrarEn { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public DateTime FechaInicio { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public DateTime FechaFin { get; set; }
}
