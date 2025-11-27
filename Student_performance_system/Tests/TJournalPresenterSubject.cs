using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyLibrary;
using MyLibrary.Presenter;
using MyLibrary.View;
using System;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class TJournalPresenterSubject
    {
        [TestMethod]
        [DataRow(new string[] { "Математика", "Физика", "Химия" }, DisplayName = "Три предмета")]
        [DataRow(new string[] { "Программирование", "Алгоритмы" }, DisplayName = "Два ИТ предмета")]
        public void GetSubjects_WithMockService_ReturnsSameData(string[] serviceSubjects)
        {
            // Arrange
            var mockView = new Mock<IJournalView>();
            var mockService = new Mock<JournalService>();

            mockService.Setup(s => s.GetSubjects()).Returns(serviceSubjects.ToList());
            var presenter = new JournalPresenter(mockView.Object, mockService.Object);

            // Act
            var result = presenter.GetSubjects();

            // Assert
            CollectionAssert.AreEqual(serviceSubjects, result.ToArray());
            mockService.Verify(s => s.GetSubjects(), Times.Once);
        }
    }
}
