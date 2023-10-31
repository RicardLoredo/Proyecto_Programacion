using Models.DTO;
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
    public partial class TarjetaMascotaControl : UserControl
    {
        public TarjetaMascotaControl()
        {
            InitializeComponent();
        }

        private async void btnActualizar_Click(object sender, EventArgs e)
        {
            AgregarMascota formMascotas = new AgregarMascota();

            formMascotas.pnlMascota.Visible = true;
            formMascotas.pnlDueño.Visible = false;
            formMascotas.btnAgregarMascota.Visible = false;
            formMascotas.btnActualizarMascota.Visible = true;
            formMascotas.IdMascota.Text = IdMascota.Text;
            formMascotas.txtNombre_Mascota.Text = lblnombre_mascota.Text;
            formMascotas.txtedad_mascota.Text = lblEdad.Text;
            formMascotas.txtEspecie.Text = lblEspecie.Text;
            formMascotas.txtSexo.Text = lblSexo.Text;
            formMascotas.txtRaza.Text = lblrazza.Text;
            formMascotas.txtcolor_mascota.Text = lblColor.Text;
            formMascotas.ImagenMascota.BackgroundImage = pictureBox1.BackgroundImage;

            formMascotas.Show();

            formMascotas.btnActualizarMascota.Click +=async  (senderGuardar, eGuardar) => {

                ConexionApi api = new ConexionApi();

                MemoryStream bytes = new MemoryStream();
                formMascotas.ImagenMascota.BackgroundImage.Save(bytes, formMascotas.ImagenMascota.BackgroundImage.RawFormat);

                int idMascota = int.Parse(formMascotas.IdMascota.Text);

                DtoActualizarMascotas mascota_actualizada = new DtoActualizarMascotas
                {
                    IdMascotas = idMascota,
                    Nombre_mascota = formMascotas.txtNombre_Mascota.Text,
                    Edad_mascota = formMascotas.txtedad_mascota.Text,
                    Sexo_mascota = formMascotas.txtSexo.Text,
                    Especie_mascota = formMascotas.txtEspecie.Text,
                    Raza_mascota = formMascotas.txtRaza.Text,
                    Color_mascota = formMascotas.txtcolor_mascota.Text,
                    Imagen_mascota = bytes.ToArray()
                };


                DtoActualizarMascotas resultado = await api.ActualizarMascotas(mascota_actualizada);


                if (string.IsNullOrEmpty(resultado.Error_mascota))
                {
                    MessageBox.Show("La información dela mascota se ha actualizado correctamente.");
                }
                else
                {
                    MessageBox.Show("Error: " + resultado.Error_mascota);
                }

            };
        }
    }
}
