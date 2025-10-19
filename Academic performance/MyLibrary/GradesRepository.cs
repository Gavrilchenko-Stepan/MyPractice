using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    public class GradesRepository : IGradesRepository
    {
        private List<Grade> _grades = new List<Grade>();
        private List<Student> _students = new List<Student>();
        private List<Subject> _subjects = new List<Subject>();
        private List<string> _groups = new List<string>();

        public GradesRepository()
        {
            InitializeTestData();
        }

        private void InitializeTestData()
        {
            // Группы
            _groups = new List<string> { "Группа 1", "Группа 2", "Группа 3" };

            // Дисциплины
            _subjects = new List<Subject>
            {
               new Subject { Id = 1, Name = "Высшая математика" },
               new Subject { Id = 2, Name = "Физика" },
               new Subject { Id = 3, Name = "Программирование" }
            };

            // Студенты по группам
            _students = new List<Student>
            {
               // Группа 1
               new Student { Id = 1, Name = "Абрамов Евгений Вадимович", Group = "Группа 1" },
               new Student { Id = 2, Name = "Борисенко Валерия Дмитриевна", Group = "Группа 1" },
               new Student { Id = 3, Name = "Гавриленко Антон Викторович", Group = "Группа 1" },
               new Student { Id = 4, Name = "Германова Анастасия Владимировна", Group = "Группа 1" },
            
               // Группа 2
               new Student { Id = 5, Name = "Зотов Иван Сергеевич", Group = "Группа 2" },
               new Student { Id = 6, Name = "Казаков Алексей Андреевич", Group = "Группа 2" },
               new Student { Id = 7, Name = "Камалетдинова Эльвира Радиковна", Group = "Группа 2" },
               new Student { Id = 8, Name = "Картавенкова Анастасия Михайловна", Group = "Группа 2" },
            
               // Группа 3
               new Student { Id = 9, Name = "Новикова Алина Владимировна", Group = "Группа 3" },
               new Student { Id = 10, Name = "Пиманова Светлана Сергеевна", Group = "Группа 3" },
               new Student { Id = 11, Name = "Сергеева Александра Петровна", Group = "Группа 3" },
               new Student { Id = 12, Name = "Сташковский Артемий Олегович", Group = "Группа 3" }
            };

            // Даты занятий
            var dates = new List<DateTime>
            {
               new DateTime(2025, 2, 9),
               new DateTime(2025, 2, 16),
               new DateTime(2025, 3, 2),
               new DateTime(2025, 3, 9),
               new DateTime(2025, 3, 16),
               new DateTime(2025, 3, 23),
               new DateTime(2025, 3, 30),
               new DateTime(2025, 4, 6),
               new DateTime(2025, 4, 13)
            };
        }

        public List<DateTime> GetGradeDates(int subjectId, string groupName)
        {
            var groupStudentIds = GetStudentsByGroup(groupName).Select(s => s.Id).ToList();

            return _grades
                .Where(g => g.SubjectId == subjectId && groupStudentIds.Contains(g.StudentId))
                .Select(g => g.GradeDate.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToList();
        }

        public List<GradeJournal> GetGradeJournal(int subjectId, string groupName)
        {
            var journal = new List<GradeJournal>();
            var dates = GetGradeDates(subjectId, groupName);
            var groupStudents = GetStudentsByGroup(groupName);

            foreach (var student in groupStudents)
            {
                var studentGrades = _grades
                    .Where(g => g.StudentId == student.Id && g.SubjectId == subjectId)
                    .ToList();

                var gradesByDate = dates.ToDictionary(
                    date => date,
                    date => studentGrades
                        .FirstOrDefault(g => g.GradeDate.Date == date.Date)?.GradeValue
                );

                var validGrades = studentGrades.Select(g => g.GradeValue).ToList();
                double average = validGrades.Any() ? validGrades.Average() : 0;

                var gradeJournal = new GradeJournal
                {
                    Student = student,
                    GradesByDate = gradesByDate,
                    AverageGrade = average
                };

                journal.Add(gradeJournal);
            }

            return journal;
        }

        public List<string> GetGroups()
        {
            return _groups;
        }

        public List<Student> GetStudents()
        {
            return _students;
        }

        public List<Student> GetStudentsByGroup(string groupName)
        {
            return _students
            .Where(s => s.Group == groupName)
            .OrderBy(s => s.Name)
            .ToList();
        }

        public List<Subject> GetSubjects()
        {
            return _subjects;
        }

        public void SaveGrade(int studentId, int subjectId, DateTime date, int gradeValue)
        {
            var existingGrade = _grades.FirstOrDefault(g =>
            g.StudentId == studentId &&
            g.SubjectId == subjectId &&
            g.GradeDate.Date == date.Date);

            if (existingGrade != null)
            {
                existingGrade.GradeValue = gradeValue;
            }
            else
            {
                _grades.Add(new Grade
                {
                    StudentId = studentId,
                    SubjectId = subjectId,
                    GradeDate = date,
                    GradeValue = gradeValue
                });
            }
        }
    }
}
