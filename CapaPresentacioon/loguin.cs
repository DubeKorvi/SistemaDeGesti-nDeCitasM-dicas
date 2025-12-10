using MaterialSkin;


namespace CapaPresentacioon
{
    public partial class loguin : MaterialSkin.Controls.MaterialForm
    {
        public loguin()
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
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            string usuario = tbUsuarioLoguin.Text;
            string clave = tbContrasenaLoguin.Text;

            var resultado = Ncredencial.Login(usuario, clave);

            if (resultado.Rows.Count == 0)
            {
                MessageBox.Show("Usuario o Contraseña incorrectos");
                    return;
            }

            string rol = resultado.Rows[0]["Rol"].ToString();
            int idDoctor = resultado.Rows[0]["IdDoctor"] != DBNull.Value ? Convert.ToInt32(resultado.Rows[0]["IdDoctor"]) : 0;

            FMenuPrincipal mp = new FMenuPrincipal(rol, idDoctor);

            mp.Show();

            this.Hide();
        }
    }
}
