using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cartas_aifa.Models;

[Table("Estudiantes")]
public class Estudiante
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required]
    public string Matricula { get; set; } = string.Empty;

    public string? Carrera { get; set; }

    [Required]
    public int IdF { get; set; }

    [ForeignKey("IdF")]
    public Facultad? Facultad { get; set; }
}
