using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyLibrary.Repositories;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace Tests
{
    [TestClass]
    public class TMySqlGradeRepositoryGrade
    {
        [TestMethod]
        public void GetSubjectId_SubjectExists_ShouldReturnCorrectId()
        {
            var mockConnection = new Mock<MySqlConnection>(It.IsAny<string>());
            var mockCommand = new Mock<MySqlCommand>();
            var parameters = new Mock<MySqlParameterCollection>();

            mockCommand.Setup(c => c.Parameters).Returns(parameters.Object);
            mockCommand.Setup(c => c.ExecuteScalar()).Returns(1);

            mockConnection.Setup(c => c.CreateCommand()).Returns(mockCommand.Object);
            mockConnection.Setup(c => c.State).Returns(ConnectionState.Open);

            var repository = new MySqlGradeRepository("Server=test;Database=test;Uid=test;Pwd=test; Port=test;");

            var result = repository.GetSubjectId("Математика");

            Assert.AreEqual(1, result);
            mockCommand.Verify(c => c.ExecuteScalar(), Times.Once);
        }
    }
}
