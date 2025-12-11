using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaDeGestionDeCitasMedicas;
using System.Data.SqlClient;

namespace CapaNegocio.Clases
{
    public class Paciente
    {
        public int IdPaciente { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public DateTime FechaNacimiento { get; set; }

        public static int ObtenerIdPacientePorNombre(string nombre)
        {
            using (SqlConnection con = new SqlConnection(ConexionBD.Cn))
            {
                con.Open();
                string query = "SELECT IdPaciente FROM Paciente WHERE Nombre = @n";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@n", nombre);

                object result = cmd.ExecuteScalar();
                return result == null ? 0 : Convert.ToInt32(result);
            }
        }

        public static int CrearPaciente(string nombre)
        {
            using (SqlConnection con = new SqlConnection(ConexionBD.Cn))
            {
                con.Open();
                string query = @"INSERT INTO Paciente (Nombre) VALUES (@n); 
                         SELECT SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@n", nombre);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }
    }


}
