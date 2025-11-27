using MyLibrary.Repositories;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary.Repositories
{
    public class MySqlStudentRepository : IStudentRepository
    {
        private readonly string _connectionString;

        public MySqlStudentRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Student> GetStudentsByGroup(string groupName)
        {
            List<Student> students = new List<Student>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string sql = @"
                SELECT 
                    s.student_id, 
                    CONCAT(s.last_name, ' ', s.first_name, ' ', COALESCE(s.middle_name, '')) as full_name, 
                    s.group_name 
                FROM Students s 
                WHERE s.group_name = @GroupName 
                ORDER BY s.last_name, s.first_name";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@GroupName", groupName);

                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                students.Add(new Student
                                {
                                    StudentId = reader.GetInt32("student_id"),
                                    FullName = reader.GetString("full_name"),
                                    GroupName = reader.GetString("group_name")
                                });
                            }
                        }
                    }
                }

                return students;
            }
            catch (MySqlException ex)
            {
                throw new Exception($"Ошибка базы данных при получении студентов: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при получении студентов: {ex.Message}", ex);
            }
        }

        public List<string> GetGroups()
        {
            List<string> groups = new List<string>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string sql = @"SELECT DISTINCT group_name FROM Students ORDER BY group_name";
                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                groups.Add(reader.GetString("group_name"));
                            }
                        }
                    }
                }
                return groups;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при получении списка групп: {ex.Message}", ex);
            }
        }
    }
}
