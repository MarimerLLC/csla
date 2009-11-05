namespace MethodCallerTest
{
  partial class Form1
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
      this.resultTextBox = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // resultTextBox
      // 
      this.resultTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.resultTextBox.Location = new System.Drawing.Point(0, 0);
      this.resultTextBox.Multiline = true;
      this.resultTextBox.Name = "resultTextBox";
      this.resultTextBox.ReadOnly = true;
      this.resultTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.resultTextBox.Size = new System.Drawing.Size(553, 651);
      this.resultTextBox.TabIndex = 0;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(553, 651);
      this.Controls.Add(this.resultTextBox);
      this.Name = "Form1";
      this.Text = "MethodCaller Test";
      this.Load += new System.EventHandler(this.Form1_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox resultTextBox;
  }
}

