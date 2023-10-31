using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class DtoMostrarMascotas
    {
        public int IdMascotas { get; set; }
        public int IdDueño { get; set; }

        public string Nombre_mascota { get; set; } 

        public string Edad_mascota { get; set; }

        public string Sexo_mascota { get; set; }

        public string Especie_mascota { get; set; }

        public string Raza_mascota { get; set; }

        public string Color_mascota {get; set; }

        public byte[] Imagen_mascota { get; set; }

    }
}
