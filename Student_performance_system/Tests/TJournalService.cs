using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyLibrary;
using System;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class TJournalService
    {
        private JournalService _journalService;

        /// Тест 1: Успешная загрузка журнала оценок с корректными данными
        [TestMethod]
        public void GetJournalData_WithValidGroupAndSubject_ReturnsCorrectJournalData()
        {
            string groupName = "П-10";
            string subjectName = "Математика";

            List<Row> exp = new List<Row>
            {
                new Row
                {
                    Student = new Student { StudentId = 1, FullName = "Иванов Иван Иванович", GroupName = "П-10" },
                    Grades = new List<(DateTime, int?)>
                    {
                        (new DateTime(2024, 1, 15), 5),
                        (new DateTime(2024, 1, 22), 4)
                    }
                },
                new Row
                {
                    Student = new Student { StudentId = 2, FullName = "Петров Петр Петрович", GroupName = "П-10" },
                    Grades = new List<(DateTime, int?)>
                    {
                        (new DateTime(2024, 1, 15), 4)
                    }
                },
                new Row
                {
                    Student = new Student { StudentId = 3, FullName = "Сидорова Анна Сергеевна", GroupName = "П-10" },
                    Grades = new List<(DateTime, int?)>
                    {
                        (new DateTime(2024, 1, 22), 3)
                    }
                }
            };

            JournalData result = _journalService.GetJournalData(groupName, subjectName);

            Assert.AreEqual("П-10", result.GroupName);
            Assert.AreEqual("Математика", result.SubjectName);
            AssertJournalData(result, exp);
        }

        /// Тест 2: Загрузка журнала для несуществующей группы
        [TestMethod]
        public void GetJournalData_WithNonExistentGroup_ReturnsEmptyJournalData()
        {
            string groupName = "НЕСУЩЕСТВУЮЩАЯ-ГРУППА";
            string subjectName = "Математика";

            List<Row> exp = new List<Row>();

           JournalData result = _journalService.GetJournalData(groupName, subjectName);

            Assert.AreEqual("НЕСУЩЕСТВУЮЩАЯ-ГРУППА", result.GroupName);
            Assert.AreEqual("Математика", result.SubjectName);
            AssertJournalData(result, exp);
        }

        /// Тест 3: Загрузка журнала для несуществующего предмета
        [TestMethod]
        public void GetJournalData_WithNonExistentSubject_ReturnsStudentsWithoutGrades()
        {
            string groupName = "П-20";
            string subjectName = "Несуществующий предмет";

            List<Row> exp = new List<Row>
            {
                new Row
                {
                    Student = new Student { StudentId = 4, FullName = "Кузнецов Алексей Викторович", GroupName = "П-20" },
                    Grades = new List<(DateTime, int?)>()
                },
                new Row
                {
                    Student = new Student { StudentId = 5, FullName = "Смирнова Ольга Дмитриевна", GroupName = "П-20" },
                    Grades = new List<(DateTime, int?)>()
                }
            };

            JournalData result = _journalService.GetJournalData(groupName, subjectName);

            Assert.AreEqual("П-20", result.GroupName);
            Assert.AreEqual("Несуществующий предмет", result.SubjectName);
            AssertJournalData(result, exp);
        }

        private void AssertJournalData(JournalData actual, List<Row> expected)
        {
            // Проверяем основные свойства
            Assert.AreEqual(expected.Count, actual.Data.Count);

            for (int i = 0; i < expected.Count; i++)
            {
                var expectedRow = expected[i];
                var actualRow = actual.Data[i];

                // Проверяем студента
                Assert.AreEqual(expectedRow.Student.StudentId, actualRow.Student.StudentId);
                Assert.AreEqual(expectedRow.Student.FullName, actualRow.Student.FullName);
                Assert.AreEqual(expectedRow.Student.GroupName, actualRow.Student.GroupName);

                // Проверяем оценки
                Assert.AreEqual(expectedRow.Grades.Count, actualRow.Grades.Count);

                for (int j = 0; j < expectedRow.Grades.Count; j++)
                {
                    var (expectedDate, expectedGrade) = expectedRow.Grades[j];
                    var (actualDate, actualGrade) = actualRow.Grades[j];

                    Assert.AreEqual(expectedDate, actualDate);
                    Assert.AreEqual(expectedGrade, actualGrade);
                }
            }
        }
    }
}
