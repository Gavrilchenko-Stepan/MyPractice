using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyLibrary;
using MyLibrary.Repositories;
using System;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class TJournalServiceGroup
    {
        [TestMethod]
        [DataRow(new string[] { "П-10", "П-20" }, DisplayName = "Две группы П-10, П-20")]
        [DataRow(new string[] { "П-10" }, DisplayName = "Одна группа П-10")]
        [DataRow(new string[] { "ИТ-101", "ИТ-102", "ИТ-103" }, DisplayName = "Три ИТ группы")]
        [DataRow(new string[] { "Группа-А", "Группа-Б" }, DisplayName = "Группы с дефисами")]
        [DataRow(new string[] { }, DisplayName = "Пустой список групп")]
        public void GetGroups_WithValidData_ReturnsSameDataFromRepository(string[] expectedGroups)
        {
            var mockStudentRepository = new Mock<IStudentRepository>();
            var mockGradeRepository = new Mock<IGradeRepository>();
            var mockCommandRepository = new Mock<IJournalCommandRepository>();

            mockStudentRepository.Setup(r => r.GetGroups()).Returns(expectedGroups.ToList());
            var service = new JournalService(mockStudentRepository.Object, mockGradeRepository.Object, mockCommandRepository.Object);

            var result = service.GetGroups();

            CollectionAssert.AreEqual(expectedGroups, result.ToArray());
            mockStudentRepository.Verify(r => r.GetGroups(), Times.Once);
            mockGradeRepository.VerifyNoOtherCalls();
            mockCommandRepository.VerifyNoOtherCalls();
        }
    }
}
