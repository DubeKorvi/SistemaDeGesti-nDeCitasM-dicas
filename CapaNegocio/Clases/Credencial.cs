using System;
using System.Data;
using Microsoft.Data.SqlClient;
using SistemaDeGestionDeCitasMedicas;

namespace CapaNegocio.Clases
{
    public class Credencial
    {
        public int IdCredencial { get; set; }
        public string Usuario { get; set; }
        public string Clave { get; set; }
        public string Rol { get; set; } // Secretaria, Medico
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        private class DCredencial
        {
            public DataTable Login(string usuario, string clave)
            {
                using (SqlConnection con = new SqlConnection(ConexionBD.Cn))
                {
                    
                    SqlCommand cmd = new SqlCommand(
                        "SELECT IdCredencial, Usuario, Rol, IdDoctor FROM Credencial WHERE Usuario=@u AND Clave=@c",
                        con);
                    cmd.Parameters.AddWithValue("@u", usuario);
                    cmd.Parameters.AddWithValue("@c", clave);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        public DataTable Login(string usuario, string clave)
        {
            DCredencial d = new DCredencial();
            return d.Login(usuario, clave);
        }
    }
}