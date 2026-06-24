using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cartas_aifa.Models;

[Table("Facultades")]
public class Facultad
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public int IdU { get; set; }

    [ForeignKey("IdU")]
    public Universidad? Universidad { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public string NombreF { get; set; } = string.Empty;
}
