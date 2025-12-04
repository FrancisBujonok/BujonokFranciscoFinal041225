using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tp3_Programacion.Models
{
    public class Socio
    {
        [Key]
        public int SocioID { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Documento { get; set; }
        public string CorreoElectronico { get; set; }
        public ICollection<Reserva> Reservas { get; set; }
    }
}
