using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib
{
    public class AverageGradeService
    {
        private readonly IGradesRepository _gradesRepository;

        public AverageGradeService(IGradesRepository gradesRepository)
        {
            _gradesRepository = gradesRepository;
        }

        public string CalculateAverage(int studentId, int subjectId)
        {
            if (studentId <= 0)
                return "Ошибка: ID студента должен быть положительным числом";

            if (subjectId <= 0)
                return "Ошибка: ID дисциплины должен быть положительным числом";

            try
            {
                var grades = _gradesRepository.GetStudentGrades(studentId, subjectId);

                if (grades == null || !grades.Any())
                    return $"Не найдено оценок для студента c ID {studentId} по дисциплине c ID {subjectId}";

                double average = grades.Average(g => g.GradeValue);

                return $"Средний балл студента c ID {studentId} по дисциплине c ID {subjectId}: {average:F2}";
            }
            catch (Exception ex)
            {
                return $"Ошибка при расчете среднего балла: {ex.Message}";
            }
        }
    }
}
