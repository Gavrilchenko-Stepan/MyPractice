using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyLibrary;
using MyLibrary.DataModel.JournalData;
using MyLibrary.DataModel;
using MyLibrary.DataModel.JournalData;
using MyLibrary.Presenter;
using MyLibrary.Repositories;
using MyLibrary.View;
using System;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class TJournalService
    {
        /// Тест 1 и 4: Загрузка журнала для разных сценариев с валидными данными
        [TestMethod]
        [DataRow("П-10", "Математика", new int[] { 1, 2, 3 }, new string[] { "Иванов Иван Иванович", "Петров Петр Петрович", "Сидорова Анна Сергеевна" },
            new string[] { "П-10", "П-10", "П-10" },
            new int[] { 5, 4, 3 },
            new int[] { 4, 5, 3 })]
        [DataRow("Н-11", "Физика", new int[] { 6, 7 }, new string[] { "Николаев Дмитрий Александрович", "Федорова Елена Игоревна" },
            new string[] { "Н-11", "Н-11" },
            new int[] { -1, -1 },
            new int[] { -1, -1 })]
        public void GetJournalData_WithValidData_ReturnsCorrectJournalData(
            string groupName,
            string subjectName,
            int[] studentIds,
            string[] fullNames,
            string[] groupNames,
            int?[] firstGrades,
            int?[] secondGrades)
        {

            var mockStudentRepository = new Mock<IStudentRepository>();
            var mockGradeRepository = new Mock<IGradeRepository>();
            var mockJournalCommandRepository = new Mock<IJournalCommandRepository>();

            var journalService = new JournalService(
                mockStudentRepository.Object,
                mockGradeRepository.Object,
                mockJournalCommandRepository.Object);

            var students = new List<Student>
        {
            new Student { StudentId = 1, FullName = "Иванов Иван Иванович", GroupName = groupName },
            new Student { StudentId = 2, FullName = "Петров Петр Петрович", GroupName = groupName }
        };

            var grades = new List<Grade>
        {
            new Grade { StudentId = 1, LessonDate = new DateTime(2024, 1, 15), LessonNumber = 1, GradeValue = 5 },
            new Grade { StudentId = 1, LessonDate = new DateTime(2024, 1, 22), LessonNumber = 2, GradeValue = 4 },
            new Grade { StudentId = 2, LessonDate = new DateTime(2024, 1, 15), LessonNumber = 1, GradeValue = 3 }
        };

            mockStudentRepository
                .Setup(x => x.GetStudentsByGroup(groupName))
                .Returns(students);

            mockGradeRepository
                .Setup(x => x.GetGradesByGroupAndSubject(groupName, subjectName))
                .Returns(grades);

            var result = journalService.GetJournalData(groupName, subjectName);

            Assert.AreEqual(groupName, result.GroupName);
            Assert.AreEqual(subjectName, result.SubjectName);
            Assert.AreEqual(2, result.Rows.Count);

            // Проверяем вызовы репозиториев
            mockStudentRepository.Verify(x => x.GetStudentsByGroup(groupName), Times.Once);
            mockGradeRepository.Verify(x => x.GetGradesByGroupAndSubject(groupName, subjectName), Times.Once);
        }

        /// Тест 2 и 3: Загрузка журнала для несуществующих групп и предметов
        [TestMethod]
        [DataRow("НЕСУЩЕСТВУЮЩАЯ-ГРУППА", "Математика")]
        [DataRow("П-20", "Несуществующий предмет")]
        public void GetJournalData_WithInvalidRequests_ReturnsEmptyOrStudentsWithoutGrades(string groupName, string subjectName, int expectedStudentCount)
        {
            var mockStudentRepository = new Mock<IStudentRepository>();
            var mockGradeRepository = new Mock<IGradeRepository>();
            var mockJournalCommandRepository = new Mock<IJournalCommandRepository>();

            var journalService = new JournalService(
                mockStudentRepository.Object,
                mockGradeRepository.Object,
                mockJournalCommandRepository.Object);

            mockStudentRepository
                .Setup(x => x.GetStudentsByGroup(groupName))
                .Returns(new List<Student>());

            mockGradeRepository
                .Setup(x => x.GetGradesByGroupAndSubject(groupName, subjectName))
                .Returns(new List<Grade>());

            var result = journalService.GetJournalData(groupName, subjectName);

            Assert.AreEqual(groupName, result.GroupName);
            Assert.AreEqual(subjectName, result.SubjectName);
            Assert.AreEqual(expectedStudentCount, result.Rows.Count);
        }
    }
}
