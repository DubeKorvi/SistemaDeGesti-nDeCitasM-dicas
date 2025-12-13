using System;
using System.Data;
using Microsoft.Data.SqlClient;
using SistemaDeGestionDeCitasMedicas;

namespace CapaNegocio.Clases
{
    public class Diagnostico
    {
        public int IdDiagnostico { get; set; }
        public Cita2 Cita { get; set; }
        public string Descripcion { get; set; }

      
        // Guarda un diagnóstico completo con datos del paciente
       
        public static void GuardarDiagnostico(string nombrePaciente, string telefono, DateTime fecha, string descripcion)
        {
            using (SqlConnection con = new SqlConnection(ConexionBD.Cn))
            {
                con.Open();

                // Buscar el paciente
                int idPaciente = 0;
                string queryBuscarPaciente = "SELECT IdPaciente FROM Paciente WHERE Nombre = @nombre";
                SqlCommand cmdBuscar = new SqlCommand(queryBuscarPaciente, con);
                cmdBuscar.Parameters.AddWithValue("@nombre", nombrePaciente);

                object result = cmdBuscar.ExecuteScalar();

                if (result == null)
                {
                    throw new Exception($"El paciente '{nombrePaciente}' no existe en el sistema. Debe registrarlo primero en Gestión de Citas.");
                }

                idPaciente = Convert.ToInt32(result);

                // Actualizar el teléfono si se proporcionó
                if (!string.IsNullOrEmpty(telefono))
                {
                    string queryActualizar = "UPDATE Paciente SET Telefono = @tel WHERE IdPaciente = @id";
                    SqlCommand cmdActualizar = new SqlCommand(queryActualizar, con);
                    cmdActualizar.Parameters.AddWithValue("@tel", telefono);
                    cmdActualizar.Parameters.AddWithValue("@id", idPaciente);
                    cmdActualizar.ExecuteNonQuery();
                }

                // Buscar la última cita del paciente (si existe)
                int idCita = 0;
                string queryBuscarCita = @"SELECT TOP 1 IdCita 
                                  FROM Cita 
                                  WHERE IdPaciente = @idPac 
                                  ORDER BY Fecha DESC, Hora DESC";
                SqlCommand cmdCita = new SqlCommand(queryBuscarCita, con);
                cmdCita.Parameters.AddWithValue("@idPac", idPaciente);

                object resultCita = cmdCita.ExecuteScalar();
                if (resultCita != null)
                {
                    idCita = Convert.ToInt32(resultCita);
                }

                // Insertar el diagnóstico CON EL NOMBRE
                string queryDiagnostico = @"INSERT INTO Diagnostico (Nombre, IdCita, Descripcion)
                                   VALUES (@nombre, @cita, @desc)";
                SqlCommand cmdDiag = new SqlCommand(queryDiagnostico, con);
                cmdDiag.Parameters.AddWithValue("@nombre", nombrePaciente);  // ← AGREGAR ESTO
                cmdDiag.Parameters.AddWithValue("@cita", idCita == 0 ? (object)DBNull.Value : idCita);
                cmdDiag.Parameters.AddWithValue("@desc", descripcion);
                cmdDiag.ExecuteNonQuery();
            }
        }

       
        // Muestra todos los diagnósticos con información del paciente
        
        public static DataTable MostrarDiagnosticos()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ConexionBD.Cn))
            {
                string query = @"
            SELECT D.IdDiagnostico,
                   D.Nombre AS Paciente,
                   ISNULL(P.Telefono, 'N/A') AS Telefono,
                   D.Fecha,
                   D.Descripcion
            FROM Diagnostico D
            LEFT JOIN Cita C ON D.IdCita = C.IdCita
            LEFT JOIN Paciente P ON C.IdPaciente = P.IdPaciente
            ORDER BY D.Fecha DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

      
        // Busca diagnósticos por nombre del paciente
       
        public static DataTable BuscarDiagnosticoPorNombre(string nombre)
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ConexionBD.Cn))
            {
                string query = @"
            SELECT D.IdDiagnostico,
                   D.Nombre AS Paciente,
                   ISNULL(P.Telefono, 'N/A') AS Telefono,
                   D.Fecha,
                   D.Descripcion
            FROM Diagnostico D
            LEFT JOIN Cita C ON D.IdCita = C.IdCita
            LEFT JOIN Paciente P ON C.IdPaciente = P.IdPaciente
            WHERE D.Nombre LIKE @nombre
            ORDER BY D.Fecha DESC";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@nombre", "%" + nombre + "%");

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

       
        // Obtiene el teléfono de un paciente por su nombre
      
        public static string ObtenerTelefonoPaciente(string nombre)
        {
            using (SqlConnection con = new SqlConnection(ConexionBD.Cn))
            {
                con.Open();
                string query = "SELECT Telefono FROM Paciente WHERE Nombre = @nombre";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@nombre", nombre);

                object result = cmd.ExecuteScalar();
                return result != null ? result.ToString() : "";
            }
        }
    }
}