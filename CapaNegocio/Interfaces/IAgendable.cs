using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaNegocio.Clases;

namespace CapaNegocio.Interfaces
{
    internal interface IAgendable
    {
        Task<bool> AgendarCita(Cita cita);
        Task<bool> CancelarCita(int idCita);
    }
}
