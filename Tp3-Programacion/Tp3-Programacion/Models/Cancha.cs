using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tp3_Programacion.Models
{
    public class Cancha
    {
        [Key]
        public int CanchaID { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Tipo { get; set; }
        public bool Activa { get; set; } = true;

        //para las reservas asociadas
        public ICollection<Reserva> Reservas { get; set; }
    }
}
