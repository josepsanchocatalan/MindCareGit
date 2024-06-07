using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindCare.POJO
{
    public class UsuariosPOJO
    {
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Correo { get; set; }
        public string Rol {  get; set; }


        //Constructores 

        public UsuariosPOJO() { }

        public UsuariosPOJO(string nombre, string apellidos, string correo, string rol)
        {
            Nombre = nombre;
            Apellidos = apellidos;
            Correo = correo;
            Rol = rol;
        }
    }
}
