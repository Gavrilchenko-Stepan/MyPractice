using MyLibrary;
using MyLibrary.DataModel;
using MyLibrary.DataModel.JournalData;
using MyLibrary.Presenter;
using MyLibrary.Repositories;
using MyLibrary.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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

        public string GroupName => cmbGroups.SelectedItem?.ToString() == "Выберите группу" ? null : cmbGroups.SelectedItem?.ToString();
        public string SubjectName => "Математика";

        public MainForm()
        {
            InitializeComponent();

            InitializeServices();
            ShowLoginForm();
            Shown += (s, e) =>
            {
                LoadGroups();
                LoadJournalAutomatically();
            };

            dataGridViewJournal.ColumnHeaderMouseDoubleClick += DataGridViewJournal_ColumnHeaderMouseDoubleClick;
            cmbGroups.SelectedIndexChanged += CmbGroups_SelectedIndexChanged;
        }

        private void CmbGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbGroups.SelectedItem != null)
            {
                string selectedItem = cmbGroups.SelectedItem.ToString();

                if (selectedItem == "Выберите группу")
                {
                    // Очищаем журнал если выбрана подсказка
                    dataGridViewJournal.DataSource = null;
                    dataGridViewJournal.Columns.Clear();
                    this.Text = "Журнал оценок - выберите группу";
                }
                else
                {
                    // УДАЛЯЕМ подсказку из списка после выбора реальной группы
                    if (cmbGroups.Items.Contains("Выберите группу"))
                    {
                        cmbGroups.Items.Remove("Выберите группу");
                    }

                    // Загружаем журнал для выбранной группы
                    _presenter.LoadJournal();
                }
            }
        }

        private void LoadGroups()
        {
            try
            {
                var groups = _presenter.GetGroups();
                cmbGroups.Items.Clear();

                // ДОБАВЛЯЕМ подсказку только если есть реальные группы
                if (groups.Count > 0)
                {
                    cmbGroups.Items.Add("Выберите группу");
                    cmbGroups.Items.AddRange(groups.ToArray());
                    cmbGroups.SelectedIndex = 0; // Выбираем подсказку
                }
                else
                {
                    cmbGroups.Items.AddRange(groups.ToArray());
                    if (cmbGroups.Items.Count > 0)
                        cmbGroups.SelectedIndex = 0;
                }

                // ОЧИЩАЕМ журнал
                dataGridViewJournal.DataSource = null;
                dataGridViewJournal.Columns.Clear();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка при загрузке групп: {ex.Message}");

            dataGridViewJournal.CellDoubleClick += DataGridViewJournal_CellDoubleClick;
            dataGridViewJournal.Click += DataGridViewJournal_Click;
        }

        private void DataGridViewJournal_Click(object sender, EventArgs e)
        {
            foreach (Control control in dataGridViewJournal.Controls.OfType<TextBox>().ToList())
            {
                control.Dispose();
            }
        }

        private void DataGridViewJournal_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 2) return;
            if (e.ColumnIndex >= dataGridViewJournal.Columns.Count - 1) return; // Пропускаем колонку среднего балла

            StartGradeEditing(e.RowIndex, e.ColumnIndex);
        }

        private void StartGradeEditing(int rowIndex, int columnIndex)
        {
            try
            {
                RowData rowData = dataGridViewJournal.Rows[rowIndex].DataBoundItem as RowData;
                if (rowData?.Student == null) return;

                // Получаем дату из заголовка колонки
                string columnHeaderText = dataGridViewJournal.Columns[columnIndex].HeaderText.ToString();

                if (!TryParseDate(columnHeaderText, out DateTime lessonDate, out int? lessonNumber))
                {
                    ShowErrorMessage("Неверный формат даты в заголовке колонки");
                    return;
                }

                // Находим текущую оценку
                Grade currentGrade = rowData.Grades?.FirstOrDefault(g =>
                    g.LessonDate.Date == lessonDate.Date &&
                    g.LessonNumber == lessonNumber);

                int? currentGradeValue = currentGrade?.GradeValue;

                // Создаем TextBox для редактирования
                var cellRect = dataGridViewJournal.GetCellDisplayRectangle(columnIndex, rowIndex, true);

                var textBox = new TextBox
                {
                    Text = currentGradeValue?.ToString() ?? "",
                    Bounds = cellRect,
                    Font = dataGridViewJournal.Font,
                    BackColor = Color.LightYellow,
                    BorderStyle = BorderStyle.FixedSingle,
                    TextAlign = HorizontalAlignment.Center,
                    MaxLength = 1
                };

                textBox.KeyDown += (s, ke) =>
                {
                    if (ke.KeyCode == Keys.Enter)
                    {
                        SaveGrade(rowData.Student.StudentId, lessonDate, lessonNumber, textBox.Text);
                        textBox.Dispose();
                    }
                    else if (ke.KeyCode == Keys.Escape)
                    {
                        textBox.Dispose();
                    }
                };

                textBox.LostFocus += (s, ev) =>
                {
                    if (!textBox.IsDisposed)
                    {
                        SaveGrade(rowData.Student.StudentId, lessonDate, lessonNumber, textBox.Text);
                        textBox.Dispose();
                    }
                };

                dataGridViewJournal.Controls.Add(textBox);
                textBox.Focus();
                textBox.SelectAll();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка при начале редактирования: {ex.Message}");
            }
        }

        private void SaveGrade(int studentId, DateTime lessonDate, int? lessonNumber, string input)
        {
            try
            {
                int? gradeValue = null;

                if (!string.IsNullOrWhiteSpace(input))
                {
                    if (int.TryParse(input, out int grade) && grade >= 2 && grade <= 5)
                    {
                        gradeValue = grade;
                    }
                    else
                    {
                        ShowErrorMessage("Оценка должна быть числом от 2 до 5");
                        return;
                    }
                }

                _presenter.UpdateGrade(studentId, lessonDate, lessonNumber, gradeValue);

                dataGridViewJournal.Invalidate();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка при сохранении оценки: {ex.Message}");
            }
        }

        private void DataGridViewJournal_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex < 2 || e.ColumnIndex >= dataGridViewJournal.Columns.Count - 1) return;

            var column = dataGridViewJournal.Columns[e.ColumnIndex];
            var headerRect = dataGridViewJournal.GetCellDisplayRectangle(e.ColumnIndex, -1, true);

            string currentHeader = column.HeaderText.ToString();
            string editText = currentHeader;

            // Автоматически добавляем скобки, если их нет
            if (!currentHeader.Contains("(") || !currentHeader.Contains(")"))
            {
                // Извлекаем дату (формат "dd.MM.")
                string datePart = currentHeader.TrimEnd('.');
                if (DateTime.TryParseExact(datePart, "dd.MM", null, System.Globalization.DateTimeStyles.None, out DateTime date))
                {
                    editText = $"{date:dd.MM.}()";
                }
            }

            var textBox = new TextBox
            {
                Text = editText,
                Bounds = headerRect,
                Font = dataGridViewJournal.ColumnHeadersDefaultCellStyle.Font,
                BackColor = Color.LightYellow
            };

            // Устанавливаем курсор между скобками
            if (editText.EndsWith("()"))
            {
                textBox.Select(editText.Length - 1, 0);
            }
            else
            {
                textBox.SelectAll();
            }

            textBox.KeyDown += (s, ke) =>
            {
                if (ke.KeyCode == Keys.Enter) SaveEdit(column, textBox.Text);
                if (ke.KeyCode == Keys.Escape) textBox.Dispose();
            };

            textBox.LostFocus += (s, ev) =>
            {
                SaveEdit(column, textBox.Text);
                textBox.Dispose();
            };

            dataGridViewJournal.Controls.Add(textBox);
            textBox.Focus();
        }

        private void SaveEdit(DataGridViewColumn column, string newHeader)
        {
            if (TryParseDate(newHeader, out DateTime newDate, out int? newNumber) &&
                TryParseDate(column.HeaderText.ToString(), out DateTime oldDate, out int? oldNumber))
            {
                if (newDate > DateTime.Now)
                {
                    ShowErrorMessage("Дата не может быть в будущем");
                    return;
                }

                if (newDate != oldDate || newNumber != oldNumber)
                {
                    _presenter.EditLessonDate(
                        new LessonData(oldDate, oldNumber),
                        new LessonData(newDate, newNumber)
                    );
                }
            }
            else
            {
                ShowErrorMessage("Неверный формат даты. Пример: 15.02 или 15.02(2)");
            }
        }

        private bool TryParseDate(string headerText, out DateTime date, out int? lessonNumber)
        {
            date = DateTime.MinValue;
            lessonNumber = null;

            if (string.IsNullOrEmpty(headerText))
                return false;

            string datePart = headerText;

            // Парсим номер пары из скобок
            if (headerText.Contains("(") && headerText.Contains(")"))
            {
                int openBracket = headerText.IndexOf('(');
                int closeBracket = headerText.IndexOf(')');

                datePart = headerText.Substring(0, openBracket).Trim();

                string numberPart = headerText.Substring(openBracket + 1, closeBracket - openBracket - 1).Trim();
                if (int.TryParse(numberPart, out int number) && number >= 1 && number <= 5)
                {
                    lessonNumber = number;
                }
                else if (!string.IsNullOrEmpty(numberPart))
                {
                    return false; // Некорректный номер пары
                }
            }

            // Парсим дату (форматы: "dd.MM", "dd.MM.")
            datePart = datePart.Trim().TrimEnd('.');

            // Добавляем текущий год для корректного парсинга
            string fullDateString = datePart + $".{DateTime.Now:yyyy}";

            return DateTime.TryParseExact(fullDateString, "dd.MM.yyyy", null,
                System.Globalization.DateTimeStyles.None, out date);
        }

        private void InitializeServices()
        {
            IUserRepository userRepository = new UserRepository();
            _authService = new AuthService(userRepository);

            InitializeRepositoriesAndPresenter();
        }

        private void InitializeRepositoriesAndPresenter()
        {
            string connectionString = IniConfig.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Строка подключения не может быть пустой");

            var studentRepository = new MySqlStudentRepository(IniConfig.ConnectionString);
            var gradeRepository = new MySqlGradeRepository(IniConfig.ConnectionString);
            var journalCommandRepository = new MySqlJournalCommandRepository(connectionString);

            var journalService = new JournalService(studentRepository, gradeRepository, journalCommandRepository);

            // Передаем this как IJournalView
            _presenter = new JournalPresenter(this, journalService);
        
        }

        private void LoadJournalAutomatically()
        {
            ///_presenter.LoadJournal();
            dataGridViewJournal.DataSource = null;
            dataGridViewJournal.Columns.Clear();
            this.Text = "Журнал оценок - выберите группу";
        }

        private void DataGridViewJournal_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 2) return; // Пропускаем заголовки, номер и студента

            RowData rowData = dataGridViewJournal.Rows[e.RowIndex].DataBoundItem as RowData;
            if (rowData == null) return;

            // Получаем колонку и ее имя
            var column = dataGridViewJournal.Columns[e.ColumnIndex];
            string columnName = column.Name;

            // Парсим дату и номер пары из имени колонки
            if (TryParseDateFromColumnName(columnName, out DateTime lessonDate, out int? lessonNumber))
            {
                // Ищем оценку для этой даты и номера пары
                Grade grade = rowData.Grades?.FirstOrDefault(g =>
                    g.LessonDate.Date == lessonDate.Date &&
                    g.LessonNumber == lessonNumber);

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
            }
            else
            {
                e.Value = "";
                e.CellStyle.BackColor = Color.White;
            }

            e.FormattingApplied = true;
        }

        private bool TryParseDateFromColumnName(string columnName, out DateTime date, out int? lessonNumber)
        {
            date = DateTime.MinValue;
            lessonNumber = null;

            try
            {
                // Формат: "20241215_1" или "20241215_"
                if (columnName.Contains("_"))
                {
                    var parts = columnName.Split('_');

                    // Парсим дату из первой части
                    if (DateTime.TryParseExact(parts[0], "yyyyMMdd", null,
                        System.Globalization.DateTimeStyles.None, out date))
                    {
                        // Парсим номер пары из второй части
                        if (parts.Length > 1 && !string.IsNullOrEmpty(parts[1]) &&
                            int.TryParse(parts[1], out int number))
                        {
                            lessonNumber = number;
                        }
                        return true;
                    }
                }
            }
            catch
            {
                // Игнорируем ошибки парсинга
            }

            return false;
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

                dataGridViewJournal.AutoGenerateColumns = false;


                // Добавляем колонку номера
                dataGridViewJournal.Columns.Add("Number", "№");
                dataGridViewJournal.Columns["Number"].Width = 40;
                dataGridViewJournal.Columns["Number"].Frozen = true;
                dataGridViewJournal.Columns["Number"].ReadOnly = true;

                // Добавляем колонку студента
                dataGridViewJournal.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "StudentName",
                    HeaderText = "Студент",
                    DataPropertyName = "StudentName", // Связываем с свойством StudentName из RowData
                    Frozen = true,
                    Width = 200,
                    ReadOnly = true
                });

                var allLessons = journalData.Rows
                        .SelectMany(r => r.Grades)
                        .Select(g => new { Date = g.LessonDate, LessonNumber = g.LessonNumber })
                        .Distinct()
                        .OrderBy(x => x.Date)
                        .ThenBy(x => x.LessonNumber)
                        .ToList();

                // Создаем колонки для каждой пары
                foreach (var lesson in allLessons)
                {
                    string columnName = $"{lesson.Date:yyyyMMdd}_{lesson.LessonNumber}";

                    string headerText;
                    if (lesson.LessonNumber.HasValue)
                    {
                        headerText = $"{lesson.Date:dd.MM.}({lesson.LessonNumber})";
                    }
                    else
                    {
                        headerText = $"{lesson.Date:dd.MM.}";
                    }

                    var dateColumn = new DataGridViewTextBoxColumn
                    {
                        Name = columnName, // Важно: одинаковый формат для сопоставления
                        HeaderText = headerText,
                        Width = 70,
                        DefaultCellStyle = new DataGridViewCellStyle
                        {
                            Alignment = DataGridViewContentAlignment.MiddleCenter
                        }
                    };
                    dataGridViewJournal.Columns.Add(dateColumn);
                }

                // Добавляем колонку среднего балла
                dataGridViewJournal.Columns.Add(new DataGridViewTextBoxColumn
                {
                    Name = "Average",
                    HeaderText = "Средний балл",
                    Width = 100,
                    DefaultCellStyle = new DataGridViewCellStyle
                    {
                        Alignment = DataGridViewContentAlignment.MiddleCenter
                    }
                });

                // Устанавливаем источник данных
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

        public void ShowSuccessMessage(string message)
        {
            MessageBox.Show(message, "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public LessonData GetNewLessonData()
        {
            using (var dialog = new AddLessonDateDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return new LessonData(dialog.SelectedDate, dialog.LessonNumber);
                }
            }
            return null;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(GroupName))
            {
                ShowErrorMessage("Сначала выберите группу");
                return;
            }

            try
            {
                _presenter.AddLessonDate();
            }
            catch (Exception ex)
            {
                ShowErrorMessage($"Ошибка: {ex.Message}");
            }
        }
    }
}
