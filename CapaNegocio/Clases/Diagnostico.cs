using System;
using System.Data;
using Microsoft.Data.SqlClient;
using SistemaDeGestionDeCitasMedicas;

namespace CapaNegocio.Clases
{
    public class Diagnostico
    {
        public int IdDiagnostico { get; set; }
        public Cita Cita { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; }

        // ✔ Guardar diagnóstico (ya no depende del paciente)
        public static void GuardarDiagnostico(int idCita, DateTime fecha, string descripcion)
        {
            using (SqlConnection con = new SqlConnection(ConexionBD.Cn))
            {
                con.Open();
                string query = @"INSERT INTO Diagnostico (IdCita, Fecha, Descripcion)
                                 VALUES (@cita, @fecha, @desc)";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@cita", idCita); // puede ser 0 si no hay cita
                cmd.Parameters.AddWithValue("@fecha", fecha);
                cmd.Parameters.AddWithValue("@desc", descripcion);

                cmd.ExecuteNonQuery();
            }
        }

        // ✔ Mostrar todos los diagnósticos
        public static DataTable MostrarDiagnosticos()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ConexionBD.Cn))
            {
                string query = @"
                SELECT D.IdDiagnostico,
                       ISNULL(P.Nombre,'N/A') AS Paciente,
                       ISNULL(P.Telefono,'') AS Telefono,
                       D.Fecha,
                       D.Descripcion
                FROM Diagnostico D
                LEFT JOIN Cita C ON D.IdCita = C.IdCita
                LEFT JOIN Paciente P ON C.IdPaciente = P.IdPaciente";

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        // ✔ Buscar diagnóstico por nombre del paciente
        public static DataTable BuscarDiagnosticoPorNombre(string nombre)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ConexionBD.Cn))
            {
                con.Open();

                string query = @"
                SELECT D.IdDiagnostico,
                       ISNULL(P.Nombre,'N/A') AS Paciente,
                       ISNULL(P.Telefono,'') AS Telefono,
                       D.Fecha,
                       D.Descripcion
                FROM Diagnostico D
                LEFT JOIN Cita C ON D.IdCita = C.IdCita
                LEFT JOIN Paciente P ON C.IdPaciente = P.IdPaciente
                WHERE P.Nombre LIKE @nombre";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@nombre", "%" + nombre + "%");

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }
    }
}