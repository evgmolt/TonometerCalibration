namespace TCalibr
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timerRead = new System.Windows.Forms.Timer(this.components);
            this.labPort = new System.Windows.Forms.Label();
            this.labADCValue = new System.Windows.Forms.Label();
            this.panMessages = new System.Windows.Forms.Panel();
            this.butRepeat = new System.Windows.Forms.Button();
            this.labCoeff = new System.Windows.Forms.Label();
            this.labTargetPressure = new System.Windows.Forms.Label();
            this.labPressButton = new System.Windows.Forms.Label();
            this.butWrite = new System.Windows.Forms.Button();
            this.butContinue = new System.Windows.Forms.Button();
            this.labMessage = new System.Windows.Forms.Label();
            this.panConnect = new System.Windows.Forms.Panel();
            this.panValue = new System.Windows.Forms.Panel();
            this.butSetZero = new System.Windows.Forms.Button();
            this.tbWarning = new System.Windows.Forms.TextBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.timerStatus = new System.Windows.Forms.Timer(this.components);
            this.panMessages.SuspendLayout();
            this.panConnect.SuspendLayout();
            this.panValue.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerRead
            // 
            this.timerRead.Enabled = true;
            this.timerRead.Tick += new System.EventHandler(this.timerRead_Tick);
            // 
            // labPort
            // 
            this.labPort.AutoSize = true;
            this.labPort.Dock = System.Windows.Forms.DockStyle.Top;
            this.labPort.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labPort.Location = new System.Drawing.Point(0, 0);
            this.labPort.Name = "labPort";
            this.labPort.Size = new System.Drawing.Size(45, 19);
            this.labPort.TabIndex = 0;
            this.labPort.Text = "label1";
            // 
            // labADCValue
            // 
            this.labADCValue.AutoSize = true;
            this.labADCValue.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.labADCValue.Location = new System.Drawing.Point(30, 118);
            this.labADCValue.Name = "labADCValue";
            this.labADCValue.Size = new System.Drawing.Size(19, 21);
            this.labADCValue.TabIndex = 1;
            this.labADCValue.Text = "0";
            // 
            // panMessages
            // 
            this.panMessages.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panMessages.Controls.Add(this.butRepeat);
            this.panMessages.Controls.Add(this.labCoeff);
            this.panMessages.Controls.Add(this.labTargetPressure);
            this.panMessages.Controls.Add(this.labPressButton);
            this.panMessages.Controls.Add(this.butWrite);
            this.panMessages.Controls.Add(this.butContinue);
            this.panMessages.Controls.Add(this.labMessage);
            this.panMessages.Dock = System.Windows.Forms.DockStyle.Top;
            this.panMessages.Location = new System.Drawing.Point(0, 0);
            this.panMessages.Name = "panMessages";
            this.panMessages.Size = new System.Drawing.Size(429, 183);
            this.panMessages.TabIndex = 2;
            // 
            // butRepeat
            // 
            this.butRepeat.Location = new System.Drawing.Point(281, 139);
            this.butRepeat.Name = "butRepeat";
            this.butRepeat.Size = new System.Drawing.Size(100, 23);
            this.butRepeat.TabIndex = 2;
            this.butRepeat.Text = "Повторить";
            this.butRepeat.UseVisualStyleBackColor = true;
            this.butRepeat.Click += new System.EventHandler(this.butRepeat_Click);
            // 
            // labCoeff
            // 
            this.labCoeff.AutoSize = true;
            this.labCoeff.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.labCoeff.Location = new System.Drawing.Point(30, 101);
            this.labCoeff.Name = "labCoeff";
            this.labCoeff.Size = new System.Drawing.Size(75, 21);
            this.labCoeff.TabIndex = 5;
            this.labCoeff.Text = "labCoeff";
            // 
            // labTargetPressure
            // 
            this.labTargetPressure.AutoSize = true;
            this.labTargetPressure.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.labTargetPressure.ForeColor = System.Drawing.Color.Red;
            this.labTargetPressure.Location = new System.Drawing.Point(293, 32);
            this.labTargetPressure.Name = "labTargetPressure";
            this.labTargetPressure.Size = new System.Drawing.Size(50, 21);
            this.labTargetPressure.TabIndex = 4;
            this.labTargetPressure.Text = "0,015";
            // 
            // labPressButton
            // 
            this.labPressButton.AutoSize = true;
            this.labPressButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.labPressButton.Location = new System.Drawing.Point(30, 62);
            this.labPressButton.Name = "labPressButton";
            this.labPressButton.Size = new System.Drawing.Size(125, 21);
            this.labPressButton.TabIndex = 3;
            this.labPressButton.Text = "labPressButton";
            // 
            // butWrite
            // 
            this.butWrite.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.butWrite.Location = new System.Drawing.Point(154, 139);
            this.butWrite.Name = "butWrite";
            this.butWrite.Size = new System.Drawing.Size(100, 23);
            this.butWrite.TabIndex = 1;
            this.butWrite.Text = "Записать";
            this.butWrite.UseVisualStyleBackColor = true;
            this.butWrite.Click += new System.EventHandler(this.butWrite_Click);
            // 
            // butContinue
            // 
            this.butContinue.Location = new System.Drawing.Point(30, 139);
            this.butContinue.Name = "butContinue";
            this.butContinue.Size = new System.Drawing.Size(100, 23);
            this.butContinue.TabIndex = 0;
            this.butContinue.Text = "Продолжить";
            this.butContinue.UseVisualStyleBackColor = true;
            this.butContinue.Click += new System.EventHandler(this.butContinue_Click);
            // 
            // labMessage
            // 
            this.labMessage.AutoSize = true;
            this.labMessage.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.labMessage.Location = new System.Drawing.Point(30, 32);
            this.labMessage.Name = "labMessage";
            this.labMessage.Size = new System.Drawing.Size(257, 21);
            this.labMessage.TabIndex = 0;
            this.labMessage.Text = "Установите значение давления";
            // 
            // panConnect
            // 
            this.panConnect.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panConnect.Controls.Add(this.labPort);
            this.panConnect.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panConnect.Location = new System.Drawing.Point(0, 426);
            this.panConnect.Name = "panConnect";
            this.panConnect.Size = new System.Drawing.Size(429, 24);
            this.panConnect.TabIndex = 3;
            // 
            // panValue
            // 
            this.panValue.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panValue.Controls.Add(this.butSetZero);
            this.panValue.Controls.Add(this.tbWarning);
            this.panValue.Controls.Add(this.listView1);
            this.panValue.Controls.Add(this.labADCValue);
            this.panValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panValue.Location = new System.Drawing.Point(0, 183);
            this.panValue.Name = "panValue";
            this.panValue.Size = new System.Drawing.Size(429, 243);
            this.panValue.TabIndex = 4;
            // 
            // butSetZero
            // 
            this.butSetZero.Location = new System.Drawing.Point(30, 183);
            this.butSetZero.Name = "butSetZero";
            this.butSetZero.Size = new System.Drawing.Size(100, 23);
            this.butSetZero.TabIndex = 4;
            this.butSetZero.Text = "Коррекция \"0\"";
            this.butSetZero.UseVisualStyleBackColor = true;
            this.butSetZero.Click += new System.EventHandler(this.button1_Click);
            // 
            // tbWarning
            // 
            this.tbWarning.BackColor = System.Drawing.SystemColors.Control;
            this.tbWarning.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbWarning.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.tbWarning.ForeColor = System.Drawing.Color.Red;
            this.tbWarning.Location = new System.Drawing.Point(24, 24);
            this.tbWarning.Multiline = true;
            this.tbWarning.Name = "tbWarning";
            this.tbWarning.Size = new System.Drawing.Size(192, 74);
            this.tbWarning.TabIndex = 3;
            this.tbWarning.TabStop = false;
            this.tbWarning.Text = "Убедитесь, что клапан открыт и здесь отображается 0  ±1";
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listView1.Location = new System.Drawing.Point(222, 24);
            this.listView1.Name = "listView1";
            this.listView1.Scrollable = false;
            this.listView1.Size = new System.Drawing.Size(180, 97);
            this.listView1.TabIndex = 2;
            this.listView1.TabStop = false;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Давление";
            this.columnHeader1.Width = 90;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Значение";
            this.columnHeader2.Width = 90;
            // 
            // timerStatus
            // 
            this.timerStatus.Enabled = true;
            this.timerStatus.Interval = 300;
            this.timerStatus.Tick += new System.EventHandler(this.timerStatus_Tick);
            // 
            // Form1
            // 
            this.AcceptButton = this.butWrite;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(429, 450);
            this.Controls.Add(this.panValue);
            this.Controls.Add(this.panConnect);
            this.Controls.Add(this.panMessages);
            this.Name = "Form1";
            this.Text = "Калибровка тонометра";
            this.panMessages.ResumeLayout(false);
            this.panMessages.PerformLayout();
            this.panConnect.ResumeLayout(false);
            this.panConnect.PerformLayout();
            this.panValue.ResumeLayout(false);
            this.panValue.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timerRead;
        private Label labPort;
        private Label labADCValue;
        private Panel panMessages;
        private Panel panConnect;
        private Panel panValue;
        private Label labMessage;
        private System.Windows.Forms.Timer timerStatus;
        private Button butWrite;
        private Button butContinue;
        private Label labTargetPressure;
        private Label labPressButton;
        private ListView listView1;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private Label labCoeff;
        private TextBox tbWarning;
        private Button butSetZero;
        private Button butRepeat;
    }
}