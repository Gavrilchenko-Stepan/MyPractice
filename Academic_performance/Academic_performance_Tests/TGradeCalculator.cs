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
            double expectedAverage = 4.67;

            double result = calculator.CalculateAverageGrade(grades);

            Assert.AreEqual(expectedAverage, result, 0.01);
        }

        [TestMethod]
        public void CalculateAverageGrade_WithEmptyGrades_ReturnsZero()
        {
            GradeCalculator calculator = new GradeCalculator();

            List<int> grades = new List<int>();
            double expected = 0.0;

            double result = calculator.CalculateAverageGrade(grades);

            Assert.AreEqual(expected, result);
        }
    }
}
