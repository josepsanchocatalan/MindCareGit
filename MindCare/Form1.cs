using Firebase.Database;
using Firebase.Auth; // Agrega esta directiva para usar Firebase Authentication
using Firebase.Database.Query;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Firebase.Auth.Providers;
using System.Text;
using MindCare.POJO;
using FirebaseAdmin.Messaging;
using static System.Runtime.InteropServices.JavaScript.JSType;
using String = System.String;
using System.Net;
using System.Text.RegularExpressions;
using Action = FirebaseAdmin.Messaging.Action;
using Newtonsoft.Json.Linq;
using System.Drawing;


namespace MindCare
{
    public partial class Form1 : Form
    {

        //Tabpages
        private TabPage tabPPacientes;
        private TabPage tabPHistorialPacientes;
        private TabPage tabPPsicologos;
        private TabPage tabPPsiquiatras;
        private TabPage tabPChat;

        //

        private Firebase.Database.FirebaseClient firebaseClient;
        private FirebaseAuthProvider authProvider; 

        HashSet<string> subscribedChats = new HashSet<string>();
        private Dictionary<string, IDisposable> suscripciones = new Dictionary<string, IDisposable>();


        private static readonly HttpClient client = new HttpClient();
        private const string API_KEY = "AIzaSyBsDg_yZt1KKQSxvi5aJ_m9Fsepdjc2szQ";

        public string CorreoUsuarioSeleccionado { get; set; }
        public string licenciaPsicologoSeleccionado { get; set; }
        public string licenciaPsiquiatraSeleccionado { get; set; }

        //Atributos
        private string accionPacientes = "";
        private string accionPsicologos = "";
        private string accionPsiquiatras = "";
        private bool crearpaciente = false;
        private bool crearpsicologo = false;
        private bool crearpsiquiatra = false;
        private string correoUsuarioActual = "";
        private string currentChatId;
        private string nombreChatActual;
        private bool cerrarsesion = false;

        public Form1()
        {
            InitializeComponent();
            InitializeFirebaseClient();

            this.FormClosed += new FormClosedEventHandler(Form1_FormClosed);

            bloquearTextBoxPacientes();
            bloquearTextBoxPsicologo();
            bloquearTextBoxPsiquiatras();

            botonesInicioPaciente();
            botonesInicioPsicologo();
            botonesInicioPsiquiatra();

            usuarioActualLabel();
            treeviewinicial();
            disenyoApp();

            limpiarLabelsPacientesPaciente();
            limpiarLabelsPsicologo();
            limpiarLabelsPsiquiatra();

            ListBoxChatsRecientes();

            eventos();
            tabPages();

        }


        private void tabPages()
        {
            tabPPacientes = tabPagePacientes;
            tabControlVentanas.TabPages.Remove(tabPPacientes);

            tabPHistorialPacientes = tabPageHistorialPacientes;
            tabControlPacientes.TabPages.Remove(tabPHistorialPacientes);

            tabPPsicologos = tabPagePsicologos;
            tabControlVentanas.TabPages.Remove(tabPPsicologos);

            tabPPsiquiatras = tabPagePsiquiatras;
            tabControlVentanas.TabPages.Remove(tabPPsiquiatras);

            tabPChat = tabPageChat;
            tabControlVentanas.TabPages.Remove(tabPChat);

        }

        private void eventos()
        {
            txtMessage.KeyPress += txtMessage_KeyPress;
        }

        //Funciones de utilidad
        //Inicializar FirebaseClient
        private void InitializeFirebaseClient()
        {
            firebaseClient = new Firebase.Database.FirebaseClient("https://mindcare-54560-default-rtdb.firebaseio.com/");
        }

        //Diseño de inicialización de form1
        private void disenyoApp()
        {
            this.MaximizeBox = false;
            this.MinimizeBox = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;
        }


        //Validador de NIF
        private bool ValidarDNI(String nif)
        {
            if (string.IsNullOrEmpty(nif))
            {
                return false;
            }

            nif = nif.Trim().ToUpper();

            var regex = new Regex(@"^\d{8}[A-Z]$");
            if (!regex.IsMatch(nif))
            {
                return false;
            }

            var numero = int.Parse(nif.Substring(0, 8));
            var letra = nif[8];

            var letras = "TRWAGMYFPDXBNJZSQVHLCKE";
            var letraCalculada = letras[numero % 23];

            return letra == letraCalculada;
        }




        //Limpiar los labels de pacientes
        private void limpiarLabelsPacientesPaciente()
        {
            labelDNIPaciente.Text = "";
            labelNombrePaciente.Text = "";
            labelApellidosPaciente.Text = "";
            labelCorreoPaciente.Text = "";
            labelDireccionPaciente.Text = "";
            labelPoblacionPaciente.Text = "";
            labelProvinciaPaciente.Text = "";
            labelPaisPaciente.Text = "";
            labelEdadPaciente.Text = "";
            labelTelefonoPaciente.Text = "";
            labelTelefono2Paciente.Text = "";
        }

        //Limpiar los campos de pacientes
        private void limpiarCamposPacientesPaciente()
        {
            textBoxDNI.Text = String.Empty;
            textBoxNombre.Text = String.Empty;
            textBoxApellidos.Text = String.Empty;
            textBoxEmail.Text = String.Empty;
            textBoxDireccion.Text = String.Empty;
            textBoxPoblacion.Text = String.Empty;
            textBoxProvincia.Text = String.Empty;
            textBoxPais.Text = String.Empty;
            textBoxEdad.Text = String.Empty;
            textBoxTelefono.Text = String.Empty;
            textBoxTelefono2.Text = String.Empty;
        }

        //Bloquear textbox de pacientes
        private void bloquearTextBoxPacientes()
        {
            textBoxDNI.Enabled = false;
            textBoxApellidos.Enabled = false;
            textBoxNombre.Enabled = false;
            textBoxEmail.Enabled = false;
            textBoxDireccion.Enabled = false;
            textBoxPoblacion.Enabled = false;
            textBoxProvincia.Enabled = false;
            textBoxPais.Enabled = false;
            textBoxEdad.Enabled = false;
            textBoxTelefono.Enabled = false;
            textBoxTelefono2.Enabled = false;
        }

        private void desbloquearTextBoxPacientes()
        {
            textBoxDNI.Enabled = true;
            textBoxApellidos.Enabled = true;
            textBoxNombre.Enabled = true;
            textBoxEmail.Enabled = true;
            textBoxDireccion.Enabled = true;
            textBoxPoblacion.Enabled = true;
            textBoxProvincia.Enabled = true;
            textBoxPais.Enabled = true;
            textBoxEdad.Enabled = true;
            textBoxTelefono.Enabled = true;
            textBoxTelefono2.Enabled = true;
        }

        //Recuperar datos del usuario actual
        private async void usuarioActualLabel()
        {
            correoUsuarioActual = inicioSesion.email;

            await CargarDatosUsuario(correoUsuarioActual);
        }

        private async Task CargarDatosUsuario(string email)
        {
            try
            {
                var usuarios = await firebaseClient.Child("usuarios").OnceAsync<UsuariosPOJO>();
                var usuario = usuarios.FirstOrDefault(u => u.Object.Correo.Equals(email, StringComparison.OrdinalIgnoreCase));

                if (usuario != null)
                {
                    labelDatosUsuario.Text = $"{usuario.Object.Rol} | {usuario.Object.Apellidos}, {usuario.Object.Nombre}";
                }
                else
                {
                    labelDatosUsuario.Text = "Error al mostrar los datos del usuario";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message);
            }
        }

        private async Task<ProfesionalPOJO> ObtenerProfesionalPorCorreo(string correo)
        {
            try
            {
                var profesionales = await firebaseClient.Child("profesionales")
                    .OnceAsync<ProfesionalPOJO>();

                return profesionales.FirstOrDefault(p => p.Object.Correo == correo)?.Object;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener el profesional: " + ex.Message, "¡Nos hemos topado con una barricada!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }




        //Botones
        private void botonesInicioPaciente()
        {
            btnCerrarPagePacientes.Enabled = true;
            btnCrearPaciente.Enabled = true;
            btnModificarPaciente.Enabled = false;
            btnGuardarPaciente.Enabled = false;
            btnCancelarPaciente.Enabled = false;
            btnEliminarPaciente.Enabled = false;
            btnBuscarPaciente.Enabled = true;
            btnLimpiarCamposPacientes.Enabled = false;
            btnReportesPaciente.Enabled = false;
        }
        private void botonesInicioPsiquiatra()
        {
            btnCerrarPagePsiquiatras.Enabled = true;
            btnCrearPsiquiatraToolStrip.Enabled = true;
            btnModificarPsiquiatraToolStrip.Enabled = false;
            btnGuardarPsiquiatraToolStrip.Enabled = false;
            btnCancelarPsiquiatraToolStrip.Enabled = false;
            btnEliminarPsiquiatra.Enabled = false;
            btnBuscarPsiquiatraToolStrip.Enabled = true;
            btnLimpiarCamposPsiquiatra.Enabled = false;
            btnReportesPsiquiatraToolStrip.Enabled = false;
        }

        //Boton guardar paciente
        private async void btnGuardarPaciente_ClickAsync(object sender, EventArgs e)
        {

            correoUsuarioActual = inicioSesion.email;

            validarPaciente();

            if (accionPacientes.Equals("Crear"))
            {
                
                if (accionPacOK == true)
                {
                    await crearPaciente();

                    if (crearpaciente.Equals(true))
                    {
                        await registrarPaciente(textBoxEmail.Text, textBoxDNI.Text, textBoxNombre.Text, textBoxApellidos.Text);
                        botonesInicioPaciente();
                        accionPacientes = "";
                    }
                } else if (accionPacOK == false)
                {
                    accionPacientes = "Crear";
                }
                
            }
            else if (accionPacientes.Equals("Modificar"))
            {
                if (accionPacOK == true)
                {
                    await ActualizarPaciente(textBoxDNI.Text, textBoxNombre.Text, textBoxApellidos.Text, textBoxEmail.Text, textBoxDireccion.Text, textBoxPoblacion.Text, textBoxProvincia.Text, textBoxPais.Text, int.Parse(textBoxEdad.Text), textBoxTelefono.Text, textBoxTelefono2.Text);
                } else if (accionPacOK == false) 
                {
                    accionPacientes = "Modificar";
                }
            }
        }

        //Boton crearpaciente
        private void btnCrearPaciente_Click(object sender, EventArgs e)
        {
            textBoxDNI.Enabled = true;
            textBoxDNI.Focus();
            accionPacientes = "Crear";
            btnCrearPaciente.Enabled = false;
            btnGuardarPaciente.Enabled = true;
            btnCancelarPaciente.Enabled = true;
            btnLimpiarCamposPacientes.Enabled = true;
        }

        //Boton buscarpaciente
        private void btnBuscarPaciente_Click(object sender, EventArgs e)
        {
            FormBuscarPaciente buscar = new FormBuscarPaciente();

            buscar.correoPacienteBusqueda += FormBuscar_correoPacienteBusqueda;
            buscar.ShowDialog();
        }

        //Evento que le pasa el paciente seleccionado al formulario
        private void FormBuscar_correoPacienteBusqueda(object sender, string correo)
        {
            CorreoUsuarioSeleccionado = correo;

            MostrarDatosPaciente(CorreoUsuarioSeleccionado);
            bloquearTextBoxPacientes();

            btnCrearPaciente.Enabled = false;
            btnModificarPaciente.Enabled = true;
            btnGuardarPaciente.Enabled = true;
            btnCancelarPaciente.Enabled = true;
            btnEliminarPaciente.Enabled = false;
            btnBuscarPaciente.Enabled = true;
            btnLimpiarCamposPacientes.Enabled = true;
            btnReportesPaciente.Enabled = true;
        }

        //Boton modificarpaciente
        private void btnModificarPaciente_Click(object sender, EventArgs e)
        {
            accionPacientes = "Modificar";
            textBoxDNI.Enabled = false;
            btnModificarPaciente.Enabled = false;
            btnGuardarPaciente.Enabled = true;
            btnCancelarPaciente.Enabled = true;
            btnLimpiarCamposPacientes.Enabled = true;
            btnEliminarPaciente.Enabled = true;
        }

        //Boton eliminarpaciente
        private void btnEliminarPaciente_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxDNI.Text))
            {
                var confirmarEliminar = MessageBox.Show("¿Estás seguro que deseas eliminar este paciente?", "¿Confirmar?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmarEliminar == DialogResult.Yes)
                {
                    EliminarPaciente(textBoxDNI.Text);
                }
            }
        }

        //Boton cancelarpaciente
        private void btnCancelarPaciente_Click(object sender, EventArgs e)
        {
            botonesInicioPaciente();
            limpiarLabelsPacientesPaciente();
            limpiarCamposPacientesPaciente();
            textBoxDNI.Enabled = false;
        }


        //CRUD
        //Crear paciente
        private async Task crearPaciente()
        {
            if (ValidarDNI(textBoxDNI.Text) == true)
            {
                string profesionalCorreo = correoUsuarioActual;

                var profesional = await ObtenerProfesionalPorCorreo(profesionalCorreo);

                if (profesional == null)
                {
                    MessageBox.Show("Profesional no encontrado", "¡Nos hemos topado con una barricada!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                var paciente = new pacientesPOJO
                {
                    NIF = textBoxDNI.Text,
                    Nombre = textBoxNombre.Text,
                    Apellidos = textBoxApellidos.Text,
                    Correo = textBoxEmail.Text,
                    Direccion = textBoxDireccion.Text,
                    Poblacion = textBoxPoblacion.Text,
                    Provincia = textBoxProvincia.Text,
                    Pais = textBoxPais.Text,
                    Edad = int.Parse(textBoxEdad.Text),
                    Telefono = textBoxTelefono.Text,
                    Telefono2 = textBoxTelefono2.Text,
                    ProfesionalActual = profesional.NumeroLicencia

                };

                try
                {
                    var result = await firebaseClient.Child("pacientes").PostAsync(paciente);

                    MessageBox.Show("Paciente creado con éxito. ID del nuevo nodo: " + result.Key, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    crearpaciente = true;

                    //limpiarCamposPacientesPaciente();
                    limpiarLabelsPacientesPaciente();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al crear el paciente: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                labelDNIPaciente.Text = "NIF incorrecto. Ej: 12345678X";
            }
        }

        //RegistrarPaciente
        private async Task registrarPaciente(string email, string password, string nombre, string apellidos)
        {
            if (ValidarDNI(textBoxDNI.Text) == true)
            {
                try
                {
                    var content = new StringContent($"{{\"email\":\"{email}\",\"password\":\"{password}\",\"returnSecureToken\":true}}", Encoding.UTF8, "application/json");
                    var response = await client.PostAsync($"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={API_KEY}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var usuario = new UsuariosPOJO
                        {
                            Nombre = nombre,
                            Apellidos = apellidos,
                            Correo = email,
                            Rol = "Paciente"
                        };

                        await firebaseClient.Child("usuarios").PostAsync(usuario);

                        MessageBox.Show("Usuario registrado correctamente.\n\nUsuario: " + email + "\nContraseña: " + password);
                        limpiarCamposPacientesPaciente();
                    }
                    else
                    {
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        MessageBox.Show("Error: " + errorMessage);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
            else
            {
                labelDNIPaciente.Text = "NIF incorrecto. Ej: 12345678X";
            }
        }

        //Modificar paciente
        private async Task ActualizarPaciente(string nif, string nombre, string apellidos, string correo, string direccion, string poblacion, string provincia, string pais, int edad, string telefono, string telefono2)
        {
            try
            {
                var nuevoPaciente = new pacientesPOJO
                {
                    NIF = nif,
                    Nombre = nombre,
                    Apellidos = apellidos,
                    Correo = correo,
                    Direccion = direccion,
                    Poblacion = poblacion,
                    Provincia = provincia,
                    Pais = pais,
                    Edad = edad,
                    Telefono = telefono,
                    Telefono2 = telefono2
                };

                await firebaseClient.Child("pacientes").Child(nif).PutAsync(nuevoPaciente);

                var usuarioSnapshot = await firebaseClient.Child("usuarios").OrderBy("Correo").EqualTo(correo).OnceAsync<UsuariosPOJO>();
                var usuario = usuarioSnapshot.FirstOrDefault();

                if (usuario != null)
                {
                    usuario.Object.Nombre = nombre;
                    usuario.Object.Apellidos = apellidos;
                    usuario.Object.Correo = correo;
                    usuario.Object.Rol = "Paciente";

                    await firebaseClient.Child("usuarios").Child(usuario.Key).PutAsync(usuario.Object);
                }
                else
                {
                    MessageBox.Show("No se encontró el usuario correspondiente al paciente.");
                }

                MessageBox.Show("Paciente actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al actualizar el paciente: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Mostrar paciente
        private async Task MostrarDatosPaciente(string correo)
        {
            try
            {
                if (!string.IsNullOrEmpty(correo))
                {
                    var pacientes = await firebaseClient.Child("pacientes").OrderBy("Correo").EqualTo(correo).OnceAsync<Dictionary<string, object>>();

                    if (pacientes.Any())
                    {
                        var pacienteData = pacientes.First().Object;

                        textBoxDNI.Text = pacienteData["NIF"].ToString();
                        textBoxNombre.Text = pacienteData["Nombre"].ToString();
                        textBoxApellidos.Text = pacienteData["Apellidos"].ToString();
                        textBoxEmail.Text = pacienteData["Correo"].ToString();
                        textBoxDireccion.Text = pacienteData["Direccion"].ToString();
                        textBoxPoblacion.Text = pacienteData["Poblacion"].ToString();
                        textBoxProvincia.Text = pacienteData["Provincia"].ToString();
                        textBoxPais.Text = pacienteData["Pais"].ToString();
                        textBoxEdad.Text = pacienteData["Edad"].ToString();
                        textBoxTelefono.Text = pacienteData["Telefono"].ToString();
                        textBoxTelefono2.Text = pacienteData["Telefono2"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("No se encontraron datos para el paciente con el correo electrónico proporcionado.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("El correo electrónico proporcionado está vacío.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos del paciente: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Metodo eliminarpaciente
        private async Task EliminarPaciente(string nif)
        {
            try
            {
                var pacientes = await firebaseClient.Child("pacientes").OrderBy("NIF").EqualTo(nif).OnceAsync<Dictionary<string, object>>();

                if (pacientes.Any())
                {
                    var pacienteKey = pacientes.First().Key;

                    await firebaseClient.Child("pacientes").Child(pacienteKey).DeleteAsync();

                    var usuarios = await firebaseClient.Child("usuarios").OrderBy("Correo").EqualTo(CorreoUsuarioSeleccionado).OnceAsync<UsuariosPOJO>();
                    var usuarioKey = usuarios.FirstOrDefault()?.Key;

                    if (usuarioKey != null)
                    {
                        await firebaseClient.Child("usuarios").Child(usuarioKey).DeleteAsync();
                    }

                    MessageBox.Show("Paciente eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    limpiarLabelsPacientesPaciente();
                    limpiarCamposPacientesPaciente();
                    botonesInicioPaciente();
                }
                else
                {
                    MessageBox.Show("No se encontró el paciente con el NIF proporcionado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnLimpiarCamposPacientes.Enabled = true;
                    btnCancelarPaciente.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar el paciente: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnLimpiarCamposPacientes.Enabled = true;
                btnCancelarPaciente.Enabled = true;
            }
        }






        //TreeView
        private void treeviewinicial()
        {
            treeViewMenu.Nodes.Clear();
            botonActualLabel.Text = ":)";
        }

        private bool PaginaYaCreada(TabPage pagina)
        {
            foreach (TabPage existingPage in tabControlVentanas.TabPages)
            {
                if (existingPage.Name == pagina.Name)
                {
                    return true;
                }
            }
            return false;
        }
        private bool PaginaYaCreadaPacientes(TabPage pagina)
        {
            foreach (TabPage existingPage in tabControlPacientes.TabPages)
            {
                if (existingPage.Name == pagina.Name)
                {
                    return true;
                }
            }
            return false;
        }

        private void treeViewMenu_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            TreeNode clickedNode = e.Node;

            if (clickedNode.Text.Equals("Paciente"))
            {
                if (PaginaYaCreada(tabPagePacientes))
                {
                    tabControlVentanas.SelectedTab = tabPagePacientes;
                    tabControlPacientes.SelectedTab = tabPagePacientesPaciente;
                }
                else
                {
                    tabControlVentanas.TabPages.Add(tabPPacientes);
                    tabControlVentanas.SelectedTab = tabPagePacientes;
                    FormBuscarPaciente formBuscar = new FormBuscarPaciente();

                    formBuscar.correoPacienteBusqueda += FormBuscar_correoPacienteBusqueda;
                    formBuscar.ShowDialog();
                }
            }
            else if (clickedNode.Text.Equals("Historial de paciente"))
            {
                if (PaginaYaCreada(tabPagePacientes))
                {
                    if (!PaginaYaCreada(tabPagePacientes))
                    {
                        tabControlVentanas.TabPages.Add(tabPagePacientes);
                    }
                    tabControlVentanas.SelectedTab = tabPagePacientes;

                    if (!PaginaYaCreadaPacientes(tabPageHistorialPacientes))
                    {
                        tabControlPacientes.TabPages.Add(tabPageHistorialPacientes);
                    }
                    tabControlPacientes.SelectedTab = tabPageHistorialPacientes;

                }
                else
                {
                    tabControlVentanas.TabPages.Add(tabPPacientes);
                    tabControlVentanas.SelectedTab = tabPagePacientes;

                    tabControlPacientes.TabPages.Add(tabPHistorialPacientes);
                    tabControlPacientes.SelectedTab = tabPageHistorialPacientes;
                }
            }
            else if (clickedNode.Text.Equals("Psicologos"))
            {
                if (PaginaYaCreada(tabPagePsicologos))
                {
                    tabControlVentanas.SelectedTab = tabPagePsicologos;
                }
                else
                {
                    tabControlVentanas.TabPages.Add(tabPPsicologos);
                    tabControlVentanas.SelectedTab = tabPagePsicologos;
                    FormBuscarPsicologo formBuscar = new FormBuscarPsicologo();

                    formBuscar.licenciaPsicologoBusqueda += FormBuscarPsicologo_licenciaPsicologoBusqueda;
                    formBuscar.ShowDialog();
                }
            }
            else if (clickedNode.Text.Equals("Psiquiatras"))
            {
                if (PaginaYaCreada(tabPagePsiquiatras))
                {
                    tabControlVentanas.SelectedTab = tabPagePsiquiatras;
                }
                else
                {
                    tabControlVentanas.TabPages.Add(tabPPsiquiatras);
                    tabControlVentanas.SelectedTab = tabPagePsiquiatras;
                    FormBuscarPsiquiatra formBuscar = new FormBuscarPsiquiatra();

                    formBuscar.licenciaPsiquiatraBusqueda += FormBuscarPsiquiatra_licenciaPsiquiatraBusqueda;
                    formBuscar.ShowDialog();
                }
            }
            else if (clickedNode.Text.Equals("Chat"))
            {
                if (PaginaYaCreada(tabPageChat))
                {
                    tabControlVentanas.SelectedTab = tabPageChat;
                }
                else
                {
                    tabControlVentanas.TabPages.Add(tabPChat);
                    tabControlVentanas.SelectedTab = tabPageChat;

                }
            }



        }


        //Botones toolstrip
        private void btnPacientesToolStrip_Click(object sender, EventArgs e)
        {
            btnPacientesToolStrip.BackColor = Color.Salmon;
            btnPsicologosToolStrip.BackColor = Color.AliceBlue;
            btnPsiquiatrasToolStrip.BackColor = Color.AliceBlue;
            btnChatToolStrip.BackColor = Color.AliceBlue;

            treeViewMenu.Nodes.Clear();
            botonActualLabel.Text = "Pacientes";

            TreeNode pacientes = new TreeNode("Pacientes");
            treeViewMenu.Nodes.Add(pacientes);

            TreeNode paciente = new TreeNode("Paciente");
            TreeNode historialPaciente = new TreeNode("Historial de paciente");

            pacientes.Nodes.Add(paciente);
            pacientes.Nodes.Add(historialPaciente);

            pacientes.Expand();
        }


        private void btnPsicologosToolStrip_Click(object sender, EventArgs e)
        {
            btnPsicologosToolStrip.BackColor = Color.Salmon;
            btnPacientesToolStrip.BackColor = Color.AliceBlue;
            btnPsiquiatrasToolStrip.BackColor = Color.AliceBlue;
            btnChatToolStrip.BackColor = Color.AliceBlue;

            treeViewMenu.Nodes.Clear();
            botonActualLabel.Text = "Psicologos";

            TreeNode psicologos = new TreeNode("Psicologos");
            treeViewMenu.Nodes.Add(psicologos);

        }

        private void btnPsiquiatrasToolStrip_Click(object sender, EventArgs e)
        {
            btnPsiquiatrasToolStrip.BackColor = Color.Salmon;
            btnPacientesToolStrip.BackColor = Color.AliceBlue;
            btnPsicologosToolStrip.BackColor = Color.AliceBlue;
            btnChatToolStrip.BackColor = Color.AliceBlue;

            treeViewMenu.Nodes.Clear();
            botonActualLabel.Text = "Psiquiatras";

            TreeNode psiquiatras = new TreeNode("Psiquiatras");
            treeViewMenu.Nodes.Add(psiquiatras);
        }

        private void btnChatToolStrip_Click(object sender, EventArgs e)
        {
            btnChatToolStrip.BackColor = Color.Salmon;
            btnPsiquiatrasToolStrip.BackColor = Color.AliceBlue;
            btnPacientesToolStrip.BackColor = Color.AliceBlue;
            btnPsicologosToolStrip.BackColor = Color.AliceBlue;

            treeViewMenu.Nodes.Clear();
            botonActualLabel.Text = "Chat";

            TreeNode chat = new TreeNode("Chat");
            treeViewMenu.Nodes.Add(chat);
        }







        private void textBoxDNI_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBoxDNI.Text))
            {
                desbloquearTextBoxPacientes();
            }
            else
            {
                bloquearTextBoxPacientes();
                textBoxDNI.Enabled = true;
                textBoxDNI.Focus();
            }

            string nif = textBoxDNI.Text;

            CargarDatos(nif);

        }


        //Psiquiatras
        //Metodos para psiquiatras
        private void limpiarCamposPsiquiatras()
        {
            textBoxLicenciaPsiquiatra.Clear();
            textBoxNIFPsiquiatra.Clear();
            textBoxNombrePsiquiatra.Clear();
            textBoxApellidosPsiquiatra.Clear();
            textBoxEdadPsiquiatra.Clear();
            textBoxDireccionPsiquiatra.Clear();
            textBoxPoblacionPsiquiatra.Clear();
            textBoxProvinciaPsiquiatra.Clear();
            textBoxPaisPsiquiatra.Clear();
            textBoxDireccionConsultorioPsiquiatra.Clear();
            textBoxTelefono1Psiquiatra.Clear();
            textBoxTelefono2Psiquiatra.Clear();
            textBoxObservacionesPsiquiatra.Clear();
            textBoxEspecialidadPsiquiatra.Clear();
            textBoxEmailPsiquiatra.Clear();

        }

        private void limpiarLabelsPsiquiatra()
        {
            labelLicenciaPsiquiatra.Text = "";
            labelNIFPsiquiatra.Text = "";
            labelNombrePsiquiatra.Text = "";
            labelApellidosPsiquiatra.Text = "";
            labelEdadPsiquiatra.Text = "";
            labelDireccionPsiquiatra.Text = "";
            labelPoblacionPsiquiatra.Text = "";
            labelProvinciaPsiquiatra.Text = "";
            labelPaisPsiquiatra.Text = "";
            labelDireccionConsultorioPsiquiatra.Text = "";
            labelTelefono1Psiquiatra.Text = "";
            labelTelefono2Psiquiatra.Text = "";
            labelEspecialidadPsiquiatra.Text = "";
            labelEmailPsiquiatra.Text = "";
        }


        //PSICOLOGOS
        //Metodos para psicologo
        private void limpiarCamposPsicologos()
        {
            textBoxNombrePsicologo.Text = "";
            textBoxApellidosPsicologo.Text = "";
            textBoxDirConsPsicologo.Text = "";
            textBoxDireccionPsicologo.Text = "";
            textBoxEdadPsicologo.Text = "";
            textBoxEmailPsicologo.Text = "";
            textBoxEspecialidadPsicologo.Text = "";
            textBoxLicenciaPsicologo.Text = "";
            textBoxNIFPsicologo.Text = "";
            textBoxObservacionesPsicologo.Text = "";
            textBoxPoblacionPsicologo.Text = "";
            textBoxProvinciaPsicologo.Text = "";
            textBoxPaisPsicologo.Text = "";
            textBoxTelefono1Psicologo.Text = "";
            textBoxTelefono2Psicologo.Text = "";
        }

        private void limpiarLabelsPsicologo()
        {
            labelNombrePsicologo.Text = "";
            labelApellidosPsicologo.Text = "";
            labelDirConsPsicologo.Text = "";
            labelDireccionPsicologo.Text = "";
            labelEdadPsicologo.Text = "";
            labelEmailPsicologo.Text = "";
            labelEspecialidadPsicologo.Text = "";
            labelNLicenciaPsicologo.Text = "";
            labelNIFPsicologo.Text = "";
            labelPoblacionPsicologo.Text = "";
            labelProvinciaPsicologo.Text = "";
            labelPaisPsicologo.Text = "";
            labelTelefonoPsicologo.Text = "";
            labelTelefono2Psicologo.Text = "";
        }

        private void desbloquearTextBoxPsicologo()
        {
            textBoxNombrePsicologo.Enabled = true;
            textBoxNIFPsicologo.Enabled = true;
            textBoxApellidosPsicologo.Enabled = true;
            textBoxDirConsPsicologo.Enabled = true;
            textBoxDireccionPsicologo.Enabled = true;
            textBoxEdadPsicologo.Enabled = true;
            textBoxEspecialidadPsicologo.Enabled = true;
            textBoxLicenciaPsicologo.Enabled = true;
            textBoxPoblacionPsicologo.Enabled = true;
            textBoxProvinciaPsicologo.Enabled = true;
            textBoxPaisPsicologo.Enabled = true;
            textBoxTelefono1Psicologo.Enabled = true;
            textBoxTelefono2Psicologo.Enabled = true;
            textBoxEmailPsicologo.Enabled = true;
            textBoxPrecioPsicologo.Enabled = true;
        }

        private void bloquearTextBoxPsicologo()
        {
            textBoxNombrePsicologo.Enabled = false;
            textBoxNIFPsicologo.Enabled = false;
            textBoxApellidosPsicologo.Enabled = false;
            textBoxDirConsPsicologo.Enabled = false;
            textBoxDireccionPsicologo.Enabled = false;
            textBoxEdadPsicologo.Enabled = false;
            textBoxEspecialidadPsicologo.Enabled = false;
            textBoxLicenciaPsicologo.Enabled = false;
            textBoxPoblacionPsicologo.Enabled = false;
            textBoxProvinciaPsicologo.Enabled = false;
            textBoxPaisPsicologo.Enabled = false;
            textBoxTelefono1Psicologo.Enabled = false;
            textBoxTelefono2Psicologo.Enabled = false;
            textBoxEmailPsicologo.Enabled = false;
            textBoxPrecioPsicologo.Enabled = false;
        }

        private void desbloquearTextBoxPsiquiatras()
        {
            textBoxNombrePsiquiatra.Enabled = true;
            textBoxNIFPsiquiatra.Enabled = true;
            textBoxApellidosPsiquiatra.Enabled = true;
            textBoxDireccionConsultorioPsiquiatra.Enabled = true;
            textBoxDireccionPsiquiatra.Enabled = true;
            textBoxEdadPsiquiatra.Enabled = true;
            textBoxEspecialidadPsiquiatra.Enabled = true;
            textBoxLicenciaPsiquiatra.Enabled = true;
            textBoxPoblacionPsiquiatra.Enabled = true;
            textBoxProvinciaPsiquiatra.Enabled = true;
            textBoxPaisPsiquiatra.Enabled = true;
            textBoxTelefono1Psiquiatra.Enabled = true;
            textBoxTelefono2Psiquiatra.Enabled = true;
            textBoxPrecioSesionPsiquiatra.Enabled = true;
            textBoxEmailPsiquiatra.Enabled = true;
            textBoxObservacionesPsiquiatra.Enabled = true;
        }

        private void bloquearTextBoxPsiquiatras()
        {
            textBoxNombrePsiquiatra.Enabled = false;
            textBoxNIFPsiquiatra.Enabled = false;
            textBoxApellidosPsiquiatra.Enabled = false;
            textBoxDireccionConsultorioPsiquiatra.Enabled = false;
            textBoxDireccionPsiquiatra.Enabled = false;
            textBoxEdadPsiquiatra.Enabled = false;
            textBoxEspecialidadPsiquiatra.Enabled = false;
            textBoxLicenciaPsiquiatra.Enabled = false;
            textBoxPoblacionPsiquiatra.Enabled = false;
            textBoxProvinciaPsiquiatra.Enabled = false;
            textBoxPaisPsiquiatra.Enabled = false;
            textBoxTelefono1Psiquiatra.Enabled = false;
            textBoxTelefono2Psiquiatra.Enabled = false;
            textBoxPrecioSesionPsiquiatra.Enabled = false;
            textBoxEmailPsiquiatra.Enabled = false;
            textBoxObservacionesPsiquiatra.Enabled = false;
        }

        private void botonesInicioPsicologo()
        {
            btnCerrarPagePsicologos.Enabled = true;
            btnCrearPsicologo.Enabled = true;
            btnModificarPsicologo.Enabled = false;
            btnGuardarPsicologo.Enabled = false;
            btnCancelarPsicologo.Enabled = false;
            btnEliminarPsicologo.Enabled = false;
            btnBuscarPsicologo.Enabled = true;
            btnLimpiarCamposPsicologo.Enabled = false;
            btnReportesPsicologo.Enabled = false;
        }

        private void textBoxLicenciaPsicologo_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBoxLicenciaPsicologo.Text))
            {
                desbloquearTextBoxPsicologo();
            }
            else
            {
                bloquearTextBoxPsicologo();
                textBoxLicenciaPsicologo.Enabled = true;
                textBoxLicenciaPsicologo.Focus();
            }
        }


        //CRUD psicologo

        //Crear psicólogo
        private async Task CrearPsicologo()
        {
                if (ValidarDNI(textBoxNIFPsicologo.Text))
                {
                    var psicologo = new PsicologosPOJO
                    {
                        NumeroLicencia = textBoxLicenciaPsicologo.Text,
                        Nombre = textBoxNombrePsicologo.Text,
                        Apellidos = textBoxApellidosPsicologo.Text,
                        Edad = int.Parse(textBoxEdadPsicologo.Text),
                        Direccion = textBoxDireccionPsicologo.Text,
                        Poblacion = textBoxPoblacionPsicologo.Text,
                        Provincia = textBoxProvinciaPsicologo.Text,
                        Pais = textBoxPaisPsicologo.Text,
                        NIF = textBoxNIFPsicologo.Text,
                        Correo = textBoxEmailPsicologo.Text,
                        Especialidad = textBoxEspecialidadPsicologo.Text,
                        HorariosAtencion = textBoxObservacionesPsicologo.Text,
                        Tarifas = int.Parse(textBoxPrecioPsicologo.Text),
                        Telefono1 = textBoxTelefono1Psicologo.Text,
                        Telefono2 = textBoxTelefono2Psicologo.Text,
                        DireccionConsultorio = textBoxDirConsPsicologo.Text,
                    };

                    try
                    {
                        var result = await firebaseClient.Child("psicologos").PostAsync(psicologo);

                        MessageBox.Show("Psicólogo creado con éxito. ID del nuevo nodo: " + result.Key, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        crearpsicologo = true;

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al crear el psicólogo: " + ex.Message, "¡Nos hemos topado con una barricada!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        crearpsicologo = false;
                    }
                }
                else
                {
                    labelNIFPsicologo.Text = "NIF incorrecto. Ej: 12345678X";
                }
        }

        //Registrar psicologo
        private async Task registrarPsicologo(string email, string password, string nombre, string apellidos)
        {
            if (ValidarDNI(textBoxNIFPsicologo.Text))
            {
                if (crearpsicologo == true)
                {

                    try
                    {
                        var content = new StringContent($"{{\"email\":\"{email}\",\"password\":\"{password}\",\"returnSecureToken\":true}}", Encoding.UTF8, "application/json");
                        var response = await client.PostAsync($"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={API_KEY}", content);

                        if (response.IsSuccessStatusCode)
                        {
                            var usuario = new UsuariosPOJO
                            {
                                Nombre = nombre,
                                Apellidos = apellidos,
                                Correo = email,
                                Rol = "Psicólogo"
                            };

                            await firebaseClient.Child("usuarios").PostAsync(usuario);

                            MessageBox.Show("Usuario registrado correctamente.\n\nUsuario: " + email + "\nContraseña: " + password);
                        }
                        else
                        {
                            var errorMessage = await response.Content.ReadAsStringAsync();
                            MessageBox.Show("Error: " + errorMessage);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
                else
                {

                }
            } else
            {
                labelNIFPsicologo.Text = "NIF incorrecto. Ej: 12345678X";
            }
        }

        //Modificar psicologo
        private async Task ActualizarPsicologo(string nlicencia, string nombre, string apellidos, int edad, string direccion, string poblacion, string provincia, string pais, string nif, string correo, string especialidad,
            string horariosatencion, string tarifas, string telefono1, string telefono2, string direccionconsultorio)
        {
                if (ValidarDNI(textBoxNIFPsicologo.Text) == true)
                {
                    try
                    {
                        var nuevoPsicololgo = new PsicologosPOJO
                        {
                            NumeroLicencia = nlicencia,
                            Nombre = nombre,
                            Apellidos = apellidos,
                            Edad = edad,
                            Direccion = direccion,
                            Poblacion = poblacion,
                            Provincia = provincia,
                            Pais = pais,
                            NIF = nif,
                            Correo = correo,
                            Especialidad = especialidad,
                            HorariosAtencion = horariosatencion,
                            Tarifas = int.Parse(tarifas),
                            Telefono1 = telefono1,
                            Telefono2 = telefono2,
                            DireccionConsultorio = direccionconsultorio
                        };

                        await firebaseClient.Child("psicologos").Child(nif).PutAsync(nuevoPsicololgo);

                        var usuarioSnapshot = await firebaseClient.Child("usuarios").OrderBy("NIF").EqualTo(nif).OnceAsync<UsuariosPOJO>();
                        var usuario = usuarioSnapshot.FirstOrDefault();

                        if (usuario != null)
                        {
                            usuario.Object.Nombre = nombre;
                            usuario.Object.Apellidos = apellidos;
                            usuario.Object.Correo = correo;
                            usuario.Object.Rol = "Psicólogo";
                            usuario.Object.Rol = "Psicólogo";

                            await firebaseClient.Child("usuarios").Child(usuario.Key).PutAsync(usuario.Object);
                        }
                        else
                        {
                            MessageBox.Show("No se encontró el usuario correspondiente al paciente.");
                        }

                        MessageBox.Show("Paciente actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al actualizar el paciente: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    labelNIFPsicologo.Text = "NIF inválido. Ej: 12345678X";
                }
        }

        //Boton crearPsicologo
        private void btnCrearPsicologo_Click(object sender, EventArgs e)
        {
            textBoxLicenciaPsicologo.Enabled = true;
            textBoxLicenciaPsicologo.Focus();
            accionPsicologos = "Crear";
            btnCrearPsicologo.Enabled = false;
            btnGuardarPsicologo.Enabled = true;
            btnCancelarPsicologo.Enabled = true;
            btnLimpiarCamposPsicologo.Enabled = true;
        }

        //Boton modificarPsicologo
        private void btnModificarPsicologo_Click(object sender, EventArgs e)
        {
            accionPsicologos = "Modificar";
            textBoxNIFPsicologo.Enabled = false;
            btnModificarPsicologo.Enabled = false;
            btnGuardarPsicologo.Enabled = true;
            btnCancelarPsicologo.Enabled = true;
            btnLimpiarCamposPsicologo.Enabled = true;
            btnEliminarPsicologo.Enabled = true;
        }

        //Boton guardarPsicologo
        private async void btnGuardarPsicologo_Click(object sender, EventArgs e)
        {
            validarPsicologo();

                if (accionPsicologos.Equals("Crear"))
                {
                    if (accionPsicoOK == true) { 
                    
                        await CrearPsicologo();

                        if (crearpsicologo.Equals(true))
                        {
                            await registrarPsicologo(textBoxEmailPsicologo.Text, textBoxNIFPsicologo.Text, textBoxNombrePsicologo.Text, textBoxApellidosPsicologo.Text);
                            botonesInicioPsicologo();
                            limpiarLabelsPsicologo();
                            limpiarCamposPsicologos();
                        }
                    } 
                    else 
                    {
                    accionPsicologos = "Crear";
                    }
                }
                else if (accionPsicologos.Equals("Modificar"))
                {
                if (accionPsicoOK == true)
                {
                    await ActualizarPsicologo(textBoxLicenciaPsicologo.Text, textBoxNombrePsicologo.Text, textBoxApellidosPsicologo.Text, int.Parse(textBoxEdadPsicologo.Text), textBoxDireccionPsicologo.Text, textBoxPoblacionPsicologo.Text, textBoxProvinciaPsicologo.Text, textBoxPaisPsicologo.Text, textBoxNIFPsicologo.Text, textBoxEmailPsicologo.Text, textBoxEspecialidadPsicologo.Text, textBoxObservacionesPsicologo.Text, textBoxPrecioPsicologo.Text, textBoxTelefono1Psicologo.Text, textBoxTelefono2Psicologo.Text, textBoxDirConsPsicologo.Text);
                }
                else
                {
                    accionPsicologos = "Modificar";
                }
                }
        }

        //Boton eliminarPsicologo
        private async void btnEliminarPsicologo_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxLicenciaPsicologo.Text))
            {
                var confirmarEliminar = MessageBox.Show("¿Estás seguro que deseas eliminar este psicólogo?", "¿Confirmar?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmarEliminar == DialogResult.Yes)
                {
                    eliminarPsicologo(textBoxLicenciaPsicologo.Text);
                }
            }

            botonesInicioPsicologo();

        }

        private void btnCancelarPsicologo_Click(object sender, EventArgs e)
        {
            botonesInicioPsicologo();
            limpiarLabelsPsicologo();
            limpiarCamposPsicologos();
            textBoxNIFPsicologo.Enabled = false;
        }

        private async Task eliminarPsicologo(string licencia)
        {
            try
            {
                var psicologos = await firebaseClient.Child("psicologos").OrderBy("NumeroLicencia").EqualTo(licencia).OnceAsync<Dictionary<string, object>>();

                if (psicologos.Any())
                {
                    var psicologosKey = psicologos.First().Key;

                    await firebaseClient.Child("psicologos").Child(psicologosKey).DeleteAsync();

                    var usuarios = await firebaseClient.Child("usuarios").OrderBy("Correo").EqualTo(CorreoUsuarioSeleccionado).OnceAsync<UsuariosPOJO>();
                    var usuarioKey = usuarios.FirstOrDefault()?.Key;

                    if (usuarioKey != null)
                    {
                        await firebaseClient.Child("usuarios").Child(usuarioKey).DeleteAsync();
                    }

                    MessageBox.Show("Psicólogo eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    limpiarLabelsPsicologo();
                    limpiarCamposPsicologos();
                    botonesInicioPsicologo();
                }
                else
                {
                    MessageBox.Show("No se encontró el psicólogo con el número de licencia proporcionado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnLimpiarCamposPsicologo.Enabled = true;
                    btnCancelarPsicologo.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar el psicólogo: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnLimpiarCamposPsicologo.Enabled = true;
                btnCancelarPsicologo.Enabled = true;
            }
        }

        private void btnBuscarPsicologo_Click(object sender, EventArgs e)
        {
            FormBuscarPsicologo buscar = new FormBuscarPsicologo();
            buscar.licenciaPsicologoBusqueda += FormBuscarPsicologo_licenciaPsicologoBusqueda;
            buscar.ShowDialog();

            btnCrearPsicologo.Enabled = false;
            btnModificarPsicologo.Enabled = true;
            btnGuardarPsicologo.Enabled = false;
            btnCancelarPsicologo.Enabled = false;
            btnEliminarPsicologo.Enabled = false;
            btnBuscarPsicologo.Enabled = true;
            btnLimpiarCamposPsicologo.Enabled = true;
            btnReportesPsicologo.Enabled = true;
        }

        private void FormBuscarPsicologo_licenciaPsicologoBusqueda(object? sender, string licencia)
        {
            licenciaPsicologoSeleccionado = licencia;
            MostrarDatosPsicologo(licencia);
            bloquearTextBoxPacientes();

            btnCrearPaciente.Enabled = false;
            btnModificarPaciente.Enabled = true;
            btnGuardarPaciente.Enabled = true;
            btnCancelarPaciente.Enabled = true;
            btnEliminarPaciente.Enabled = false;
            btnBuscarPaciente.Enabled = true;
            btnLimpiarCamposPacientes.Enabled = true;
            btnReportesPaciente.Enabled = true;
        }

        private async Task MostrarDatosPsicologo(string licencia)
        {
            try
            {
                if (!string.IsNullOrEmpty(licencia))
                {
                    var psicologos = await firebaseClient.Child("psicologos")
                        .OrderBy("NumeroLicencia")
                        .EqualTo(licencia)
                        .OnceAsync<Dictionary<string, object>>();

                    if (psicologos.Any())
                    {
                        var psicologoData = psicologos.First().Object;

                        textBoxLicenciaPsicologo.Text = psicologoData["NumeroLicencia"].ToString();
                        textBoxNIFPsicologo.Text = psicologoData["NIF"].ToString();
                        textBoxNombrePsicologo.Text = psicologoData["Nombre"].ToString();
                        textBoxApellidosPsicologo.Text = psicologoData["Apellidos"].ToString();
                        textBoxEmailPsicologo.Text = psicologoData["Correo"].ToString();
                        textBoxDireccionPsicologo.Text = psicologoData["Direccion"].ToString();
                        textBoxPoblacionPsicologo.Text = psicologoData["Poblacion"].ToString();
                        textBoxProvinciaPsicologo.Text = psicologoData["Provincia"].ToString();
                        textBoxPaisPsicologo.Text = psicologoData["Pais"].ToString();
                        textBoxEdadPsicologo.Text = psicologoData["Edad"].ToString();
                        textBoxTelefono1Psicologo.Text = psicologoData["Telefono1"].ToString();
                        textBoxTelefono2Psicologo.Text = psicologoData["Telefono2"].ToString();
                        textBoxEspecialidadPsicologo.Text = psicologoData["Especialidad"].ToString();
                        textBoxObservacionesPsicologo.Text = psicologoData["HorariosAtencion"].ToString();
                        textBoxPrecioPsicologo.Text = psicologoData["Tarifas"].ToString();
                        textBoxDirConsPsicologo.Text = psicologoData["DireccionConsultorio"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("No se encontraron datos para el psicólogo con el número de licencia proporcionado.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("El número de licencia proporcionado está vacío.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos del psicólogo: " + ex.Message, "¡Nos hemos topado con una barricada!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }











        //Mensajes
        private async void btnBuscarUsuario_Click(object sender, EventArgs e)
        {
            string searchQuery = textBoxBuscarUsuarioChat.Text.ToLower();
            var usuarios = await firebaseClient.Child("usuarios").OnceAsync<Dictionary<string, object>>();

            listBoxResultadosBusqueda.Items.Clear();

            foreach (var usuario in usuarios)
            {
                var usuarioData = usuario.Object;
                if (usuarioData.ContainsKey("Correo"))
                {
                    string correoUsuario = usuarioData["Correo"].ToString().ToLower();
                    if (correoUsuario.Contains(searchQuery))
                    {
                        listBoxResultadosBusqueda.Items.Add(new KeyValuePair<string, string>(usuario.Key, usuarioData["Correo"].ToString()));
                    }
                }
            }
        }

        private async void listBoxResultadosBusqueda_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxResultadosBusqueda.SelectedItem != null)
            {
                var selectedUser = (KeyValuePair<string, string>)listBoxResultadosBusqueda.SelectedItem;
                string selectedUserCorreo = selectedUser.Value;
                string currentUserCorreo = inicioSesion.email;

                string existingChatId = await ObtenerChatExistente(currentUserCorreo, selectedUserCorreo);
                if (existingChatId != null)
                {
                    CargarChatAsync(existingChatId);
                    ListBoxChatsRecientes();
                    labelNameChatActual.Text = selectedUser.Value;
                }
                else if (existingChatId == null)
                {
                    var chatId = await CrearOObtenerChat(selectedUserCorreo);
                    if (chatId != null)
                    {
                        CargarChatAsync(chatId);
                        ListBoxChatsRecientes();
                        labelNameChatActual.Text = selectedUser.Value;
                    }
                }
            }
        }

        private async Task<string> CrearOObtenerChat(string selectedUserCorreo)
        {
            listBoxMessages.Items.Clear();

            var currentUserCorreo = inicioSesion.email;

            var existingChatId = await ObtenerChatExistente(currentUserCorreo, selectedUserCorreo);
            if (existingChatId != null)
            {
                return existingChatId;
            }

            var userChatsRef = firebaseClient.Child("user_chats");
            var userChatsSnapshot = await userChatsRef.OnceSingleAsync<Dictionary<string, object>>();
            if (userChatsSnapshot == null)
            {
                await userChatsRef.PutAsync(true);
            }

            var nuevoChatData = new Dictionary<string, object>
    {
        { "participants", new List<string> { currentUserCorreo, selectedUserCorreo } },
        { "lastMessage", "Chat Iniciado" },
        { "lastMessageDateTime", DateTime.Now.ToString() }
    };

            var result = await userChatsRef.PostAsync(nuevoChatData);

            var nuevoChat = new
            {
                participantes = new List<string> { currentUserCorreo, selectedUserCorreo },
                messages = new Dictionary<string, ChatMessage>
        {
            { Guid.NewGuid().ToString(), new ChatMessage("Sistema", "Chat Iniciado") }
        }
            };

            await firebaseClient.Child("chats").Child(result.Key).PutAsync(nuevoChat);

            ObservarCambiosEnMensajes(result.Key);

            return result.Key;
        }



        private async void CargarChatsRecientes()
        {
            var currentUserCorreo = inicioSesion.email;

            try
            {
                var userChatsRef = firebaseClient.Child("user_chats");
                var userChatsSnapshot = await userChatsRef.OnceSingleAsync<object>();

                if (userChatsSnapshot == null || userChatsSnapshot is bool)
                {
                    listBoxChatsRecientes.Items.Clear();
                    Console.WriteLine("No user chats found.");
                    return;
                }

                var userChats = JToken.FromObject(userChatsSnapshot);

                listBoxChatsRecientes.Items.Clear();

                foreach (var chatIdObj in userChats.Children())
                {
                    if (chatIdObj is JProperty chatIdProp)
                    {
                        string chatId = chatIdProp.Name;
                        var chatData = await firebaseClient.Child("chats").Child(chatId).OnceSingleAsync<object>();

                        if (chatData is JObject chatDataObject)
                        {
                            var chatDataDict = chatDataObject.ToObject<Dictionary<string, object>>();
                            var participants = ((JArray)chatDataDict["participantes"]).ToObject<List<string>>();

                            string otherUserCorreo = participants.First(p => p != currentUserCorreo);
                            var otherUserData = await firebaseClient.Child("usuarios").OrderBy("Correo").EqualTo(otherUserCorreo).OnceSingleAsync<Dictionary<string, object>>();

                            string otherUserName = otherUserData["Nombre"].ToString();
                            listBoxChatsRecientes.Items.Add(new KeyValuePair<string, string>(chatId, otherUserName));
                            Console.WriteLine($"Loaded recent chat: {chatId} with user: {otherUserName}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading recent chats: {ex.Message}");
            }
        }







        private async void btnSend_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentChatId))
            {
                MessageBox.Show("No chat selected.");
                return;
            }

            string chatId = currentChatId;
            var message = new ChatMessage(inicioSesion.email, txtMessage.Text);

            try
            {
                await firebaseClient.Child("chats").Child(chatId).Child("messages").PostAsync(message);
                txtMessage.Clear();
                Console.WriteLine("Message sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
            }
        }

        private async void CargarChatAsync(string chatId)
        {
            currentChatId = chatId;

            var messagesSnapshot = await firebaseClient.Child("chats").Child(chatId).Child("messages").OnceAsync<ChatMessage>();
            foreach (var message in messagesSnapshot)
            {
                listBoxMessages.Items.Add($"{message.Object.Sender}: {message.Object.Message}");
            }

            ObservarCambiosEnMensajes(chatId);
        }

        private async void ListBoxChatsRecientes()
        {
            string emailActual = inicioSesion.email;
            List<Chat> chatsRecientes = await ObtenerChatsRecientes(emailActual);

            listBoxChatsRecientes.DataSource = chatsRecientes;
            listBoxChatsRecientes.DisplayMember = "DisplayNombreParticipantes";
        }

        private async Task<List<Chat>> ObtenerChatsRecientes(string email)
        {
            var chats = await firebaseClient
                .Child("chats")
                .OnceAsync<Chat>();

            var chatsRecientes = chats
                .Where(chat => chat.Object.Participantes != null && chat.Object.Participantes.Contains(email))
                .Select(chat => new Chat
                {
                    Id = chat.Key,
                    Participantes = chat.Object.Participantes,
                    Messages = chat.Object.Messages ?? new Dictionary<string, ChatMessage>()
                })
                .ToList();

            return chatsRecientes;
        }

        private void ObservarCambiosEnMensajes(string chatId)
        {
            if (!suscripciones.ContainsKey(chatId))
            {
                var suscripcion = firebaseClient.Child("chats").Child(chatId).Child("messages").AsObservable<ChatMessage>().Subscribe(d =>
                {
                    if (d.EventType == Firebase.Database.Streaming.FirebaseEventType.InsertOrUpdate)
                    {
                        if (listBoxMessages != null && listBoxMessages.IsHandleCreated)
                        {
                            listBoxMessages.Invoke(new System.Action(() =>
                            {
                                if (listBoxMessages != null)
                                {
                                    listBoxMessages.Items.Add($"{d.Object.Sender}: {d.Object.Message}");
                                    listBoxMessages.SelectedIndex = listBoxMessages.Items.Count - 1;
                                    listBoxMessages.SelectedIndex = -1;
                                }
                            }));
                        }
                    }
                });

                suscripciones.Add(chatId, suscripcion);
            }
        }

        private void listBoxChatsRecientes_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxChatsRecientes.SelectedItem != null)
            {
                listBoxMessages.Items.Clear();
                Chat chatSeleccionado = (Chat)listBoxChatsRecientes.SelectedItem;
                currentChatId = chatSeleccionado.Id;
                CargarChatAsync(chatSeleccionado.Id);
                labelNameChatActual.Text = chatSeleccionado.DisplayNombreParticipantes;
            }
        }



        private async Task<string> ObtenerChatExistente(string currentUserCorreo, string selectedUserCorreo)
        {
            listBoxMessages.Items.Clear();
            var userChatsRef = firebaseClient.Child("chats");
            var userChatsSnapshot = await userChatsRef.OnceSingleAsync<Dictionary<string, object>>();

            if (userChatsSnapshot != null)
            {
                foreach (var chatData in userChatsSnapshot)
                {
                    if (chatData.Value is Dictionary<string, object> chatDict)
                    {
                        var participants = chatDict.ContainsKey("participantes") && chatDict["participantes"] is List<string>
                            ? (List<string>)chatDict["participantes"]
                            : new List<string>();

                        if (participants.Contains(currentUserCorreo) && participants.Contains(selectedUserCorreo))
                        {
                            return chatData.Key;
                        }
                    }
                }
            }

            return null;
        }

        private void btnInfoApp_Click(object sender, EventArgs e)
        {
            ControlVersiones cv = new ControlVersiones();
            cv.ShowDialog();
        }



        private async void btnBorrarChatsChat_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentChatId))
            {
                try
                {
                    if (suscripciones.ContainsKey(currentChatId))
                    {
                        suscripciones[currentChatId].Dispose();
                        suscripciones.Remove(currentChatId);
                    }

                    await firebaseClient.Child("chats").Child(currentChatId).DeleteAsync();

                    listBoxMessages.Items.Clear();
                    labelNameChatActual.Text = string.Empty;
                    currentChatId = string.Empty;

                    ListBoxChatsRecientes();

                    MessageBox.Show("Conversación borrada correctamente.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al borrar la conversación: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("No se ha seleccionado ninguna conversación para borrar.");
            }
        }

        private void btnCerrarChatsChat_Click(object sender, EventArgs e)
        {
            listBoxMessages.Items.Clear();
            currentChatId = null;
            labelNameChatActual.Text = string.Empty;
        }

        private void txtMessage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnSend.PerformClick();

                txtMessage.Focus();
            }
        }


        //Crud historial de sesiones de paciente

        private async void CrearHistorialSesionPaciente()
        {
            if (dataGridViewHistorialPaciente.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridViewHistorialPaciente.SelectedRows[0];

                string nif = row.Cells["NIF"].Value.ToString();
                string licencia = row.Cells["LicenciaProfesionalActual"].Value.ToString();
                string motivo = row.Cells["MotivoConsulta"].Value.ToString();
                string medicamentos = row.Cells["TomaMedicamentos"].Value.ToString();
                string observaciones = row.Cells["ObersvacionesTerapeuta"].Value.ToString();
                string duracion = row.Cells["DuracionSesion"].Value.ToString();
                string resumen = row.Cells["ResumenSesion"].Value.ToString();
                string consentimiento = row.Cells["ConsentimientoInformativo"].Value.ToString();

                historialPacientesPOJO historial = new historialPacientesPOJO(nif, licencia, motivo, medicamentos, observaciones, duracion, resumen, consentimiento);

                try
                {
                    await firebaseClient.Child("historialPacientes").PostAsync(historial);

                    MessageBox.Show("Registro creado exitosamente en la base de datos.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al crear el registro en la base de datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            else
            {
                MessageBox.Show("Por favor, seleccione una fila primero.", "Sin selección", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnCrearHistorialToolStrip_Click(object sender, EventArgs e)
        {
            CrearHistorialSesionPaciente();
        }

        private async void CargarDatos(string filtroNIF = null)
        {
            try
            {
                var pacientes = await firebaseClient.Child("historialPacientes").OnceAsync<Dictionary<string, object>>();

                dataGridViewHistorialPaciente.Rows.Clear();

                foreach (var paciente in pacientes)
                {
                    var pacienteData = paciente.Object;

                    if (filtroNIF == null || pacienteData["NIF"].ToString() == filtroNIF)
                    {
                        dataGridViewHistorialPaciente.Rows.Add(pacienteData["NIF"], pacienteData["LicenciaProfesionalActual"], pacienteData["ConsentimientoInformativo"], pacienteData["DuracionSesion"], pacienteData["MotivoConsulta"], pacienteData["ObersvacionesTerapeuta"], pacienteData["ResumenSesion"], pacienteData["TomaMedicamentos"]);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnModificarHistorialPaciente_Click(object sender, EventArgs e)
        {
            ModificarHistorialSesionPaciente();
        }

        private async void ModificarHistorialSesionPaciente()
        {
            if (dataGridViewHistorialPaciente.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridViewHistorialPaciente.SelectedRows[0];

                string nif = row.Cells["NIF"].Value.ToString(); 
                string licencia = row.Cells["LicenciaProfesionalActual"].Value.ToString();
                string motivo = row.Cells["MotivoConsulta"].Value.ToString();
                string medicamentos = row.Cells["TomaMedicamentos"].Value.ToString();
                string observaciones = row.Cells["ObersvacionesTerapeuta"].Value.ToString();
                string duracion = row.Cells["DuracionSesion"].Value.ToString();
                string resumen = row.Cells["ResumenSesion"].Value.ToString();
                string consentimiento = row.Cells["ConsentimientoInformativo"].Value.ToString();

                historialPacientesPOJO historial = new historialPacientesPOJO(nif, licencia, motivo, medicamentos, observaciones, duracion, resumen, consentimiento);

                try
                {
                    await firebaseClient.Child("historialPacientes").Child(nif).PutAsync(historial);

                    MessageBox.Show("Registro modificado exitosamente en la base de datos.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al modificar el registro en la base de datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            else
            {
                MessageBox.Show("Por favor, seleccione una fila primero.", "Sin selección", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void EliminarHistorialSesionPaciente()
        {
            if (dataGridViewHistorialPaciente.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridViewHistorialPaciente.SelectedRows[0];

                string nif = row.Cells["NIF"].Value.ToString();

                try
                {
                    await firebaseClient.Child("historialPacientes").Child(nif).DeleteAsync();

                    MessageBox.Show("Registro eliminado exitosamente de la base de datos.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al eliminar el registro de la base de datos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            else
            {
                MessageBox.Show("Por favor, seleccione una fila primero.", "Sin selección", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnBorrarHistorialPaciente_Click(object sender, EventArgs e)
        {
            EliminarHistorialSesionPaciente();
        }

        private void btnCerrarPagePacientes_Click(object sender, EventArgs e)
        {
            tabControlVentanas.TabPages.Remove(tabPPacientes);
        }

        private void btnCerrarPagePsicologos_Click(object sender, EventArgs e)
        {
            tabControlVentanas.TabPages.Remove(tabPPsicologos);
        }

        private void btnCerrarPagePsiquiatras_Click(object sender, EventArgs e)
        {
            tabControlVentanas.TabPages.Remove(tabPPsiquiatras);
        }

        private void btnCerrarPageChat_Click(object sender, EventArgs e)
        {
            tabControlVentanas.TabPages.Remove(tabPChat);
        }

        private void btnCerrarApp_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }



        private void btnCerrarSesion_Click(object sender, EventArgs e)
        {
            inicioSesion inicio = new inicioSesion();
            inicio.ShowDialog();
            this.Hide();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }


        //CRUD psiquiatra

        //Crear psiquiatra
        private async Task CrearPsiquiatra()
        {
            if (ValidarDNI(textBoxNIFPsicologo.Text))
            {
                var psiquiatra = new PsiquiatrasPOJO
                {
                    NumeroLicencia = textBoxLicenciaPsiquiatra.Text,
                    Nombre = textBoxNombrePsiquiatra.Text,
                    Apellidos = textBoxApellidosPsiquiatra.Text,
                    Edad = int.Parse(textBoxEdadPsiquiatra.Text),
                    Direccion = textBoxDireccionPsiquiatra.Text,
                    Poblacion = textBoxPoblacionPsiquiatra.Text,
                    Provincia = textBoxProvinciaPsiquiatra.Text,
                    Pais = textBoxPaisPsiquiatra.Text,
                    NIF = textBoxNIFPsiquiatra.Text,
                    Correo = textBoxEmailPsiquiatra.Text,
                    Especialidad = textBoxEspecialidadPsiquiatra.Text,
                    HorariosAtencion = textBoxObservacionesPsiquiatra.Text,
                    Tarifas = int.Parse(textBoxPrecioSesionPsiquiatra.Text),
                    Telefono1 = textBoxTelefono1Psiquiatra.Text,
                    Telefono2 = textBoxTelefono2Psiquiatra.Text,
                    DireccionConsultorio = textBoxDireccionConsultorioPsiquiatra.Text,
                };

                try
                {
                    var result = await firebaseClient.Child("psicologos").PostAsync(psiquiatra);

                    MessageBox.Show("Psiquiatra creado con éxito. ID del nuevo nodo: " + result.Key, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    crearpsiquiatra = true;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al crear el psiquiatra: " + ex.Message, "¡Nos hemos topado con una barricada!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    crearpsiquiatra = false;
                }
            } else
            {
                labelNIFPsiquiatra.Text = "NIF incorrecto. Ej: 12345678X";
            }
        }

        //Registrar psicologo
        private async Task registrarPsiquiatra(string email, string password, string nombre, string apellidos)
        {
            if (ValidarDNI(textBoxNIFPsicologo.Text))
            {
                if (crearpsiquiatra == true)
                {

                    try
                    {
                        var content = new StringContent($"{{\"email\":\"{email}\",\"password\":\"{password}\",\"returnSecureToken\":true}}", Encoding.UTF8, "application/json");
                        var response = await client.PostAsync($"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={API_KEY}", content);

                        if (response.IsSuccessStatusCode)
                        {
                            var usuario = new UsuariosPOJO
                            {
                                Nombre = nombre,
                                Apellidos = apellidos,
                                Correo = email,
                                Rol = "Psiquiatra"
                            };

                            await firebaseClient.Child("usuarios").PostAsync(usuario);

                            MessageBox.Show("Usuario registrado correctamente.\n\nUsuario: " + email + "\nContraseña: " + password);
                        }
                        else
                        {
                            var errorMessage = await response.Content.ReadAsStringAsync();
                            MessageBox.Show("Error: " + errorMessage);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                    }
                }
                else
                {

                }
            } else
            {
                labelNIFPsiquiatra.Text = "NIF incorrecto. Ej: 12345678X";
            }
        }

        //Modificar psiquiatra
        private async Task ActualizarPsiquiatra(string nlicencia, string nombre, string apellidos, int edad, string direccion, string poblacion, string provincia, string pais, string nif, string correo, string especialidad,
            string horariosatencion, string tarifas, string telefono1, string telefono2, string direccionconsultorio)
        {

            if (ValidarDNI(textBoxNIFPsiquiatra.Text) == true)
            {
                try
                {
                    var nuevoPsiquiatra = new PsiquiatrasPOJO
                    {
                        NumeroLicencia = nlicencia,
                        Nombre = nombre,
                        Apellidos = apellidos,
                        Edad = edad,
                        Direccion = direccion,
                        Poblacion = poblacion,
                        Provincia = provincia,
                        Pais = pais,
                        NIF = nif,
                        Correo = correo,
                        Especialidad = especialidad,
                        HorariosAtencion = horariosatencion,
                        Tarifas = int.Parse(tarifas),
                        Telefono1 = telefono1,
                        Telefono2 = telefono2,
                        DireccionConsultorio = direccionconsultorio
                    };

                    await firebaseClient.Child("psiquiatras").Child(nif).PutAsync(nuevoPsiquiatra);

                    var usuarioSnapshot = await firebaseClient.Child("usuarios").OrderBy("NIF").EqualTo(nif).OnceAsync<UsuariosPOJO>();
                    var usuario = usuarioSnapshot.FirstOrDefault();

                    if (usuario != null)
                    {
                        usuario.Object.Nombre = nombre;
                        usuario.Object.Apellidos = apellidos;
                        usuario.Object.Correo = correo;
                        usuario.Object.Rol = "Psiquiatra";

                        await firebaseClient.Child("usuarios").Child(usuario.Key).PutAsync(usuario.Object);
                    }
                    else
                    {
                        MessageBox.Show("No se encontró el usuario correspondiente al psiquiatra.");
                    }

                    MessageBox.Show("Psiquiatra actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al actualizar el psiquiatra: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                labelNIFPsicologo.Text = "NIF inválido. Ej: 12345678X";
            }
        }

        private async Task eliminarPsiquiatra(string licencia)
        {
            try
            {
                var psiquiatras = await firebaseClient.Child("psiquiatras").OrderBy("NumeroLicencia").EqualTo(licencia).OnceAsync<Dictionary<string, object>>();

                if (psiquiatras.Any())
                {
                    var psiquiatrasKey = psiquiatras.First().Key;

                    await firebaseClient.Child("psicologos").Child(psiquiatrasKey).DeleteAsync();

                    var usuarios = await firebaseClient.Child("usuarios").OrderBy("Correo").EqualTo(CorreoUsuarioSeleccionado).OnceAsync<UsuariosPOJO>();
                    var usuarioKey = usuarios.FirstOrDefault()?.Key;

                    if (usuarioKey != null)
                    {
                        await firebaseClient.Child("usuarios").Child(usuarioKey).DeleteAsync();
                    }

                    MessageBox.Show("Psiquiatra eliminado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    limpiarLabelsPsicologo();
                    limpiarCamposPsicologos();
                    botonesInicioPsicologo();
                }
                else
                {
                    MessageBox.Show("No se encontró el psiquiatra con el número de licencia proporcionado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    btnLimpiarCamposPsicologo.Enabled = true;
                    btnCancelarPsicologo.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al eliminar el psiquiatra: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnLimpiarCamposPsicologo.Enabled = true;
                btnCancelarPsicologo.Enabled = true;
            }
        }


        private void btnEliminarPsiquiatra_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxLicenciaPsiquiatra.Text))
            {
                var confirmarEliminar = MessageBox.Show("¿Estás seguro que deseas eliminar este psiquiatra?", "¿Confirmar?", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmarEliminar == DialogResult.Yes)
                {
                    eliminarPsiquiatra(textBoxLicenciaPsiquiatra.Text);
                }
            }


        }

        private void btnCrearPsiquiatraToolStrip_Click(object sender, EventArgs e)
        {
            textBoxLicenciaPsiquiatra.Enabled = true;
            textBoxLicenciaPsiquiatra.Focus();
            accionPsiquiatras = "Crear";
            btnCrearPsiquiatraToolStrip.Enabled = false;
            btnGuardarPsiquiatraToolStrip.Enabled = true;
            btnCancelarPsiquiatraToolStrip.Enabled = true;
            btnLimpiarCamposPsiquiatra.Enabled = true;
        }

        private void btnModificarPsiquiatraToolStrip_Click(object sender, EventArgs e)
        {
            accionPsiquiatras = "Modificar";
            textBoxNIFPsiquiatra.Enabled = false;
            btnModificarPsiquiatraToolStrip.Enabled = false;
            btnGuardarPsiquiatraToolStrip.Enabled = true;
            btnCancelarPsiquiatraToolStrip.Enabled = true;
            btnLimpiarCamposPsiquiatra.Enabled = true;
            btnEliminarPsiquiatra.Enabled = true;
        }

        private async void btnGuardarPsiquiatraToolStrip_Click(object sender, EventArgs e)
        {
            if (accionPsiquiatras.Equals("Crear"))
            {
                if (!string.IsNullOrEmpty(textBoxNIFPsiquiatra.Text))
                {
                    await CrearPsiquiatra();

                    if (crearpsiquiatra.Equals(true))
                    {
                        await registrarPsiquiatra(textBoxEmailPsiquiatra.Text, textBoxNIFPsiquiatra.Text, textBoxNombrePsiquiatra.Text, textBoxApellidosPsiquiatra.Text);
                        botonesInicioPsiquiatra();
                        limpiarLabelsPsiquiatra();
                        limpiarCamposPsiquiatras();
                    }
                }
                else
                {
                    labelNIFPsiquiatra.Text = "El NIF no puede estar vacío";
                }
            }
            else if (accionPsiquiatras.Equals("Modificar"))
            {
                await ActualizarPsiquiatra(textBoxLicenciaPsiquiatra.Text, textBoxNombrePsiquiatra.Text, textBoxApellidosPsiquiatra.Text, int.Parse(textBoxEdadPsiquiatra.Text), textBoxDireccionPsiquiatra.Text, textBoxPoblacionPsiquiatra.Text, textBoxProvinciaPsiquiatra.Text, textBoxPaisPsiquiatra.Text, textBoxNIFPsiquiatra.Text, textBoxEmailPsiquiatra.Text, textBoxEspecialidadPsiquiatra.Text, textBoxObservacionesPsiquiatra.Text, textBoxPrecioSesionPsiquiatra.Text, textBoxTelefono1Psiquiatra.Text, textBoxTelefono2Psiquiatra.Text, textBoxDireccionConsultorioPsiquiatra.Text);
            }
        }

        private void btnCancelarPsiquiatraToolStrip_Click(object sender, EventArgs e)
        {
            botonesInicioPsiquiatra();
            limpiarLabelsPsiquiatra();
            limpiarCamposPsiquiatras();
            textBoxNIFPsiquiatra.Enabled = false;
        }

        private void btnBuscarPsiquiatraToolStrip_Click(object sender, EventArgs e)
        {
            FormBuscarPsiquiatra buscar = new FormBuscarPsiquiatra();
            buscar.licenciaPsiquiatraBusqueda += FormBuscarPsiquiatra_licenciaPsiquiatraBusqueda;
            buscar.ShowDialog();

            btnCrearPsiquiatraToolStrip.Enabled = false;
            btnModificarPsiquiatraToolStrip.Enabled = true;
            btnGuardarPsiquiatraToolStrip.Enabled = false;
            btnCancelarPsiquiatraToolStrip.Enabled = false;
            btnEliminarPsiquiatra.Enabled = false;
            btnBuscarPsiquiatraToolStrip.Enabled = true;
            btnLimpiarCamposPsiquiatra.Enabled = true;
            btnReportesPsiquiatraToolStrip.Enabled = true;
        }

        private void FormBuscarPsiquiatra_licenciaPsiquiatraBusqueda(object? sender, string licencia)
        {
            licenciaPsiquiatraSeleccionado = licencia;
            MostrarDatosPsiquiatra(licencia);
            bloquearTextBoxPsiquiatras();

            btnCrearPsiquiatraToolStrip.Enabled = false;
            btnModificarPsiquiatraToolStrip.Enabled = true;
            btnGuardarPsiquiatraToolStrip.Enabled = true;
            btnCancelarPsiquiatraToolStrip.Enabled = true;
            btnEliminarPsiquiatra.Enabled = false;
            btnBuscarPsiquiatraToolStrip.Enabled = true;
            btnLimpiarCamposPsiquiatra.Enabled = true;
            btnReportesPsiquiatraToolStrip.Enabled = true;
        }

        private async Task MostrarDatosPsiquiatra(string licencia)
        {
            try
            {
                if (!string.IsNullOrEmpty(licencia))
                {
                    var psiquiatras = await firebaseClient.Child("psiquiatras")
                        .OrderBy("NumeroLicencia")
                        .EqualTo(licencia)
                        .OnceAsync<Dictionary<string, object>>();

                    if (psiquiatras.Any())
                    {
                        var psicologoData = psiquiatras.First().Object;

                        textBoxLicenciaPsiquiatra.Text = psicologoData["NumeroLicencia"].ToString();
                        textBoxNIFPsiquiatra.Text = psicologoData["NIF"].ToString();
                        textBoxNombrePsiquiatra.Text = psicologoData["Nombre"].ToString();
                        textBoxApellidosPsiquiatra.Text = psicologoData["Apellidos"].ToString();
                        textBoxEmailPsiquiatra.Text = psicologoData["Correo"].ToString();
                        textBoxDireccionPsiquiatra.Text = psicologoData["Direccion"].ToString();
                        textBoxPoblacionPsiquiatra.Text = psicologoData["Poblacion"].ToString();
                        textBoxProvinciaPsiquiatra.Text = psicologoData["Provincia"].ToString();
                        textBoxPaisPsiquiatra.Text = psicologoData["Pais"].ToString();
                        textBoxEdadPsiquiatra.Text = psicologoData["Edad"].ToString();
                        textBoxTelefono1Psiquiatra.Text = psicologoData["Telefono1"].ToString();
                        textBoxTelefono2Psiquiatra.Text = psicologoData["Telefono2"].ToString();
                        textBoxEspecialidadPsiquiatra.Text = psicologoData["Especialidad"].ToString();
                        textBoxObservacionesPsiquiatra.Text = psicologoData["HorariosAtencion"].ToString();
                        textBoxPrecioSesionPsiquiatra.Text = psicologoData["Tarifas"].ToString();
                        textBoxDireccionConsultorioPsiquiatra.Text = psicologoData["DireccionConsultorio"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("No se encontraron datos para el psiquiatra con el número de licencia proporcionado.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("El número de licencia proporcionado está vacío.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los datos del psiquiatra: " + ex.Message, "¡Nos hemos topado con una barricada!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBoxLicenciaPsiquiatra_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBoxLicenciaPsiquiatra.Text))
            {
                desbloquearTextBoxPsiquiatras();
            }
            else
            {
                bloquearTextBoxPsiquiatras();
                textBoxLicenciaPsiquiatra.Enabled = true;
                textBoxLicenciaPsiquiatra.Focus();
            }
        }

        private void btnMindCareInfo_Click(object sender, EventArgs e)
        {
            infoMindCare info = new infoMindCare();

            info.ShowDialog();
        }


        string error = "";
        bool nifPacOK = false;
        bool nombrePacOK = false;
        bool apellidosPacOK = false;
        bool emailPacOK = false;
        bool dirPacOK = false;
        bool pobPacOK = false;
        bool provPacOK = false;
        bool paisPacOK = false;
        bool edadPacOK = false;
        bool tel1PacOK = false;
        bool accionPacOK = false;
        

        private void validarPaciente()
        {
            string nifPaciente = textBoxDNI.Text;
            string nombrePaciente = textBoxNombre.Text;
            string apellidosPaciente = textBoxApellidos.Text;
            string emailPaciente = textBoxEmail.Text;
            string direccionPaciente = textBoxDireccion.Text;
            string poblacionPaciente = textBoxPoblacion.Text;
            string provinciaPaciente = textBoxProvincia.Text;
            string paisPaciente = textBoxPais.Text;
            string edadPaciente = textBoxEdad.Text;
            string telefono1 = textBoxTelefono.Text;
            string telefono2 = textBoxTelefono2.Text;

            error = "Se han encontrado los siguientes errores:";

            nifPacOK = ValidarLongitud(nifPaciente, 9, 9, "La longitud del NIF debe ser de 9 caracteres.") && ValidarDNI(nifPaciente);
            nombrePacOK = ValidarLongitud(nombrePaciente, 1, 30, "La longitud del nombre debe ser mayor que 1 y menor que 31.");
            apellidosPacOK = ValidarLongitud(apellidosPaciente, 1, 30, "La longitud del nombre debe ser mayor que 1 y menor que 31.");
            emailPacOK = ValidarLongitud(emailPaciente, 10, 40, "La longitud del email debe ser mayor que 5 y menor que 41.") && ComprobarFormatoEmail(emailPaciente);
            dirPacOK = ValidarLongitud(direccionPaciente, 1, 50, "La longitud de la dirección debe ser mayor que 1 y menor que 51.");
            pobPacOK = ValidarLongitud(poblacionPaciente, 1, 20, "La longitud de la población debe ser mayor que 1 y menor que 21.");
            provPacOK = ValidarLongitud(provinciaPaciente, 1, 20, "La longitud de la provincia debe ser mayor que 1 y menor que 21.");
            paisPacOK = ValidarLongitud(paisPaciente, 1, 20, "La longitud del pais debe ser mayor que 1 y menor que 21");
            edadPacOK = ValidarLongitud(edadPaciente, 1, 3, "La edad debe ser de 0 a 100");
            tel1PacOK = ValidarNumero(telefono1, 9, "La longitud del numero debe ser de 9 digitos");

            if (nifPacOK && nombrePacOK && apellidosPacOK && emailPacOK && dirPacOK && pobPacOK && provPacOK && paisPacOK && edadPacOK && tel1PacOK)
            {
                limpiarLabelsPacientesPaciente();
                accionPacOK = true;
            }
            else
            {
                accionPacOK = false;
            }

            if (nifPacOK == false)
            {
                labelDNIPaciente.Text = "Longitud entre 1 y 10 caracteres";
            }
            else
            {
                labelDNIPaciente.Text = "";
            }

            if (nombrePacOK == false)
            {
                labelNombrePaciente.Text = "Longitud entre 1 y 30.";
            }
            else
            {
                labelNombrePaciente.Text = "";
            }

            if (apellidosPacOK == false)
            {
                labelApellidosPaciente.Text = "Longitud entre 1 y 30.";
            }
            else
            {
                labelApellidosPaciente.Text = "";
            }

            if (pobPacOK == false)
            {
                labelPoblacionPaciente.Text = "Longitud entre 1 y 20.";
            }
            else
            {
                labelPoblacionPaciente.Text = "";
            }

            if (dirPacOK == false)
            {
                labelDireccionPaciente.Text = "Longitud entre 1 y 50.";
            }
            else
            {
                labelDireccionPaciente.Text = "";
            }

            if (provPacOK == false)
            {
                labelProvinciaPaciente.Text = "Longitud entre 1 y 20.";
            }
            else
            {
                labelProvinciaPaciente.Text = "";
            }

            if (paisPacOK == false)
            {
                labelPaisPaciente.Text = "Longitud entre 1 y 20.";
            }
            else
            {
                labelPaisPaciente.Text = "";
            }

            if (tel1PacOK == false)
            {
                labelTelefonoPaciente.Text = "Longitud debe tener 9 números enteros";
            }
            else
            {
                labelTelefonoPaciente.Text = "";
            }

            if (emailPacOK == false)
            {
                labelCorreoPaciente.Text = "Formato: ejemplo@gmail.com";
            }
            else
            {
                labelCorreoPaciente.Text = "";
            }
        }

        //Validar psicologo

        bool nLicenciaOK = false;
        bool nifPsicoOK = false;
        bool nombrePsicoOK = false;
        bool apellidosPsicoOK = false;
        bool emailPsicoOK = false;
        bool dirPsicoOK = false;
        bool pobPsicoOK = false;
        bool provPsicoOK = false;
        bool paisPsicoOK = false;
        bool edadPsicoOK = false;
        bool tel1PsicoOK = false;
        bool especPsicoOK = false;
        bool obserPsicoOK = false;
        bool dirConsPsicoOK = false;
        bool tarifasPsicoOK = false;
        bool accionPsicoOK = false;


        private void validarPsicologo()
        {
            string numLicenciaPsicolog = textBoxLicenciaPsicologo.Text;
            string nifPsicologo = textBoxNIFPsicologo.Text;
            string nombrePsicologo = textBoxNombrePsicologo.Text;
            string apellidosPsicologo = textBoxApellidosPsicologo.Text;
            string emailPsicologo = textBoxEmailPsicologo.Text;
            string direccionPsicologo = textBoxDireccionPsicologo.Text;
            string poblacionPsicologo = textBoxPoblacionPsicologo.Text;
            string provinciaPsicologo = textBoxProvinciaPsicologo.Text;
            string paisPsicologo = textBoxPaisPsicologo.Text;
            string edadPsicologo = textBoxEdadPsicologo.Text;
            string telefono1 = textBoxTelefono1Psicologo.Text;
            string telefono2 = textBoxTelefono2Psicologo.Text;
            string especialidadPsicologo = textBoxEspecialidadPsicologo.Text;
            string observacionesPsicologo = textBoxObservacionesPsicologo.Text;
            string dirConsPsicologo = textBoxDirConsPsicologo.Text;
            string tarifaPsicologo = textBoxPrecioPsicologo.Text;


            error = "Se han encontrado los siguientes errores:";

            nLicenciaOK = ValidarLongitud(numLicenciaPsicolog, 7, 9, "La longitud del NIF debe ser de 7 a 9 caracteres.");
            nifPsicoOK = ValidarLongitud(nifPsicologo, 9, 9, "La longitud del NIF debe ser de 9 caracteres.") && ValidarDNI(nifPsicologo);
            nombrePsicoOK = ValidarLongitud(nombrePsicologo, 1, 30, "La longitud del nombre debe ser mayor que 1 y menor que 31.");
            apellidosPsicoOK = ValidarLongitud(apellidosPsicologo, 1, 30, "La longitud del nombre debe ser mayor que 1 y menor que 31.");
            emailPsicoOK = ValidarLongitud(emailPsicologo, 10, 40, "La longitud del email debe ser mayor que 5 y menor que 41.") && ComprobarFormatoEmail(emailPsicologo);
            dirPsicoOK = ValidarLongitud(direccionPsicologo, 1, 50, "La longitud de la dirección debe ser mayor que 1 y menor que 51.");
            pobPsicoOK = ValidarLongitud(poblacionPsicologo, 1, 20, "La longitud de la población debe ser mayor que 1 y menor que 21.");
            provPsicoOK = ValidarLongitud(provinciaPsicologo, 1, 20, "La longitud de la provincia debe ser mayor que 1 y menor que 21.");
            paisPsicoOK = ValidarLongitud(paisPsicologo, 1, 20, "La longitud del pais debe ser mayor que 1 y menor que 21");
            edadPsicoOK = ValidarLongitud(edadPsicologo, 1, 3, "La edad debe ser de 0 a 100");
            tel1PsicoOK = ValidarNumero(telefono1, 9, "La longitud del numero debe ser de 9 digitos");
            especPsicoOK = ValidarLongitud(especialidadPsicologo, 1, 30, "La longitud del nombre debe ser mayor que 1 y menor que 31.");
            dirConsPsicoOK = ValidarLongitud(dirConsPsicologo, 1, 30, "La longitud del nombre debe ser mayor que 1 y menor que 31.");
            tarifasPsicoOK = ValidarLongitud(tarifaPsicologo, 1, 30, "La longitud del nombre debe ser mayor que 1 y menor que 31.");

            if (nLicenciaOK && nifPsicoOK && nombrePsicoOK && apellidosPsicoOK && emailPsicoOK && dirPsicoOK && pobPsicoOK && provPsicoOK && paisPsicoOK && edadPsicoOK && tel1PsicoOK && especPsicoOK && dirConsPsicoOK && tarifasPsicoOK)
            {
                limpiarLabelsPsicologo();
                accionPsicoOK = true;
            }
            else
            {
                accionPsicoOK = false;
            }

            if (nLicenciaOK == false)
            {
                labelNLicenciaPsicologo.Text = "Longitud entre 7 y 9 caracteres";
            }
            else
            {
                labelNLicenciaPsicologo.Text = "";
            }

            if (nifPsicoOK == false)
            {
                labelNIFPsicologo.Text = "Ej: 12345678X.";
            }
            else
            {
                labelNIFPsicologo.Text = "";
            }

            if (nombrePsicoOK == false)
            {
                labelNIFPsicologo.Text = "Longitud entre 1 y 30.";
            }
            else
            {
                labelNIFPsicologo.Text = "";
            }

            if (apellidosPsicoOK == false)
            {
                labelApellidosPsicologo.Text = "Longitud entre 1 y 20.";
            }
            else
            {
                labelApellidosPsicologo.Text = "";
            }

            if (dirPsicoOK == false)
            {
                labelDireccionPsicologo.Text = "Longitud entre 1 y 50.";
            }
            else
            {
                labelDireccionPsicologo.Text = "";
            }

            if (provPsicoOK == false)
            {
                labelProvinciaPsicologo.Text = "Longitud entre 1 y 20.";
            }
            else
            {
                labelProvinciaPsicologo.Text = "";
            }

            if (paisPsicoOK == false)
            {
                labelPaisPsicologo.Text = "Longitud entre 1 y 20.";
            }
            else
            {
                labelPaisPsicologo.Text = "";
            }

            if (tel1PsicoOK == false)
            {
                labelTelefonoPsicologo.Text = "Longitud debe tener 9 números enteros";
            }
            else
            {
                labelTelefonoPsicologo.Text = "";
            }

            if (emailPsicoOK == false)
            {
                labelEmailPsicologo.Text = "Formato: ejemplo@gmail.com";
            }
            else
            {
                labelEmailPsicologo.Text = "";
            }

            if (especPsicoOK == false)
            {
                labelEspecialidadPsicologo.Text = "Longitud invalida";
            }
            else
            {
                labelEspecialidadPsicologo.Text = "";
            }

            if (dirConsPsicoOK == false)
            {
                labelDirConsPsicologo.Text = "Longitud inválida";
            }
            else
            {
                labelDirConsPsicologo.Text = "";
            }

            if (tarifasPsicoOK == false)
            {
                labelTarifaPsicologo.Text = "Longitud inválida";
            } 
            else
            {
                labelTarifaPsicologo.Text = "";
            }

        }


        //Validar psiquiatra
        bool nLicenciaPsiquiOK = false;
        bool nifPsiquiOK = false;
        bool nombrePsiquiOK = false;
        bool apellidosPsiquiOK = false;
        bool emailPsiquiOK = false;
        bool dirPsiquiOK = false;
        bool pobPsiquiOK = false;
        bool provPsiquiOK = false;
        bool paisPsiquiOK = false;
        bool edadPsiquiOK = false;
        bool tel1PsiquiOK = false;
        bool especPsiquiOK = false;
        bool obserPsiquiOK = false;
        bool dirConsPsiquiOK = false;
        bool tarifasPsiquiOK = false;
        bool accionPsiquiOK = false;


        private void validarPsiquiatra()
        {
            string numLicenciaPsiquiatra = textBoxLicenciaPsiquiatra.Text;
            string nifPsiquiatrao = textBoxNIFPsiquiatra.Text;
            string nombrePsiquiatra = textBoxNombrePsiquiatra.Text;
            string apellidosPsiquiatra = textBoxApellidosPsiquiatra.Text;
            string emailPsiquiatra = textBoxEmailPsiquiatra.Text;
            string direccionPsiquiatra = textBoxDireccionPsiquiatra.Text;
            string poblacionPsiquiatra = textBoxPoblacionPsiquiatra.Text;
            string provinciaPsiquiatra = textBoxProvinciaPsiquiatra.Text;
            string paisPsiquiatra = textBoxPaisPsiquiatra.Text;
            string edadPsiquiatra = textBoxEdadPsiquiatra.Text;
            string telefono1 = textBoxTelefono1Psiquiatra.Text;
            string especialidadPsiquiatra = textBoxEspecialidadPsiquiatra.Text;
            string dirConsPsiquiatra = textBoxDireccionConsultorioPsiquiatra.Text;
            string tarifaPsiquiatra = textBoxPrecioSesionPsiquiatra.Text;


            error = "Se han encontrado los siguientes errores:";

            nLicenciaOK = ValidarLongitud(numLicenciaPsiquiatra, 7, 9, "La longitud del NIF debe ser de 7 a 9 caracteres.");
            nifPsicoOK = ValidarLongitud(nifPsiquiatrao, 9, 9, "La longitud del NIF debe ser de 9 caracteres.") && ValidarDNI(nifPsiquiatrao);
            nombrePsicoOK = ValidarLongitud(nombrePsiquiatra, 1, 30, "La longitud del nombre debe ser mayor que 1 y menor que 31.");
            apellidosPsicoOK = ValidarLongitud(apellidosPsiquiatra, 1, 30, "La longitud del nombre debe ser mayor que 1 y menor que 31.");
            emailPsicoOK = ValidarLongitud(emailPsiquiatra, 10, 40, "La longitud del email debe ser mayor que 5 y menor que 41.") && ComprobarFormatoEmail(emailPsiquiatra);
            dirPsicoOK = ValidarLongitud(direccionPsiquiatra, 1, 50, "La longitud de la dirección debe ser mayor que 1 y menor que 51.");
            pobPsicoOK = ValidarLongitud(poblacionPsiquiatra, 1, 20, "La longitud de la población debe ser mayor que 1 y menor que 21.");
            provPsicoOK = ValidarLongitud(provinciaPsiquiatra, 1, 20, "La longitud de la provincia debe ser mayor que 1 y menor que 21.");
            paisPsicoOK = ValidarLongitud(paisPsiquiatra, 1, 20, "La longitud del pais debe ser mayor que 1 y menor que 21");
            edadPsicoOK = ValidarLongitud(edadPsiquiatra, 1, 3, "La edad debe ser de 0 a 100");
            tel1PsicoOK = ValidarNumero(telefono1, 9, "La longitud del numero debe ser de 9 digitos");
            especPsicoOK = ValidarLongitud(especialidadPsiquiatra, 1, 30, "La longitud del nombre debe ser mayor que 1 y menor que 31.");
            dirConsPsicoOK = ValidarLongitud(dirConsPsiquiatra, 1, 30, "La longitud del nombre debe ser mayor que 1 y menor que 31.");
            tarifasPsicoOK = ValidarLongitud(tarifaPsiquiatra, 1, 30, "La longitud del nombre debe ser mayor que 1 y menor que 31.");

            if (nLicenciaOK && nifPsicoOK && nombrePsicoOK && apellidosPsicoOK && emailPsicoOK && dirPsicoOK && pobPsicoOK && provPsicoOK && paisPsicoOK && edadPsicoOK && tel1PsicoOK && especPsicoOK && dirConsPsicoOK && tarifasPsicoOK)
            {
                limpiarLabelsPsiquiatra();
                accionPsiquiOK = true;
            }
            else
            {
                accionPsiquiOK = false;
            }

            if (nLicenciaOK == false)
            {
                labelLicenciaPsiquiatra.Text = "Longitud entre 7 y 9 caracteres";
            }
            else
            {
                labelLicenciaPsiquiatra.Text = "";
            }

            if (nifPsicoOK == false)
            {
                labelNIFPsiquiatra.Text = "Ej: 12345678X.";
            }
            else
            {
                labelNIFPsiquiatra.Text = "";
            }

            if (nombrePsiquiOK == false)
            {
                labelNombrePsiquiatra.Text = "Longitud entre 1 y 30.";
            }
            else
            {
                labelNombrePsiquiatra.Text = "";
            }

            if (apellidosPsiquiOK == false)
            {
                labelApellidosPsiquiatra.Text = "Longitud entre 1 y 20.";
            }
            else
            {
                labelApellidosPsiquiatra.Text = "";
            }

            if (dirPsiquiOK == false)
            {
                labelDireccionPsiquiatra.Text = "Longitud entre 1 y 50.";
            }
            else
            {
                labelDireccionPsiquiatra.Text = "";
            }

            if (provPsiquiOK == false)
            {
                labelProvinciaPsiquiatra.Text = "Longitud entre 1 y 20.";
            }
            else
            {
                labelProvinciaPsiquiatra.Text = "";
            }

            if (paisPsiquiOK == false)
            {
                labelPaisPsiquiatra.Text = "Longitud entre 1 y 20.";
            }
            else
            {
                labelPaisPsiquiatra.Text = "";
            }

            if (tel1PsiquiOK == false)
            {
                labelTelefono1Psiquiatra.Text = "Longitud debe tener 9 números enteros";
            }
            else
            {
                labelTelefono1Psiquiatra.Text = "";
            }

            if (emailPsicoOK == false)
            {
                labelEmailPsiquiatra.Text = "Formato: ejemplo@gmail.com";
            }
            else
            {
                labelEmailPsiquiatra.Text = "";
            }

            if (especPsiquiOK == false)
            {
                labelEspecialidadPsiquiatra.Text = "Longitud invalida";
            }
            else
            {
                labelEspecialidadPsiquiatra.Text = "";
            }

            if (dirConsPsiquiOK == false)
            {
                labelDireccionConsultorioPsiquiatra.Text = "Longitud inválida";
            }
            else
            {
                labelDireccionConsultorioPsiquiatra.Text = "";
            }

            if (tarifasPsiquiOK == false)
            {
                labelPrecioSesionPsiquiatra.Text = "Longitud inválida";
            }
            else
            {
                labelPrecioSesionPsiquiatra.Text = "";
            }

        }






        //Validadores
        private bool ValidarLongitud(string valor, int min, int max, string mensajeError)
        {
            int longitud = valor.Length;
            if (longitud >= min && longitud <= max)
            {
                return true;
            }
            else
            {
                error += "\n   - " + mensajeError + " Actualmente: " + longitud + ".";
                return false;
            }
        }

        private bool ValidarNumero(string valor, int longitud, string mensajeError)
        {
            if (valor.Length == longitud && valor.All(char.IsDigit))
            {
                return true;
            }
            else
            {
                error += "\n   - " + mensajeError;
                return false;
            }
        }


        public static bool ComprobarFormatoEmail(String emailCliente)
        {
            String sFormato;
            sFormato = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(emailCliente, sFormato))
            {
                if (Regex.Replace(emailCliente, sFormato, String.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }






    }
}

