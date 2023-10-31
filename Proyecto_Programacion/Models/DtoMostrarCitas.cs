using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class DtoMostrarCitas
    {
        public int IdCita { get; set; }

        public string Dueño { get; set; }

        public string Mascota { get; set; }

        public string Motivo_Consulta { get; set; }

        public DateTime Fecha_Consulta { get; set; }

        public string Hora { get; set; }

        public bool Activo { get; set; }   

    }
}
