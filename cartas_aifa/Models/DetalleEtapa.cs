using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cartas_aifa.Models;

[Table("DetallesEtapas")]
public class DetalleEtapa
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Este campo es obligatorio.")]
    public string TipoEtapa { get; set; } = string.Empty;

    public ICollection<DetallesTipoCarta>? TiposDeCartas { get; set; }
}
