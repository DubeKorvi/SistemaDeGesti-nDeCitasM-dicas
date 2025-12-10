using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio.Clases
{
    public class Credencial
    {
        public int IdCredencial { get; set; }
        public string Usuario { get; set; }
        public string Clave { get; set; }
        public string Rol { get; set; } // Secretaria, Medico
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}
