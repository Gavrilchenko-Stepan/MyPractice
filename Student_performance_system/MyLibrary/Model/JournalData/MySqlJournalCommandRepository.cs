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

        // МЕТОД ДЛЯ РЕДАКТИРОВАНИЯ
        public bool EditLessonDate(string groupName, string subjectName, DateTime oldDate, int? oldLessonNumber, DateTime newDate, int? newLessonNumber)
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
                        var result = subjectCommand.ExecuteScalar();
                        if (result == null)
                            throw new Exception($"Предмет '{subjectName}' не найден");
                        subjectId = Convert.ToInt32(result);
                    }

                    // SQL запрос для обновления даты и номера пары
                    string updateSql = @"
                    UPDATE Grades g
                    INNER JOIN Students s ON g.student_id = s.student_id
                    SET g.grade_date = @NewDate, g.lesson_number = @NewLessonNumber
                    WHERE s.group_name = @GroupName 
                    AND g.subject_id = @SubjectId 
                    AND g.grade_date = @OldDate";

                    // Добавляем условие для старого номера пары
                    if (oldLessonNumber.HasValue)
                    {
                        updateSql += " AND g.lesson_number = @OldLessonNumber";
                    }
                    else
                    {
                        updateSql += " AND g.lesson_number IS NULL";
                    }

                    using (MySqlCommand updateCommand = new MySqlCommand(updateSql, connection))
                    {
                        updateCommand.Parameters.AddWithValue("@GroupName", groupName);
                        updateCommand.Parameters.AddWithValue("@SubjectId", subjectId);
                        updateCommand.Parameters.AddWithValue("@OldDate", oldDate.Date);
                        updateCommand.Parameters.AddWithValue("@NewDate", newDate.Date);

                        // Параметры для нового номера пары
                        if (newLessonNumber.HasValue)
                        {
                            updateCommand.Parameters.AddWithValue("@NewLessonNumber", newLessonNumber.Value);
                        }
                        else
                        {
                            updateCommand.Parameters.AddWithValue("@NewLessonNumber", DBNull.Value);
                        }

                        // Параметры для старого номера пары
                        if (oldLessonNumber.HasValue)
                        {
                            updateCommand.Parameters.AddWithValue("@OldLessonNumber", oldLessonNumber.Value);
                        }

                        int affectedRows = updateCommand.ExecuteNonQuery();
                        return affectedRows > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при редактировании даты занятия: {ex.Message}", ex);
            }
        }

        public bool UpdateGrade(int studentId, int subjectId, DateTime lessonDate, int? lessonNumber, int? gradeValue)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();

                    string checkSql = @"SELECT COUNT(*) FROM Grades 
                                      WHERE student_id = @StudentId 
                                      AND subject_id = @SubjectId 
                                      AND grade_date = @LessonDate 
                                      AND (lesson_number = @LessonNumber OR (lesson_number IS NULL AND @LessonNumber IS NULL))";

                    int existingRecords;
                    using (MySqlCommand checkCommand = new MySqlCommand(checkSql, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@StudentId", studentId);
                        checkCommand.Parameters.AddWithValue("@SubjectId", subjectId);
                        checkCommand.Parameters.AddWithValue("@LessonDate", lessonDate.Date);
                        checkCommand.Parameters.AddWithValue("@LessonNumber", lessonNumber ?? (object)DBNull.Value);
                        existingRecords = Convert.ToInt32(checkCommand.ExecuteScalar());
                    }

                    if (existingRecords > 0)
                    {
                        string updateSql = @"UPDATE Grades SET grade_value = @GradeValue 
                                           WHERE student_id = @StudentId 
                                           AND subject_id = @SubjectId 
                                           AND grade_date = @LessonDate 
                                           AND (lesson_number = @LessonNumber OR (lesson_number IS NULL AND @LessonNumber IS NULL))";

                        using (MySqlCommand updateCommand = new MySqlCommand(updateSql, connection))
                        {
                            updateCommand.Parameters.AddWithValue("@StudentId", studentId);
                            updateCommand.Parameters.AddWithValue("@SubjectId", subjectId);
                            updateCommand.Parameters.AddWithValue("@LessonDate", lessonDate.Date);
                            updateCommand.Parameters.AddWithValue("@LessonNumber", lessonNumber ?? (object)DBNull.Value);
                            updateCommand.Parameters.AddWithValue("@GradeValue", gradeValue ?? (object)DBNull.Value);

                            return updateCommand.ExecuteNonQuery() > 0;
                        }
                    }
                    else
                    {
                        string insertSql = @"INSERT INTO Grades (student_id, subject_id, grade_date, lesson_number, grade_value) 
                                           VALUES (@StudentId, @SubjectId, @LessonDate, @LessonNumber, @GradeValue)";

                        using (MySqlCommand insertCommand = new MySqlCommand(insertSql, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@StudentId", studentId);
                            insertCommand.Parameters.AddWithValue("@SubjectId", subjectId);
                            insertCommand.Parameters.AddWithValue("@LessonDate", lessonDate.Date);
                            insertCommand.Parameters.AddWithValue("@LessonNumber", lessonNumber ?? (object)DBNull.Value);
                            insertCommand.Parameters.AddWithValue("@GradeValue", gradeValue ?? (object)DBNull.Value);

                            return insertCommand.ExecuteNonQuery() > 0;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при обновлении оценки: {ex.Message}", ex);
            }
        }
    }
}
