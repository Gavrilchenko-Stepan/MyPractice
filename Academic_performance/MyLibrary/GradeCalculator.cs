using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    public class GradeCalculator
    {
        public object CalculateAverageGrade(List<int> grades, string disciplineName)
        {
            if (grades == null || grades.Count == 0)
                throw new InvalidOperationException($"По дисциплине {disciplineName} нет данных об оценках.");

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
