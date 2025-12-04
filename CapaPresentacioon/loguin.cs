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
    }
}
