using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cartas_aifa.Models;

[Table("SubdireccionesAifa")]
public class SubdireccionAifa
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int IdDir { get; set; }

    [ForeignKey("IdDir")]
    public DireccionAifa? Direccion { get; set; }

    [Required]
    public string NombreSub { get; set; } = string.Empty;
}
