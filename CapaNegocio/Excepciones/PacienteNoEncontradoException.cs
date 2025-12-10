using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio.Excepciones
{
    public class PacienteNoEncontradoException : Exception
    {
        public PacienteNoEncontradoException() : base("El paciente no fue encontrado.") { }
        public PacienteNoEncontradoException(string message) : base(message) { }
    }
}
