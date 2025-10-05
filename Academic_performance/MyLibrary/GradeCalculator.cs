using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    public class GradeCalculator
    {
        public double CalculateAverageGrade(List<int> grades)
        {
            if (grades == null || grades.Count == 0)
                return 0.0;

            double sum = 0;
            foreach (var grade in grades)
            {
                sum += grade;
            }

            double average = sum / grades.Count;
            return Math.Round(average, 2);
        }
    }
}
