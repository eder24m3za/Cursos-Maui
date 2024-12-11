using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursos.Models
{
    public class User
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public string correo { get; set; }
        public string password { get; set; }
    }
}
