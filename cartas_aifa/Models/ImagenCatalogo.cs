using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cartas_aifa.Models;

[Table("ImagenesCatalogo")]
public class ImagenCatalogo
{
    [Key]
    public int Id { get; set; }

    [Required]
    [Display(Name = "Nombre del Logo")]
    public string NombreArchivo { get; set; } = string.Empty;

    public string? UrlImagen { get; set; }

    [Display(Name = "Fecha de Registro")]
    public DateTime FechaCarga { get; set; } = DateTime.Now;
}
