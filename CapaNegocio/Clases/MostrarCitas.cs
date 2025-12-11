using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SistemaDeGestionDeCitasMedicas;
using System.Data.SqlClient;
using CapaNegocio.Excepciones;


namespace CapaNegocio.Clases
{
    public class GestionDeCitas
    {
        public static DataTable MostrarCitas()
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(ConexionBD.Cn))
            {
                string query = @"SELECT C.IdCita, P.Nombre AS Paciente, D.Nombre AS Doctor, C.Fecha, C.Hora, C.Estado, C.Motivo
                                 FROM Cita C
                                 INNER JOIN Paciente P ON C.IdPaciente = P.IdPaciente
                                 INNER JOIN Doctor D ON C.IdDoctor = D.IdDoctor";

                SqlCommand cmd = new SqlCommand(query, con);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            return dt;
        }

        public static void AgendarCita(int idPaciente, int idDoctor, DateTime fecha, TimeSpan hora, string motivo)
        {
            using (SqlConnection con = new SqlConnection(ConexionBD.Cn))
            {
                con.Open();
                string query = @"INSERT INTO Cita (IdPaciente, IdDoctor, Fecha, Hora, Estado, Motivo)
                         VALUES (@p, @d, @f, @h, 'Agendada', @m)";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@p", idPaciente);
                cmd.Parameters.AddWithValue("@d", idDoctor);
                cmd.Parameters.AddWithValue("@f", fecha);
                cmd.Parameters.AddWithValue("@h", hora);
                cmd.Parameters.AddWithValue("@m", motivo);

                cmd.ExecuteNonQuery();
            }
        }

        public static void EditarCita(int idCita, int idPaciente, int idDoctor, DateTime fecha, TimeSpan hora, string motivo)
        {
            using (SqlConnection con = new SqlConnection(ConexionBD.Cn))
            {
                con.Open();
                string query = @"UPDATE Cita 
                         SET IdPaciente=@p, IdDoctor=@d, Fecha=@f, Hora=@h, Motivo=@m
                         WHERE IdCita=@id";
                SqlCommand cmd = new SqlCommand(query, con);

                cmd.Parameters.AddWithValue("@id", idCita);
                cmd.Parameters.AddWithValue("@p", idPaciente);
                cmd.Parameters.AddWithValue("@d", idDoctor);
                cmd.Parameters.AddWithValue("@f", fecha);
                cmd.Parameters.AddWithValue("@h", hora);
                cmd.Parameters.AddWithValue("@m", motivo);

                cmd.ExecuteNonQuery();
            }
        }


        public static void CancelarCita(int idCita)
        {
            using (SqlConnection con = new SqlConnection(ConexionBD.Cn))
            {
                con.Open();
                string query = @"UPDATE Cita SET Estado='Cancelada' WHERE IdCita=@id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", idCita);
                cmd.ExecuteNonQuery();
            }
        }


        public static int ObtenerIdPacientePorNombre(string nombre)
        {
            using (SqlConnection con = new SqlConnection(ConexionBD.Cn))
            {
                con.Open();
                string query = "SELECT IdPaciente FROM Paciente WHERE Nombre = @n";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@n", nombre);

                object result = cmd.ExecuteScalar();

                if (result == null)
                {
                    throw new PacienteNoEncontradoException();
                }

                return Convert.ToInt32(result);
            }
        }

        public static int BuscarIdPacientePorNombre(string nombre)
        {
            using (SqlConnection con = new SqlConnection(ConexionBD.Cn))
            {
                con.Open();

                string query = "SELECT IdPaciente FROM Paciente WHERE Nombre = @n";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@n", nombre);

                object result = cmd.ExecuteScalar();

                if (result == null)
                    throw new PacienteNoEncontradoException();

                return Convert.ToInt32(result);
            }
        }

        public static Cita ObtenerCita(int idCita)
        {
            using (SqlConnection con = new SqlConnection(ConexionBD.Cn))
            {
                string query = @"
            SELECT C.IdCita, C.IdPaciente, P.Nombre AS NombrePaciente,
                   C.IdDoctor, D.Nombre AS NombreDoctor,
                   C.Fecha, C.Hora, C.Estado, C.Motivo
            FROM Cita C
            LEFT JOIN Paciente P ON C.IdPaciente = P.IdPaciente
            LEFT JOIN Doctor D ON C.IdDoctor = D.IdDoctor
            WHERE C.IdCita = @id";

                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@id", idCita);

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        var cita = new Cita
                        {
                            IdCita = Convert.ToInt32(dr["IdCita"]),
                            Fecha = Convert.ToDateTime(dr["Fecha"]),
                            Hora = TimeSpan.Parse(dr["Hora"].ToString()),
                            Estado = dr["Estado"] == DBNull.Value ? null : dr["Estado"].ToString(),
                            Motivo = dr["Motivo"] == DBNull.Value ? null : dr["Motivo"].ToString(),
                            Paciente = new Paciente
                            {
                                IdPaciente = dr["IdPaciente"] == DBNull.Value ? 0 : Convert.ToInt32(dr["IdPaciente"]),
                                Nombre = dr["NombrePaciente"] == DBNull.Value ? "" : dr["NombrePaciente"].ToString()
                            },
                            Doctor = new Doctor
                            {
                                IdDoctor = dr["IdDoctor"] == DBNull.Value ? 0 : Convert.ToInt32(dr["IdDoctor"]),
                                Nombre = dr["NombreDoctor"] == DBNull.Value ? "" : dr["NombreDoctor"].ToString()
                            }
                        };

                        return cita;
                    }
                }
            }

            return null;
        }

    }
}
