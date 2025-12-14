using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio.Clases
{
    // Clase encargada de manejar los eventos principales del sistema de gestión de citas.
    // Implementa el patrón de diseño Observer para notificar a los suscriptores sobre eventos importantes.
    public class EventosSistema
    {
        // Evento que se dispara cuando se agenda una nueva cita
        public event EventHandler<string> CitaAgendada;

        // Evento que se dispara cuando se cancela una cita
        public event EventHandler<string> CitaCancelada;

        // Evento que se dispara cuando se atiende a un paciente
        public event EventHandler<string> PacienteAtendido;

        // Método para disparar el evento CitaAgendada
        public void OnCitaAgendada(string mensaje) => CitaAgendada?.Invoke(this, mensaje);

        // Método para disparar el evento CitaCancelada
        public void OnCitaCancelada(string mensaje) => CitaCancelada?.Invoke(this, mensaje);

        // Método para disparar el evento PacienteAtendid
        public void OnPacienteAtendido(string mensaje) => PacienteAtendido?.Invoke(this, mensaje);
    }
}
