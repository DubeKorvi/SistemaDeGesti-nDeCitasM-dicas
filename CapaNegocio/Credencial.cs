using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class Credencial
    {
        public DataTable Login(string usuario, string clave)
        {
            DCredencial d = new DCredencial();
            return d.Login(usuario, clave);
        }

    }
}
