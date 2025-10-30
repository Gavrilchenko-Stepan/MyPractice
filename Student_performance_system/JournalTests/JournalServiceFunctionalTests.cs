using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace JournalTests
{
    [TestClass]
    public class JournalServiceFunctionalTests
    {
        private JournalService _journalService;
        private IStudentRepository _studentRepository;
        private IGradeRepository _gradeRepository;

        // Тест 1: Успешная загрузка журнала оценок с корректными данными
        [TestMethod]
        public void GetJournalData_WithValidGroupAndSubject_ReturnsCorrectJournalData()
        {
            string groupName = "П-10";
            string subjectName = "Математика";

            var expectedGrades = new List<(int StudentId, DateTime Date, int? Grade)>
            {
                (1, new DateTime(2024, 1, 15), 5),
                (1, new DateTime(2024, 1, 22), 4),
                (2, new DateTime(2024, 1, 15), 4),
                (3, new DateTime(2024, 1, 22), 3)
            };

            JournalData result = _journalService.GetJournalData(groupName, subjectName);

            Assert.AreEqual("П-10", result.GroupName);
            Assert.AreEqual("Математика", result.SubjectName);
            Assert.AreEqual(3, result.Students.Count);
            Assert.AreEqual(2, result.LessonDates.Count);

            // Проверяем все ожидаемые оценки
            foreach (var expected in expectedGrades)
            {
                Assert.IsTrue(result.Grades.ContainsKey((expected.StudentId, expected.Date)));
                Assert.AreEqual(expected.Grade, result.Grades[(expected.StudentId, expected.Date)]);
            }

            // Проверяем конкретные значения
            Assert.AreEqual("Иванов Иван Иванович", result.Students[0].FullName);
            Assert.AreEqual(5, result.Grades[(1, new DateTime(2024, 1, 15))]);
        }

        // Тест 2: Загрузка журнала для несуществующей группы
        [TestMethod]
        public void GetJournalData_WithNonExistentGroup_ReturnsEmptyJournalData()
        {
            string groupName = "НЕСУЩЕСТВУЮЩАЯ-ГРУППА";
            string subjectName = "Математика";

            var expectedGrades = new List<(int StudentId, DateTime Date, int? Grade)>();

            JournalData result = _journalService.GetJournalData(groupName, subjectName);

            Assert.AreEqual("НЕСУЩЕСТВУЮЩАЯ-ГРУППА", result.GroupName);
            Assert.AreEqual("Математика", result.SubjectName);
            Assert.AreEqual(0, result.Students.Count);
            Assert.AreEqual(0, result.LessonDates.Count);
            Assert.AreEqual(0, result.Grades.Count);

            // Проверяем что словарь оценок пустой
            foreach (var expected in expectedGrades)
            {
                Assert.IsFalse(result.Grades.ContainsKey((expected.StudentId, expected.Date)));
            }
        }

        //Тест 3: Загрузка журнала для несуществующего предмета
        [TestMethod]
        public void GetJournalData_WithNonExistentSubject_ReturnsStudentsWithoutGrades()
        {
            // Arrange
            string groupName = "П-20";
            string subjectName = "Несуществующий предмет";

            var expectedGrades = new List<(int StudentId, DateTime Date, int? Grade)>();

            JournalData result = _journalService.GetJournalData(groupName, subjectName);

            Assert.AreEqual("П-20", result.GroupName);
            Assert.AreEqual("Несуществующий предмет", result.SubjectName);
            Assert.AreEqual(2, result.Students.Count);
            Assert.AreEqual(0, result.LessonDates.Count);
            Assert.AreEqual(0, result.Grades.Count);
            Assert.AreEqual("Кузнецов Алексей Викторович", result.Students[0].FullName);

            foreach (var expected in expectedGrades)
            {
                Assert.IsFalse(result.Grades.ContainsKey((expected.StudentId, expected.Date)));
            }
        }

        // Тест 4: Загрузка журнала при отсутствии оценок, но с проведенными занятиями
        [TestMethod]
        public void GetJournalData_WithLessonDatesButNoGrades_ReturnsStudentsWithDatesAndNoGrades()
        {
            // Arrange
            string groupName = "Н-11";
            string subjectName = "Физика";

            // Ожидаемые данные: студенты, даты, но все оценки NULL
            var expectedGrades = new List<(int StudentId, DateTime Date, int? Grade)>
            {
                (6, new DateTime(2024, 1, 10), null),  // Студент 6, дата 10.01, оценка NULL
                (6, new DateTime(2024, 1, 17), null),  // Студент 6, дата 17.01, оценка NULL
                (7, new DateTime(2024, 1, 10), null),  // Студент 7, дата 10.01, оценка NULL
                (7, new DateTime(2024, 1, 17), null)   // Студент 7, дата 17.01, оценка NULL
            };

            JournalData result = _journalService.GetJournalData(groupName, subjectName);

            // Проверяем основные свойства
            Assert.AreEqual("Н-11", result.GroupName);
            Assert.AreEqual("Физика", result.SubjectName);

            // Проверяем студентов
            Assert.AreEqual(2, result.Students.Count);
            Assert.AreEqual("Николаев Дмитрий Александрович", result.Students[0].FullName);
            Assert.AreEqual("Федорова Елена Игоревна", result.Students[1].FullName);

            // Проверяем даты занятий
            Assert.AreEqual(2, result.LessonDates.Count);
            CollectionAssert.Contains(result.LessonDates, new DateTime(2024, 1, 10));
            CollectionAssert.Contains(result.LessonDates, new DateTime(2024, 1, 17));

            // Проверяем что ВСЕ ожидаемые комбинации студент-дата НЕ содержатся в словаре оценок
            foreach (var expected in expectedGrades)
            {
                Assert.IsFalse(result.Grades.ContainsKey((expected.StudentId, expected.Date)));
            }

            // Дополнительная проверка: убеждаемся что словарь оценок полностью пустой
            Assert.AreEqual(0, result.Grades.Count);
        }
    }
}
