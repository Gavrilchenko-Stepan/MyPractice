using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyLibrary;
using System;
using System.Collections.Generic;

namespace Academic_performance_Tests
{
    [TestClass]
    public class TGradeCalculator
    {
        private GradeCalculator _calculator;
        private const string DisciplineName = "Математика";
        [TestInitialize]
        public void Setup()
        {
            _calculator = new GradeCalculator();
        }

        [TestMethod]
        public void CalculateAverageGrade_WithAllGrades_ReturnsCorrectAverage()
        {
            var grades = new List<int> { 4, 5, 5 };
            var expectedAverage = 4.67;

            var result = _calculator.CalculateAverageGrade(grades, DisciplineName);

            Assert.AreEqual(expectedAverage, (double)result, 0.01);
        }

        [TestMethod]
        public void CalculateAverageGrade_WithEmptyGrades_ReturnsNoDataMessage()
        {
            var grades = new List<int>();
            var expectedMessage = $"По дисциплине {DisciplineName} нет данных об оценках.";

            var exception = Assert.ThrowsException<InvalidOperationException>(() =>
_calculator.CalculateAverageGrade(grades, DisciplineName));

            Assert.AreEqual(expectedMessage, exception.Message);
        }
    }
}
