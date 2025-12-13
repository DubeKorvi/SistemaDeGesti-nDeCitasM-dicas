using System.Data;
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

        public static DataTable ObtenerTodos()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ConexionBD.Cn))
            {
                string query = "SELECT IdDoctor, Nombre, Especialidad FROM Doctor";
                SqlDataAdapter da = new SqlDataAdapter(query, con);
                da.Fill(dt);
            }
            return dt;
        }
    }
}