using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    public class RowData
    {
        public Student Student { get; set; }
        public List<Grade> Grades { get; set; }

        public string StudentName => Student?.FullName;

        public double? AverageGrade
        {
            get
            {
                if (Grades == null || !Grades.Any())
                    return null;

                var validGrades = Grades
                    .Where(g => g.GradeValue.HasValue && g.GradeValue.Value >= 2 && g.GradeValue.Value <= 5)
                    .Select(g => g.GradeValue.Value)
                    .ToList();

                if (!validGrades.Any())
                    return null;

                return Math.Round(validGrades.Average(), 2);
            }
        }
    }
}
