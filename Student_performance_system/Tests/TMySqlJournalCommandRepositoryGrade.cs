using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyLibrary.Repositories;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace Tests
{
    [TestClass]
    public class TMySqlJournalCommandRepositoryGrade
    {
        [DataTestMethod]
        [DataRow(1, "UPDATE существующей записи")]
        [DataRow(0, "INSERT новой записи")]
        public void UpdateGrade_РазныеСценарии_ДолженКорректноВыполнитьОперацию(int existingRecords, string description)
        {
            var mockConnection = new Mock<MySqlConnection>("строка_подключения");
            var checkCommand = new Mock<MySqlCommand>();
            var mainCommand = new Mock<MySqlCommand>();

            var checkParameters = new Mock<MySqlParameterCollection>();
            var mainParameters = new Mock<MySqlParameterCollection>();

            checkCommand.Setup(c => c.Parameters).Returns(checkParameters.Object);
            checkCommand.Setup(c => c.ExecuteScalar()).Returns(existingRecords);

            mainCommand.Setup(c => c.Parameters).Returns(mainParameters.Object);
            mainCommand.Setup(c => c.ExecuteNonQuery()).Returns(1);

            var commandSequence = new MockSequence();
            mockConnection.InSequence(commandSequence).Setup(c => c.CreateCommand()).Returns(checkCommand.Object);
            mockConnection.InSequence(commandSequence).Setup(c => c.CreateCommand()).Returns(mainCommand.Object);

            mockConnection.Setup(c => c.State).Returns(ConnectionState.Open);

            var repository = new MySqlJournalCommandRepository("тестовая_строка_подключения");

            var result = repository.UpdateGrade(1, 1, DateTime.Now, 1, 5);

            Assert.IsTrue(result, description);
            checkCommand.Verify(c => c.ExecuteScalar(), Times.Once, description);
            mainCommand.Verify(c => c.ExecuteNonQuery(), Times.Once, description);
        }
    }
}
