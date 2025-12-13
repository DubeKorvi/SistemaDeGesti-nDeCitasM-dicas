using System;
using System.Data;
using Microsoft.Data.SqlClient;
using SistemaDeGestionDeCitasMedicas;

namespace CapaNegocio.Clases
{
    public class ObtenerDoc
    {
        public static DataTable ObtenerDocDis(DateTime fecha, TimeSpan hora)
        {
            DataTable dt = new DataTable();

            try
            {
                using (SqlConnection con = new SqlConnection(ConexionBD.Cn))
                {
                    con.Open();

                    string query = @"
                        SELECT D.IdDoctor, D.Nombre
                        FROM Doctor D
                        INNER JOIN Disponibilidad Disp ON D.IdDoctor = Disp.IdDoctor
                        WHERE @fecha BETWEEN Disp.FechaEntrada AND Disp.FechaSalida
                          AND @hora BETWEEN Disp.HoraEntrada AND Disp.HoraSalida
                          AND D.IdDoctor NOT IN (
                              SELECT ISNULL(IdDoctor, 0)
                              FROM Cita
                              WHERE Fecha = @fecha 
                                AND Hora = @hora
                                AND Estado != 'Cancelada'
                          );";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.Add("@fecha", SqlDbType.Date).Value = fecha.Date;
                    cmd.Parameters.Add("@hora", SqlDbType.Time).Value = hora;

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                // Si hay error, retorna tabla vacía
                dt = new DataTable();
            }

            return dt;
        }
    }
}