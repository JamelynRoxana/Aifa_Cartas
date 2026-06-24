using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cartas_aifa.Models;

[Table("Carreras")]
public class Carrera
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public int IdF { get; set; }

    [ForeignKey("IdF")]
    public Facultad? Facultad { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public string NombreCarrera { get; set; } = string.Empty;
}
