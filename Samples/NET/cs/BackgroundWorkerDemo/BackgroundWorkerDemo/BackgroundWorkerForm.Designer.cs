namespace BackgroundWorkerDemo
{
    partial class BackgroundWorkerForm
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
          System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BackgroundWorkerForm));
          this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
          this.CslaButton = new System.Windows.Forms.Button();
          this.BWButton = new System.Windows.Forms.Button();
          this.listBox1 = new System.Windows.Forms.ListBox();
          this.button2 = new System.Windows.Forms.Button();
          this.cslaBackgroundWorker1 = new Csla.Threading.BackgroundWorker();
          this.backgroundWorker2 = new Csla.Threading.BackgroundWorker();
          this.button1 = new System.Windows.Forms.Button();
          this.textBox1 = new System.Windows.Forms.TextBox();
          this.SuspendLayout();
          // 
          // backgroundWorker1
          // 
          this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.Test_DoWork);
          this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.Test_RunWorkerCompleted);
          // 
          // CslaButton
          // 
          this.CslaButton.Location = new System.Drawing.Point(500, 41);
          this.CslaButton.Name = "CslaButton";
          this.CslaButton.Size = new System.Drawing.Size(305, 23);
          this.CslaButton.TabIndex = 0;
          this.CslaButton.Text = "Csla.Threading.BackgroundWorker";
          this.CslaButton.UseVisualStyleBackColor = true;
          this.CslaButton.Click += new System.EventHandler(this.CslaButton_Click);
          // 
          // BWButton
          // 
          this.BWButton.Location = new System.Drawing.Point(501, 12);
          this.BWButton.Name = "BWButton";
          this.BWButton.Size = new System.Drawing.Size(305, 23);
          this.BWButton.TabIndex = 1;
          this.BWButton.Text = "System.ComponentModel.BackgroundWorker";
          this.BWButton.UseVisualStyleBackColor = true;
          this.BWButton.Click += new System.EventHandler(this.BWButton_Click);
          // 
          // listBox1
          // 
          this.listBox1.FormattingEnabled = true;
          this.listBox1.Location = new System.Drawing.Point(13, 13);
          this.listBox1.Name = "listBox1";
          this.listBox1.Size = new System.Drawing.Size(481, 420);
          this.listBox1.TabIndex = 2;
          // 
          // button2
          // 
          this.button2.Location = new System.Drawing.Point(500, 70);
          this.button2.Name = "button2";
          this.button2.Size = new System.Drawing.Size(305, 23);
          this.button2.TabIndex = 4;
          this.button2.Text = "Csla BackgroundWorker reporting progress";
          this.button2.UseVisualStyleBackColor = true;
          this.button2.Click += new System.EventHandler(this.button2_Click);
          // 
          // cslaBackgroundWorker1
          // 
          this.cslaBackgroundWorker1.WorkerReportsProgress = false;
          this.cslaBackgroundWorker1.WorkerSupportsCancellation = false;
          this.cslaBackgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.Test_DoWork);
          this.cslaBackgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.Test_RunWorkerCompleted);
          // 
          // backgroundWorker2
          // 
          this.backgroundWorker2.WorkerReportsProgress = true;
          this.backgroundWorker2.WorkerSupportsCancellation = false;
          this.backgroundWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker2_DoWork);
          this.backgroundWorker2.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker2_RunWorkerCompleted);
          this.backgroundWorker2.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker2_ProgressChanged);
          // 
          // button1
          // 
          this.button1.Location = new System.Drawing.Point(500, 124);
          this.button1.Name = "button1";
          this.button1.Size = new System.Drawing.Size(305, 23);
          this.button1.TabIndex = 5;
          this.button1.Text = "Clear list";
          this.button1.UseVisualStyleBackColor = true;
          this.button1.Click += new System.EventHandler(this.button1_Click_1);
          // 
          // textBox1
          // 
          this.textBox1.Location = new System.Drawing.Point(501, 168);
          this.textBox1.Multiline = true;
          this.textBox1.Name = "textBox1";
          this.textBox1.ReadOnly = true;
          this.textBox1.Size = new System.Drawing.Size(304, 265);
          this.textBox1.TabIndex = 6;
          this.textBox1.Text = resources.GetString("textBox1.Text");
          // 
          // BackgroundWorkerForm
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.ClientSize = new System.Drawing.Size(817, 442);
          this.Controls.Add(this.textBox1);
          this.Controls.Add(this.button1);
          this.Controls.Add(this.button2);
          this.Controls.Add(this.listBox1);
          this.Controls.Add(this.BWButton);
          this.Controls.Add(this.CslaButton);
          this.Name = "BackgroundWorkerForm";
          this.Text = "BackgroundWorker";
          this.ResumeLayout(false);
          this.PerformLayout();

        }

        #endregion

        private Csla.Threading.BackgroundWorker cslaBackgroundWorker1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button CslaButton;
        private System.Windows.Forms.Button BWButton;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button2;
        private Csla.Threading.BackgroundWorker backgroundWorker2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
    }
}