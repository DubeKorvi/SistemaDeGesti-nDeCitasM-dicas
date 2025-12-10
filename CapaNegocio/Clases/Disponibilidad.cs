using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio.Clases
{
    public class Disponibilidad
    {
        public int IdDisponibilidad { get; set; }
        public Doctor Doctor { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan HoraEntrada { get; set; }
        public TimeSpan HoraSalida { get; set; }
        public bool EstaDisponible { get; set; } = true;
    }
}
