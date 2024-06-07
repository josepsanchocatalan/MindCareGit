using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindCare.POJO
{
    public class ProfesionalPOJO
    {
        public string NumeroLicencia {  get; set; }
        public string Nombre {  get; set; }
        public string Apellidos { get; set; }
        public string Correo {  get; set; }

        public ProfesionalPOJO() { }
        
        public ProfesionalPOJO(string numeroLicencia, string nombre, string apellidos, string correo)
        {
            NumeroLicencia = numeroLicencia;
            Nombre = nombre;
            Apellidos = apellidos;
            Correo = correo;
        }

    }
}
