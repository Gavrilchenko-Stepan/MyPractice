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
        private string _currentGroupName;
        private string _currentSubjectName;

        public string GroupName => _currentGroupName;
        public string SubjectName => _currentSubjectName;

        public MainForm()
        {
            InitializeComponent();

            _currentGroupName = AppConfig.DefaultGroup;
            _currentSubjectName = AppConfig.DefaultSubject;

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
                if (this.InvokeRequired) /// гарантирует, что обновление DataGridView всегда происходит в правильном потоке
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

                dataGridViewJournal.Columns.Add("Number", "№");
                dataGridViewJournal.Columns["Number"].Width = 30;

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
                    var columnName = date.ToString("dd.MM.");
                    dataGridViewJournal.Columns.Add(columnName, columnName);
                    dataGridViewJournal.Columns[columnName].Width = 80;
                    dataGridViewJournal.Columns[columnName].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                }

                dataGridViewJournal.Columns.Add("Average", "Средний балл");
                dataGridViewJournal.Columns["Average"].Width = 100;
                dataGridViewJournal.Columns["Average"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // Заполняем данные
                for (int i = 0; i < journalData.Rows.Count; i++)
                {
                    var rowData = journalData.Rows[i];
                    var rowIndex = dataGridViewJournal.Rows.Add();

                    dataGridViewJournal.Rows[rowIndex].Cells["Number"].Value = i + 1;
                    dataGridViewJournal.Rows[rowIndex].Cells["Student"].Value = rowData.Student.FullName;

                    // Заполняем оценки по датам
                    foreach (var date in allDates)
                    {
                        var dateColumnName = date.ToString("dd.MM.");
                        var cell = dataGridViewJournal.Rows[rowIndex].Cells[dateColumnName];
                        
                        var grade = rowData.Grades.FirstOrDefault(g =>
                    g.LessonDate.ToString("dd.MM.") == dateColumnName);

                        if (grade != null)
                        {
                            cell.Value = grade.GradeValue.ToString();
                            cell.Style.BackColor = GetGradeColor(grade.GradeValue.Value);
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
