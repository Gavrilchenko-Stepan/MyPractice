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
    public partial class AddLessonDateDialog : Form
    {
        public DateTime SelectedDate => dateTimePickerDate.Value;
        public int? LessonNumber => checkBoxLessonNumber.Checked ? (int?)numericUpDownLessonNumber.Value : null;

        public AddLessonDateDialog()
        {
            InitializeComponent();
        }

        private void checkBoxLessonNumber_CheckedChanged(object sender, EventArgs e)
        {
            // Включаем/выключаем NumericUpDown в зависимости от состояния CheckBox
            numericUpDownLessonNumber.Enabled = checkBoxLessonNumber.Checked;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (SelectedDate > DateTime.Now)
            {
                MessageBox.Show("Дата занятия не может быть в будущем",
                              "Ошибка ввода",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Warning);
                return;
            }

            if (LessonNumber.HasValue && (LessonNumber < 1 || LessonNumber > 5))
            {
                MessageBox.Show("Номер пары должен быть от 1 до 5",
                              "Ошибка ввода",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Warning);
                return;
            }

            // Если все проверки пройдены - закрываем форму с результатом OK
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
