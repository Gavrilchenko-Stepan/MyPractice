using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    public class Row
    {
        public Student Student { get; set; }
        public List<(DateTime Date, int? Grade)> Grades { get; set; } = new List<(DateTime, int?)>();
    }
}
