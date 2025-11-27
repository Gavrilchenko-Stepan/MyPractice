using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyLibrary.DataModel.JournalData;
using MyLibrary.Repositories;
using MyLibrary;
using System;
using System.Collections.Generic;
using MyLibrary.DataModel;
using MyLibrary.Presenter;
using MyLibrary.View;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class TJournalServiceDate
    {
        // ТЕСТ 3.1: Автоматическая сортировка дат
        [TestMethod]
        public void GetJournalData_DateSorting_ShouldOrderDatesCorrectly()
        {
            var studentRepoMock = new Mock<IStudentRepository>();
            var gradeRepoMock = new Mock<IGradeRepository>();
            var commandRepoMock = new Mock<IJournalCommandRepository>();

            var journalService = new JournalService(studentRepoMock.Object, gradeRepoMock.Object, commandRepoMock.Object);

            DateTime newDate = new DateTime(2025, 02, 08);
            var existingDates = new List<DateTime> {
                new DateTime(2025, 02, 01),
                new DateTime(2025, 02, 15)
            };

            var students = new List<Student>
            {
                new Student { StudentId = 1, FullName = "Иванов Иван", GroupName = "П-10" }
            };

            var allGrades = new List<Grade>();

            // Добавляем оценки для всех дат
            foreach (var date in existingDates.Concat(new[] { newDate }))
            {
                allGrades.Add(new Grade
                {
                    StudentId = 1,
                    LessonDate = date,
                    GradeValue = 5
                });
            }

            studentRepoMock.Setup(r => r.GetStudentsByGroup("П-10")).Returns(students);
            gradeRepoMock.Setup(r => r.GetGradesByGroupAndSubject("П-10", "Математика")).Returns(allGrades);

            var result = journalService.GetJournalData("П-10", "Математика");
            var sortedDates = result.Rows[0].Grades.Select(g => g.LessonDate).OrderBy(d => d).ToList();

            Assert.AreEqual(3, sortedDates.Count);
            Assert.AreEqual(new DateTime(2025, 02, 01), sortedDates[0]);
            Assert.AreEqual(newDate, sortedDates[1]);
            Assert.AreEqual(new DateTime(2025, 02, 15), sortedDates[2]);
        }

        // ТЕСТ 3.2: Сортировка дат с разными номерами пар
        [TestMethod]
        public void GetJournalData_MultipleLessonsSameDay_ShouldGroupAndOrderCorrectly()
        {
            var studentRepoMock = new Mock<IStudentRepository>();
            var gradeRepoMock = new Mock<IGradeRepository>();
            var commandRepoMock = new Mock<IJournalCommandRepository>();

            var journalService = new JournalService(studentRepoMock.Object, gradeRepoMock.Object, commandRepoMock.Object);

            var students = new List<Student>
            {
                new Student { StudentId = 1, FullName = "Иванов Иван", GroupName = "П-10" }
            };

            var grades = new List<Grade>
            {
                new Grade { StudentId = 1, LessonDate = new DateTime(2025, 02, 15), LessonNumber = 2, GradeValue = 5 },
                new Grade { StudentId = 1, LessonDate = new DateTime(2025, 02, 15), LessonNumber = 1, GradeValue = 4 },
                new Grade { StudentId = 1, LessonDate = new DateTime(2025, 02, 20), LessonNumber = null, GradeValue = 3 }
            };

            studentRepoMock.Setup(r => r.GetStudentsByGroup("П-10")).Returns(students);
            gradeRepoMock.Setup(r => r.GetGradesByGroupAndSubject("П-10", "Математика")).Returns(grades);

            var result = journalService.GetJournalData("П-10", "Математика");
            var studentGrades = result.Rows[0].Grades;

            Assert.AreEqual(3, studentGrades.Count);

            // Проверяем сортировку по дате и номеру пары
            var orderedGrades = studentGrades
                .OrderBy(g => g.LessonDate)
                .ThenBy(g => g.LessonNumber)
                .ToList();

            Assert.AreEqual(1, orderedGrades[0].LessonNumber); // первая пара 15.02
            Assert.AreEqual(2, orderedGrades[1].LessonNumber); // вторая пара 15.02
            Assert.IsNull(orderedGrades[2].LessonNumber);      // 20.02 без номера пары
        }

        // ТЕСТ 4: Валидация номера пары
        [DataTestMethod]
        [DataRow("2025-02-22", 6, "Номер пары должен быть от 1 до 5")]
        [DataRow("2025-02-22", 0, "Номер пары должен быть от 1 до 5")]
        public void AddLessonDate_InvalidLessonNumber_ShouldThrowException(
            string dateStr, int? lessonNumber, string expectedMessage)
        {
            var studentRepoMock = new Mock<IStudentRepository>();
            var gradeRepoMock = new Mock<IGradeRepository>();
            var commandRepoMock = new Mock<IJournalCommandRepository>();

            var journalService = new JournalService(studentRepoMock.Object, gradeRepoMock.Object, commandRepoMock.Object);

            DateTime date = DateTime.Parse(dateStr);

            var exception = Assert.ThrowsException<ArgumentException>(() =>
                journalService.AddLessonDate("П-10", "Математика", date, lessonNumber));

            Assert.IsTrue(exception.Message.Contains(expectedMessage));
            commandRepoMock.Verify(r => r.AddLessonDate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int?>()), Times.Never);
        }
    }
}
