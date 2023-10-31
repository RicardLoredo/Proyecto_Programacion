using Models.DTO;
using Proyecto_Programacion.Models;
using Proyecto_Programacion.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proyecto_Programacion
{
    public partial class AgregarMascota : Form
    {
        private int? _id;


        public AgregarMascota(int id) : this()
        {
            _id = id;
        }

        public AgregarMascota()
        {
            InitializeComponent();
        }

        private void AregarMascota_Load(object sender, EventArgs e)
        {
        }

        private async void btnAgregarDueño_Click(object sender, EventArgs e)
        {
            ConexionApi api = new ConexionApi();

            DtoAgregarDueño pDueño = new DtoAgregarDueño();
            pDueño.Nombre = txtNombre.Text;
            pDueño.Apellido_paterno = txtPaterno.Text;
            pDueño.Apellido_materno = txtMaterno.Text;
            pDueño.Telefono = txtTelefono.Text;
            pDueño.Direccion = txtDireccion.Text;

            var resultadoDueño = await api.InsertarDueño(pDueño);

            if (string.IsNullOrEmpty(resultadoDueño.Error))
            {
                MessageBox.Show("Dueño insertado correctamente");

                Mascotas formMascotas = Application.OpenForms.OfType<Mascotas>().FirstOrDefault();

                if (formMascotas != null)
                {
                    formMascotas.MostrarDueños();
                }
            }
            else
            {
                MessageBox.Show("Error al insertar dueño: " + resultadoDueño.Error);
            }


        }


        private async void btnAgregarMascota_Click(object sender, EventArgs e)
        {
            ConexionApi api = new ConexionApi();

            DTOAgregarMascotas pMascotas = new DTOAgregarMascotas();
            pMascotas.IdDueño = Convert.ToInt32(_id);
            pMascotas.Nombre_mascota = txtNombre_Mascota.Text;
            pMascotas.Edad_mascota = txtedad_mascota.Text;
            pMascotas.Sexo_mascota = txtSexo.Text;
            pMascotas.Especie_mascota = txtEspecie.Text;
            pMascotas.Raza_mascota = txtRaza.Text;
            pMascotas.Color_mascota = txtcolor_mascota.Text;

            if (ImagenMascota.BackgroundImage != null)
            {
                MemoryStream bytes = new MemoryStream();
                ImagenMascota.BackgroundImage.Save(bytes, ImagenMascota.BackgroundImage.RawFormat);
                pMascotas.Imagen_mascota = bytes.ToArray();
            }
            else
            {
                string imagen_default = (@"C:\\Users\\mario\\OneDrive\\Escritorio\\Proyecto_Programacion\\Proyecto_Programacion\\Resources\\predeterminada.jpg");
                if (File.Exists(imagen_default))
                {
                    byte[] imagen_default_bytes = File.ReadAllBytes(imagen_default);
                    pMascotas.Imagen_mascota = imagen_default_bytes;
                }
            }
            var resultado_mascota = await api.InsertarMascotas(pMascotas);
            if (string.IsNullOrEmpty(resultado_mascota.Error_mascota))
            {
                MessageBox.Show("Mascota insertado correctamente");
                this.Close();
            }
            else
            {
                MessageBox.Show("Error al insertar mascota: " + resultado_mascota.Error_mascota);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog imagenbuscada = new OpenFileDialog();
            DialogResult imagen_resultado = imagenbuscada.ShowDialog();
            if (imagen_resultado == DialogResult.OK)
            {
                ImagenMascota.BackgroundImage = Image.FromFile(imagenbuscada.FileName);
            }
        }
    }
}
