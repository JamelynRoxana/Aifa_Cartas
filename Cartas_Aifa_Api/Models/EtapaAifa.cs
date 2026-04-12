using Cartas_Aifa_Api.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("EtapasAifa")]
public class EtapaAifa
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int IdEstudiante { get; set; }
    [ForeignKey("IdEstudiante")]
    public virtual Estudiante Estudiante { get; set; }

    [Required]
    public int IdDetalleEtapa { get; set; }
    [ForeignKey("IdDetalleEtapa")]
    public virtual DetalleEtapa DetalleEtapa { get; set; }

    [Required]
    public int IdDireccion { get; set; }
    [ForeignKey("IdDireccion")]
    public virtual DireccionAifa Direccion { get; set; }

    [Required]
    public int IdSubdireccion { get; set; }
    [ForeignKey("IdSubdireccion")]
    public virtual SubdireccionAifa Subdireccion { get; set; }

    [Required]
    public DateTime FechaInicio { get; set; }

    [Required]
    public DateTime FechaFin { get; set; }

    public bool Activa { get; set; } = true;

    // Relación: Una etapa puede tener el trámite (Aceptación/Término)
    public virtual ICollection<TramiteAcademico> Tramites { get; set; }
}