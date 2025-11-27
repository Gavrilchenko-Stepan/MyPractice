using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyLibrary;
using MyLibrary.Repositories;
using System;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class TJournalServiceSubject
    {
        [TestMethod]
        [DataRow(new string[] { "Математика", "Физика" }, DisplayName = "Два предмета")]
        [DataRow(new string[] { "Математика" }, DisplayName = "Один предмет")]
        public void GetSubjects_WithMockRepository_ReturnsSameData(string[] mockSubjects)
        {
            // Arrange
            var mockRepository = new Mock<IGradeRepository>();
            var mockStudentRepo = new Mock<IStudentRepository>();
            var mockCommandRepo = new Mock<IJournalCommandRepository>();

            mockRepository.Setup(r => r.GetSubjects()).Returns(mockSubjects.ToList());
            var service = new JournalService(mockStudentRepo.Object, mockRepository.Object, mockCommandRepo.Object);

            // Act
            var result = service.GetSubjects();

            // Assert
            CollectionAssert.AreEqual(mockSubjects, result.ToArray());
            mockRepository.Verify(r => r.GetSubjects(), Times.Once);
        }
    }
}
