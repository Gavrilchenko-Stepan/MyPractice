using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib
{
    public class GradesRepository : IGradesRepository
    {
        private List<Grade> _grades;
        public GradesRepository()
        {
            _grades = new List<Grade>
            {
                new Grade { StudentId = 1, SubjectId = 1, GradeValue = 5, GradeDate = new DateTime(2024, 1, 15) },
                new Grade { StudentId = 1, SubjectId = 1, GradeValue = 4, GradeDate = new DateTime(2024, 1, 20) },
                new Grade { StudentId = 1, SubjectId = 1, GradeValue = 5, GradeDate = new DateTime(2024, 1, 25) },
                new Grade { StudentId = 1, SubjectId = 2, GradeValue = 3, GradeDate = new DateTime(2024, 1, 18) },
                new Grade { StudentId = 2, SubjectId = 1, GradeValue = 4, GradeDate = new DateTime(2024, 1, 22) }
            };
        }
        public List<Grade> GetStudentGrades(int studentId, int subjectId)
        {
            return _grades.Where(g => g.StudentId == studentId && g.SubjectId == subjectId).ToList();
        }
    }
}
