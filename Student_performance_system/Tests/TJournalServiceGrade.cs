using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyLibrary;
using MyLibrary.Repositories;
using System;

namespace Tests
{
    [TestClass]
    public class TJournalServiceGrade
    {
        [DataTestMethod]
        [DataRow(2, true, "Корректная оценка 2")]
        [DataRow(3, true, "Корректная оценка 3")]
        [DataRow(4, true, "Корректная оценка 4")]
        [DataRow(5, true, "Корректная оценка 5")]
        [DataRow(null, true, "Null оценка (удаление)")]
        public void UpdateGrade_ValidData_ShouldCallRepository(int? gradeValue, bool expectedResult, string description)
        {
            var mockStudentRepository = new Mock<IStudentRepository>();
            var mockGradeRepository = new Mock<IGradeRepository>();
            var mockJournalCommandRepository = new Mock<IJournalCommandRepository>();

            mockGradeRepository.Setup(r => r.GetSubjectId("Математика")).Returns(1);
            mockJournalCommandRepository.Setup(r =>
                r.UpdateGrade(1, 1, It.IsAny<DateTime>(), 1, gradeValue)).Returns(expectedResult);

            var journalService = new JournalService(
                mockStudentRepository.Object,
                mockGradeRepository.Object,
                mockJournalCommandRepository.Object);

            var result = journalService.UpdateGrade(1, "Математика", DateTime.Now, 1, gradeValue);

            Assert.AreEqual(expectedResult, result, description);
            mockGradeRepository.Verify(r => r.GetSubjectId("Математика"), Times.Once, description);
            mockJournalCommandRepository.Verify(r =>
                r.UpdateGrade(1, 1, It.IsAny<DateTime>(), 1, gradeValue), Times.Once, description);
        }

        [DataTestMethod]
        [DataRow(1, "Некорректная оценка 1")]
        [DataRow(6, "Некорректная оценка 6")]
        [DataRow(0, "Некорректная оценка 0")]
        public void UpdateGrade_InvalidGrades_ShouldThrowException(int invalidGrade, string description)
        {
            var mockStudentRepository = new Mock<IStudentRepository>();
            var mockGradeRepository = new Mock<IGradeRepository>();
            var mockJournalCommandRepository = new Mock<IJournalCommandRepository>();

            mockGradeRepository.Setup(r => r.GetSubjectId("Математика")).Returns(1);

            var journalService = new JournalService(
                mockStudentRepository.Object,
                mockGradeRepository.Object,
                mockJournalCommandRepository.Object);

            Assert.ThrowsException<ArgumentException>(() =>
                journalService.UpdateGrade(1, "Математика", DateTime.Now, 1, invalidGrade),
                description);
        }
    }
}
