using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyLibrary;
using MyLibrary.Presenter;
using MyLibrary.Repositories;
using MyLibrary.View;
using System;

namespace Tests
{
    [TestClass]
    public class TJournalPresenterGrade
    {
        [DataTestMethod]
        [DataRow(true, "Успешное сохранение")]
        [DataRow(false, "Ошибка сохранения")]
        public void UpdateGrade_DifferentResults_ShouldHandleCorrectly(bool serviceResult, string description)
        {
            var mockView = new Mock<IJournalView>();
            var mockJournalService = new Mock<JournalService>(
                Mock.Of<IStudentRepository>(),
                Mock.Of<IGradeRepository>(),
                Mock.Of<IJournalCommandRepository>());

            mockView.Setup(v => v.SubjectName).Returns("Математика");
            mockJournalService.Setup(s =>
                s.UpdateGrade(1, "Математика", It.IsAny<DateTime>(), 1, 5)).Returns(serviceResult);

            var loadJournalCalled = false;
            mockView.Setup(v => v.LoadJournal()).Callback(() => loadJournalCalled = true);

            var presenter = new JournalPresenter(mockView.Object, mockJournalService.Object);

            presenter.UpdateGrade(1, DateTime.Now, 1, 5);

            if (serviceResult)
            {
                mockView.Verify(v => v.ShowSuccessMessage("Оценка успешно сохранена"), Times.Once, description);
                Assert.IsTrue(loadJournalCalled, $"Журнал должен быть обновлен: {description}");
            }
            else
            {
                mockView.Verify(v => v.ShowErrorMessage("Не удалось сохранить оценку"), Times.Once, description);
            }
        }
    }
}
