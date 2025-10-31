using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    public interface IGradesRepository
    {
        List<Student> GetStudents();
        List<Student> GetStudentsByGroup(string groupName);
        List<string> GetGroups();
        List<Subject> GetSubjects();
        List<DateTime> GetGradeDates(int subjectId, string groupName);
        List<GradeJournal> GetGradeJournal(int subjectId, string groupName);
        void SaveGrade(int studentId, int subjectId, DateTime date, int gradeValue);
        void AddDateForGroup(int subjectId, string groupName, DateTime date);
    }
}
