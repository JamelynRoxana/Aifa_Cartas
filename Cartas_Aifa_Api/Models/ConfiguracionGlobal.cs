using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cartas_Aifa_Api.Models
{
    [Table("ConfiguracionesGlobales")]
    public class ConfiguracionGlobal
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string LeyendaAnualVigente { get; set; }

        public DateTime UltimaActualizacion { get; set; }
    }

}
//