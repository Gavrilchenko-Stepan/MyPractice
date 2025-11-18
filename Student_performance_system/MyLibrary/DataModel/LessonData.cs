using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary.DataModel
{
    public class LessonData
    {
        public DateTime Date { get; set; }
        public int? LessonNumber { get; set; }

        public LessonData(DateTime date, int? lessonNumber = null)
        {
            Date = date;
            LessonNumber = lessonNumber;
        }

        public override string ToString()
        {
            return LessonNumber.HasValue
                ? $"{Date:dd.MM.yyyy} (пара {LessonNumber.Value})"
                : $"{Date:dd.MM.yyyy}";
        }
    }
}
