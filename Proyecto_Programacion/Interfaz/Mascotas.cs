using Models.DTO;
using Proyecto_Programacion.Models;
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
    public partial class Mascotas : Form
    {
        
        public Mascotas()
        {
            InitializeComponent();
        }

        private void btnNuevoRegistro_Click(object sender, EventArgs e)
        {
            AgregarMascota formMascotas = new AgregarMascota();
            formMascotas.pnlMascota.Visible = false;
            formMascotas.btnAgregarMascota.Visible = false;
            formMascotas.pnlDueño.Visible = true;
            formMascotas.btnAgregarDueño.Visible = true;
            formMascotas.ShowDialog(this);

            MostrarDueños();

        }


        public async void MostrarDueños()
        {
            ConexionApi api = new ConexionApi();
            List<DtoMostrarDueños> lstDueño = await api.ObtenerDueños();

            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = lstDueño;
            dataRegistroDueño.DataSource = bindingSource;
            
            dataRegistroDueño.BorderStyle = BorderStyle.None;

            dataRegistroDueño.Columns[2].Visible = false;
            dataRegistroDueño.Columns[4].Visible = false;
            dataRegistroDueño.Columns[5].Visible = false;

            dataRegistroDueño.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dataRegistroDueño.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataRegistroDueño.DefaultCellStyle.BackColor = Color.FromArgb(219, 223, 234);
            dataRegistroDueño.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataRegistroDueño.Columns[3].HeaderText = "Nombre Dueño";

            dataRegistroDueño.Columns[0].Width = 50;
            dataRegistroDueño.Columns[1].Width = 50;
            

            dataRegistroDueño.Columns[0].DisplayIndex = 3;
            dataRegistroDueño.Columns[1].DisplayIndex = 4;
        }

        private void Mascotas_Load_1(object sender, EventArgs e)
        {
            MostrarDueños();
            dataRegistroDueño.DefaultCellStyle.Font = new Font("Arial", 15);
        }

        private async void dataRegistroDueño_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {

                if (dataRegistroDueño.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
                {
                    dataRegistroDueño.CurrentRow.Selected = true;
                    lblTelefono.Text = dataRegistroDueño.Rows[e.RowIndex].Cells[4].FormattedValue.ToString();
                    lbldireccion.Text = dataRegistroDueño.Rows[e.RowIndex].Cells[5].FormattedValue.ToString();
                    lblTelefono.Visible = true;
                    lbldireccion.Visible = true;

                    int idDueño = Convert.ToInt32(dataRegistroDueño.CurrentRow.Cells[2].Value);

                    ConexionApi api = new ConexionApi();
                    List<DtoMostrarMascotas> lstmascotas = await api.MostrarMascota(idDueño);

                    if (lstmascotas != null && lstmascotas.Count > 0)
                    {
                        DtoMostrarMascotas mascotaSeleccionada = lstmascotas[0];


                        cmbMascotas.DataSource = lstmascotas;
                        cmbMascotas.DisplayMember = "Nombre_mascota";


                        MostrarMascotaSeleccionada(mascotaSeleccionada);
                    }
                }

                if (e.RowIndex >= 0 && e.ColumnIndex == dataRegistroDueño.Columns["Delete"].Index)
                {

                    DialogResult result = MessageBox.Show("¿Estás seguro de eliminar al dueño? Todas sus mascotas también serán eliminadas.", "Confirmar eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        int idDueño = Convert.ToInt32(dataRegistroDueño.CurrentRow.Cells[2].Value);
                        ConexionApi api = new ConexionApi();
                        DtoEliminarDueño eliminarDueño = await api.EliminarDueño(idDueño);

                        if (eliminarDueño != null)
                        {
                            MostrarDueños();
                            MessageBox.Show("Dueño eliminado correctamente.", "Eliminación exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }

                if (e.RowIndex >= 0 && e.ColumnIndex == dataRegistroDueño.Columns["Actualizar"].Index)
                {
                    int idDueño = Convert.ToInt32(dataRegistroDueño.CurrentRow.Cells[2].Value);

                    ConexionApi api = new ConexionApi();
                    List<DtoDueñosPorID> mostrar_dueño_por_id = await api.ObtenerDueñosxID(idDueño);

                    DtoDueñosPorID dueños_id = mostrar_dueño_por_id.FirstOrDefault();

                    AgregarMascota formMascotas = new AgregarMascota();
                    formMascotas.pnlMascota.Visible = false;
                    formMascotas.btnAgregarDueño.Visible = false;
                    formMascotas.btnAgregarDueño.Visible = false;
                    formMascotas.btnActualizarDueño.Visible = true;
                    formMascotas.txtNombre.Text = dueños_id.Nombre;
                    formMascotas.txtPaterno.Text = dueños_id.Apellido_paterno;
                    formMascotas.txtMaterno.Text = dueños_id.Apellido_materno;
                    formMascotas.txtTelefono.Text = dueños_id.Telefono;
                    formMascotas.txtDireccion.Text = dueños_id.Direccion;

                    formMascotas.Show();

                    formMascotas.btnActualizarDueño.Click += async (senderGuardar, eGuardar) =>
                    {

                        DtoActualizarDueño dueño_actualizado = new DtoActualizarDueño
                        {
                            IdDueño = idDueño,
                            Nombre = formMascotas.txtNombre.Text,
                            Apellido_paterno = formMascotas.txtPaterno.Text,
                            Apellido_materno = formMascotas.txtMaterno.Text,
                            Telefono = formMascotas.txtTelefono.Text,
                            Direccion = formMascotas.txtDireccion.Text
                        };

                        DtoActualizarDueño resultado = await api.ActualizarDueño(idDueño, dueño_actualizado);

                        if (string.IsNullOrEmpty(resultado.Error))
                        {
                            MessageBox.Show("La información del dueño se ha actualizado correctamente.");
                            MostrarDueños();
                        }
                        else
                        {
                            MessageBox.Show("Error: " + resultado.Error);
                        }
                    };
                }
            }

        }

        private void cmbMascotas_SelectedIndexChanged(object sender, EventArgs e)
        {
            DtoMostrarMascotas mascotaSeleccionada = cmbMascotas.SelectedItem as DtoMostrarMascotas;
            MostrarMascotaSeleccionada(mascotaSeleccionada);
        }

        public void MostrarMascotaSeleccionada(DtoMostrarMascotas mascotaSeleccionada)
        {
            TarjetaMascotaControl tarjeta = new TarjetaMascotaControl();

            tarjeta.IdMascota.Text = mascotaSeleccionada.IdMascotas.ToString();
            tarjeta.lblnombre_mascota.Text = mascotaSeleccionada.Nombre_mascota.ToUpper();
            tarjeta.lblColor.Text = mascotaSeleccionada.Color_mascota;
            tarjeta.lblEdad.Text = mascotaSeleccionada.Edad_mascota.ToString();
            tarjeta.lblSexo.Text = mascotaSeleccionada.Sexo_mascota;
            tarjeta.lblEspecie.Text = mascotaSeleccionada.Especie_mascota;
            tarjeta.lblrazza.Text = mascotaSeleccionada.Raza_mascota;
            MemoryStream ms = new MemoryStream(mascotaSeleccionada.Imagen_mascota);
            tarjeta.pictureBox1.BackgroundImage = Image.FromStream(ms);

            this.pnlMostrarMascotas.Controls.Clear();
            Panel panelCentrado = tarjeta.panel4;

            panelCentrado.Anchor = AnchorStyles.None;
            panelCentrado.Dock = DockStyle.None;

            int x = (pnlMostrarMascotas.Width - panelCentrado.Width) / 2;
            int y = (pnlMostrarMascotas.Height - panelCentrado.Height) / 2;

            panelCentrado.Location = new Point(x, y);

            pnlMostrarMascotas.Controls.Add(panelCentrado);
        }

        private void btnAgregarMascota_Click_1(object sender, EventArgs e)
        {
            if (dataRegistroDueño.SelectedRows.Count > 0)
            {

                DataGridViewRow selectedRow = dataRegistroDueño.SelectedRows[0];
                int id = Convert.ToInt32(selectedRow.Cells[2].Value);
                // Pasa el ID al nuevo formulario
                AgregarMascota formMascotas = new AgregarMascota(id);
                formMascotas.pnlDueño.Visible = false;
                formMascotas.btnAgregarDueño.Visible = false;
                formMascotas.pnlMascota.Visible = true;
                formMascotas.btnAgregarMascota.Visible = true;
                formMascotas.ShowDialog(this);
            }
            else
            {
                MessageBox.Show("Selecciona un registro de dueño.");
            }


        }

        private void button1_Click(object sender, EventArgs e)
        {
            return;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Hide();

            Citas formCitas = new Citas();

            formCitas.Show();
        }
    }
}
