using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary.Repositories
{
    public class MySqlJournalCommandRepository : IJournalCommandRepository
    {
        private readonly string _connectionString;

        public MySqlJournalCommandRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public bool AddLessonDate(string groupName, string subjectName, DateTime lessonDate, int? lessonNumber = null)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    // Получаем ID предмета
                    string subjectIdSql = "SELECT subject_id FROM Subjects WHERE subject_name = @SubjectName";
                    int subjectId;
                    using (MySqlCommand subjectCommand = new MySqlCommand(subjectIdSql, connection))
                    {
                        subjectCommand.Parameters.AddWithValue("@SubjectName", subjectName);
                        subjectId = Convert.ToInt32(subjectCommand.ExecuteScalar());
                    }

                    // Получаем студентов группы
                    string studentsSql = "SELECT student_id FROM Students WHERE group_name = @GroupName";
                    List<int> studentIds = new List<int>();
                    using (MySqlCommand studentsCommand = new MySqlCommand(studentsSql, connection))
                    {
                        studentsCommand.Parameters.AddWithValue("@GroupName", groupName);
                        using (var reader = studentsCommand.ExecuteReader())
                        {
                            while (reader.Read())
                                studentIds.Add(reader.GetInt32("student_id"));
                        }
                    }

                    // Добавляем записи для каждого студента
                    string insertSql = @"
                INSERT INTO Grades (student_id, subject_id, grade_date, lesson_number, grade_value) 
                VALUES (@StudentId, @SubjectId, @LessonDate, @LessonNumber, NULL)";

                    using (MySqlCommand insertCommand = new MySqlCommand(insertSql, connection))
                    {
                        insertCommand.Parameters.Add("@StudentId", MySqlDbType.Int32);
                        insertCommand.Parameters.Add("@SubjectId", MySqlDbType.Int32);
                        insertCommand.Parameters.Add("@LessonDate", MySqlDbType.DateTime);
                        insertCommand.Parameters.Add("@LessonNumber", MySqlDbType.Int32);

                        int successCount = 0;
                        foreach (int studentId in studentIds)
                        {
                            try
                            {
                                insertCommand.Parameters["@StudentId"].Value = studentId;
                                insertCommand.Parameters["@SubjectId"].Value = subjectId;
                                insertCommand.Parameters["@LessonDate"].Value = lessonDate.Date;
                                insertCommand.Parameters["@LessonNumber"].Value = lessonNumber.HasValue ? lessonNumber.Value : (object)DBNull.Value;

                                insertCommand.ExecuteNonQuery();
                                successCount++;
                            }
                            catch (MySqlException ex) when (ex.Number == 1644)
                            {
                                // Дубликат заблокирован триггером - продолжаем со следующими студентами
                                continue;
                            }
                        }

                        return successCount > 0; // true если добавили хотя бы одну запись
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при добавлении даты занятия: {ex.Message}", ex);
            }
        }
    }
}
