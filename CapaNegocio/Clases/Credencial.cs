using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client.Platforms.Features.DesktopOs.Kerberos;
using CapaDatos;

namespace CapaNegocio.Clases
{
    public class Credencial
    {
        public int IdCredencial { get; set; }
        public string Usuario { get; set; }
        public string Clave { get; set; }
        public string Rol { get; set; } // Secretaria, Medico
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public DataTable Login(string usuario, string clave)
        {
            DCredencial d = new DCredencial();
            return d.Login(usuario, clave);
        }
    }
}
