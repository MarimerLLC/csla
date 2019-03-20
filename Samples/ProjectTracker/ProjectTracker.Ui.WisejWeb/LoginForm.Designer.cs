namespace PTWisej
{
  partial class LoginForm
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

    #region Wisej Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
			this.Cancel = new Wisej.Web.Button();
			this.OK = new Wisej.Web.Button();
			this.PasswordTextBox = new Wisej.Web.TextBox();
			this.UsernameTextBox = new Wisej.Web.TextBox();
			this.PasswordLabel = new Wisej.Web.Label();
			this.UsernameLabel = new Wisej.Web.Label();
			this.LogoPictureBox = new Wisej.Web.PictureBox();
			this.label1 = new Wisej.Web.Label();
			this.label2 = new Wisej.Web.Label();
			((System.ComponentModel.ISupportInitialize)(this.LogoPictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// Cancel
			// 
			this.Cancel.DialogResult = Wisej.Web.DialogResult.Cancel;
			this.Cancel.Location = new System.Drawing.Point(300, 161);
			this.Cancel.Name = "Cancel";
			this.Cancel.Size = new System.Drawing.Size(94, 23);
			this.Cancel.TabIndex = 12;
			this.Cancel.Text = "&Cancel";
			this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
			// 
			// OK
			// 
			this.OK.Location = new System.Drawing.Point(197, 161);
			this.OK.Name = "OK";
			this.OK.Size = new System.Drawing.Size(94, 23);
			this.OK.TabIndex = 11;
			this.OK.Text = "&OK";
			this.OK.Click += new System.EventHandler(this.OK_Click);
			// 
			// PasswordTextBox
			// 
			this.PasswordTextBox.Location = new System.Drawing.Point(174, 101);
			this.PasswordTextBox.Name = "PasswordTextBox";
			this.PasswordTextBox.PasswordChar = '*';
			this.PasswordTextBox.Size = new System.Drawing.Size(220, 20);
			this.PasswordTextBox.TabIndex = 10;
			// 
			// UsernameTextBox
			// 
			this.UsernameTextBox.Location = new System.Drawing.Point(174, 44);
			this.UsernameTextBox.Name = "UsernameTextBox";
			this.UsernameTextBox.Size = new System.Drawing.Size(220, 20);
			this.UsernameTextBox.TabIndex = 8;
			// 
			// PasswordLabel
			// 
			this.PasswordLabel.Location = new System.Drawing.Point(172, 81);
			this.PasswordLabel.Name = "PasswordLabel";
			this.PasswordLabel.Size = new System.Drawing.Size(220, 23);
			this.PasswordLabel.TabIndex = 9;
			this.PasswordLabel.Text = "&Password";
			this.PasswordLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// UsernameLabel
			// 
			this.UsernameLabel.Location = new System.Drawing.Point(172, 24);
			this.UsernameLabel.Name = "UsernameLabel";
			this.UsernameLabel.Size = new System.Drawing.Size(220, 23);
			this.UsernameLabel.TabIndex = 6;
			this.UsernameLabel.Text = "&User name";
			this.UsernameLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// LogoPictureBox
			// 
			this.LogoPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("LogoPictureBox.Image")));
			this.LogoPictureBox.Location = new System.Drawing.Point(0, 0);
			this.LogoPictureBox.Name = "LogoPictureBox";
			this.LogoPictureBox.Size = new System.Drawing.Size(165, 193);
			this.LogoPictureBox.TabIndex = 7;
			this.LogoPictureBox.TabStop = false;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.BackColor = System.Drawing.SystemColors.Info;
			this.label1.Location = new System.Drawing.Point(297, 67);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(91, 13);
			this.label1.TabIndex = 13;
			this.label1.Text = "manager or admin";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.BackColor = System.Drawing.SystemColors.Info;
			this.label2.Location = new System.Drawing.Point(297, 124);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(91, 13);
			this.label2.TabIndex = 14;
			this.label2.Text = "manager or admin";
			// 
			// LoginForm
			// 
			this.AcceptButton = this.OK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = Wisej.Web.AutoScaleMode.Font;
			this.CancelButton = this.Cancel;
			this.ClientSize = new System.Drawing.Size(401, 192);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.Cancel);
			this.Controls.Add(this.OK);
			this.Controls.Add(this.PasswordTextBox);
			this.Controls.Add(this.UsernameTextBox);
			this.Controls.Add(this.PasswordLabel);
			this.Controls.Add(this.UsernameLabel);
			this.Controls.Add(this.LogoPictureBox);
			//this.FormBorderStyle = Wisej.Web.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LoginForm";
			this.StartPosition = Wisej.Web.FormStartPosition.CenterParent;
			this.Text = "Log in";
			this.Load += new System.EventHandler(this.LoginForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.LogoPictureBox)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

    }

    #endregion

    internal Wisej.Web.Button Cancel;
    internal Wisej.Web.Button OK;
    internal Wisej.Web.TextBox PasswordTextBox;
    internal Wisej.Web.TextBox UsernameTextBox;
    internal Wisej.Web.Label PasswordLabel;
    internal Wisej.Web.Label UsernameLabel;
    internal Wisej.Web.PictureBox LogoPictureBox;
		private Wisej.Web.Label label1;
		private Wisej.Web.Label label2;
	}
}