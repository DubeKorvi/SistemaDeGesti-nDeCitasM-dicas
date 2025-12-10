using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaNegocio.Excepciones;
using CapaNegocio.Interfaces;


namespace CapaNegocio.Clases
{
    public class SistemaGestionCitas: IAgendable, IDiagnosticable
    {
        // Diccionario para manejar la agenda de citas por doctor
        // Clave: ID del doctor
        // Valor: Lista de citas del doctor
        private readonly Dictionary<int, List<Cita>> _agenda = new Dictionary<int, List<Cita>>();

        // Lista de doctores registrados en el sistema
        private readonly List<Doctor> _doctores = new List<Doctor>();

        // Lista de pacientes registrados en el sistema
        private readonly List<Paciente> _pacientes = new List<Paciente>();

        // Instancia del manejador de eventos del sistema
        private readonly EventosSistema _eventos = new EventosSistema();

        // Lista de seguros médicos disponibles
        private readonly string[] _segurosMedicos = { "SeNaSa", "ARS Humano", "ARS Palic", "ARS Universal", "Particular" };


        /// Constructor de la clase SistemaGestionCitas.
        /// Inicializa los manejadores de eventos del sistema.
        public SistemaGestionCitas()
        {
            // Suscripción a eventos con expresiones lambda para manejo de notificaciones
            _eventos.CitaAgendada += (sender, e) =>
                Console.WriteLine($"Notificación: {e} - {DateTime.Now:g}");

            _eventos.CitaCancelada += (sender, e) =>
                Console.WriteLine($"Notificación: {e} - {DateTime.Now:g}");

            _eventos.PacienteAtendido += (sender, e) =>
                Console.WriteLine($"Notificación: {e} - {DateTime.Now:g}");
        }

        // Implementación de IAgendable

    
        public async Task<bool> AgendarCita(Cita cita)
        {
            // Validar disponibilidad del doctor en la fecha y hora solicitadas
            if (!await VerificarDisponibilidad(cita.Doctor.IdDoctor, cita.Fecha, cita.Hora))
            {
                throw new HorarioNoDisponibleException("El doctor no está disponible en el horario seleccionado.");
            }

            // Verificar si el paciente ya tiene una cita en el mismo horario
            if (_agenda.Values.Any(lista =>
                lista.Any(c => c.Paciente.IdPaciente == cita.Paciente.IdPaciente &&
                              c.Fecha == cita.Fecha &&
                              c.Hora == cita.Hora)))
            {
                throw new CitaDuplicadaException("El paciente ya tiene una cita programada en ese horario.");
            }

            // Inicializar la lista de citas para el doctor si no existe
            if (!_agenda.ContainsKey(cita.Doctor.IdDoctor))
            {
                _agenda[cita.Doctor.IdDoctor] = new List<Cita>();
            }

            // Agregar la cita a la agenda del doctor
            _agenda[cita.Doctor.IdDoctor].Add(cita);

            // Notificar que se ha agendado una nueva cita
            _eventos.OnCitaAgendada($"Cita agendada para {cita.Paciente.Nombre} con el Dr. {cita.Doctor.Nombre}");

            return true;
        }


       
        public async Task<bool> CancelarCita(int idCita)
        {
            // Buscar la cita en la agenda
            foreach (var listaCitas in _agenda.Values)
            {
                var cita = listaCitas.FirstOrDefault(c => c.IdCita == idCita);
                if (cita != null)
                {
                    // Marcar la cita como cancelada
                    cita.Estado = "Cancelada";

                    // Notificar la cancelación
                    _eventos.OnCitaCancelada($"Cita #{idCita} cancelada");

                    await Task.Delay(100); // Simular operación asíncrona
                    return true;
                }
            }

            return false;
        }


        //Implementación de IDiagnosticable


       
        public async Task<bool> RegistrarDiagnostico(Diagnostico diagnostico)
        {
            try
            {
                // Validar que la cita existe
                var cita = _agenda.Values
                    .SelectMany(lista => lista)
                    .FirstOrDefault(c => c.IdCita == diagnostico.Cita.IdCita);

                if (cita == null)
                {
                    return false;
                }

                // Marcar la cita como atendida
                cita.Estado = "Atendida";

                // Notificar que se ha atendido al paciente
                _eventos.OnPacienteAtendido(
                    $"Paciente {cita.Paciente.Nombre} atendido por Dr. {cita.Doctor.Nombre}");

                await Task.Delay(100); // Simular operación asíncrona
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

      
       
        public async Task<List<Diagnostico>> ObtenerHistorial(int idPaciente)
        {
            // En una implementación real, esto consultaría a la base de datos
            await Task.Delay(100); // Simular operación asíncrona
            return new List<Diagnostico>();
        }


        // Métodos auxiliares

       
        private async Task<bool> VerificarDisponibilidad(int idDoctor, DateTime fecha, TimeSpan hora)
        {
            // En una implementación real, esto verificaría en la base de datos
            // si el doctor tiene disponibilidad en la fecha y hora solicitadas

            // Simulamos una verificación asíncrona
            await Task.Delay(50);

            // Verificar si el doctor tiene citas en el mismo horario
            if (_agenda.ContainsKey(idDoctor))
            {
                return !_agenda[idDoctor].Any(c =>
                    c.Fecha == fecha.Date &&
                    c.Hora.Hours == hora.Hours &&
                    c.Estado != "Cancelada");
            }

            return true; // Si no tiene citas, está disponible
        }


         //Métodos para cargar datos

      
        public async Task<List<Doctor>> ObtenerDoctores()
        {
            // En una implementación real, esto consultaría a la base de datos
            await Task.Delay(100); // Simular operación asíncrona
            return _doctores;
        }

        
        public async Task<List<Cita>> ObtenerCitasPorFecha(DateTime fecha)
        {
            // En una implementación real, esto consultaría a la base de datos
            await Task.Delay(100); // Simular operación asíncrona

            return _agenda.Values
                .SelectMany(lista => lista)
                .Where(c => c.Fecha.Date == fecha.Date)
                .ToList();
        }
    }
}
