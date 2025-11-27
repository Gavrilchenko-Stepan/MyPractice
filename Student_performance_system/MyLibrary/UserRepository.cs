using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    public class UserRepository : IUserRepository
    {
        private readonly string _connectionString;

        public UserRepository()
        {
            _connectionString = IniConfig.ConnectionString;

            if (string.IsNullOrEmpty(_connectionString))
                throw new InvalidOperationException("Строка подключения не настроена");
        }

        public User GetUserByLogin(string login)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"
                    SELECT u.user_id, u.login, u.password, u.teacher_id,
                           t.last_name, t.first_name, t.middle_name
                    FROM users u
                    INNER JOIN teachers t ON u.teacher_id = t.teacher_id
                    WHERE u.login = @login";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@login", login);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Login = reader["login"].ToString(),
                                Password = reader["password"].ToString(),
                                TeacherId = Convert.ToInt32(reader["teacher_id"]),
                                Teacher = new Teacher
                                {
                                    TeacherId = Convert.ToInt32(reader["teacher_id"]),
                                    LastName = reader["last_name"].ToString(),
                                    FirstName = reader["first_name"].ToString(),
                                    MiddleName = reader["middle_name"]?.ToString()
                                }
                            };
                        }
                    }
                }
            }

            return null;
        }

        public bool ValidateUser(string login, string password)
        {
            var user = GetUserByLogin(login);
            return user != null && user.Password == password;
        }
    }
}
