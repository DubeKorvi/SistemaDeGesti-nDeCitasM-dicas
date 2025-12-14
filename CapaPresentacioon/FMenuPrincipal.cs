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

        private bool modoEditarDiagnostico = false;
        private int idDiagnosticoSeleccionado = 0;

        private string Rol;
        private int IdDoctor;

        public FMenuPrincipal(string rol, int idDoctor)
        {
            InitializeComponent();

            // PRIMERO: Asignar las variables de rol e ID
            Rol = rol;
            IdDoctor = idDoctor;

            this.Text = $"Clinica SanRafael";

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

            // Configurar KeyPress handlers
            this.tbNombreGes.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbNombreGes_KeyPress);
            this.tbMotivoGes.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbMotivoGes_KeyPress);
            this.tbNombreDig.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbNombreDig_KeyPress);
            this.mtbDescripcionDig.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.mtbDescripcionDig_KeyPress);

            // Conectar evento del TabControl
            materialTabControl1.SelectedIndexChanged += tabControl_SelectedIndexChanged;

            // CONFIGURAR INTERFAZ POR ROL (SOLO UNA VEZ)
            ConfigurarInterfazPorRol();

            // Cargar datos iniciales
            CargarCitas();
            CargarTodosDoctoresDisponibilidad();
        }

        // ========================================================================
        // EVENTOS DEL TABCONTROL
        // ========================================================================

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (materialTabControl1.SelectedTab != null)
            {
                string tabName = materialTabControl1.SelectedTab.Text;

                if (tabName == "Citas Agendadas")
                {
                    CargarCitasAgendadas();
                }
                else if (tabName == "Disponibilidad")
                {
                    CargarTodosDoctoresDisponibilidad();
                }
                else if (tabName == "Gestion De Citas")
                {
                    CargarCitas();
                    LimpiarCamposGestion();
                    SetCampos(false);
                    btnAgendarGes.Enabled = true;
                    btnEditarGes.Enabled = true;
                    btnCancelarGes.Enabled = true;
                    btnGuardarGes.Enabled = false;
                }
                else if (tabName == "Diagnosticos")
                {
                    CargarDiagnosticos();
                    LimpiarCamposDiagnostico();
                    ActivarCamposDiagnostico(false);
                    btnGuardarDig.Enabled = false;
                    btnRegistrarDig.Enabled = true;
                    btnFiltrarDig.Enabled = true;
                    btnBuscarDig.Enabled = false;
                }
            }
        }

        // ========================================================================
        // SECCIÓN: DISPONIBILIDAD
        // ========================================================================

        private void CargarTodosDoctoresDisponibilidad()
        {
            try
            {
                var dt = Doctor.ObtenerTodos();

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No hay doctores en la base de datos.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                cbDoctorDis.DataSource = dt;
                cbDoctorDis.DisplayMember = "Nombre";
                cbDoctorDis.ValueMember = "IdDoctor";

                if (Rol == "Medico" && cbDoctorDis.Items.Count > 0)
                {
                    cbDoctorDis.SelectedValue = IdDoctor;
                    cbDoctorDis.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar doctores: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardarDis_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbDoctorDis.SelectedValue == null)
                {
                    MessageBox.Show("Seleccione un doctor.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                int idDoctor = Convert.ToInt32(cbDoctorDis.SelectedValue);

                DateTime fechaEntrada = dtpDiasEntraDis.Value.Date;
                DateTime fechaSalida = dtpDiasSaliDis.Value.Date;
                TimeSpan horaEntrada = dtpHoraEntDis.Value.TimeOfDay;
                TimeSpan horaSalida = dtpHoraSalDis.Value.TimeOfDay;

                if (fechaSalida < fechaEntrada)
                {
                    MessageBox.Show("La fecha de salida no puede ser menor a la fecha de entrada.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (horaSalida <= horaEntrada)
                {
                    MessageBox.Show("La hora de salida debe ser mayor a la hora de entrada.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string resultado = Disponibilidad.GuardarDisponibilidad(
                    idDoctor, fechaEntrada, fechaSalida, horaEntrada, horaSalida);

                if (resultado == "OK")
                {
                    MessageBox.Show("Disponibilidad guardada correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    dtpDiasEntraDis.Value = DateTime.Today;
                    dtpDiasSaliDis.Value = DateTime.Today;
                    dtpHoraEntDis.Value = DateTime.Today.AddHours(8);
                    dtpHoraSalDis.Value = DateTime.Today.AddHours(17);
                }
                else
                {
                    MessageBox.Show($"Error al guardar: {resultado}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ========================================================================
        // SECCIÓN: CITAS AGENDADAS
        // ========================================================================

        private void CargarCitasAgendadas()
        {
            dgvCitasAg.DataSource = GestionDeCitas.MostrarCitasAgendadas();
            FormatearGridCitasAg(dgvCitasAg);
        }

        private void FormatearGridCitasAg(DataGridView dgv)
        {
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
                MessageBox.Show("Error al buscar citas: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ========================================================================
        // SECCIÓN: DIAGNÓSTICOS
        // ========================================================================

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
            try
            {
                dgvDiagnosticos.DataSource = Diagnostico.MostrarDiagnosticos();
                FormatearGridDiagnosticos(dgvDiagnosticos);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar diagnósticos: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatearGridDiagnosticos(DataGridView dgv)
        {
            if (dgv.Columns.Contains("Fecha"))
            {
                dgv.Columns["Fecha"].DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
                dgv.Columns["Fecha"].Width = 150;
            }
            if (dgv.Columns.Contains("Paciente"))
            {
                dgv.Columns["Paciente"].Width = 150;
            }
            if (dgv.Columns.Contains("Telefono"))
            {
                dgv.Columns["Telefono"].Width = 120;
            }
            if (dgv.Columns.Contains("Descripcion"))
            {
                dgv.Columns["Descripcion"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
            if (dgv.Columns.Contains("IdDiagnostico"))
            {
                dgv.Columns["IdDiagnostico"].Width = 80;
            }

            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AllowUserToAddRows = false;
            dgv.ReadOnly = true;
        }

        private void btnRegistrarDig_Click(object sender, EventArgs e)
        {
            LimpiarCamposDiagnostico();
            ActivarCamposDiagnostico(true);

            btnGuardarDig.Enabled = true;
            btnRegistrarDig.Enabled = false;
            btnFiltrarDig.Enabled = false;

            tbNombreDig.Focus();
        }

        private void btnGuardarDig_Click(object sender, EventArgs e)
        {
            try
            {
                string nombre = tbNombreDig.Text.Trim();
                string telefono = mtbTelefonoDig.Text.Trim();
                string descripcion = mtbDescripcionDig.Text.Trim();
                DateTime fecha = dtpFechaDig.Value;

                if (string.IsNullOrEmpty(nombre))
                {
                    MessageBox.Show("Debe ingresar el nombre del paciente.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tbNombreDig.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(descripcion))
                {
                    MessageBox.Show("Debe ingresar una descripción del diagnóstico.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    mtbDescripcionDig.Focus();
                    return;
                }

                Diagnostico.GuardarDiagnostico(nombre, telefono, fecha, descripcion);

                MessageBox.Show("Diagnóstico guardado correctamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarDiagnosticos();
                LimpiarCamposDiagnostico();
                ActivarCamposDiagnostico(false);

                btnGuardarDig.Enabled = false;
                btnRegistrarDig.Enabled = true;
                btnFiltrarDig.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar diagnóstico: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnFiltrarDig_Click(object sender, EventArgs e)
        {
            tbNombreDig.Enabled = true;
            mtbTelefonoDig.Enabled = false;
            dtpFechaDig.Enabled = false;
            mtbDescripcionDig.Enabled = false;

            tbNombreDig.Clear();
            tbNombreDig.Focus();

            btnBuscarDig.Enabled = true;
            btnRegistrarDig.Enabled = false;
        }

        private void btnBuscarDig_Click(object sender, EventArgs e)
        {
            try
            {
                string nombre = tbNombreDig.Text.Trim();

                if (string.IsNullOrEmpty(nombre))
                {
                    CargarDiagnosticos();
                }
                else
                {
                    DataTable dt = Diagnostico.BuscarDiagnosticoPorNombre(nombre);
                    dgvDiagnosticos.DataSource = dt;
                    FormatearGridDiagnosticos(dgvDiagnosticos);

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show($"No se encontraron diagnósticos para '{nombre}'.",
                            "Sin resultados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                tbNombreDig.Clear();
                ActivarCamposDiagnostico(false);
                btnBuscarDig.Enabled = false;
                btnRegistrarDig.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvDiagnosticos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                DataGridViewRow row = dgvDiagnosticos.Rows[e.RowIndex];

                tbNombreDig.Text = row.Cells["Paciente"].Value.ToString();
                mtbTelefonoDig.Text = row.Cells["Telefono"].Value.ToString();
                dtpFechaDig.Value = Convert.ToDateTime(row.Cells["Fecha"].Value);
                mtbDescripcionDig.Text = row.Cells["Descripcion"].Value.ToString();

                ActivarCamposDiagnostico(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar diagnóstico: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRegistrarDig_Click_1(object sender, EventArgs e)
        {
            LimpiarCamposDiagnostico();
            ActivarCamposDiagnostico(true);

            btnGuardarDig.Enabled = true;
            btnRegistrarDig.Enabled = false;
            btnFiltrarDig.Enabled = false;

            tbNombreDig.Focus();
        }

        private void btnGuardarDig_Click_1(object sender, EventArgs e)
        {
            try
            {
                string nombre = tbNombreDig.Text.Trim();
                string telefono = mtbTelefonoDig.Text.Trim();
                string descripcion = mtbDescripcionDig.Text.Trim();
                DateTime fecha = dtpFechaDig.Value;

                if (string.IsNullOrEmpty(nombre))
                {
                    MessageBox.Show("Debe ingresar el nombre del paciente.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tbNombreDig.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(descripcion))
                {
                    MessageBox.Show("Debe ingresar una descripción del diagnóstico.", "Validación",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    mtbDescripcionDig.Focus();
                    return;
                }

                Diagnostico.GuardarDiagnostico(nombre, telefono, fecha, descripcion);

                MessageBox.Show("Diagnóstico guardado correctamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                CargarDiagnosticos();
                LimpiarCamposDiagnostico();
                ActivarCamposDiagnostico(false);

                btnGuardarDig.Enabled = false;
                btnRegistrarDig.Enabled = true;
                btnFiltrarDig.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar diagnóstico: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnFiltrarDig_Click_1(object sender, EventArgs e)
        {
            tbNombreDig.Enabled = true;
            mtbTelefonoDig.Enabled = false;
            dtpFechaDig.Enabled = false;
            mtbDescripcionDig.Enabled = false;

            tbNombreDig.Clear();
            tbNombreDig.Focus();

            btnBuscarDig.Enabled = true;
            btnRegistrarDig.Enabled = false;
        }

        private void btnBuscarDig_Click_1(object sender, EventArgs e)
        {
            try
            {
                string nombre = tbNombreDig.Text.Trim();

                if (string.IsNullOrEmpty(nombre))
                {
                    CargarDiagnosticos();
                }
                else
                {
                    DataTable dt = Diagnostico.BuscarDiagnosticoPorNombre(nombre);
                    dgvDiagnosticos.DataSource = dt;
                    FormatearGridDiagnosticos(dgvDiagnosticos);

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show($"No se encontraron diagnósticos para el paciente '{nombre}'.",
                            "Sin resultados", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                tbNombreDig.Clear();
                ActivarCamposDiagnostico(false);
                btnBuscarDig.Enabled = false;
                btnRegistrarDig.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al buscar: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvDiagnosticos_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                DataGridViewRow row = dgvDiagnosticos.Rows[e.RowIndex];

                tbNombreDig.Text = row.Cells["Paciente"].Value.ToString();
                mtbTelefonoDig.Text = row.Cells["Telefono"].Value.ToString();
                dtpFechaDig.Value = Convert.ToDateTime(row.Cells["Fecha"].Value);
                mtbDescripcionDig.Text = row.Cells["Descripcion"].Value.ToString();

                ActivarCamposDiagnostico(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar diagnóstico: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ========================================================================
        // SECCIÓN: GESTIÓN DE CITAS
        // ========================================================================

        private string ObtenerNombreDoctor(int idDoctor)
        {
            try
            {
                var dt = Doctor.ObtenerTodos();
                foreach (DataRow row in dt.Rows)
                {
                    if (Convert.ToInt32(row["IdDoctor"]) == idDoctor)
                    {
                        return row["Nombre"].ToString();
                    }
                }
                return "Doctor";
            }
            catch
            {
                return "Doctor";
            }
        }

        private void ConfigurarInterfazPorRol()
        {
            if (Rol == "Medico")
            {
                // Para MÉDICOS: Disponibilidad, Citas Agendadas y Diagnósticos
                foreach (TabPage tab in materialTabControl1.TabPages)
                {
                    string tabName = tab.Name.ToLower();
                    string tabText = tab.Text.ToLower();

                    // Habilitar tabs para médicos
                    if (tabName == "tabpage2" ||                          
                        tabText.Contains("disponibilidad") ||
                        tabText.Contains("citas agendadas") ||
                        tabText.Contains("diagnostico"))
                    {
                        tab.Enabled = true;
                    }
                    else
                    {
                        tab.Enabled = false;
                    }
                }

                // Seleccionar la primera pestaña habilitada
                materialTabControl1.SelectedTab = materialTabControl1.TabPages
                    .Cast<TabPage>()
                    .FirstOrDefault(t => t.Enabled);

                this.Text = $"Clinica SanRafael - Dr. {ObtenerNombreDoctor(IdDoctor)}";
            }
            else if (Rol == "Secretaria")
            {
                // Para SECRETARIAS: Todo excepto Disponibilidad
                foreach (TabPage tab in materialTabControl1.TabPages)
                {
                    string tabText = tab.Text.ToLower();

                    if (tabText.Contains("disponibilidad"))
                    {
                        tab.Enabled = false;
                    }
                    else
                    {
                        tab.Enabled = true;
                    }
                }

                this.Text = "Clinica SanRafael - Secretaría";
            }
            else
            {
                // Rol desconocido - deshabilitar todo por seguridad
                foreach (TabPage tab in materialTabControl1.TabPages)
                {
                    tab.Enabled = false;
                }

                MessageBox.Show("Rol no reconocido. Contacte al administrador.",
                    "Error de Permisos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CargarDoctores()
        {
            try
            {
                var dt = Doctor.ObtenerTodos();

                cbDoctorGes.DataSource = dt;
                cbDoctorGes.DisplayMember = "Nombre";
                cbDoctorGes.ValueMember = "IdDoctor";

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("No hay doctores registrados en el sistema.",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar doctores: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            dtFechaGes.Value = cita.Fecha.Date;
            dtpHoraGes.Value = DateTime.Today.Add(cita.Hora);

            CargarDoctores();

            if (cita.Doctor != null && cita.Doctor.IdDoctor != 0)
            {
                try
                {
                    cbDoctorGes.SelectedValue = cita.Doctor.IdDoctor;
                }
                catch
                {
                    MessageBox.Show($"Nota: El Dr. {cita.Doctor.Nombre} no está en la lista.",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

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
            cbDoctorGes.DataSource = null;
            cbDoctorGes.Items.Clear();
            dtFechaGes.Value = DateTime.Today;
            dtpHoraGes.Value = DateTime.Now;
            idCitaSeleccionada = 0;
            modoEditar = false;
        }

        private void CargarCitas()
        {
            dgvGestion.DataSource = GestionDeCitas.MostrarCitas();
            FormatearGridGestion(dgvGestion);
        }

        private void FormatearGridGestion(DataGridView dgv)
        {
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

        private void btnAgendarGes_Click(object sender, EventArgs e)
        {
            modoEditar = false;
            idCitaSeleccionada = 0;

            LimpiarCamposGestion();
            SetCampos(true);

            dtFechaGes.Value = DateTime.Today;
            dtpHoraGes.Value = DateTime.Now;

            CargarDoctores();

            btnEditarGes.Enabled = false;
            btnCancelarGes.Enabled = false;
            btnCancelarOpeGes.Enabled = true;
            btnGuardarGes.Enabled = true;
            btnAgendarGes.Enabled = false;

            tbNombreGes.Focus();
        }

        private void btnEditarGes_Click(object sender, EventArgs e)
        {
            if (dgvGestion.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione una cita para editar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            idCitaSeleccionada = Convert.ToInt32(dgvGestion.SelectedRows[0].Cells["IdCita"].Value);

            SetCampos(true);
            RecargarCita(idCitaSeleccionada);

            modoEditar = true;

            btnGuardarGes.Enabled = true;
            btnEditarGes.Enabled = false;
            btnCancelarGes.Enabled = false;
            btnCancelarOpeGes.Enabled = true;
            btnAgendarGes.Enabled = false;
        }

        private void btnGuardarGes_Click(object sender, EventArgs e)
        {
            try
            {
                string nombrePaciente = tbNombreGes.Text.Trim();
                if (string.IsNullOrEmpty(nombrePaciente))
                {
                    MessageBox.Show("Ingrese el nombre del paciente.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tbNombreGes.Focus();
                    return;
                }

                int idPaciente = Paciente.ObtenerIdPacientePorNombre(nombrePaciente);
                if (idPaciente == 0)
                {
                    DialogResult result = MessageBox.Show(
                        $"El paciente '{nombrePaciente}' no existe. ¿Desea crearlo?",
                        "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        idPaciente = Paciente.CrearPaciente(nombrePaciente);
                    }
                    else
                    {
                        return;
                    }
                }

                if (cbDoctorGes.SelectedValue == null)
                {
                    MessageBox.Show("Seleccione un doctor.", "Aviso",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cbDoctorGes.Focus();
                    return;
                }
                int idDoctor = Convert.ToInt32(cbDoctorGes.SelectedValue);

                DateTime fecha = dtFechaGes.Value.Date;
                TimeSpan hora = dtpHoraGes.Value.TimeOfDay;
                string motivo = tbMotivoGes.Text.Trim();

                if (modoEditar && idCitaSeleccionada > 0)
                {
                    GestionDeCitas.EditarCita(idCitaSeleccionada, idPaciente, idDoctor, fecha, hora, motivo);
                    MessageBox.Show("Cita actualizada correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    GestionDeCitas.AgendarCita(idPaciente, idDoctor, fecha, hora, motivo);
                    MessageBox.Show("Cita agendada correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

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
            catch (HorarioNoDisponibleException ex)
            {
                MessageBox.Show(ex.Message, "Horario no disponible",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (CitaDuplicadaException ex)
            {
                MessageBox.Show(ex.Message, "Conflicto de horario",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar la cita: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelarGes_Click(object sender, EventArgs e)
        {
            if (dgvGestion.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione una cita para cancelar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idCita = Convert.ToInt32(dgvGestion.SelectedRows[0].Cells["IdCita"].Value);
            string estadoActual = dgvGestion.SelectedRows[0].Cells["Estado"].Value.ToString();

            if (estadoActual == "Cancelada")
            {
                MessageBox.Show("Esta cita ya está cancelada.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult dr = MessageBox.Show(
                "¿Está seguro que desea cancelar esta cita?",
                "Confirmar Cancelación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (dr == DialogResult.Yes)
            {
                try
                {
                    GestionDeCitas.CancelarCita(idCita);
                    MessageBox.Show("Cita cancelada correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    CargarCitas();
                    LimpiarCamposGestion();
                    SetCampos(false);

                    btnAgendarGes.Enabled = true;
                    btnEditarGes.Enabled = true;
                    btnCancelarGes.Enabled = true;
                    btnGuardarGes.Enabled = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cancelar la cita: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void dgvGestion_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            try
            {
                idCitaSeleccionada = Convert.ToInt32(dgvGestion.Rows[e.RowIndex].Cells["IdCita"].Value);
                RecargarCita(idCitaSeleccionada);

                SetCampos(false);
                btnAgendarGes.Enabled = true;
                btnEditarGes.Enabled = true;
                btnCancelarGes.Enabled = true;
                btnGuardarGes.Enabled = false;
                modoEditar = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al seleccionar la cita: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelarGes_Click_1(object sender, EventArgs e)
        {
            if (dgvGestion.SelectedRows.Count == 0)
            {
                MessageBox.Show("Seleccione una cita para cancelar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int idCita = Convert.ToInt32(dgvGestion.SelectedRows[0].Cells["IdCita"].Value);
            string estadoActual = dgvGestion.SelectedRows[0].Cells["Estado"].Value.ToString();

            if (estadoActual == "Cancelada")
            {
                MessageBox.Show("Esta cita ya está cancelada.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult result = MessageBox.Show(
                "¿Está seguro que desea cancelar esta cita?\n\n" +
                "La cita cambiará su estado a 'Cancelada'.",
                "Confirmar Cancelación de Cita",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                try
                {
                    GestionDeCitas.CancelarCita(idCita);

                    MessageBox.Show("Cita cancelada correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    CargarCitas();
                    LimpiarCamposGestion();
                    SetCampos(false);

                    idCitaSeleccionada = 0;
                    modoEditar = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cancelar la cita: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnCancelarOpeGes_Click(object sender, EventArgs e)
        {
            LimpiarCamposGestion();
            SetCampos(false);

            btnAgendarGes.Enabled = true;
            btnEditarGes.Enabled = true;
            btnCancelarGes.Enabled = true;
            btnCancelarOpeGes.Enabled = false;
            btnGuardarGes.Enabled = false;

            modoEditar = false;
            idCitaSeleccionada = 0;
        }

        // ========================================================================
        // VALIDACIONES KEYPRESS
        // ========================================================================

        private void tbNombreGes_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true;
            }
        }

        private void tbMotivoGes_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true;
            }
        }

        private void tbNombreDig_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true;
            }
        }

        private void mtbDescripcionDig_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != ' ')
            {
                e.Handled = true;
            }
        }
    }
}