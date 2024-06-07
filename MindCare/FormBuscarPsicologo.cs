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
    public partial class FormBuscarPsicologo : Form
    {
        private FirebaseClient firebaseClient;

        private string licenciaSeleccionada = "";

        public event EventHandler<string> licenciaPsicologoBusqueda;

        public FormBuscarPsicologo()
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
                var psicologos = await firebaseClient.Child("psicologos").OnceAsync<Dictionary<string, object>>();

                dataGridViewResultados.Rows.Clear();

                foreach (var psicologo in psicologos)
                {
                    var psicologoData = psicologo.Object;
                    dataGridViewResultados.Rows.Add(psicologoData["NumeroLicencia"], psicologoData["Nombre"], psicologoData["Apellidos"], psicologoData["DireccionConsultorio"], psicologoData["Correo"]);
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
            OnLicenciaPsicologoBusqueda(licenciaSeleccionada);
            this.Close();
        }

        protected virtual void OnLicenciaPsicologoBusqueda(string licencia)
        {
            licenciaPsicologoBusqueda?.Invoke(this, licencia);
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

