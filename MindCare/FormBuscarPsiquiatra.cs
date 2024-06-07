using Firebase.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MindCare
{
    public partial class FormBuscarPsiquiatra : Form
    {
        private FirebaseClient firebaseClient;

        // Declarar una variable para almacenar el valor de la celda seleccionada
        private string licenciaSeleccionada = "";

        public event EventHandler<string> licenciaPsiquiatraBusqueda;

        public FormBuscarPsiquiatra()
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
                var psiquiatras = await firebaseClient.Child("psiquiatras").OnceAsync<Dictionary<string, object>>();

                dataGridViewResultados.Rows.Clear();

                foreach (var psiquiatra in psiquiatras)
                {
                    var psiquiatraData = psiquiatra.Object;
                    dataGridViewResultados.Rows.Add(psiquiatraData["NumeroLicencia"], psiquiatraData["Nombre"], psiquiatraData["Apellidos"], psiquiatraData["DireccionConsultorio"], psiquiatraData["Correo"]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message, "¡Nos hemos topado con una barricada!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridViewResultados_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var cell = dataGridViewResultados.Rows[e.RowIndex].Cells["NumeroLicencia"];
                if (cell.Value != null && !string.IsNullOrEmpty(cell.Value.ToString()))
                {
                    licenciaSeleccionada = cell.Value.ToString();
                    MessageBox.Show($"Valor de NumeroLicencia: {licenciaSeleccionada}", "Valor de la celda", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnAceptar.Enabled = true;
                }
                else
                {
                    MessageBox.Show("El valor de la licencia está vacío o nulo.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }


        private void btnAceptar_Click(object sender, EventArgs e)
        {
            OnLicenciaPsiquiatraBusqueda(licenciaSeleccionada);
            this.Close();
        }

        // Declarar un método para invocar el evento correoPacienteBusqueda
        protected virtual void OnLicenciaPsiquiatraBusqueda(string licencia)
        {
            licenciaPsiquiatraBusqueda?.Invoke(this, licencia);
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

