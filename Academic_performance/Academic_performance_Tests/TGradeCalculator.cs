using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyLibrary;
using System;
using System.Collections.Generic;

namespace Academic_performance_Tests
{
    [TestClass]
    public class TGradeCalculator
    {
        [TestMethod]
        public void CalculateAverageGrade_WithAllGrades_ReturnsCorrectAverage()
        {
            GradeCalculator calculator = new GradeCalculator();

            List<int> grades = new List<int> { 4, 5, 5 };
            const string DisciplineName = "Математика";
            double expectedAverage = 4.67;

            object result = calculator.CalculateAverageGrade(grades, DisciplineName);

            Assert.AreEqual(expectedAverage, (double)result, 0.01);
        }

        [TestMethod]
        public void CalculateAverageGrade_WithEmptyGrades_ReturnsNoDataMessage()
        {
            GradeCalculator calculator = new GradeCalculator();

            List<int> grades = new List<int>();
            const string DisciplineName = "Математика";
            var expectedMessage = $"По дисциплине {DisciplineName} нет данных об оценках.";

            object result = calculator.CalculateAverageGrade(grades, DisciplineName);

            Assert.AreEqual(expectedMessage, result);
        }
    }
}
