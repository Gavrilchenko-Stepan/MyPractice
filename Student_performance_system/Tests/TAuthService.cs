using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyLibrary;
using System;

namespace Tests
{
    [TestClass]
    public class TAuthService
    {
        [TestMethod]
        [DataRow("ivanov", "Password123")]
        public void Login_WithValidCredentials_ShouldCreateUserSessionAndOpenMainForm(string login, string password)
        {
            var userRepositoryMock = new Mock<IUserRepository>();
            var authService = new AuthService(userRepositoryMock.Object);

            var expectedUser = new User
            {
                Login = login,
                Password = password,
                Teacher = new Teacher
                {
                    FirstName = "Иван",
                    LastName = "Иванов",
                    MiddleName = "Иванович"
                }
            };

            userRepositoryMock
                .Setup(repo => repo.ValidateUser(login, password))
                .Returns(true);
            userRepositoryMock
                .Setup(repo => repo.GetUserByLogin(login))
                .Returns(expectedUser);

            bool loginResult = authService.Login(login, password);

            Assert.IsTrue(loginResult, "Логин должен быть успешным");
            Assert.IsTrue(authService.IsAuthenticated, "Должна быть создана сессия пользователя");
            Assert.IsNotNull(authService.CurrentUser, "Текущий пользователь должен быть установлен");
            Assert.AreEqual(login, authService.CurrentUser.Login, "Должен быть установлен корректный пользователь");
        }

        [TestMethod]
        [DataRow("petrov", "Password432")]
        public void Login_WithInvalidPassword_ShouldShowErrorMessageAndKeepFormOpen(string login, string password)
        {
            var userRepositoryMock = new Mock<IUserRepository>();
            var authService = new AuthService(userRepositoryMock.Object);

            userRepositoryMock
                .Setup(repo => repo.ValidateUser(login, password))
                .Returns(false);

            bool loginResult = authService.Login(login, password);

            Assert.IsFalse(loginResult, "Логин должен завершиться неудачей");
            Assert.IsFalse(authService.IsAuthenticated, "Сессия не должна быть создана");
            Assert.IsNull(authService.CurrentUser, "Текущий пользователь не должен быть установлен");
        }

        [TestMethod]
        [DataRow("nonexistent_user", "AnyPassword123")]
        public void Login_WithNonExistentLogin_ShouldShowErrorMessageAndKeepFormActive(string login, string password)
        {
            var userRepositoryMock = new Mock<IUserRepository>();
            var authService = new AuthService(userRepositoryMock.Object);

            userRepositoryMock
                .Setup(repo => repo.ValidateUser(login, password))
                .Returns(false);
            userRepositoryMock
                .Setup(repo => repo.GetUserByLogin(login))
                .Returns((User)null);

            bool loginResult = authService.Login(login, password);

            Assert.IsFalse(loginResult, "Логин должен завершиться неудачей");
            Assert.IsFalse(authService.IsAuthenticated, "Сессия не должна быть создана");
            Assert.IsNull(authService.CurrentUser, "Текущий пользователь не должен быть установлен");
        }
    }
}
