using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin;
using CapaNegocio;

namespace CapaPresentacioon
{
    public partial class FMenuPrincipal : MaterialSkin.Controls.MaterialForm
    {

        private string Rol;
        private int IdDoctor;
        public FMenuPrincipal(string rol, int idDoctor)
        {
            InitializeComponent();

            // Configurar MaterialSkin
            var materialSkinManager = MaterialSkin.MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkin.MaterialSkinManager.Themes.LIGHT;

            // --- CONFIGURACIÓN CON COLORES AZULES (similar al diseño de la imagen) ---
            materialSkinManager.ColorScheme = new MaterialSkin.ColorScheme(
                MaterialSkin.Primary.Blue600,       // Azul principal
                MaterialSkin.Primary.Blue800,       // Azul "dark" del menú y cabecera
                MaterialSkin.Primary.Blue400,       // Azul más claro
                MaterialSkin.Accent.LightBlue200,   // Accent suave estilo Windows 11
                MaterialSkin.TextShade.WHITE        // Texto blanco para contrastar
            );


            Rol = rol;
            IdDoctor = idDoctor;

            ConfigurarInterfazPorRol();
        }


        private void ConfigurarInterfazPorRol()
        {
            if (Rol == "Doctor")
            {
                foreach (TabPage tab in materialTabControl1.TabPages)
                {
                    // Solo habilitar las que el doctor puede ver
                    if (tab.Text == "Disponibilidad" ||
                        tab.Text == "Citas Agendadas" ||
                        tab.Text == "Diagnosticos")
                    {
                        tab.Enabled = true;
                    }
                    else
                    {
                        tab.Enabled = false;
                    }
                }
            }
            else if (Rol == "Secretaria")
            {
                foreach (TabPage tab in materialTabControl1.TabPages)
                {
                    // Secretaria puede ver todo MENOS disponibilidad
                    if (tab.Text == "Disponibilidad")
                        tab.Enabled = false;
                    else
                        tab.Enabled = true;
                }
            }
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void materialCard2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void LblDiag_Click(object sender, EventArgs e)
        {

        }

        private void materialButton2_Click(object sender, EventArgs e)
        {

        }

        private void materialTextBox21_Click(object sender, EventArgs e)
        {

        }

        private void materialLabel2_Click(object sender, EventArgs e)
        {

        }

        private void btnEditarGes_Click(object sender, EventArgs e)
        {

        }

        private void btnCancelarGes_Click(object sender, EventArgs e)
        {

        }

        private void lblBusacarCitAg_Click(object sender, EventArgs e)
        {

        }

        private void lblTelefonoGes_Click(object sender, EventArgs e)
        {

        }

        private void mtbTelefonoDig_Click(object sender, EventArgs e)
        {

        }

        private void materialCard7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void materialCard10_Paint(object sender, PaintEventArgs e)
        {

        }

        private void materialLabel10_Click(object sender, EventArgs e)
        {

        }
    }
}
