namespace MainForm
{
    partial class AddLessonDateDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelDate = new System.Windows.Forms.Label();
            this.dateTimePickerDate = new System.Windows.Forms.DateTimePicker();
            this.checkBoxLessonNumber = new System.Windows.Forms.CheckBox();
            this.numericUpDownLessonNumber = new System.Windows.Forms.NumericUpDown();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLessonNumber)).BeginInit();
            this.SuspendLayout();
            // 
            // labelDate
            // 
            this.labelDate.Location = new System.Drawing.Point(20, 20);
            this.labelDate.Name = "labelDate";
            this.labelDate.Size = new System.Drawing.Size(100, 20);
            this.labelDate.TabIndex = 0;
            this.labelDate.Text = "Дата занятия:";
            // 
            // dateTimePickerDate
            // 
            this.dateTimePickerDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateTimePickerDate.Location = new System.Drawing.Point(122, 20);
            this.dateTimePickerDate.Name = "dateTimePickerDate";
            this.dateTimePickerDate.Size = new System.Drawing.Size(130, 20);
            this.dateTimePickerDate.TabIndex = 1;
            // 
            // checkBoxLessonNumber
            // 
            this.checkBoxLessonNumber.Location = new System.Drawing.Point(20, 60);
            this.checkBoxLessonNumber.Name = "checkBoxLessonNumber";
            this.checkBoxLessonNumber.Size = new System.Drawing.Size(150, 20);
            this.checkBoxLessonNumber.TabIndex = 2;
            this.checkBoxLessonNumber.Text = "Указать номер пары:";
            this.checkBoxLessonNumber.UseVisualStyleBackColor = true;
            this.checkBoxLessonNumber.CheckedChanged += new System.EventHandler(this.checkBoxLessonNumber_CheckedChanged);
            // 
            // numericUpDownLessonNumber
            // 
            this.numericUpDownLessonNumber.Enabled = false;
            this.numericUpDownLessonNumber.Location = new System.Drawing.Point(180, 60);
            this.numericUpDownLessonNumber.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDownLessonNumber.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownLessonNumber.Name = "numericUpDownLessonNumber";
            this.numericUpDownLessonNumber.Size = new System.Drawing.Size(50, 20);
            this.numericUpDownLessonNumber.TabIndex = 3;
            this.numericUpDownLessonNumber.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(150, 100);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 25);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "Ок";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(235, 100);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 25);
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Отмена";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // AddLessonDateDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 161);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.numericUpDownLessonNumber);
            this.Controls.Add(this.checkBoxLessonNumber);
            this.Controls.Add(this.dateTimePickerDate);
            this.Controls.Add(this.labelDate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Location = new System.Drawing.Point(130, 20);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddLessonDateDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Добавить дату занятия";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLessonNumber)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelDate;
        private System.Windows.Forms.DateTimePicker dateTimePickerDate;
        private System.Windows.Forms.CheckBox checkBoxLessonNumber;
        private System.Windows.Forms.NumericUpDown numericUpDownLessonNumber;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
    }
}