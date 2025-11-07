using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary.Repositories
{
    public class MySqlGradeRepository : IGradeRepository
    {
        private readonly string _connectionString;

        public MySqlGradeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Grade> GetGradesByGroupAndSubject(string groupName, string subjectName)
        {
            List<Grade> grades = new List<Grade>();

            using (MySqlConnection connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                string sql = @"
                SELECT 
                    g.student_id, 
                    g.grade_date as lesson_date, 
                    CAST(g.grade_value as SIGNED) as grade_value
                FROM Grades g 
                INNER JOIN Students s ON g.student_id = s.student_id 
                INNER JOIN Subjects sub ON g.subject_id = sub.subject_id 
                WHERE s.group_name = @GroupName AND sub.subject_name = @SubjectName
                ORDER BY g.grade_date";

                using (MySqlCommand command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@GroupName", groupName);
                    command.Parameters.AddWithValue("@SubjectName", subjectName);

                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            grades.Add(new Grade
                            {
                                StudentId = reader.GetInt32(0),
                                LessonDate = reader.GetDateTime("lesson_date"),
                                GradeValue = reader.IsDBNull(reader.GetOrdinal("grade_value"))
                                                    ? (int?)null                          // Если в БД NULL - сохраняем null
                                                    : reader.GetInt32("grade_value")      // Если в БД число - сохраняем число
                            });
                        }
                    }
                }
            }

            return grades;
        }
    }
}
