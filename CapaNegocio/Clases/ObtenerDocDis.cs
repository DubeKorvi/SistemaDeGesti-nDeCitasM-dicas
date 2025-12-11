using System;
using System.Data;
using System.Data.SqlClient;
using SistemaDeGestionDeCitasMedicas;

namespace CapaNegocio.Clases
{
    public class ObtenerDoc
    {
        public static DataTable ObtenerDocDis(DateTime fecha, TimeSpan hora)
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ConexionBD.Cn))
            {
                string query = @"
        SELECT D.IdDoctor, D.Nombre
        FROM Doctor D
        WHERE D.IdDoctor IN (
            SELECT IdDoctor
            FROM Disponibilidad
            WHERE CAST(FechaEntrada AS DATE) = @fecha
              AND @hora >= HoraEntrada
              AND @hora <= HoraSalida
        )
        AND D.IdDoctor NOT IN (
            SELECT IdDoctor
            FROM Cita
            WHERE Fecha = @fecha AND Hora = @hora
        );";

                SqlCommand cmd = new SqlCommand(query, con);

                // ░░ CORRECCIÓN 1 → Enviar solo la fecha
                cmd.Parameters.Add("@fecha", SqlDbType.Date).Value = fecha.Date;

                // ░░ CORRECCIÓN 2 → Comparación correcta de TIME
                cmd.Parameters.Add("@hora", SqlDbType.Time).Value = hora;

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }
    }
}
