using Models.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto_Programacion
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {


            ConexionApi api = new ConexionApi();
            DtoUsuario pUsuario = new DtoUsuario();
            pUsuario.Usuario = txtUsuario.Text;
            pUsuario.Contraseña = txtContraseña.Text;
            var r = await api.Token(pUsuario);

            if (TokenMaster.Token != null)
            {
                MessageBox.Show("Bienvenido.");
                this.Hide();
                Mascotas formMascotas = new Mascotas();
                formMascotas.Show();
            }
            else
            {
                MessageBox.Show(TokenMaster.Error.ToString());
            }

        }

        private void Login_Load(object sender, EventArgs e)
        {

        }
    }
}
