using MyLibrary.DataModel.JournalData;
using MyLibrary.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary
{
    public class JournalService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IGradeRepository _gradeRepository;

        public JournalService(IStudentRepository studentRepository, IGradeRepository gradeRepository)
        {
            _studentRepository = studentRepository;
            _gradeRepository = gradeRepository;
        }

        public JournalData GetJournalData(string groupName, string subjectName)
        {
            try
            {
                var students = _studentRepository.GetStudentsByGroup(groupName);
                var grades = _gradeRepository.GetGradesByGroupAndSubject(groupName, subjectName);

                var journalData = new JournalData
                {
                    GroupName = groupName,
                    SubjectName = subjectName,
                    Rows = new List<RowData>()
                };

                foreach (var student in students)
                {
                    journalData.Rows.Add(new RowData
                    {
                        Student = student,
                        Grades = grades
                    });
                }

                return journalData;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка при загрузке данных журнала: {ex.Message}", ex);
            }
        }
    }
}
