using System;
using System.Windows.Forms;

namespace MindCare
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // Iniciar la aplicación con la instancia de FirestoreDb
            ApplicationConfiguration.Initialize();
            Application.Run(new inicioSesion());
        }
    }
}
