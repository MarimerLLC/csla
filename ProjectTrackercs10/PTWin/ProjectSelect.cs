using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using ProjectTracker.Library;

namespace PTWin
{
	/// <summary>
	/// Summary description for ProjectSelect.
	/// </summary>
	public class ProjectSelect : System.Windows.Forms.Form
	{
    private MSDN.DataListView.DataListView dvDisplay;
    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.Button btnCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

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
      this.dvDisplay = new MSDN.DataListView.DataListView();
      this.btnOK = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // dvDisplay
      // 
      this.dvDisplay.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right);
      this.dvDisplay.AutoDiscover = false;
      this.dvDisplay.DataSource = null;
      this.dvDisplay.DisplayMember = "";
      this.dvDisplay.FullRowSelect = true;
      this.dvDisplay.MultiSelect = false;
      this.dvDisplay.Name = "dvDisplay";
      this.dvDisplay.Size = new System.Drawing.Size(296, 272);
      this.dvDisplay.TabIndex = 0;
      this.dvDisplay.View = System.Windows.Forms.View.Details;
      this.dvDisplay.DoubleClick += new System.EventHandler(this.btnOK_Click);
      // 
      // btnOK
      // 
      this.btnOK.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
      this.btnOK.Location = new System.Drawing.Point(304, 8);
      this.btnOK.Name = "btnOK";
      this.btnOK.TabIndex = 1;
      this.btnOK.Text = "OK";
      this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
      // 
      // btnCancel
      // 
      this.btnCancel.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Location = new System.Drawing.Point(304, 40);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.TabIndex = 2;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // ProjectSelect
      // 
      this.AcceptButton = this.btnOK;
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(392, 270);
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.btnCancel,
                                                                  this.btnOK,
                                                                  this.dvDisplay});
      this.Name = "ProjectSelect";
      this.Text = "ProjectSelect";
      this.ResumeLayout(false);

    }
		#endregion

    string result = string.Empty;

    public ProjectSelect(string title)
    {
      InitializeComponent();
      Text = title;
      ProjectTracker.Library.ProjectList list = 
        ProjectTracker.Library.ProjectList.GetProjectList();
      dvDisplay.AutoDiscover = false;
      dvDisplay.Columns.Add("ID", "ID", 0);
      dvDisplay.Columns.Add("Project name", "Name", dvDisplay.Width);
      dvDisplay.DataSource = list;
      dvDisplay.Focus();
    }

    private void btnOK_Click(object sender, System.EventArgs e)
    {
      if(dvDisplay.SelectedItems.Count > 0)
        result = dvDisplay.SelectedItems[0].Text;
      else
        result = string.Empty;
      Close();
    }

    private void btnCancel_Click(object sender, System.EventArgs e)
    {
      result = string.Empty;
      Close();
    }

    public string Result
    {
      get
      {
        return result;
      }
    }
  }
}
