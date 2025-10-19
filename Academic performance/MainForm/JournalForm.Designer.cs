namespace MainForm
{
    partial class JournalForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonAddDate = new System.Windows.Forms.Button();
            this.comboBoxSubjects = new System.Windows.Forms.ComboBox();
            this.comboBoxGroups = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridViewJournal = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewJournal)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonSave);
            this.panel1.Controls.Add(this.buttonAddDate);
            this.panel1.Controls.Add(this.comboBoxSubjects);
            this.panel1.Controls.Add(this.comboBoxGroups);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(510, 86);
            this.panel1.TabIndex = 0;
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(181, 51);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(75, 23);
            this.buttonSave.TabIndex = 5;
            this.buttonSave.Text = "Сохранить";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // buttonAddDate
            // 
            this.buttonAddDate.Location = new System.Drawing.Point(25, 51);
            this.buttonAddDate.Name = "buttonAddDate";
            this.buttonAddDate.Size = new System.Drawing.Size(108, 23);
            this.buttonAddDate.TabIndex = 4;
            this.buttonAddDate.Text = "Добавить дату";
            this.buttonAddDate.UseVisualStyleBackColor = true;
            this.buttonAddDate.Click += new System.EventHandler(this.buttonAddDate_Click);
            // 
            // comboBoxSubjects
            // 
            this.comboBoxSubjects.FormattingEnabled = true;
            this.comboBoxSubjects.Location = new System.Drawing.Point(371, 11);
            this.comboBoxSubjects.Name = "comboBoxSubjects";
            this.comboBoxSubjects.Size = new System.Drawing.Size(121, 21);
            this.comboBoxSubjects.TabIndex = 3;
            this.comboBoxSubjects.SelectedIndexChanged += new System.EventHandler(this.comboBoxSubjects_SelectedIndexChanged);
            // 
            // comboBoxGroups
            // 
            this.comboBoxGroups.FormattingEnabled = true;
            this.comboBoxGroups.Location = new System.Drawing.Point(70, 11);
            this.comboBoxGroups.Name = "comboBoxGroups";
            this.comboBoxGroups.Size = new System.Drawing.Size(121, 21);
            this.comboBoxGroups.TabIndex = 2;
            this.comboBoxGroups.SelectedIndexChanged += new System.EventHandler(this.comboBoxGroups_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(295, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Дисциплина";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Группа";
            // 
            // dataGridViewJournal
            // 
            this.dataGridViewJournal.AllowUserToAddRows = false;
            this.dataGridViewJournal.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewJournal.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewJournal.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewJournal.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.dataGridViewJournal.Location = new System.Drawing.Point(0, 104);
            this.dataGridViewJournal.Name = "dataGridViewJournal";
            this.dataGridViewJournal.Size = new System.Drawing.Size(1013, 557);
            this.dataGridViewJournal.TabIndex = 1;
            this.dataGridViewJournal.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewJournal_CellDoubleClick);
            // 
            // JournalForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1013, 661);
            this.Controls.Add(this.dataGridViewJournal);
            this.Controls.Add(this.panel1);
            this.Name = "JournalForm";
            this.Text = "Журнал оценок";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewJournal)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonAddDate;
        private System.Windows.Forms.ComboBox comboBoxSubjects;
        private System.Windows.Forms.ComboBox comboBoxGroups;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.DataGridView dataGridViewJournal;
    }
}

