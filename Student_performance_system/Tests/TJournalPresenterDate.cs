using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyLibrary.DataModel;
using MyLibrary.Model.JournalData;
using MyLibrary.Presenter;
using MyLibrary.Repositories;
using MyLibrary.View;
using MyLibrary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class TJournalPresenterDate
    {
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

        // ТЕСТ 2.1 и 2.2: Попытка добавления существующей даты
        [DataTestMethod]
        [DataRow("2025-02-15", null, false, "Занятие 15.02.2025 уже существует в журнале")]
        [DataRow("2025-02-15", 2, false, "Занятие 15.02.2025 (пара 2) уже существует в журнале")]
        public void AddLessonDate_ExistingDate_ShouldShowErrorMessage(
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

            presenter.AddLessonDate();

            viewMock.Verify(v => v.ShowErrorMessage(expectedMessage), Times.Once);
            viewMock.Verify(v => v.ShowSuccessMessage(It.IsAny<string>()), Times.Never);
            viewMock.Verify(v => v.DisplayJournal(It.IsAny<JournalData>()), Times.Never);
        }
    }
}
