using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary.Repositories
{
    public interface IJournalCommandRepository
    {
        bool AddLessonDate(string groupName, string subjectName, DateTime lessonDate, int? lessonNumber = null);
        bool EditLessonDate(string groupName, string subjectName, DateTime oldDate, int? oldLessonNumber, DateTime newDate, int? newLessonNumber);
        bool UpdateGrade(int studentId, int subjectId, DateTime lessonDate, int? lessonNumber, int? gradeValue);
    }
}
