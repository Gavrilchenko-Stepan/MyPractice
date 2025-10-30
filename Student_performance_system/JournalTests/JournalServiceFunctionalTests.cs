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
            // Arrange
            string groupName = "П-10";
            string subjectName = "Математика";

            // Act
            JournalData result = _journalService.GetJournalData(groupName, subjectName);

            // Assert
            Assert.AreEqual("П-10", result.GroupName);
            Assert.AreEqual("Математика", result.SubjectName);
            Assert.AreEqual(3, result.Students.Count);
            Assert.AreEqual(2, result.LessonDates.Count);
            Assert.AreEqual(4, result.Grades.Count);
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
    }
}
