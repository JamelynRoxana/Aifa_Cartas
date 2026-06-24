using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cartas_aifa.Models;

[Table("AutoridadesAifa")]
public class AutoridadCarta
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public string Nombre { get; set; } = string.Empty;
    public string? Rango { get; set; }
    public string? Puesto { get; set; }
}
