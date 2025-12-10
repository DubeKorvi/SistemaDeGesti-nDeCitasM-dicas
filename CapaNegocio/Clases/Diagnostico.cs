using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio.Clases
{
    public class Diagnostico
    {
        public int IdDiagnostico { get; set; }
        public Cita Cita { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }
    }
}
