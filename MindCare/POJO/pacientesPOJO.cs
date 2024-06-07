using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindCare
{
    internal class pacientesPOJO
    {
        //Atributos
        public string NIF { get; set; }
        public string Nombre { get; set; }
        public string Apellidos {  get; set; }
        public string Correo { get; set; }
        public string Direccion {  get; set; }
        public string Poblacion { get; set; }
        public string Provincia { get; set; }
        public string Pais {  get; set; }
        public int Edad {  get; set; }
        public string Telefono {  get; set; }
        public string Telefono2 {  get; set; }
        public string ProfesionalActual {  get; set; }


        //Constructores
        public pacientesPOJO() { }

        public pacientesPOJO(string nif, string nombre, string apellidos, string correo, string direccion, string poblacion, string provincia, 
                             string pais, int edad, string telefono, string telefono2, string profesionalActual)
        {
            NIF = nif;
            Nombre = nombre;
            Apellidos = apellidos;
            Correo = correo;
            Direccion = direccion;
            Poblacion = poblacion;
            Provincia = provincia;
            Pais = pais;
            Edad = edad;
            Telefono = telefono;
            Telefono2 = telefono2;
            ProfesionalActual = profesionalActual;
        }


    }
}
