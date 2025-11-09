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

            _currentGroupName = IniConfig.DefaultGroup;
            _currentSubjectName = IniConfig.DefaultSubject;

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
                var studentRepository = new MySqlStudentRepository(IniConfig.ConnectionString);
                var gradeRepository = new MySqlGradeRepository(IniConfig.ConnectionString);
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

        private void DataGridViewJournal_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 2) return; // Пропускаем заголовки, номер и студента

            RowData rowData = dataGridViewJournal.Rows[e.RowIndex].DataBoundItem as RowData;
            if (rowData == null) return;

            // Получаем дату из заголовка колонки
            string columnHeaderText = dataGridViewJournal.Columns[e.ColumnIndex].HeaderText;

            // Ищем оценку для этой даты
            Grade grade = rowData.Grades?.FirstOrDefault(g =>
                g.LessonDate.ToString("dd.MM.") == columnHeaderText);

            if (grade?.GradeValue.HasValue == true)
            {
                e.Value = grade.GradeValue.Value.ToString();
                e.CellStyle.BackColor = GetGradeColor(grade.GradeValue.Value);
                e.CellStyle.ForeColor = Color.Black;
                e.CellStyle.Font = new Font(dataGridViewJournal.Font, FontStyle.Bold);
            }
            else
            {
                e.Value = "";
                e.CellStyle.BackColor = Color.White;
            }

            e.FormattingApplied = true;
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

                dataGridViewJournal.DataSource = null;
                dataGridViewJournal.Columns.Clear();

                dataGridViewJournal.CellFormatting -= DataGridViewJournal_CellFormatting;
                dataGridViewJournal.CellFormatting += DataGridViewJournal_CellFormatting;

                dataGridViewJournal.Columns.Add("Number", "№");
                dataGridViewJournal.Columns["Number"].Width = 40;
                dataGridViewJournal.Columns["Number"].Frozen = true;
                dataGridViewJournal.Columns["Number"].ReadOnly = true;

                // Колонка студента
                dataGridViewJournal.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Student",
                    HeaderText = "Студент",
                    DataPropertyName = "StudentName",
                    Frozen = true,
                    Width = 200,
                    ReadOnly = true
                });

                // Получаем все даты
                List<DateTime> allDates = journalData.Rows
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

                dataGridViewJournal.DataSource = journalData.Rows;

                for (int i = 0; i < dataGridViewJournal.Rows.Count; i++)
                {
                    dataGridViewJournal.Rows[i].Cells["Number"].Value = (i + 1).ToString();
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
