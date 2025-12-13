using System;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CapaNegocio.Clases;
using MaterialSkin;

namespace CapaPresentacioon
{
    public partial class loguin : MaterialSkin.Controls.MaterialForm
    {
        private CancellationTokenSource _cancellationTokenSource;
        private System.Windows.Forms.Timer progressTimer;
        private int progressValue = 0;
        private const int PROGRESS_MAX = 100;
        private const int PROGRESS_STEP = 1;
        private const int TIMER_INTERVAL = 20;

        public loguin()
        {
            InitializeComponent();

            this.Text = "Clinica SanRafael - Login";

            // Configurar MaterialSkin
            var materialSkinManager = MaterialSkin.MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkin.MaterialSkinManager.Themes.LIGHT;

            materialSkinManager.ColorScheme = new MaterialSkin.ColorScheme(
                MaterialSkin.Primary.Blue600,
                MaterialSkin.Primary.Blue800,
                MaterialSkin.Primary.Blue400,
                MaterialSkin.Accent.LightBlue200,
                MaterialSkin.TextShade.WHITE
            );

            progressBarLogin.Visible = false;
            progressBarLogin.Style = ProgressBarStyle.Continuous;
            progressBarLogin.Height = 10;
            progressBarLogin.Maximum = PROGRESS_MAX;

            progressBarLogin.ForeColor = Color.LightGray;
            progressBarLogin.BackColor = Color.DodgerBlue;

            progressTimer = new System.Windows.Forms.Timer();
            progressTimer.Interval = TIMER_INTERVAL;
            progressTimer.Tick += ProgressTimer_Tick;

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

        private void loguin_FormClosing(object sender, FormClosingEventArgs e)
        {
            _cancellationTokenSource?.Cancel();
        }

        private void loguin_FormClosed(object sender, FormClosedEventArgs e)
        {
            _cancellationTokenSource?.Cancel();
        }

        private void SetLoguinControlsEnabled(bool enabled)
        {
            tbUsuarioLoguin.Enabled = enabled;
            tbContrasenaLoguin.Enabled = enabled;
            materialButton1.Enabled = enabled;
            this.UseWaitCursor = !enabled;
        }

        private async void materialButton1_Click(object sender, EventArgs e)
        {
            string usuario = tbUsuarioLoguin.Text.Trim();
            string clave = tbContrasenaLoguin.Text.Trim();

            // Validaciones básicas
            if (string.IsNullOrEmpty(usuario))
            {
                MessageBox.Show("Ingrese su usuario.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbUsuarioLoguin.Focus();
                return;
            }

            if (string.IsNullOrEmpty(clave))
            {
                MessageBox.Show("Ingrese su contraseña.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tbContrasenaLoguin.Focus();
                return;
            }

            try
            {
                SetLoguinControlsEnabled(false);

                progressValue = 0;
                progressBarLogin.Value = 0;
                progressBarLogin.Visible = true;
                progressTimer.Start();

                var credencial = new CapaNegocio.Clases.Credencial();

                DataTable resultado = await Task.Run(() =>
                    credencial.Login(usuario, clave));

                await Task.Delay(800);

                if (resultado == null || resultado.Rows.Count == 0)
                {
                    MessageBox.Show("Usuario o Contraseña incorrectos",
                        "Error de autenticación",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // Limpiar campos
                    tbContrasenaLoguin.Clear();
                    tbUsuarioLoguin.Focus();
                    return;
                }

                // ⭐ Obtener datos del usuario autenticado
                string rol = resultado.Rows[0]["Rol"].ToString();
                int idDoctor = resultado.Rows[0]["IdDoctor"] != DBNull.Value ?
                    Convert.ToInt32(resultado.Rows[0]["IdDoctor"]) : 0;

                progressBarLogin.Value = PROGRESS_MAX;
                await Task.Delay(200);

                // ⭐ Abrir el menú principal con el rol y el ID del doctor
                this.Hide();
                FMenuPrincipal mp = new FMenuPrincipal(rol, idDoctor);
                mp.FormClosed += (s, args) => this.Close();
                mp.Show();
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Inicio de sesión cancelado", "Cancelado",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error durante el inicio de sesión: {ex.Message}",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
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