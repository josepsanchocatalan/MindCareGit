using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MindCare
{
    public partial class FormBuscarPaciente : Form
    {
        private FirebaseClient firebaseClient;

        // Declarar una variable para almacenar el valor de la celda seleccionada
        private string correoSeleccionado = "";

        public event EventHandler<string> correoPacienteBusqueda;

        public FormBuscarPaciente()
        {
            InitializeComponent();
            InitializeFirebaseClient();

            CargarDatos();

            disenyoInicial();
        }

        private void disenyoInicial()
        {
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

        }

        private void InitializeFirebaseClient()
        {
            firebaseClient = new FirebaseClient("https://mindcare-54560-default-rtdb.firebaseio.com/");
        }

        private async void CargarDatos()
        {
            try
            {
                var pacientes = await firebaseClient.Child("pacientes").OnceAsync<Dictionary<string, object>>();

                dataGridViewResultados.Rows.Clear();

                foreach (var paciente in pacientes)
                {
                    var pacienteData = paciente.Object;
                    dataGridViewResultados.Rows.Add(pacienteData["NIF"], pacienteData["Nombre"], pacienteData["Apellidos"], pacienteData["Edad"], pacienteData["Correo"]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridViewResultados_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var cell = dataGridViewResultados.Rows[e.RowIndex].Cells["Correo"];
                if (cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()))
                {
                    correoSeleccionado = cell.Value.ToString();

                    btnAceptar.Enabled = true;
                }
                else
                {
                    MessageBox.Show("El valor de correo está vacío o nulo.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            // Enviar el valor de la celda al formulario principal a través del evento correoPacienteBusqueda
            OnCorreoPacienteBusqueda(correoSeleccionado);
            this.Close();
        }

        // Declarar un método para invocar el evento correoPacienteBusqueda
        protected virtual void OnCorreoPacienteBusqueda(string correo)
        {
            correoPacienteBusqueda?.Invoke(this, correo);
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
