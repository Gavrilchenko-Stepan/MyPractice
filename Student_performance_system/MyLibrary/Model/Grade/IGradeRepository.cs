using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary.Repositories
{
    public interface IGradeRepository
    {
        List<Grade> GetGradesByGroupAndSubject(string groupName, string subjectName);
        List<DateTime> GetLessonDates(string groupName, string subjectName);
    }
}
