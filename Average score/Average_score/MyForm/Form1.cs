using MyLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            IGradesRepository repository = new GradesRepository();
            AverageGradeService averageGradeService = new AverageGradeService(repository);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void buttonCalculation_Click(object sender, EventArgs e)
        {
            IGradesRepository repository = new GradesRepository();
            AverageGradeService averageGradeService = new AverageGradeService(repository);
            if (!int.TryParse(textIDstudent.Text, out int studentId))
            {
                MessageBox.Show("Пожалуйста, введите корректный ID студента", "Ошибка ввода",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(textIDdiscipline.Text, out int subjectId))
            {
                MessageBox.Show("Пожалуйста, введите корректный ID дисциплины", "Ошибка ввода",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string result = averageGradeService.CalculateAverage(studentId, subjectId);
            richTextBoxResult.Text = result;
        }
    }
}
