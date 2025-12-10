using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio.Excepciones
{
    public class HorarioNoDisponibleException : Exception
    {
        public HorarioNoDisponibleException() : base("El horario seleccionado no está disponible.") { }
        public HorarioNoDisponibleException(string message) : base(message) { }
    }
}
