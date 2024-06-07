using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindCare.POJO
{
    public class historialPacientesPOJO
    {

        public string NIF { get; set; }
        public string LicenciaProfesionalActual { get; set; }
        public string MotivoConsulta { get; set; }
        public string TomaMedicamentos { get; set; }
        public string ObersvacionesTerapeuta { get; set; }
        public string DuracionSesion { get; set; }
        public string ResumenSesion { get; set; }
        public string ConsentimientoInformativo { get; set; }

        public historialPacientesPOJO() { }

        public historialPacientesPOJO(string nif, string licenciaprofesionalactual, string motivoconsulta, string tomamedicamentos, string observacionesterapeuta, string duracionsesion, string resumensesion, string consentimientoinformativo)
        {
            NIF = nif;
            LicenciaProfesionalActual = licenciaprofesionalactual;
            MotivoConsulta = motivoconsulta;
            TomaMedicamentos = tomamedicamentos;
            ObersvacionesTerapeuta = observacionesterapeuta;
            DuracionSesion = duracionsesion;
            ResumenSesion = resumensesion;
            ConsentimientoInformativo = consentimientoinformativo;

        }

    }
}
