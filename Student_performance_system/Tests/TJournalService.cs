using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyLibrary;
using System;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class TJournalService
    {
        /// Тест 1 и 4: Загрузка журнала для разных сценариев с валидными данными
        [TestMethod]
        [DataRow("П-10", "Математика", new int[] { 1, 2, 3 }, new string[] { "Иванов Иван Иванович", "Петров Петр Петрович", "Сидорова Анна Сергеевна" },
            new string[] { "П-10", "П-10", "П-10" },
            new int[] { 5, 4, 3 },
            new int[] { 4, 5, 3 })]
        [DataRow("Н-11", "Физика", new int[] { 6, 7 }, new string[] { "Николаев Дмитрий Александрович", "Федорова Елена Игоревна" },
            new string[] { "Н-11", "Н-11" },
            new int[] { -1, -1 },
            new int[] { -1, -1 })]
        public void GetJournalData_WithValidData_ReturnsCorrectJournalData(
            string groupName,
            string subjectName,
            int[] studentIds,
            string[] fullNames,
            string[] groupNames,
            int?[] firstGrades,
            int?[] secondGrades)
        {

            JournalService journalService_ = new JournalService();

            List<Row> expected = new List<Row>();
            for (int i = 0; i < studentIds.Length; i++)
            {
                // Конвертируем -1 в null
                int? firstGrade = firstGrades[i] == -1 ? null : firstGrades[i];
                int? secondGrade = secondGrades[i] == -1 ? null : secondGrades[i];

                expected.Add(new Row
                {
                    Student = new Student { StudentId = studentIds[i], FullName = fullNames[i], GroupName = groupNames[i] },
                    Grades = new List<(DateTime, int?)>
                    {
                        (new DateTime(2024, 1, 15), firstGrades[i]),
                        (new DateTime(2024, 1, 22), secondGrades[i])
                    }
                });
            }

            JournalData result = journalService_.GetJournalData(groupName, subjectName);

            Assert.AreEqual(groupName, result.GroupName);
            Assert.AreEqual(subjectName, result.SubjectName);
            Assert.AreEqual(expected.Count, result.Data.Count);

            for (int i = 0; i < expected.Count; i++)
            {
                Assert.AreEqual(expected[i].Student.StudentId, result.Data[i].Student.StudentId);
                Assert.AreEqual(expected[i].Student.FullName, result.Data[i].Student.FullName);
                Assert.AreEqual(expected[i].Student.GroupName, result.Data[i].Student.GroupName);
                CollectionAssert.AreEqual(expected[i].Grades, result.Data[i].Grades);
            }
        }
    }
}
