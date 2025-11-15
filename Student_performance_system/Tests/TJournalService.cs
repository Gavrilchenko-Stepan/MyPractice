using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyLibrary;
using MyLibrary.DataModel.JournalData;
using MyLibrary.DataModel;
using MyLibrary.Model.JournalData;
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

            JournalService journalService_ = new JournalService();

            List<Row> expected = new List<Row>();
            for (int i = 0; i < studentIds.Length; i++)
            {
                // Конвертируем -1 в null
                int? firstGrade = firstGrades[i] == -1 ? null : firstGrades[i];
                int? secondGrade = secondGrades[i] == -1 ? null : secondGrades[i];

                expected.Add(new Row
                {
                    Student = new Student { StudentId = studentIds[i], FullName = fullNames[i], GroupName = groupNames[i] },
                    Grades = new List<(DateTime, int?)>
                    {
                        (new DateTime(2024, 1, 15), firstGrades[i]),
                        (new DateTime(2024, 1, 22), secondGrades[i])
                    }
                });
            }

            JournalData result = journalService_.GetJournalData(groupName, subjectName);

            Assert.AreEqual(groupName, result.GroupName);
            Assert.AreEqual(subjectName, result.SubjectName);
            Assert.AreEqual(expected.Count, result.Data.Count);

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i].Student.StudentId, result.Data[i].Student.StudentId);
                Assert.AreEqual(expected[i].Student.FullName, result.Data[i].Student.FullName);
                Assert.AreEqual(expected[i].Student.GroupName, result.Data[i].Student.GroupName);
                CollectionAssert.AreEqual(expected[i].Grades, result.Data[i].Grades);
            }
        }

        /// Тест 2 и 3: Загрузка журнала для несуществующих групп и предметов
        [TestMethod]
        [DataRow("НЕСУЩЕСТВУЮЩАЯ-ГРУППА", "Математика")]
        [DataRow("П-20", "Несуществующий предмет")]
        public void GetJournalData_WithInvalidRequests_ReturnsEmptyOrStudentsWithoutGrades(string groupName, string subjectName, int expectedStudentCount)
        {
            JournalService journalService_ = new JournalService();

            JournalData result = journalService_.GetJournalData(groupName, subjectName);

            Assert.AreEqual(groupName, result.GroupName);
            Assert.AreEqual(subjectName, result.SubjectName);
            Assert.AreEqual(expectedStudentCount, result.Data.Count);
        }

        // ТЕСТ 1.1 и 1.2: Успешное добавление новой даты (с номером пары и без)
        [DataTestMethod]
        [DataRow("2025-02-22", null, true, "Дата занятия 22.02.2025 успешно добавлена")]
        [DataRow("2025-02-22", 3, true, "Дата занятия 22.02.2025 (пара 3) успешно добавлена")]
        public void AddLessonDate_NewValidDate_ShouldAddSuccessfully(
            string dateStr, int? lessonNumber, bool expectedSuccess, string expectedMessage)
        {
            var studentRepoMock = new Mock<IStudentRepository>();
            var gradeRepoMock = new Mock<IGradeRepository>();
            var commandRepoMock = new Mock<IJournalCommandRepository>();
            var viewMock = new Mock<IJournalView>();

            var journalService = new JournalService(studentRepoMock.Object, gradeRepoMock.Object, commandRepoMock.Object);

            var presenter = new JournalPresenter(viewMock.Object, journalService);

            DateTime newDate = DateTime.Parse(dateStr);
            var lessonData = new LessonData(newDate, lessonNumber);

            viewMock.Setup(v => v.GetNewLessonData()).Returns(lessonData);
            viewMock.Setup(v => v.GroupName).Returns("П-10");
            viewMock.Setup(v => v.SubjectName).Returns("Математика");

            commandRepoMock
                .Setup(r => r.AddLessonDate("П-10", "Математика", newDate, lessonNumber))
                .Returns(expectedSuccess);

            // Настройка данных для проверки обновления журнала
            var students = new List<Student>
            {
                new Student { StudentId = 1, FullName = "Иванов Иван", GroupName = "П-10" },
                new Student { StudentId = 2, FullName = "Петров Петр", GroupName = "П-10" }
            };

            var grades = new List<Grade>
            {
                new Grade { StudentId = 1, LessonDate = new DateTime(2025, 02, 01), GradeValue = 5 },
                new Grade { StudentId = 1, LessonDate = newDate, GradeValue = null },
                new Grade { StudentId = 2, LessonDate = new DateTime(2025, 02, 01), GradeValue = 4 },
                new Grade { StudentId = 2, LessonDate = newDate, GradeValue = null }
            };

            studentRepoMock.Setup(r => r.GetStudentsByGroup("П-10")).Returns(students);
            gradeRepoMock.Setup(r => r.GetGradesByGroupAndSubject("П-10", "Математика")).Returns(grades);

            presenter.AddLessonDate();

            commandRepoMock.Verify(r => r.AddLessonDate("П-10", "Математика", newDate, lessonNumber), Times.Once);
            viewMock.Verify(v => v.ShowSuccessMessage(expectedMessage), Times.Once);
            viewMock.Verify(v => v.ShowErrorMessage(It.IsAny<string>()), Times.Never);

            // Проверяем, что журнал обновляется после успешного добавления
            viewMock.Verify(v => v.DisplayJournal(It.IsAny<JournalData>()), Times.Once);
        }
    }
}
