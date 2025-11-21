using MyLibrary.DataModel;
using MyLibrary.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary.Presenter
{
    public class JournalPresenter
    {
        private readonly IJournalView _view;
        private readonly JournalService _journalService;

        public JournalPresenter(IJournalView view, JournalService journalService)
        {
            _view = view;
            _journalService = journalService;
        }

        public void LoadJournal()
        {
            try
            {
                var journalData = _journalService.GetJournalData(_view.GroupName, _view.SubjectName);
                _view.DisplayJournal(journalData);
            }
            catch (Exception ex)
            {
                _view.ShowErrorMessage($"Не удалось загрузить журнал: {ex.Message}");
            }
        }

        public void AddLessonDate()
        {
            try
            {
                LessonData lessonData = _view.GetNewLessonData();

                if (lessonData == null)
                    return; // Пользователь отменил ввод

                // Вызов сервиса
                bool success = _journalService.AddLessonDate(
                    _view.GroupName,
                    _view.SubjectName,
                    lessonData);

                // Обработка результата
                if (success)
                {
                    LoadJournal();
                    ShowSuccessMessage(lessonData);
                }
                else
                {
                    ShowErrorMessage(lessonData);
                }
            }
            catch (Exception ex)
            {
                _view.ShowErrorMessage($"Ошибка при добавлении даты: {ex.Message}");
            }
        }

        private void ShowSuccessMessage(LessonData lessonData)
        {
            string message = $"Дата занятия {lessonData} успешно добавлена";
            _view.ShowSuccessMessage(message);
        }

        private void ShowErrorMessage(LessonData lessonData)
        {
            string message = $"Занятие {lessonData} уже существует в журнале";
            _view.ShowErrorMessage(message);
        }
    }
}
