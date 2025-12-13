using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio.Excepciones
{
    public class CitaDuplicadaException : Exception
    {
        public CitaDuplicadaException() : base("Ya existe una cita para este paciente en el horario seleccionado.") { }
        public CitaDuplicadaException(string message) : base(message) { }
    }
}
