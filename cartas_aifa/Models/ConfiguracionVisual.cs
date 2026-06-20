using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cartas_aifa.Models;

[Table("ConfiguracionesVisuales")]
public class ConfiguracionVisual
{
    [Key]
    public int Id { get; set; }

    public int ImagenId { get; set; }

    [ForeignKey("ImagenId")]
    public ImagenCatalogo? Imagen { get; set; }

    [Required]
    public decimal CoordX { get; set; }
    [Required]
    public decimal CoordY { get; set; }
    public decimal Ancho { get; set; }
    public decimal Alto { get; set; }

    public int AnioAplicacion { get; set; }
    public string? MostrarEn { get; set; }

    public string? Posicion { get; set; }

    /// <summary>Opacidad de 0 a 100 (solo para imágenes de fondo)</summary>
    public int Opacidad { get; set; } = 100;

    [Required]
    public DateTime FechaInicio { get; set; }
    [Required]
    public DateTime FechaFin { get; set; }
}
