using MyLibrary.DataModel;
using MyLibrary.DataModel.JournalData;
using MyLibrary.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    public class JournalService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IGradeRepository _gradeRepository;
        private readonly IJournalCommandRepository _journalCommandRepository;

        public JournalService(IStudentRepository studentRepository, IGradeRepository gradeRepository, IJournalCommandRepository journalCommandRepository)
        {
            _studentRepository = studentRepository;
            _gradeRepository = gradeRepository;
            _journalCommandRepository = journalCommandRepository;
        }

        public JournalData GetJournalData(string groupName, string subjectName)
        {
            try
            {
                List<Student> students = _studentRepository.GetStudentsByGroup(groupName);
                List<Grade> allGrades = _gradeRepository.GetGradesByGroupAndSubject(groupName, subjectName);

                Dictionary<int, List<Grade>> gradesByStudent = allGrades
                .GroupBy(g => g.StudentId)
                .ToDictionary(g => g.Key, g => g.ToList());

                JournalData journalData = new JournalData
                {
                    GroupName = groupName,
                    SubjectName = subjectName,
                    Rows = new BindingList<RowData>()
                };

                foreach (var student in students)
                {
                    // Находим оценки для этого студента
                    List<Grade> studentGrades = gradesByStudent.ContainsKey(student.StudentId)
                        ? gradesByStudent[student.StudentId]
                        : new List<Grade>();

                    journalData.Rows.Add(new RowData
                    {
                        Student = student,
                        Grades = studentGrades
                    });
                }

                return journalData;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при загрузке данных журнала: {ex.Message}", ex);
            }
        }

        public bool AddLessonDate(string groupName, string subjectName, DateTime lessonDate, int? lessonNumber = null)
        {
            // Валидация бизнес-правил
            if (string.IsNullOrEmpty(groupName) || string.IsNullOrEmpty(subjectName))
                throw new ArgumentException("Группа и предмет обязательны");

            if (lessonDate > DateTime.Now.Date)
                throw new ArgumentException("Дата не может быть в будущем");

            if (lessonNumber.HasValue && (lessonNumber < 1 || lessonNumber > 5))
                throw new ArgumentException("Номер пары должен быть от 1 до 5");

            return _journalCommandRepository.AddLessonDate(groupName, subjectName, lessonDate, lessonNumber);
        }

        public bool AddLessonDate(string groupName, string subjectName, LessonData lessonData)
        {
            return AddLessonDate(groupName, subjectName, lessonData.Date, lessonData.LessonNumber);
        }

        // МЕТОД ДЛЯ РЕДАКТИРОВАНИЯ
        public bool EditLessonDate(string groupName, string subjectName, DateTime oldDate, int? oldLessonNumber, DateTime newDate, int? newLessonNumber)
        {
            // Валидация бизнес-правил
            if (string.IsNullOrEmpty(groupName) || string.IsNullOrEmpty(subjectName))
                throw new ArgumentException("Группа и предмет обязательны");

            if (newDate > DateTime.Now)
                throw new ArgumentException("Новая дата не может быть в будущем");

            if (newLessonNumber.HasValue && (newLessonNumber < 1 || newLessonNumber > 5))
                throw new ArgumentException("Номер пары должен быть от 1 до 5");

            // Проверяем, что что-то изменилось
            if (oldDate == newDate && oldLessonNumber == newLessonNumber)
                throw new ArgumentException("Необходимо изменить дату или номер пары");

            // Вызываем метод репозитория для редактирования
            return _journalCommandRepository.EditLessonDate(
                groupName,
                subjectName,
                oldDate,
                oldLessonNumber,
                newDate,
                newLessonNumber);
        }

        public List<string> GetGroups()
        {
            return _studentRepository.GetGroups();
        public bool UpdateGrade(int studentId, string subjectName, DateTime lessonDate, int? lessonNumber, int? gradeValue)
        {
            if (gradeValue.HasValue && (gradeValue < 2 || gradeValue > 5))
                throw new ArgumentException("Оценка должна быть от 2 до 5");

            if (lessonDate > DateTime.Now.Date)
                throw new ArgumentException("Дата занятия не может быть в будущем");

            int subjectId = _gradeRepository.GetSubjectId(subjectName);

            return _journalCommandRepository.UpdateGrade(studentId, subjectId, lessonDate, lessonNumber, gradeValue);
        }
    }
}
