using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace PTClient
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Form1()
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
				if (components != null) 
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
      this.btnGetProject = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // btnGetProject
      // 
      this.btnGetProject.Location = new System.Drawing.Point(120, 48);
      this.btnGetProject.Name = "btnGetProject";
      this.btnGetProject.TabIndex = 0;
      this.btnGetProject.Text = "Get Project";
      this.btnGetProject.Click += new System.EventHandler(this.btnGetProject_Click);
      // 
      // Form1
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(456, 310);
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.btnGetProject});
      this.Name = "Form1";
      this.Text = "Form1";
      this.Load += new System.EventHandler(this.Form1_Load);
      this.ResumeLayout(false);

    }
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());
		}

    private void Form1_Load(object sender, System.EventArgs e)
    {
    
    }

    private System.Windows.Forms.Button btnGetProject;

    #region Security

    private PTService.ProjectTracker _service;

    private PTService.ProjectTracker WebService
    {
      get
      {
        if( _service == null)
        {
          // Create the web service proxy
          _service = new PTService.ProjectTracker();

//          // Create the security credentials
//          PTService.CSLACredentials cred = new PTService.CSLACredentials();
//          cred.Username = "rocky";
//          cred.Password = "lhotka";
//
//          // Provide the credentials to the service proxy
//          _service.CSLACredentialsValue = cred;
        }
        // Return the service proxy for use
        return _service;
      }
    }

    #endregion

    private void btnGetProject_Click(object sender, System.EventArgs e)
    {
      PTService.ProjectInfo [] proj = WebService.GetProjectList();
      MessageBox.Show(proj.Length.ToString(), "Number of projects returned");
    }
	}
}
