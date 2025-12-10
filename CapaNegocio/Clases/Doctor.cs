using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SistemaDeGestionDeCitasMedicas;

namespace CapaNegocio.Clases
{
    public class Doctor
    {
        public int IdDoctor { get; set; }
        public string Nombre { get; set; }

        public string Especialidad { get; set; }
        public string Telefono { get; set; }

        public static DataTable ObtenerDoctores()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ConexionBD.Cn))
            {
                string query = "SELECT IdDoctor, Nombre FROM Doctor";

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);

                da.Fill(dt);
            }

            return dt;
        }
    }
}
