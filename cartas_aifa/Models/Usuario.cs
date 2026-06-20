using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cartas_aifa.Models;

[Table("Usuarios")]
public class Usuario
{
    [Key]
    public int Id { get; set; }

    public string? Nombre { get; set; }

    public string? Correo { get; set; }

    public string? Contrasena { get; set; }

    public string? Rol { get; set; }

    [NotMapped]
    public bool EsAdmin => (Rol ?? "").Equals("Administrativo", StringComparison.OrdinalIgnoreCase);

    // Contraseña temporal - caducidad y uso único
    public string? ContrasenaTemporal { get; set; }
    public DateTime? ContrasenaTemporalExpira { get; set; }
    public bool ContrasenaTempUsada { get; set; } = false;
}
