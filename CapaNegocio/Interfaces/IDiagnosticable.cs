using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapaNegocio.Clases;

namespace CapaNegocio.Interfaces
{
    internal interface IDiagnosticable
    {
        Task<bool> RegistrarDiagnostico(Diagnostico diagnostico);
        Task<List<Diagnostico>> ObtenerHistorial(int idPaciente);
    }
}
