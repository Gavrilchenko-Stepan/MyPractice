using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    public class User
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public int TeacherId { get; set; }
        public Teacher Teacher { get; set; }
    }
}
