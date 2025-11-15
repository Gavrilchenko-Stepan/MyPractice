using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyLibrary.Model.JournalData;
using MyLibrary.Repositories;
using MyLibrary;
using System;
using System.Collections.Generic;
using MyLibrary.DataModel;
using MyLibrary.Presenter;
using MyLibrary.View;

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
    }
}
