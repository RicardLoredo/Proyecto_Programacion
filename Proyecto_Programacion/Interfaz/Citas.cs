using Models.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Proyecto_Programacion.Interfaz;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace Proyecto_Programacion
{
    public partial class Citas : Form
    {
        public Citas()
        {
            InitializeComponent();
        }

        private async void Citas_Load(object sender, EventArgs e)
        {
            CargarDueños_combobox();
            CargarCitas();

        }
        private string telefonoDueño;

        private async void CargarDueños_combobox()
        {
            
            ConexionApi api = new ConexionApi();
            List<DtoMostrarDueños> lstDueño = await api.ObtenerDueños();
            Dictionary<string, DtoMostrarDueños> dueñosDict = lstDueño.ToDictionary(d => d.NombreC, d => d);
            cmbxDueños.DataSource = new BindingSource(dueñosDict.Keys, null);
            cmbxDueños.SelectedItem = null;
            cmbxDueños.SelectedIndexChanged += async (sender, e) =>
            {
                if (cmbxDueños.SelectedItem != null)
                {
                    string nombreDueño = cmbxDueños.SelectedItem.ToString();
                    DtoMostrarDueños dueñoSeleccionado = dueñosDict[nombreDueño];
                    int idDueño = dueñoSeleccionado.IdDueño;
                    telefonoDueño = dueñoSeleccionado.Telefono;
                    List<DtoMostrarMascotas> lstMascotas = await api.MostrarMascota(idDueño);

                    var mascotasInfo = lstMascotas.Select(m => new {m.Nombre_mascota,m.IdMascotas }).ToList();
                    cmbxMascotas.DataSource = mascotasInfo;
                    cmbxMascotas.DisplayMember = "Nombre_mascota";
                    cmbxMascotas.ValueMember = "IdMascotas";

                }
            };

        }

        private async void CargarCitas()
        {
            ConexionApi api = new ConexionApi();
            List<DtoMostrarCitas> lstCitas = await api.MostrarCitas();

            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = lstCitas;
            dgvCitas.DataSource = bindingSource;


            dgvCitas.Columns[0].Visible = false;
            dgvCitas.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgvCitas.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvCitas.DefaultCellStyle.BackColor = Color.FromArgb(219, 223, 234);
            dgvCitas.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvCitas.Columns[4].HeaderText = "Fecha Consulta";
            dgvCitas.Columns[3].HeaderText = "Motivo Consulta";
            dgvCitas.Columns[4].DefaultCellStyle.Format = "dd/MM/yyyy";


            //cmbxFechas.Items.Clear();
            foreach (DataGridViewRow row in dgvCitas.Rows)
            {
                var dates = dgvCitas.Rows
                    .Cast<DataGridViewRow>()
                    .Select(rows => Convert.ToDateTime(rows.Cells[4].Value))
                    .Distinct()
                    .OrderBy(date => date)
                    .ToList();
                cmbxFechas.DataSource = dates;
            }
        }

        //FILTRAR DATAGRIDVIEW
        private async void cmbxFechas_SelectedIndexChanged(object sender, EventArgs e)
        {
            DateTime fechaSeleccionada = DateTime.Parse(cmbxFechas.SelectedItem.ToString());

            ConexionApi api = new ConexionApi();
            List<DtoMostrarCitas> lstCitas = await api.MostrarCitas();

            var lstCitas_filtradasxFecha = lstCitas.Where(cita => cita.Fecha_Consulta == fechaSeleccionada).ToList();

            dgvCitas.DataSource = lstCitas_filtradasxFecha;

            dgvCitas.Refresh();
        }


        //BOTONES PARA CAMBIAR
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();

            Mascotas formMascotas = new Mascotas();

            formMascotas.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            return;
        }


        //AGREGAR CITA
        private async void btnAgendarCita_Click(object sender, EventArgs e)
        {
            ConexionApi api = new ConexionApi();
            DtoAgregarCita pCita = new DtoAgregarCita();

            pCita.IdMascotas = ((dynamic)cmbxMascotas.SelectedItem).IdMascotas;
            pCita.Motivo_consulta = txtMotivo.Text;
            pCita.Fecha_consulta = comboFecha.Value;
            pCita.Hora = txtHora.Text;

            var resultadoCita = await api.AgregarCita(pCita);

            if (string.IsNullOrEmpty(resultadoCita.Error))
            {

                MessageBox.Show("Cita agendada correctamente");

                //TWILIO
                var accountSid = "AC4ffa3bccbc48e62c0881c1f456101bb4";
                var authToken = "667742d1135345d9934d8263bd088164";
                TwilioClient.Init(accountSid, authToken);
                string numero_telefono_sinespacio = telefonoDueño.Replace("-", "");
                var messageOptions = new CreateMessageOptions(
                  new PhoneNumber($"whatsapp:+521{numero_telefono_sinespacio}"));
                messageOptions.From = new PhoneNumber("whatsapp:+14155238886");
                messageOptions.Body = $"La cita de {((dynamic)cmbxMascotas.SelectedItem).Nombre_mascota} es el dia {comboFecha.Value.ToString("dd/MM/yyyy")} a las {txtHora.Text} por el motivo de {txtMotivo.Text} " ;

                var message = MessageResource.Create(messageOptions);
                Console.WriteLine(message.Body);

                CargarCitas();

            }
            else
            {
                MessageBox.Show("Error al agendar cita: " + resultadoCita.Error);
            }


        }

        //ACTUALIZAR

        private void dgvCitas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dgvCitas.CurrentRow.Selected = true;
        }

        private void btnActualizar_citas_Click(object sender, EventArgs e)
        {
            if (dgvCitas.SelectedRows.Count > 0)
            {
                ActualizarCita formActualizar = new ActualizarCita();
                DataGridViewRow selectedRow = dgvCitas.SelectedRows[0];
                int id = Convert.ToInt32(selectedRow.Cells[0].Value);
                // Pasa el ID al nuevo formulario

                formActualizar.lblDueño.Text = selectedRow.Cells[1].Value.ToString();
                formActualizar.lblMascota.Text = selectedRow.Cells[2].Value.ToString();
                formActualizar.txtMotivo.Text = selectedRow.Cells[3].Value.ToString();
                formActualizar.comboFecha.Text = selectedRow.Cells[4].Value.ToString();
                formActualizar.txtHora.Text = selectedRow.Cells[5].Value.ToString();

                if (Convert.ToBoolean(selectedRow.Cells[6].Value))
                {
                    formActualizar.rbSi.Checked = true;
                    formActualizar.rbNo.Checked = false;
                }
                else
                {
                    formActualizar.rbSi.Checked = false;
                    formActualizar.rbNo.Checked = true;
                }

                formActualizar.Show();

                formActualizar.btnAgendarCita.Click += async (senderActualizar, eActualizar) =>
                {
                    int confirmada = 0;
                    if (formActualizar.rbSi.Checked)
                    {
                        confirmada = 1;
                    }
                    else if (formActualizar.rbNo.Checked)
                    {
                        confirmada = 0;
                    }

                    DtoActualizarCita cita_actualizada= new DtoActualizarCita
                    {
                        IdCitas = id,
                        Motivo_consulta = formActualizar.txtMotivo.Text,
                        Fecha_consulta = formActualizar.comboFecha.Value,
                        Hora = formActualizar.txtHora.Text,
                        Activo = Convert.ToBoolean(confirmada)
                    };

                    ConexionApi api = new ConexionApi();

                    DtoActualizarCita resultado = await api.ActualizarCita(cita_actualizada);

                    if (string.IsNullOrEmpty(resultado.Error))
                    {
                        MessageBox.Show("La información de la cita se a actualizado correctamente");

                    }
                    else
                    {
                        MessageBox.Show("Error al actualizar cita: " + resultado.Error);
                    }

                };
            }
            else
            {
                MessageBox.Show("Selecciona un registro de cita.");
            }
        }
    }
}
