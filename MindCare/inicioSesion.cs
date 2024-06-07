using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MindCare
{
    public partial class inicioSesion : Form
    {
        private static readonly HttpClient client = new HttpClient();
        private const string API_KEY = "AIzaSyBsDg_yZt1KKQSxvi5aJ_m9Fsepdjc2szQ";

        public static string email { get; private set; }

        public inicioSesion()
        {
            InitializeComponent();
            disenyoApp();
        }

        private void disenyoApp()
        {
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;
            eventos();
            inicio();
        }

        private void eventos()
        {
            txtPassword.KeyPress += txtPassword_KeyPress;
        }

        private void inicio()
        {
            txtPassword.PasswordChar = '*';
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnEntrar.PerformClick();

                txtEmail.Focus();
            }
        }

        private async Task SignIn(string email, string password)
        {
            try
            {
                var content = new StringContent($"{{\"email\":\"{email}\",\"password\":\"{password}\",\"returnSecureToken\":true}}", Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={API_KEY}", content);

                if (response.IsSuccessStatusCode)
                {
                    Form1 formularioPrincipal = new Form1();
                    formularioPrincipal.Show();

                    this.Hide();
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    MessageBox.Show("El usuario o contraseña no son correctos", "¡Upppss!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private async void btnEntrar_Click(object sender, EventArgs e)
        {
            email = txtEmail.Text;
            string password = txtPassword.Text;

            await SignIn(email, password);
        }
    }
}
