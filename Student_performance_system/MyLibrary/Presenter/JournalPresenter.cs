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
    }
}
