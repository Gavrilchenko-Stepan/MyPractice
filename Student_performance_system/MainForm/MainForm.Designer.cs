namespace MainForm
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonLogout = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.dataGridViewJournal = new System.Windows.Forms.DataGridView();
            this.SelectPanel = new System.Windows.Forms.Panel();
            this.cmbGroups = new System.Windows.Forms.ComboBox();
            this.lblGroup = new System.Windows.Forms.Label();
            this.lblSubject = new System.Windows.Forms.Label();
            this.cmbSubjects = new System.Windows.Forms.ComboBox();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewJournal)).BeginInit();
            this.SelectPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonLogout,
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1184, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonLogout
            // 
            this.toolStripButtonLogout.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripButtonLogout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButtonLogout.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButtonLogout.Image")));
            this.toolStripButtonLogout.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonLogout.Name = "toolStripButtonLogout";
            this.toolStripButtonLogout.Size = new System.Drawing.Size(46, 22);
            this.toolStripButtonLogout.Text = "Выход";
            this.toolStripButtonLogout.Click += new System.EventHandler(this.toolStripButtonLogout_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(150, 22);
            this.toolStripButton1.Text = "Добавить дату занятия";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // dataGridViewJournal
            // 
            this.dataGridViewJournal.AllowUserToAddRows = false;
            this.dataGridViewJournal.AllowUserToDeleteRows = false;
            this.dataGridViewJournal.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewJournal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewJournal.Location = new System.Drawing.Point(0, 72);
            this.dataGridViewJournal.Name = "dataGridViewJournal";
            this.dataGridViewJournal.ReadOnly = true;
            this.dataGridViewJournal.RowHeadersVisible = false;
            this.dataGridViewJournal.Size = new System.Drawing.Size(1184, 489);
            this.dataGridViewJournal.TabIndex = 2;
            // 
            // SelectPanel
            // 
            this.SelectPanel.Controls.Add(this.cmbSubjects);
            this.SelectPanel.Controls.Add(this.lblSubject);
            this.SelectPanel.Controls.Add(this.cmbGroups);
            this.SelectPanel.Controls.Add(this.lblGroup);
            this.SelectPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.SelectPanel.Location = new System.Drawing.Point(0, 25);
            this.SelectPanel.Name = "SelectPanel";
            this.SelectPanel.Size = new System.Drawing.Size(1184, 47);
            this.SelectPanel.TabIndex = 3;
            // 
            // cmbGroups
            // 
            this.cmbGroups.FormattingEnabled = true;
            this.cmbGroups.Location = new System.Drawing.Point(82, 13);
            this.cmbGroups.Name = "cmbGroups";
            this.cmbGroups.Size = new System.Drawing.Size(142, 21);
            this.cmbGroups.TabIndex = 1;
            // 
            // lblGroup
            // 
            this.lblGroup.Location = new System.Drawing.Point(28, 16);
            this.lblGroup.Name = "lblGroup";
            this.lblGroup.Size = new System.Drawing.Size(48, 13);
            this.lblGroup.TabIndex = 0;
            this.lblGroup.Text = "Группа:";
            // 
            // lblSubject
            // 
            this.lblSubject.AutoSize = true;
            this.lblSubject.Location = new System.Drawing.Point(262, 16);
            this.lblSubject.Name = "lblSubject";
            this.lblSubject.Size = new System.Drawing.Size(73, 13);
            this.lblSubject.TabIndex = 2;
            this.lblSubject.Text = "Дисциплина:";
            // 
            // cmbSubjects
            // 
            this.cmbSubjects.FormattingEnabled = true;
            this.cmbSubjects.Location = new System.Drawing.Point(341, 13);
            this.cmbSubjects.Name = "cmbSubjects";
            this.cmbSubjects.Size = new System.Drawing.Size(205, 21);
            this.cmbSubjects.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1184, 561);
            this.Controls.Add(this.dataGridViewJournal);
            this.Controls.Add(this.SelectPanel);
            this.Controls.Add(this.toolStrip1);
            this.Location = new System.Drawing.Point(10, 15);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Журнал оценок";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewJournal)).EndInit();
            this.SelectPanel.ResumeLayout(false);
            this.SelectPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButtonLogout;
        private System.Windows.Forms.DataGridView dataGridViewJournal;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.Panel SelectPanel;
        private System.Windows.Forms.Label lblGroup;
        private System.Windows.Forms.ComboBox cmbGroups;
        private System.Windows.Forms.ComboBox cmbSubjects;
        private System.Windows.Forms.Label lblSubject;
    }
}

