using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using SistemaDeGestionDeCitasMedicas;
using Microsoft.Data.SqlClient;

namespace CapaDatos
{
    public class DCredencial 
    { 
        public DataTable Login(string usuario, string clave)
        {
            using (SqlConnection con = new SqlConnection(ConexionBD.Cn))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Credencial WHERE Usuario=@u AND Clave=@c", con);
                cmd.Parameters.AddWithValue("@u", usuario);
                cmd.Parameters.AddWithValue("@c", clave);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();

                da.Fill(dt);
                return dt;
            }
        }
    }
}
