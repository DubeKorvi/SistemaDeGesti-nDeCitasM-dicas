using System.Data;
using System.Drawing.Text;
using System.Threading;
using System.Threading.Tasks;
using CapaNegocio.Clases;
using MaterialSkin;
using static CapaNegocio.Clases.Credencial;



namespace CapaPresentacioon
{
    public partial class loguin : MaterialSkin.Controls.MaterialForm
    {
        private CancellationTokenSource _cancellationTokenSource;
        private System.Windows.Forms.Timer progressTimer;
        private int progressValue = 0;
        private const int PROGRESS_MAX = 100;
        private const int PROGRESS_STEP = 1;
        private const int TIMER_INTERVAL = 20; // ms

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


            progressBarLogin.Visible = false;
            progressBarLogin.Style = ProgressBarStyle.Continuous;
            progressBarLogin.Height = 10;
            progressBarLogin.Maximum = PROGRESS_MAX;

            //Personalizar colores de la barra de progreso
            progressBarLogin.ForeColor = Color.LightGray;
            progressBarLogin.BackColor = Color.DodgerBlue;

            // Configurar el temporizador para la animación
            progressTimer = new System.Windows.Forms.Timer();
            progressTimer.Interval = TIMER_INTERVAL;
            progressTimer.Tick += ProgressTimer_Tick;

            // Asegurarse de que la barra use los colores del tema
            var skinManager = MaterialSkin.MaterialSkinManager.Instance;
            progressBarLogin.BackColor = skinManager.ColorScheme.LightPrimaryColor;
            progressBarLogin.ForeColor = skinManager.ColorScheme.PrimaryColor;


        }

        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            if (progressValue < PROGRESS_MAX)
            {
                progressValue += PROGRESS_STEP;
                progressBarLogin.Value = Math.Min(progressValue, PROGRESS_MAX);
            }
            else
            {
                progressTimer.Stop();
            }
        }


        // configuracion inicial de la barra de progreso
        private void loguin_FormClosing(object sender, FormClosingEventArgs e)
        {
            _cancellationTokenSource?.Cancel();
        }

        // Metodo para manejar el cierre del formulario
        private void loguin_FormClosed(object sender, FormClosedEventArgs e)
        {
            _cancellationTokenSource?.Cancel();
        }


      /*  private void materialProgressBar1_Click(object sender, EventArgs e)
        {
        }*/

        // Habilitar o deshabilitar controles de loguin
        private void SetLoguinControlsEnabled(bool enabled)
        {
            tbUsuarioLoguin.Enabled = enabled;
            tbContrasenaLoguin.Enabled = enabled;
            materialButton1.Enabled = enabled;
            this.UseWaitCursor = !enabled;
        }

        // Ahora es asincrono 
        private async void materialButton1_Click(object sender, EventArgs e)
        {
            string usuario = tbUsuarioLoguin.Text;
            string clave = tbContrasenaLoguin.Text;

            try
            {
                // Desabilitar controles durante la autenticacion 
                SetLoguinControlsEnabled(false);

                // Mostrar barra de progreso
                // Iniciar la animación
                progressValue = 0;
                progressBarLogin.Value = 0;
                progressBarLogin.Visible = true;
                progressTimer.Start();

                //crear una instancia de la clase Credencial
                var credencial = new CapaNegocio.Clases.Credencial();

                //crear un token de cancelacion
                // _cancellationTokenSource = new CancellationTokenSource();

                //Ejecutar la autenticacion en segundo plano
                DataTable resultado = await Task.Run(() =>
                credencial.Login(usuario, clave));

                // Si la autenticación es muy rápida, esperar un mínimo para mostrar la animación
                await Task.Delay(800);

                //Verificar el resultado
                if (resultado == null || resultado.Rows.Count == 0)
                {
                    MessageBox.Show("Usuario o Contraseña incorrectos", "Error de autentificación",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // si llegamos aqui el login fue exitoso

                string rol = resultado.Rows[0]["Rol"].ToString();
                int idDoctor = resultado.Rows[0]["IdDoctor"] != DBNull.Value ?
                    Convert.ToInt32(resultado.Rows[0]["IdDoctor"]) : 0;

                // Completar la barra de progreso
                progressBarLogin.Value = PROGRESS_MAX;
                await Task.Delay(200); // Pequeña pausa para ver la barra completa

                // Ocultar este formulario y mostrar el menu principal
                this.Hide();
                FMenuPrincipal mp = new FMenuPrincipal(rol, idDoctor);
                mp.FormClosed += (s, args) => this.Close(); // Cerrar la aplicacion cuando se cierre el menu 
                mp.Show();

            }

            catch (OperationCanceledException)
            {
                // El usuario cancelo la operacion
                MessageBox.Show("Inicio de sesion cancelado");
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Error durante el inicio de sesion: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                //Restaurar controles de loguin
                //Detener y ocultar la barra de progreso    
                progressTimer.Stop();
                SetLoguinControlsEnabled(true);
                progressBarLogin.Visible = false;
            }

        }

        private void progressBarLogin_Click(object sender, EventArgs e)
        {

        }
    }


}
