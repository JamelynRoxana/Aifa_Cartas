using Cartas_Aifa_Api.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("DetallesEtapas")]
public class DetalleEtapa
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string TipoEtapa { get; set; } // "Estancia I", "Servicio Social"

    public virtual ICollection<DetallesTipoCarta> TiposDeCartas { get; set; }
}