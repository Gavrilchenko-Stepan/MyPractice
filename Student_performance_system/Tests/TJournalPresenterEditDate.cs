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

        //Тест 2: Парсинг дат + Валидация ошибок
        [TestMethod]
        [DataRow("15.02", "2025-02-15", null, true)] // простая дата
        [DataRow("15.02(2)", "2025-02-15", 2, true)] // дата с номером
        [DataRow("15.02.2025", "2025-02-15", null, true)] // дата с годом
        [DataRow("1.3", "2025-03-01", null, true)] // короткий формат
        [DataRow("1.3(4)", "2025-03-01", 4, true)] // короткий формат с номером
        [DataRow("18-02", "2025-02-18", null, true)] // с дефисами
        [DataRow("18-02(3)", "2025-02-18", 3, true)] // с дефисами и номером
        [DataRow("", "0001-01-01", null, false)] // пустая строка
        [DataRow("abc", "0001-01-01", null, false)] // не дата
        [DataRow("15.02(6)", "0001-01-01", null, false)] // неверный номер пары
        [DataRow("32.01", "0001-01-01", null, false)] // неверная дата
        public void TryParseDate_VariousInputs_ParsesCorrectly(string input, string expectedDateStr, int? expectedLessonNumber, bool expectedResult)
        {
            DateTime expectedDate = DateTime.Parse(expectedDateStr);

            bool result = TryParseDate(input, out DateTime actualDate, out int? actualLessonNumber);

            Assert.AreEqual(expectedResult, result, $"Результат парсинга для: {input}");

            if (expectedResult)
            {
                Assert.AreEqual(expectedDate, actualDate, $"Дата должна быть: {expectedDate}");
                Assert.AreEqual(expectedLessonNumber, actualLessonNumber, $"Номер пары должен быть: {expectedLessonNumber}");
            }
        }

        //Тест 3: Ошибки редактирования
        [TestMethod]
        [DataRow("2025-02-15", "2026-02-18", null, null, "Новая дата не может быть в будущем")] // будущая дата
        [DataRow("2025-02-15", "2025-12-31", 1, 2, "Новая дата не может быть в будущем")] // будущая дата с номером
        public void EditDate_ErrorScenarios_ShowsErrorMessage(string oldDateStr, string newDateStr, int? oldNumber, int? newNumber, string expectedError)
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
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<int?>(),
                It.IsAny<DateTime>(), It.IsAny<int?>()))
                .Throws(new ArgumentException(expectedError));

            presenter.EditLessonDate(
                new LessonData(oldDate, oldNumber),
                new LessonData(newDate, newNumber));

            mockView.Verify(v => v.ShowErrorMessage(It.Is<string>(msg =>
                msg.Contains("Ошибка при редактировании даты") &&
                msg.Contains(expectedError))),
                Times.Once);
        }
    }
}
