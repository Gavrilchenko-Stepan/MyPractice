using MyLibrary;
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
    public partial class JournalForm : Form
    {
        private readonly IGradesRepository _gradesRepository;
        private List<DateTime> _currentDates;
        private int _currentSubjectId;
        private string _currentGroup;
        public JournalForm()
        {
            InitializeComponent();
            _gradesRepository = new GradesRepository();
            _currentDates = new List<DateTime>();

            InitializeComboBoxes();
            InitializeDataGrid();
            LoadJournalData();
        }

        private void InitializeComboBoxes()
        {
            // Заполнение групп
            comboBoxGroups.DataSource = _gradesRepository.GetGroups();

            // Заполнение дисциплин
            comboBoxSubjects.DataSource = _gradesRepository.GetSubjects();
            comboBoxSubjects.DisplayMember = "Name";
            comboBoxSubjects.ValueMember = "Id";

            if (comboBoxGroups.Items.Count > 0)
            {
                comboBoxGroups.SelectedIndex = 0;
                _currentGroup = comboBoxGroups.SelectedItem.ToString();
            }

            if (comboBoxSubjects.Items.Count > 0)
            {
                comboBoxSubjects.SelectedIndex = 0;
                var selectedSubject = (Subject)comboBoxSubjects.SelectedItem;
                _currentSubjectId = selectedSubject.Id;
            }
        }

        private void InitializeDataGrid()
        {
            dataGridViewJournal.Columns.Clear();
        }

        private void LoadJournalData()
        {
            if (comboBoxSubjects.SelectedValue == null || comboBoxGroups.SelectedItem == null)
                return;

            var selectedSubject = (Subject)comboBoxSubjects.SelectedItem;
            _currentSubjectId = selectedSubject.Id;

            _currentGroup = comboBoxGroups.SelectedItem.ToString();

            var journalData = _gradesRepository.GetGradeJournal(_currentSubjectId, _currentGroup);
            _currentDates = _gradesRepository.GetGradeDates(_currentSubjectId, _currentGroup);

            FillDataGrid(journalData);
            ApplyFormatting();

            // Обновляем заголовок формы
            this.Text = $"Журнал оценок - {_currentGroup} - {comboBoxSubjects.Text}";
        }

        private void FillDataGrid(List<GradeJournal> journalData)
        {
            dataGridViewJournal.DataSource = null;
            dataGridViewJournal.Rows.Clear();

            // Используем DataTable для гарантированного создания всех колонок
            var dataTable = new DataTable();

            // Колонки с датами
            foreach (var date in _currentDates)
            {
                string columnName = GetDateColumnName(date); // "Date_20250209"
                dataTable.Columns.Add(columnName, typeof(string));
            }

            // Колонка среднего балла
            dataTable.Columns.Add("Средний", typeof(string));

            // Заполняем данные
            foreach (var journal in journalData)
            {
                var row = dataTable.NewRow();

                // Оценки по датам
                foreach (var date in _currentDates)
                {
                    string columnName = GetDateColumnName(date);
                    int? gradeValue = journal.GradesByDate.ContainsKey(date) ?
                        journal.GradesByDate[date] : null;
                    row[columnName] = gradeValue?.ToString() ?? "";
                }

                // Средний балл
                row[_currentDates.Count] = journal.AverageGrade > 0 ? journal.AverageGrade.ToString("F2") : "";

                dataTable.Rows.Add(row);
            }

            dataGridViewJournal.DataSource = dataTable;
            UpdateRowHeaders(journalData);
        }

        private string GetDateColumnName(DateTime date)
        {
            return $"{date:dd.MM}";
        }

        private void UpdateRowHeaders(List<GradeJournal> journalData)
        {
            for (int i = 0; i < journalData.Count; i++)
            {
                if (i < dataGridViewJournal.Rows.Count)
                {
                    dataGridViewJournal.Rows[i].HeaderCell.Value =
                        $"{i + 1}. {journalData[i].Student.Name}";
                }
            }
        }

        private void ApplyFormatting()
        {
            foreach (DataGridViewRow row in dataGridViewJournal.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.OwningColumn.Name != "Average")
                    {
                        string value = cell.Value?.ToString() ?? "";
                        ApplyCellFormatting(cell, value);
                    }
                }
            }
        }

        private void ApplyCellFormatting(DataGridViewCell cell, string value)
        {
            cell.Style.BackColor = Color.White;
            cell.Style.ForeColor = Color.Black;
            cell.Style.Font = dataGridViewJournal.DefaultCellStyle.Font;

            switch (value)
            {
                case "5":
                    cell.Style.BackColor = Color.LightGreen;
                    cell.Style.ForeColor = Color.DarkGreen;
                    cell.Style.Font = new Font(dataGridViewJournal.Font, FontStyle.Bold);
                    break;
                case "4":
                    cell.Style.BackColor = Color.LightBlue;
                    cell.Style.ForeColor = Color.DarkBlue;
                    cell.Style.Font = new Font(dataGridViewJournal.Font, FontStyle.Bold);
                    break;
                case "3":
                    cell.Style.BackColor = Color.LightYellow;
                    cell.Style.ForeColor = Color.OrangeRed;
                    cell.Style.Font = new Font(dataGridViewJournal.Font, FontStyle.Bold);
                    break;
                case "2":
                    cell.Style.BackColor = Color.LightCoral;
                    cell.Style.ForeColor = Color.DarkRed;
                    cell.Style.Font = new Font(dataGridViewJournal.Font, FontStyle.Bold);
                    break;
                default:
                    cell.Style.ForeColor = Color.Gray;
                    break;
            }
        }

        private void comboBoxGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxGroups.SelectedItem != null)
            {
                LoadJournalData();
            }
        }

        private void comboBoxSubjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxSubjects.SelectedItem != null)
            {
                LoadJournalData();
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveAllGrades();
                MessageBox.Show("Оценки сохранены!", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonAddDate_Click(object sender, EventArgs e)
        {
            using (var form = new AddDateForm())
    {
        if (form.ShowDialog() == DialogResult.OK)
        {
            DateTime newDate = form.SelectedDate;
            
            // Проверяем, нет ли уже такой даты
            if (!_currentDates.Contains(newDate.Date))
            {
                try
                {
                    // Добавляем дату в репозиторий
                    _gradesRepository.AddDateForGroup(_currentSubjectId, _currentGroup, newDate);
                    
                    // Обновляем локальную коллекцию дат
                    _currentDates.Add(newDate.Date);
                    _currentDates = _currentDates.OrderBy(d => d).ToList();
                    
                    // Перезагружаем данные
                    LoadJournalData();
                    
                    MessageBox.Show($"Дата {newDate:dd.MM.yyyy} успешно добавлена!", "Успех", 
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении даты: {ex.Message}", "Ошибка", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Эта дата уже существует в журнале!", "Предупреждение", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
        }

        private void dataGridViewJournal_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var cell = dataGridViewJournal.Rows[e.RowIndex].Cells[e.ColumnIndex];

            // Пропускаем колонку среднего балла
            if (cell.OwningColumn.Name == "Average") return;

            /*using (var editForm = new EditGradeForm(cell.Value?.ToString()))
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    cell.Value = editForm.GradeValue;
                    UpdateAverageForRow(e.RowIndex);
                    ApplyCellFormatting(cell, editForm.GradeValue);
                }
            }*/
        }

        private void UpdateAverageForRow(int rowIndex)
        {
            var row = dataGridViewJournal.Rows[rowIndex];
            var grades = new List<int>();

            foreach (DataGridViewCell cell in row.Cells)
            {
                if (cell.OwningColumn.Name != "Average" && !string.IsNullOrEmpty(cell.Value?.ToString()))
                {
                    if (int.TryParse(cell.Value.ToString(), out int grade))
                    {
                        grades.Add(grade);
                    }
                }
            }

            double average = grades.Any() ? grades.Average() : 0;
            row.Cells["Average"].Value = average > 0 ? average.ToString("F2") : "";
        }

        private void SaveAllGrades()
        {
            var students = _gradesRepository.GetStudentsByGroup(_currentGroup);

            for (int rowIndex = 0; rowIndex < dataGridViewJournal.Rows.Count; rowIndex++)
            {
                var row = dataGridViewJournal.Rows[rowIndex];
                if (row.IsNewRow) continue;

                int studentId = students[rowIndex].Id;

                foreach (var date in _currentDates)
                {
                    string columnName = GetDateColumnName(date);
                    string gradeValue = row.Cells[columnName].Value?.ToString() ?? "";

                    if (!string.IsNullOrEmpty(gradeValue) && int.TryParse(gradeValue, out int grade))
                    {
                        _gradesRepository.SaveGrade(studentId, _currentSubjectId, date, grade);
                    }
                }
            }
        }
    }
}
