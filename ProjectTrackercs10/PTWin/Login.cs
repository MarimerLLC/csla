using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace PTWin
{
	public class Login : System.Windows.Forms.Form
	{
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txtUsername;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox txtPassword;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Button btnLogin;
    private System.Windows.Forms.Button btnCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Login()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
      System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Login));
      this.label1 = new System.Windows.Forms.Label();
      this.txtUsername = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.txtPassword = new System.Windows.Forms.TextBox();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.btnLogin = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(280, 40);
      this.label1.Name = "label1";
      this.label1.TabIndex = 0;
      this.label1.Text = "Username";
      // 
      // txtUsername
      // 
      this.txtUsername.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right);
      this.txtUsername.Location = new System.Drawing.Point(352, 40);
      this.txtUsername.Name = "txtUsername";
      this.txtUsername.Size = new System.Drawing.Size(184, 20);
      this.txtUsername.TabIndex = 1;
      this.txtUsername.Text = "";
      this.txtUsername.TextChanged += new System.EventHandler(this.txtUsername_TextChanged);
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(280, 72);
      this.label2.Name = "label2";
      this.label2.TabIndex = 2;
      this.label2.Text = "Password";
      // 
      // txtPassword
      // 
      this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right);
      this.txtPassword.Location = new System.Drawing.Point(352, 72);
      this.txtPassword.Name = "txtPassword";
      this.txtPassword.PasswordChar = '*';
      this.txtPassword.Size = new System.Drawing.Size(184, 20);
      this.txtPassword.TabIndex = 3;
      this.txtPassword.Text = "";
      // 
      // pictureBox1
      // 
      this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Left;
      this.pictureBox1.Image = ((System.Drawing.Bitmap)(resources.GetObject("pictureBox1.Image")));
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(266, 176);
      this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
      this.pictureBox1.TabIndex = 4;
      this.pictureBox1.TabStop = false;
      // 
      // btnLogin
      // 
      this.btnLogin.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
      this.btnLogin.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btnLogin.Enabled = false;
      this.btnLogin.Location = new System.Drawing.Point(376, 112);
      this.btnLogin.Name = "btnLogin";
      this.btnLogin.TabIndex = 5;
      this.btnLogin.Text = "&Login";
      this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
      // 
      // btnCancel
      // 
      this.btnCancel.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Location = new System.Drawing.Point(464, 112);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.TabIndex = 6;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // Login
      // 
      this.AcceptButton = this.btnLogin;
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(544, 176);
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.btnCancel,
                                                                  this.btnLogin,
                                                                  this.pictureBox1,
                                                                  this.txtPassword,
                                                                  this.label2,
                                                                  this.txtUsername,
                                                                  this.label1});
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
      this.Name = "Login";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Login";
      this.ResumeLayout(false);

    }
		#endregion

    string _username = string.Empty;
    string _password = string.Empty;

    private void btnLogin_Click(object sender, System.EventArgs e)
    {
      _username = txtUsername.Text;
      _password = txtPassword.Text;
      Close();
    }

    private void btnCancel_Click(object sender, System.EventArgs e)
    {
      _username = string.Empty;
      _password = string.Empty;
      Close();
    }

    public string Username
    {
      get
      {
        return _username;
      }
    }

    public string Password
    {
      get
      {
        return _password;
      }
    }

    private void txtUsername_TextChanged(object sender, System.EventArgs e)
    {
      btnLogin.Enabled = (txtUsername.Text.Trim().Length > 0);
    }
	}
}
