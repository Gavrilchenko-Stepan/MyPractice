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
    public class TJournalPresenterGroup
    {
        [TestMethod]
        [DataRow(new string[] { "П-10", "П-20", "П-31" }, DisplayName = "Три группы П-10, П-20, П-31")]
        [DataRow(new string[] { "ИТ-101", "ИТ-102" }, DisplayName = "Две ИТ группы")]
        [DataRow(new string[] { "Группа-1" }, DisplayName = "Одна группа")]
        [DataRow(new string[] { }, DisplayName = "Нет групп")]
        public void GetGroups_WithValidServiceData_ReturnsSameData(string[] serviceGroups)
        {
            var mockView = new Mock<IJournalView>();
            var mockService = new Mock<JournalService>();

            mockService.Setup(s => s.GetGroups()).Returns(serviceGroups.ToList());
            var presenter = new JournalPresenter(mockView.Object, mockService.Object);

            var result = presenter.GetGroups();

            CollectionAssert.AreEqual(serviceGroups, result.ToArray());
            mockService.Verify(s => s.GetGroups(), Times.Once);
        }
    }
}
