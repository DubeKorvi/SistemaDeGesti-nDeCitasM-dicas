using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using MaterialSkin;
using CapaNegocio.Clases;
using CapaNegocio.Excepciones;
using SistemaDeGestionDeCitasMedicas;

namespace CapaPresentacioon
{
    public partial class FMenuPrincipal : MaterialSkin.Controls.MaterialForm
    {
        private bool modoEditar = false;
        private int idCitaSeleccionada = 0;

        private string Rol;
        private int IdDoctor;

        public FMenuPrincipal(string rol, int idDoctor)
        {
            InitializeComponent();

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

            Rol = rol;
            IdDoctor = idDoctor;

            ConfigurarInterfazPorRol();
            CargarDoctores();
            CargarCitas(); // carga el grid de gestión (dgvGestion)
        }

        // -------------------------
        // TabControl SelectedIndexChanged
        // -------------------------
        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Asegúrate de que el texto del tab coincida exactamente con el del diseñador
            if (materialTabControl1.SelectedTab != null && materialTabControl1.SelectedTab.Text == "Citas Agendadas")
            {
                CargarCitasAgendadas();
            }
        }

        // -------------------------
        // Métodos para Citas Agendadas
        // -------------------------
        private void CargarCitasAgendadas()
        {
            dgvCitasAg.DataSource = GestionDeCitas.MostrarCitasAgendadas();
            FormatearGridCitasAg(dgvCitasAg);
        }

        private void FormatearGridCitasAg(DataGridView dgv)
        {
            // Opcional: ajustar columnas, ancho, formato de fecha/hora
            if (dgv.Columns.Contains("Fecha"))
            {
                dgv.Columns["Fecha"].DefaultCellStyle.Format = "yyyy-MM-dd";
            }
            if (dgv.Columns.Contains("Hora"))
            {
                dgv.Columns["Hora"].DefaultCellStyle.Format = @"hh\:mm";
            }
            dgv.AutoResizeColumns();
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
        }

        // -------------------------
        // Buscador (btnBuscarCitAg)
        // -------------------------
        private void btnBuscarCitAg_Click(object sender, EventArgs e)
        {
            string nombre = tbNombreCitAg.Text.Trim();

            try
            {
                if (string.IsNullOrEmpty(nombre))
                {
                    CargarCitasAgendadas();
                }
                else
                {
                    dgvCitasAg.DataSource = GestionDeCitas.BuscarCitasAgendadas(nombre);
                    FormatearGridCitasAg(dgvCitasAg);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al buscar citas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // -------------------------
        // Resto de métodos (limpieza, gestión, diagnósticos...)
        // -------------------------
        private void ActivarCamposDiagnostico(bool enable)
        {
            tbNombreDig.Enabled = enable;
            mtbTelefonoDig.Enabled = enable;
            dtpFechaDig.Enabled = enable;
            mtbDescripcionDig.Enabled = enable;
        }

        private void LimpiarCamposDiagnostico()
        {
            tbNombreDig.Text = "";
            mtbTelefonoDig.Text = "";
            mtbDescripcionDig.Text = "";
            dtpFechaDig.Value = DateTime.Today;
        }

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

            tbNombreGes.Text = cita.Paciente?.Nombre ?? "";
            if (cita.Doctor != null && cita.Doctor.IdDoctor != 0)
            {
                try
                {
                    cbDoctorGes.SelectedValue = cita.Doctor.IdDoctor;
                }
                catch
                {
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

        private void ConfigurarInterfazPorRol()
        {
            if (Rol == "Doctor")
            {
                foreach (TabPage tab in materialTabControl1.TabPages)
                {
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
                    if (tab.Text == "Disponibilidad")
                        tab.Enabled = false;
                    else
                        tab.Enabled = true;
                }
            }
        }

        private void CargarDoctores()
        {
            var dt = ObtenerDoc.ObtenerDocDis(dtFechaGes.Value, dtpHoraGes.Value.TimeOfDay);

            cbDoctorGes.DataSource = dt;
            cbDoctorGes.DisplayMember = "Nombre";
            cbDoctorGes.ValueMember = "IdDoctor";
        }

        // -------------------------
        // Eventos y botones (mantengo tu lógica, solo limpiada)
        // -------------------------
        private void dtFechaGes_ValueChanged(object sender, EventArgs e)
        {
            CargarDoctores();
        }

        private void dtpHoraGes_ValueChanged(object sender, EventArgs e)
        {
            CargarDoctores();
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
                CargarDiagnosticos();
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

        private void btnEditarGes_Click(object sender, EventArgs e)
        {
            if (dgvGestion.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione una cita para editar.", "Aviso");
                return;
            }

            idCitaSeleccionada = Convert.ToInt32(dgvGestion.SelectedRows[0].Cells["IdCita"].Value);
            RecargarCita(idCitaSeleccionada);

            SetCampos(true);
            modoEditar = true;

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

                btnAgendarGes.Enabled = true;
                btnEditarGes.Enabled = true;
                btnCancelarGes.Enabled = true;
                btnGuardarGes.Enabled = false;

                SetCampos(false);
            }
        }

        private void btnAgendarGes_Click(object sender, EventArgs e)
        {
            modoEditar = false;

            SetCampos(true);
            LimpiarCamposGestion();
            CargarDoctores();

            btnEditarGes.Enabled = false;
            btnCancelarGes.Enabled = false;
            btnGuardarGes.Enabled = true;
            btnAgendarGes.Enabled = false;
        }

        private void btnGuardarGes_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvGestion.CurrentRow == null)
                {
                    MessageBox.Show("Seleccione una cita para guardar cambios.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int idCita = Convert.ToInt32(dgvGestion.CurrentRow.Cells["IdCita"].Value);

                string nombrePaciente = tbNombreGes.Text.Trim();
                if (string.IsNullOrEmpty(nombrePaciente))
                {
                    MessageBox.Show("Ingrese el nombre del paciente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

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

                GestionDeCitas.EditarCita(idCita, idPaciente, idDoctor, fecha, hora, motivo);

                MessageBox.Show("Cita actualizada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarCitas();
                LimpiarCamposGestion();

                btnAgendarGes.Enabled = true;
                btnEditarGes.Enabled = true;
                btnCancelarGes.Enabled = true;
                btnGuardarGes.Enabled = false;

                modoEditar = false;
                idCitaSeleccionada = 0;
                SetCampos(false);
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

        private void dgvGestion_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            idCitaSeleccionada = Convert.ToInt32(dgvGestion.Rows[e.RowIndex].Cells["IdCita"].Value);
            RecargarCita(idCitaSeleccionada);

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

                int idPaciente = GestionDeCitas.ObtenerIdPacientePorNombre(nombre);

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

        // Fin de la clase
    }
}