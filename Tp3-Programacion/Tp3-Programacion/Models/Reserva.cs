using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tp3_Programacion.Models
{
    public class Reserva
    {
        [Key]
        public int ReservaID { get; set; }
        // Socio (FK)
        public int SocioID { get; set; }
        public Socio Socio { get; set; }
        // Cancha (FK)
        public int CanchaID { get; set; }
        public Cancha Cancha { get; set; }
        public DateTime FechaHora { get; set; }
    }
}
