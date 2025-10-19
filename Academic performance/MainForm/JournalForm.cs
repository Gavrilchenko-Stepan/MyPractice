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
                _currentSubjectId = (int)comboBoxSubjects.SelectedValue;
            }
        }

        private void InitializeDataGrid()
        {
            dataGridViewJournal.Columns.Clear();
            dataGridViewJournal.Rows.Clear();
        }
    }
}
