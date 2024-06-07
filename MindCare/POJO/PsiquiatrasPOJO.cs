using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindCare.POJO
{
    internal class PsiquiatrasPOJO
    {
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public int Edad { get; set; }
        public string Direccion { get; set; }
        public string Poblacion { get; set; }
        public string Provincia { get; set; }
        public string Pais { get; set; }
        public string NIF { get; set; }
        public string Correo { get; set; }
        public string NumeroLicencia { get; set; }
        public string Especialidad { get; set; }
        public string HorariosAtencion { get; set; }
        public decimal Tarifas { get; set; }
        public string Telefono1 { get; set; }
        public string Telefono2 { get; set; }
        public string DireccionConsultorio { get; set; }


        //Constructores 

        public PsiquiatrasPOJO() { }

        public PsiquiatrasPOJO(string nombre, string apellidos, int edad, string direccion, string poblacion, string provincia, string pais, string nif,
                            string correo, string numeroLicencia, string especialidad, string horariosAtencion, decimal tarifas,
                            string telefono1, string telefono2, string direccionConsultorio)
        {
            Nombre = nombre;
            Apellidos = apellidos;
            Edad = edad;
            Direccion = direccion;
            Poblacion = poblacion;
            Provincia = provincia;
            Pais = pais;
            NIF = nif;
            Correo = correo;
            NumeroLicencia = numeroLicencia;
            Especialidad = especialidad;
            HorariosAtencion = horariosAtencion;
            Tarifas = tarifas;
            Telefono1 = telefono1;
            Telefono2 = telefono2;
            DireccionConsultorio = direccionConsultorio;
        }

    }
}
