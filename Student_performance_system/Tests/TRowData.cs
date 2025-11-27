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

            Assert.IsNull(result);
        }

        [TestMethod]
        [DataRow(new int[] { 5, 4 }, new int[] { 3 }, 4.00)]
        [DataRow(new int[] { 5, 5 }, new int[] { 2 }, 4.00)]
        [DataRow(new int[] { 3, 3 }, new int[] { 5 }, 3.67)]
        public void AverageGrade_AutoRecalculates_WhenGradesChange(int[] initialGrades, int[] newGrades, double expectedFinalAverage)
        {
            var student = new Student { StudentId = 1, FullName = "Тест", GroupName = "Группа-1" };
            var rowData = new RowData
            {
                Student = student,
                Grades = initialGrades.Select((g, index) => new Grade
                {
                    StudentId = 1,
                    GradeValue = g,
                    LessonDate = DateTime.Now.AddDays(index),
                    LessonNumber = index + 1
                }).ToList()
            };

            var initialAverage = rowData.AverageGrade;

            foreach (var newGrade in newGrades)
            {
                rowData.Grades.Add(new Grade
                {
                    StudentId = 1,
                    GradeValue = newGrade,
                    LessonDate = DateTime.Now.AddDays(rowData.Grades.Count),
                    LessonNumber = rowData.Grades.Count + 1
                });
            }

            var finalAverage = rowData.AverageGrade;

            Assert.AreEqual(expectedFinalAverage, finalAverage.Value, 0.01);
            Assert.AreNotEqual(initialAverage, finalAverage);
        }
    }
}
