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
using CapaNegocio.Clases;
using CapaNegocio.Excepciones;
using SistemaDeGestionDeCitasMedicas;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;



namespace CapaPresentacioon
{


    public partial class FMenuPrincipal : MaterialSkin.Controls.MaterialForm
    {
        private bool modoEditar = false;
        int idCitaSeleccionada = 0;

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
            CargarDoctores();
            CargarCitas();
        }

        private void ActivarCamposDiagnostico(bool enable)
        {
            tbNombreDig.Enabled = enable;
            mtbTelefonoDig.Enabled = enable;
            dtpFechaDig.Enabled = enable;
            mtbDescripcionDig.Enabled = enable;
        }

        // ✔ Limpiar
        private void LimpiarCamposDiagnostico()
        {
            tbNombreDig.Text = "";
            mtbTelefonoDig.Text = "";
            mtbDescripcionDig.Text = "";
            dtpFechaDig.Value = DateTime.Today;
        }

        // ✔ Cargar diagnósticos en el grid
        private void CargarDiagnosticos()
        {
            dgvDiagnosticos.DataSource = Diagnostico.MostrarDiagnosticos();
        }
        private void RecargarCita(int id)
        {
            var cita = GestionDeCitas.ObtenerCita(id);

            if (cita == null)
            {
                MessageBox.Show("No se pudo cargar la cita seleccionada.");
                return;
            }

            // Asegúrate que cbDoctorGes.DataSource ya fue cargado con IdDoctor como ValueMember
            tbNombreGes.Text = cita.Paciente?.Nombre ?? "";
            if (cita.Doctor != null && cita.Doctor.IdDoctor != 0)
            {
                try
                {
                    cbDoctorGes.SelectedValue = cita.Doctor.IdDoctor;
                }
                catch
                {
                    // Si SelectedValue falla por DataSource distinto, intenta buscar por texto:
                    cbDoctorGes.Text = cita.Doctor.Nombre;
                }
            }
            dtFechaGes.Value = cita.Fecha.Date;
            dtpHoraGes.Value = DateTime.Today.Add(cita.Hora);
            tbMotivoGes.Text = cita.Motivo ?? "";
        }

        private void SetCampos(bool enable)
        {
            tbNombreGes.Enabled = enable;
            cbDoctorGes.Enabled = enable;
            dtFechaGes.Enabled = enable;
            dtpHoraGes.Enabled = enable;
            tbMotivoGes.Enabled = enable;
        }


        private void LimpiarCamposGestion()
        {
            tbNombreGes.Text = "";
            tbMotivoGes.Text = "";
            cbDoctorGes.SelectedIndex = -1;

            dtFechaGes.Value = DateTime.Today;
            dtpHoraGes.Value = DateTime.Now;

            idCitaSeleccionada = 0;
        }

        private void CargarCitas()
        {
            dgvGestion.DataSource = GestionDeCitas.MostrarCitas();
        }
        //TODO Metodo para la intefaz por rol
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

        //TODO Metodo para cargar los doctores en el combobox
        private void CargarDoctores()
        {
            DateTime fecha = dtFechaGes.Value.Date;
            TimeSpan hora = dtpHoraGes.Value.TimeOfDay;

            var dt = ObtenerDoc.ObtenerDocDis(dtFechaGes.Value, dtpHoraGes.Value.TimeOfDay);

            cbDoctorGes.DataSource = dt;
            cbDoctorGes.DisplayMember = "Nombre";
            cbDoctorGes.ValueMember = "IdDoctor";
        }


        private void dtFechaGes_ValueChanged(object sender, EventArgs e)
        {
            CargarDoctores();
        }

        private void dtpHoraGes_ValueChanged(object sender, EventArgs e)
        {
            CargarDoctores();
        }
        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            CargarDiagnosticos();
        }

        private void materialCard2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void LblDiag_Click(object sender, EventArgs e)
        {

        }

        private void materialButton2_Click(object sender, EventArgs e)
        {
            try
            {
                DateTime fecha = dtpFechaDig.Value;
                string descripcion = mtbDescripcionDig.Text.Trim();

                if (string.IsNullOrWhiteSpace(descripcion))
                {
                    MessageBox.Show("Debe ingresar una descripción.");
                    return;
                }

                // Guardar diagnóstico con IdCita = 0 temporal (o crear una cita dummy)
                int idCita = 0;

                using (SqlConnection con = new SqlConnection(ConexionBD.Cn))
                {
                    con.Open();
                    string query = "INSERT INTO Diagnostico (IdCita, Fecha, Descripcion) VALUES (@cita, @fecha, @desc)";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@cita", idCita);
                    cmd.Parameters.AddWithValue("@fecha", fecha);
                    cmd.Parameters.AddWithValue("@desc", descripcion);
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Diagnóstico guardado correctamente.");

                // Recargar DataGridView
                CargarDiagnosticos();

                // Limpiar campos
                mtbDescripcionDig.Clear();
                dtpFechaDig.Value = DateTime.Today;
                tbNombreDig.Clear();
                mtbTelefonoDig.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar diagnóstico: " + ex.Message);
            }
        }

        private void materialTextBox21_Click(object sender, EventArgs e)
        {

        }

        private void materialLabel2_Click(object sender, EventArgs e)
        {

        }

        private void btnEditarGes_Click(object sender, EventArgs e)
        {
            if (dgvGestion.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione una cita para editar.", "Aviso");
                return;
            }

            // Guardar id de la cita seleccionada
            idCitaSeleccionada = Convert.ToInt32(dgvGestion.SelectedRows[0].Cells["IdCita"].Value);

            // Recargar desde BD la información actual de la cita (evita inconsistencias)
            RecargarCita(idCitaSeleccionada);

            // Activar edición de campos (tu método SetCampos)
            SetCampos(true);
            modoEditar = true;

            // Botones: habilitar/inhabilitar según flujo
            btnGuardarGes.Enabled = true;
            btnEditarGes.Enabled = false;
            btnCancelarGes.Enabled = true;
            btnAgendarGes.Enabled = false;
        }

        private void btnCancelarGes_Click(object sender, EventArgs e)
        {

            if (dgvGestion.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione una cita para cancelar.");
                return;
            }

            int idCita = Convert.ToInt32(dgvGestion.SelectedRows[0].Cells["IdCita"].Value);

            DialogResult dr = MessageBox.Show(
                "¿Seguro que desea cancelar (eliminar) esta cita?",
                "Confirmación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (dr == DialogResult.Yes)
            {
                GestionDeCitas.CancelarCita(idCita);
                CargarCitas();
                LimpiarCamposGestion();

                // HABILITAR TODOS LOS BOTONES
                btnAgendarGes.Enabled = true;
                btnEditarGes.Enabled = true;
                btnCancelarGes.Enabled = true;
                btnGuardarGes.Enabled = false;

                SetCampos(false);
            }
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

        private void btnGuardarDis_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Validar nombre
                string nombrePaciente = tbNombreGes.Text.Trim();
                if (nombrePaciente == "")
                {
                    MessageBox.Show("Debe ingresar el nombre del paciente.");
                    return;
                }

                // 2. Buscar idPaciente
                int idPaciente = GestionDeCitas.ObtenerIdPacientePorNombre(nombrePaciente);

                // 3. Obtener doctor seleccionado
                if (cbDoctorGes.SelectedValue == null)
                {
                    MessageBox.Show("Debe seleccionar un doctor.");
                    return;
                }

                int idDoctor = Convert.ToInt32(cbDoctorGes.SelectedValue);

                // 4. Fecha y hora
                DateTime fecha = dtFechaGes.Value.Date;
                TimeSpan hora = dtpHoraGes.Value.TimeOfDay;

                string motivo = tbMotivoGes.Text.Trim();

                // 5. Guardar cita
                GestionDeCitas.AgendarCita(idPaciente, idDoctor, fecha, hora, motivo);

                MessageBox.Show("Cita guardada correctamente.");

                // Recargar grid
                CargarCitas();

            }
            catch (PacienteNoEncontradoException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }

        private void btnAgendarGes_Click(object sender, EventArgs e)
        {
            modoEditar = false;

            SetCampos(true);  // activar campos para escribir
            LimpiarCamposGestion();
            CargarDoctores();

            btnEditarGes.Enabled = false;
            btnCancelarGes.Enabled = false;
            btnGuardarGes.Enabled = true;   // porque sí vas a guardar una nueva cita
            btnAgendarGes.Enabled = false;
        }

        private void btnGuardarGes_Click(object sender, EventArgs e)
        {
            try
            {
                // 1) Validar fila seleccionada (usando el DataGridView que ya usas en el resto del Form)
                if (dgvGestion.CurrentRow == null)
                {
                    MessageBox.Show("Seleccione una cita para guardar cambios.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 2) Obtener ID de la cita desde la fila seleccionada
                int idCita = Convert.ToInt32(dgvGestion.CurrentRow.Cells["IdCita"].Value);

                // 3) Obtener/validar datos desde los controles reales del formulario
                string nombrePaciente = tbNombreGes.Text.Trim();
                if (string.IsNullOrEmpty(nombrePaciente))
                {
                    MessageBox.Show("Ingrese el nombre del paciente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Obtener o crear paciente (usa tus métodos de negocio existentes)
                int idPaciente = Paciente.ObtenerIdPacientePorNombre(nombrePaciente);
                if (idPaciente == 0)
                {
                    idPaciente = Paciente.CrearPaciente(nombrePaciente);
                }

                if (cbDoctorGes.SelectedValue == null)
                {
                    MessageBox.Show("Seleccione un doctor.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                int idDoctor = Convert.ToInt32(cbDoctorGes.SelectedValue);

                DateTime fecha = dtFechaGes.Value.Date;
                TimeSpan hora = dtpHoraGes.Value.TimeOfDay;
                string motivo = tbMotivoGes.Text.Trim();

                // 4) Llamada a la capa de negocio (tu método existente)
                GestionDeCitas.EditarCita(idCita, idPaciente, idDoctor, fecha, hora, motivo);

                // 5) Confirmación y refresco UI
                MessageBox.Show("Cita actualizada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarCitas();            // recargar DataGridView con MostrarCitas()
                LimpiarCamposGestion();   // tu método ya presente para limpiar controles

                // 6) **REQUERIMIENTO ESPECIAL**: después de guardar, habilitar TODOS los botones
                btnAgendarGes.Enabled = true;
                btnEditarGes.Enabled = true;
                btnCancelarGes.Enabled = true;
                btnGuardarGes.Enabled = false; // normalmente guardar queda deshabilitado hasta volver a editar

                // Reset estado interno
                modoEditar = false;
                idCitaSeleccionada = 0;
                SetCampos(false); // desactivar edición de campos
            }
            catch (PacienteNoEncontradoException ex)
            {
                MessageBox.Show(ex.Message, "Error paciente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar la cita: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void DGVGestion_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            idCitaSeleccionada = Convert.ToInt32(dgvGestion.Rows[e.RowIndex].Cells["IdCita"].Value);
            RecargarCita(idCitaSeleccionada);

            // No habilitar edición aquí: el usuario debe presionar EDITAR
            SetCampos(false);
            btnAgendarGes.Enabled = true;
            btnEditarGes.Enabled = true;
            btnCancelarGes.Enabled = true;
            btnGuardarGes.Enabled = false;

        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            try
            {
                string nombre = tbNombreDig.Text.Trim();
                string descripcion = mtbDescripcionDig.Text.Trim();
                DateTime fecha = dtpFechaDig.Value;

                if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(descripcion))
                {
                    MessageBox.Show("Debe completar todos los campos.");
                    return;
                }

                // Obtener idPaciente -> obtener su última cita o validar existencia
                int idPaciente = GestionDeCitas.ObtenerIdPacientePorNombre(nombre);

                // Obtener la cita reciente del paciente
                DataTable citas = GestionDeCitas.MostrarCitas();
                var citaPaciente = citas.AsEnumerable()
                    .Where(r => r.Field<string>("Paciente") == nombre)
                    .OrderByDescending(r => r.Field<DateTime>("Fecha"))
                    .FirstOrDefault();

                if (citaPaciente == null)
                {
                    MessageBox.Show("El paciente no tiene citas registradas.");
                    return;
                }

                int idCita = Convert.ToInt32(citaPaciente["IdCita"]);

                Diagnostico.GuardarDiagnostico(idCita, fecha, descripcion);

                MessageBox.Show("Diagnóstico registrado correctamente.");
                CargarDiagnosticos();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnBuscarDig_Click(object sender, EventArgs e)
        {
            try
            {
                string nombre = tbNombreDig.Text.Trim();

                if (string.IsNullOrWhiteSpace(nombre))
                {
                    MessageBox.Show("Debe escribir un nombre.");
                    return;
                }

                dgvDiagnosticos.DataSource = Diagnostico.BuscarDiagnosticoPorNombre(nombre);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnFiltrarDig_Click(object sender, EventArgs e)
        {
            try
            {
                string nombre = tbNombreDig.Text.Trim();

                if (string.IsNullOrWhiteSpace(nombre))
                {
                    MessageBox.Show("Ingrese un nombre para buscar.");
                    return;
                }

                dgvDiagnosticos.DataSource = Diagnostico.BuscarDiagnosticoPorNombre(nombre);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error en la búsqueda: " + ex.Message);
            }
        }
    }
}
