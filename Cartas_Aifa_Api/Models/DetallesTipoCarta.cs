using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("DetallesTipoCarta")]
public class DetallesTipoCarta
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string NombreCarta { get; set; } // "Aceptación", "Terminación"

    [Required]
    public int IdEtapa { get; set; } // FK a DetalleEtapa

    [ForeignKey("IdEtapa")]
    public virtual DetalleEtapa DetalleEtapa { get; set; }
}