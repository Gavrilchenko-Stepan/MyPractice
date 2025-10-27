using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    public class UserRepository : IUserRepository
    {
        private List<User> _users;

        public UserRepository()
        {
            _users = new List<User>
            {
                new User
                {
                    Login = "ivanov",
                    Password = "123",
                    TeacherId = 1,
                    Teacher = new Teacher
                    {
                        TeacherId = 1,
                        LastName = "Иванов",
                        FirstName = "Иван",
                        MiddleName = "Иванович"
                    }
                },
                new User
                {
                    Login = "petrov",
                    Password = "456",
                    TeacherId = 2,
                    Teacher = new Teacher
                    {
                        TeacherId = 2,
                        LastName = "Петров",
                        FirstName = "Петр",
                        MiddleName = "Петрович"
                    }
                },
                new User
                {
                    Login = "sidorova",
                    Password = "789",
                    TeacherId = 3,
                    Teacher = new Teacher
                    {
                        TeacherId = 3,
                        LastName = "Сидорова",
                        FirstName = "Мария",
                        MiddleName = "Ивановна"
                    }
                }
            };
        }

        public User GetUserByLogin(string login)
        {
            return _users.FirstOrDefault(u => u.Login == login);
        }

        public bool ValidateUser(string login, string password)
        {
            var user = GetUserByLogin(login);
            return user != null && user.Password == password;
        }
    }
}
