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
    }
}
