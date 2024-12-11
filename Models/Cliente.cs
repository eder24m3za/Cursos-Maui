using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursos.Models
{
    public class Cliente
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public long telefono { get; set; }
        public string correo { get; set; }
        public int? curso_id { get; set; }
        public string? curso { get; set; }
    }
}
