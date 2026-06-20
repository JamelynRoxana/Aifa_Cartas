using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cartas_aifa.Models;

[Table("Facultades")]
public class Facultad
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int IdU { get; set; }

    [ForeignKey("IdU")]
    public Universidad? Universidad { get; set; }

    [Required]
    public string NombreF { get; set; } = string.Empty;
}
