namespace WinFormsExample
{
  partial class MainForm
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
      this.loginButton = new System.Windows.Forms.Button();
      this.contentPanel = new System.Windows.Forms.Panel();
      this.homeButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // loginButton
      // 
      this.loginButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.loginButton.Location = new System.Drawing.Point(699, 12);
      this.loginButton.Name = "loginButton";
      this.loginButton.Size = new System.Drawing.Size(196, 34);
      this.loginButton.TabIndex = 0;
      this.loginButton.Text = "Login";
      this.loginButton.UseVisualStyleBackColor = true;
      this.loginButton.Click += new System.EventHandler(this.loginButton_Click);
      // 
      // contentPanel
      // 
      this.contentPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.contentPanel.Location = new System.Drawing.Point(1, 56);
      this.contentPanel.Name = "contentPanel";
      this.contentPanel.Size = new System.Drawing.Size(904, 512);
      this.contentPanel.TabIndex = 1;
      // 
      // homeButton
      // 
      this.homeButton.Location = new System.Drawing.Point(12, 12);
      this.homeButton.Name = "homeButton";
      this.homeButton.Size = new System.Drawing.Size(112, 34);
      this.homeButton.TabIndex = 0;
      this.homeButton.Text = "Home";
      this.homeButton.UseVisualStyleBackColor = true;
      this.homeButton.Click += new System.EventHandler(this.homeButton_Click);
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(909, 571);
      this.Controls.Add(this.homeButton);
      this.Controls.Add(this.contentPanel);
      this.Controls.Add(this.loginButton);
      this.Name = "MainForm";
      this.Text = "WinFormsExample";
      this.ResumeLayout(false);

    }

    #endregion

    private Button loginButton;
    private Panel contentPanel;
    private Button homeButton;
  }
}