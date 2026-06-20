using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cartas_aifa.Models;

[Table("DetallesTipoCarta")]
public class DetallesTipoCarta
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string NombreCarta { get; set; } = string.Empty;

    [Required]
    public int IdEtapa { get; set; }

    [ForeignKey("IdEtapa")]
    public DetalleEtapa? DetalleEtapa { get; set; }
}
