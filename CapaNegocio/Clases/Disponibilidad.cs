using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SistemaDeGestionDeCitasMedicas;

namespace CapaNegocio.Clases
{
    public class Disponibilidad
    {
        public int IdDisponibilidad { get; set; }
        public Doctor Doctor { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan HoraEntrada { get; set; }
        public TimeSpan HoraSalida { get; set; }
        public bool EstaDisponible { get; set; } = true;

        public static string GuardarDisponibilidad(int idDoctor, DateTime fecha, TimeSpan horaEntrada, TimeSpan horaSalida)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConexionBD.Cn))   // ← AQUÍ EL "new"
                {
                    string query = @"INSERT INTO Disponibilidades 
                                    (IdDoctor, Fecha, HoraEntrada, HoraSalida, EstaDisponible) 
                                     VALUES (@IdDoctor, @Fecha, @HoraEntrada, @HoraSalida, @EstaDisponible)";

                    SqlCommand cmd = new SqlCommand(query, con);

                    cmd.Parameters.AddWithValue("@IdDoctor", idDoctor);
                    cmd.Parameters.AddWithValue("@Fecha", fecha);
                    cmd.Parameters.AddWithValue("@HoraEntrada", horaEntrada);
                    cmd.Parameters.AddWithValue("@HoraSalida", horaSalida);
                    cmd.Parameters.AddWithValue("@EstaDisponible", true);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    return "OK";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
