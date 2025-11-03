using MyLibrary;
using MyLibrary.Presenter;
using MyLibrary.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MainForm
{
    public partial class MainForm : Form
    {
        private AuthService _authService;
        private JournalPresenter _presenter;

        public string GroupName => AppConfig.DefaultGroup;
        public string SubjectName => AppConfig.DefaultSubject;

        public MainForm()
        {
            InitializeComponent();
            InitializeServices();
            ShowLoginForm();
        }

        private void InitializeServices()
        {
            IUserRepository userRepository = new UserRepository();
            _authService = new AuthService(userRepository);
            var studentRepository = new MySqlStudentRepository(AppConfig.ConnectionString);
            var gradeRepository = new MySqlGradeRepository(AppConfig.ConnectionString);
            var journalService = new JournalService(studentRepository, gradeRepository);
            _presenter = new JournalPresenter(this, journalService);
        }

        private void LoadJournalAutomatically()
        {
            _presenter.LoadJournal();
        }

        private void ShowLoginForm()
        {
            using (var loginForm = new LoginForm(_authService))
            {
                if (loginForm.ShowDialog() != DialogResult.OK)
                {
                    Application.Exit();
                    return;
                }
            }
        }

        private void toolStripButtonLogout_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Вы уверены, что хотите выйти?", "Подтверждение выхода",
                                       MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _authService.Logout();
                ShowLoginForm();
            }
        }
    }
}
