using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    public class GradeJournal
    {
        public Student Student { get; set; }
        public Dictionary<DateTime, int?> GradesByDate { get; set; } = new Dictionary<DateTime, int?>();
        public double AverageGrade { get; set; }
    }
}
