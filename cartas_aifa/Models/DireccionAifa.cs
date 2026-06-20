using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cartas_aifa.Models;

[Table("DireccionesAifa")]
public class DireccionAifa
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string NombreDir { get; set; } = string.Empty;
}
