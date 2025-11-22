using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
	[TestClass]
	public class TJournalPresenterEditDate
    {
        //Тест 1: Успешное редактирование даты занятия
        [TestMethod]
        [DataRow("2025-02-15", "2025-02-18", null, null, "15.02.2025 → 18.02.2025")] // только дата
        [DataRow("2025-02-15", "2025-02-18", 1, 2, "15.02.2025 (пара 1) → 18.02.2025 (пара 2)")] // дата + номер
        [DataRow("2025-02-15", "2025-02-15", 1, 3, "15.02.2025 (пара 1) → 15.02.2025 (пара 3)")] // только номер
        [DataRow("2025-02-15", "2025-02-18", 2, null, "15.02.2025 (пара 2) → 18.02.2025")] // убрать номер
        [DataRow("2025-02-15", "2025-02-18", null, 2, "15.02.2025 → 18.02.2025 (пара 2)")] // добавить номер
        public void EditDate_Success(string oldDateStr, string newDateStr, int? oldNumber, int? newNumber, string expectedMessage)
        {
            DateTime oldDate = DateTime.Parse(oldDateStr);
            DateTime newDate = DateTime.Parse(newDateStr);

            var mockView = new Mock<IJournalView>();
            var mockJournalService = new Mock<JournalService>(
                Mock.Of<IStudentRepository>(),
                Mock.Of<IGradeRepository>(),
                Mock.Of<IJournalCommandRepository>());

            var presenter = new JournalPresenter(mockView.Object, mockJournalService.Object);

            mockView.Setup(v => v.GroupName).Returns("П-10");
            mockView.Setup(v => v.SubjectName).Returns("Математика");

            mockJournalService.Setup(s => s.EditLessonDate(
                "П-10", "Математика", oldDate, oldNumber, newDate, newNumber))
                .Returns(true);

            presenter.EditLessonDate(
                new LessonData(oldDate, oldNumber),
                new LessonData(newDate, newNumber));

            mockJournalService.Verify(s => s.EditLessonDate(
                "П-10", "Математика", oldDate, oldNumber, newDate, newNumber),
                Times.Once);

            mockView.Verify(v => v.ShowSuccessMessage(It.Is<string>(msg =>
                msg.Contains("изменена") && msg.Contains(expectedMessage))),
                Times.Once);
        }


    }
}
