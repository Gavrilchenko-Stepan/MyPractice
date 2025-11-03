using MyLibrary;
using MyLibrary.DataModel.JournalData;
using MyLibrary.Presenter;
using MyLibrary.Repositories;
using MyLibrary.View;
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
    public partial class MainForm : Form, IJournalView
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
            Shown += (s, e) => LoadJournalAutomatically();
        }

        private void InitializeServices()
        {
            IUserRepository userRepository = new UserRepository();
            _authService = new AuthService(userRepository);

            InitializeRepositoriesAndPresenter();
        }

        private void InitializeRepositoriesAndPresenter()
        {
            try
            {
                var studentRepository = new MySqlStudentRepository(AppConfig.ConnectionString);
                var gradeRepository = new MySqlGradeRepository(AppConfig.ConnectionString);
                var journalService = new JournalService(studentRepository, gradeRepository);

                // Передаем this как IJournalView
                _presenter = new JournalPresenter(this, journalService);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка инициализации: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadJournalAutomatically()
        {
            _presenter.LoadJournal();
        }

        public void DisplayJournal(JournalData journalData)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(new Action<JournalData>(DisplayJournal), journalData);
                    return;
                }

                dataGridViewJournal.Columns.Clear();
                dataGridViewJournal.Rows.Clear();

                if (journalData.Rows == null || journalData.Rows.Count == 0)
                {
                    ShowErrorMessage("Нет данных для отображения");
                    return;
                }

                // Колонка "Студент"
                dataGridViewJournal.Columns.Add("Student", "Студент");
                dataGridViewJournal.Columns["Student"].Frozen = true;
                dataGridViewJournal.Columns["Student"].Width = 200;

                // Получаем все уникальные даты
                var allDates = journalData.Rows
                    .SelectMany(r => r.Grades)
                    .Select(g => g.LessonDate)
                    .Distinct()
                    .OrderBy(d => d)
                    .ToList();

                // Создаем колонки для каждой даты
                foreach (var date in allDates)
                {
                    var columnName = date.ToString("dd.MM.yyyy");
                    dataGridViewJournal.Columns.Add(columnName, columnName);
                    dataGridViewJournal.Columns[columnName].Width = 80;
                }

                // Заполняем данные
                foreach (var rowData in journalData.Rows)
                {
                    var rowIndex = dataGridViewJournal.Rows.Add();
                    dataGridViewJournal.Rows[rowIndex].Cells["Student"].Value = rowData.Student.FullName;

                    // Заполняем оценки по датам
                    foreach (var date in allDates)
                    {
                        var dateColumnName = date.ToString("dd.MM.yyyy");
                        var cell = dataGridViewJournal.Rows[rowIndex].Cells[dateColumnName];
                        
                        var grade = rowData.Grades.FirstOrDefault(g =>
                    g.LessonDate.ToString("dd.MM.yyyy") == dateColumnName);

                        if (grade != null)
                        {
                            cell.Value = grade.GradeValue.ToString();
                            cell.Style.BackColor = GetGradeColor(grade.GradeValue);
                            cell.Style.ForeColor = Color.Black;
                            cell.Style.Font = new Font(dataGridViewJournal.Font, FontStyle.Bold);
                        }
                        else
                        {
                            cell.Value = ""; // Пусто если нет оценки
                            cell.Style.BackColor = Color.White;
                        }
                    }
                }

                this.Text = $"Журнал оценок - {journalData.GroupName} - {journalData.SubjectName}";
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка при отображении журнала: {ex.Message}");
            }
        }

        public void ShowErrorMessage(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private Color GetGradeColor(int grade)
        {
            switch (grade)
            {
                case 5:
                    return Color.LightGreen;
                case 4:
                    return Color.LightBlue;
                case 3:
                    return Color.LightYellow;
                case 2:
                    return Color.LightCoral;
                default:
                    return Color.White;
            }
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
