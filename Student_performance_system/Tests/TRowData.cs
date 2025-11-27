using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyLibrary;
using System;
using System.Linq;

namespace Tests
{
    [TestClass]
    public class TRowData
    {
        [TestMethod]
        [DataRow(new int[] { 5, 4, 3 }, 4.00)]
        [DataRow(new int[] { 5, 4, 4, 3 }, 4.00)]
        [DataRow(new int[] { 5 }, 5.00)]
        [DataRow(new int[] { 2, 3, 4 }, 3.00)]
        public void CalculateAverageGrade_WithValidGrades_ReturnsCorrectAverage(int[] grades, double expectedAverage)
        {
            var student = new Student { StudentId = 1, FullName = "Тест", GroupName = "Группа-1" };
            var rowData = new RowData
            {
                Student = student,
                Grades = grades.Select((g, index) => new Grade
                {
                    StudentId = 1,
                    GradeValue = g,
                    LessonDate = DateTime.Now.AddDays(index),
                    LessonNumber = index + 1
                }).ToList()
            };

            var result = rowData.AverageGrade;

            Assert.AreEqual(expectedAverage, result.Value, 0.01);
        }

        [TestMethod]
        [DataRow(new int[] { })]
        [DataRow(new int[] { 0, 6, -1 })]
        [DataRow(new int[] { 1, 7, 10 })]
        public void CalculateAverageGrade_WithNoValidGrades_ReturnsNull(int[] grades)
        {
            // Arrange
            var student = new Student { StudentId = 1, FullName = "Тест", GroupName = "Группа-1" };
            var rowData = new RowData
            {
                Student = student,
                Grades = grades.Select((g, index) => new Grade
                {
                    StudentId = 1,
                    GradeValue = g,
                    LessonDate = DateTime.Now.AddDays(index),
                    LessonNumber = index + 1
                }).ToList()
            };

            // Act
            var result = rowData.AverageGrade;

            // Assert
            Assert.IsNull(result);
        }
    }
}
