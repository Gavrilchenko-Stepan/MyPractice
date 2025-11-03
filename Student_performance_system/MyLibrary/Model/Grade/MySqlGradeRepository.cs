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
            var grades = new List<Grade>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var sql = @"
                SELECT 
                    g.student_id, 
                    g.grade_date as lesson_date, 
                    CAST(g.grade_value as SIGNED) as grade_value
                FROM Grades g 
                INNER JOIN Students s ON g.student_id = s.student_id 
                INNER JOIN Subjects sub ON g.subject_id = sub.subject_id 
                WHERE s.group_name = @GroupName AND sub.subject_name = @SubjectName
                ORDER BY g.grade_date";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@GroupName", groupName);
                    command.Parameters.AddWithValue("@SubjectName", subjectName);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            grades.Add(new Grade
                            {
                                StudentId = reader.GetInt32("student_id"),
                                LessonDate = reader.GetDateTime("lesson_date"),
                                GradeValue = reader.GetInt32("grade_value")
                            });
                        }
                    }
                }
            }

            return grades;
        }

        public List<DateTime> GetLessonDates(string groupName, string subjectName)
        {
            var dates = new List<DateTime>();

            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var sql = @"
                SELECT DISTINCT g.grade_date as lesson_date 
                FROM Grades g 
                INNER JOIN Students s ON g.student_id = s.student_id 
                INNER JOIN Subjects sub ON g.subject_id = sub.subject_id 
                WHERE s.group_name = @GroupName AND sub.subject_name = @SubjectName 
                ORDER BY g.grade_date";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@GroupName", groupName);
                    command.Parameters.AddWithValue("@SubjectName", subjectName);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            dates.Add(reader.GetDateTime("lesson_date"));
                        }
                    }
                }
            }

            return dates;
        }
    }
}
