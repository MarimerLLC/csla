using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Threading;
using ProjectTracker.Library;

namespace PTWin
{
	/// <summary>
	/// Summary description for ProjectEdit.
	/// </summary>
	public class ProjectEdit : System.Windows.Forms.Form
	{
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txtID;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox txtName;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox txtStarted;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox txtEnded;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.TextBox txtDescription;
    private System.Windows.Forms.GroupBox groupBox1;
    private MSDN.DataListView.DataListView dvResources;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.ContextMenu mnuRoles;
    private System.Windows.Forms.Button btnAddResource;
    private System.Windows.Forms.Button btnRemoveResource;
    private System.Windows.Forms.CheckBox chkIsDirty;
    private System.Windows.Forms.ListBox lstRules;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ProjectEdit()
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
      this.label1 = new System.Windows.Forms.Label();
      this.txtID = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.txtName = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.txtStarted = new System.Windows.Forms.TextBox();
      this.label4 = new System.Windows.Forms.Label();
      this.txtEnded = new System.Windows.Forms.TextBox();
      this.label5 = new System.Windows.Forms.Label();
      this.txtDescription = new System.Windows.Forms.TextBox();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.btnRemoveResource = new System.Windows.Forms.Button();
      this.btnAddResource = new System.Windows.Forms.Button();
      this.dvResources = new MSDN.DataListView.DataListView();
      this.mnuRoles = new System.Windows.Forms.ContextMenu();
      this.btnSave = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.chkIsDirty = new System.Windows.Forms.CheckBox();
      this.lstRules = new System.Windows.Forms.ListBox();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(16, 24);
      this.label1.Name = "label1";
      this.label1.TabIndex = 0;
      this.label1.Text = "ID";
      // 
      // txtID
      // 
      this.txtID.Location = new System.Drawing.Point(112, 24);
      this.txtID.Name = "txtID";
      this.txtID.ReadOnly = true;
      this.txtID.Size = new System.Drawing.Size(232, 20);
      this.txtID.TabIndex = 1;
      this.txtID.Text = "";
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(16, 48);
      this.label2.Name = "label2";
      this.label2.TabIndex = 2;
      this.label2.Text = "Name";
      // 
      // txtName
      // 
      this.txtName.Location = new System.Drawing.Point(112, 48);
      this.txtName.Name = "txtName";
      this.txtName.Size = new System.Drawing.Size(496, 20);
      this.txtName.TabIndex = 3;
      this.txtName.Text = "";
      // 
      // label3
      // 
      this.label3.Location = new System.Drawing.Point(16, 72);
      this.label3.Name = "label3";
      this.label3.TabIndex = 4;
      this.label3.Text = "Started";
      // 
      // txtStarted
      // 
      this.txtStarted.Location = new System.Drawing.Point(112, 72);
      this.txtStarted.Name = "txtStarted";
      this.txtStarted.TabIndex = 5;
      this.txtStarted.Text = "";
      // 
      // label4
      // 
      this.label4.Location = new System.Drawing.Point(16, 96);
      this.label4.Name = "label4";
      this.label4.TabIndex = 6;
      this.label4.Text = "Ended";
      // 
      // txtEnded
      // 
      this.txtEnded.Location = new System.Drawing.Point(112, 96);
      this.txtEnded.Name = "txtEnded";
      this.txtEnded.TabIndex = 7;
      this.txtEnded.Text = "";
      // 
      // label5
      // 
      this.label5.Location = new System.Drawing.Point(16, 120);
      this.label5.Name = "label5";
      this.label5.TabIndex = 8;
      this.label5.Text = "Description";
      // 
      // txtDescription
      // 
      this.txtDescription.Location = new System.Drawing.Point(112, 120);
      this.txtDescription.Multiline = true;
      this.txtDescription.Name = "txtDescription";
      this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.txtDescription.Size = new System.Drawing.Size(384, 120);
      this.txtDescription.TabIndex = 9;
      this.txtDescription.Text = "";
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                            this.btnRemoveResource,
                                                                            this.btnAddResource,
                                                                            this.dvResources});
      this.groupBox1.Location = new System.Drawing.Point(8, 248);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(712, 208);
      this.groupBox1.TabIndex = 10;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Resources";
      // 
      // btnRemoveResource
      // 
      this.btnRemoveResource.Location = new System.Drawing.Point(624, 56);
      this.btnRemoveResource.Name = "btnRemoveResource";
      this.btnRemoveResource.TabIndex = 2;
      this.btnRemoveResource.Text = "&Remove";
      this.btnRemoveResource.Click += new System.EventHandler(this.btnRemoveResource_Click);
      // 
      // btnAddResource
      // 
      this.btnAddResource.Location = new System.Drawing.Point(624, 24);
      this.btnAddResource.Name = "btnAddResource";
      this.btnAddResource.TabIndex = 1;
      this.btnAddResource.Text = "&Add";
      this.btnAddResource.Click += new System.EventHandler(this.btnAddResource_Click);
      // 
      // dvResources
      // 
      this.dvResources.AutoDiscover = false;
      this.dvResources.ContextMenu = this.mnuRoles;
      this.dvResources.DataSource = null;
      this.dvResources.DisplayMember = "";
      this.dvResources.FullRowSelect = true;
      this.dvResources.Location = new System.Drawing.Point(16, 24);
      this.dvResources.MultiSelect = false;
      this.dvResources.Name = "dvResources";
      this.dvResources.Size = new System.Drawing.Size(592, 168);
      this.dvResources.TabIndex = 0;
      this.dvResources.View = System.Windows.Forms.View.Details;
      this.dvResources.DoubleClick += new System.EventHandler(this.dvResources_DoubleClick);
      // 
      // btnSave
      // 
      this.btnSave.Location = new System.Drawing.Point(640, 16);
      this.btnSave.Name = "btnSave";
      this.btnSave.TabIndex = 11;
      this.btnSave.Text = "Save";
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // btnCancel
      // 
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Location = new System.Drawing.Point(640, 48);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.TabIndex = 12;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // chkIsDirty
      // 
      this.chkIsDirty.Enabled = false;
      this.chkIsDirty.Location = new System.Drawing.Point(416, 80);
      this.chkIsDirty.Name = "chkIsDirty";
      this.chkIsDirty.Size = new System.Drawing.Size(0, 0);
      this.chkIsDirty.TabIndex = 3;
      this.chkIsDirty.Text = "IsDirty";
      // 
      // lstRules
      // 
      this.lstRules.Location = new System.Drawing.Point(528, 104);
      this.lstRules.Name = "lstRules";
      this.lstRules.Size = new System.Drawing.Size(184, 134);
      this.lstRules.TabIndex = 13;
      // 
      // ProjectEdit
      // 
      this.AcceptButton = this.btnSave;
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(728, 462);
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.lstRules,
                                                                  this.btnCancel,
                                                                  this.btnSave,
                                                                  this.groupBox1,
                                                                  this.txtDescription,
                                                                  this.label5,
                                                                  this.txtEnded,
                                                                  this.label4,
                                                                  this.txtStarted,
                                                                  this.label3,
                                                                  this.txtName,
                                                                  this.label2,
                                                                  this.txtID,
                                                                  this.label1,
                                                                  this.chkIsDirty});
      this.Name = "ProjectEdit";
      this.Text = "ProjectEdit";
      this.Closing += new System.ComponentModel.CancelEventHandler(this.ProjectEdit_Closing);
      this.Load += new System.EventHandler(this.ProjectEdit_Load);
      this.groupBox1.ResumeLayout(false);
      this.ResumeLayout(false);

    }
		#endregion

    Project _project;

    public Project Project
    {
      get
      {
        return _project;
      }
      set
      {
        _project = value;
      }
    }

    private void btnSave_Click(object sender, System.EventArgs e)
    {
      try
      {
        Cursor.Current = Cursors.WaitCursor;
        _project.ApplyEdit();
        _project = (Project)_project.Save();
        DataBind();
        Cursor.Current = Cursors.Default;
      }
      catch(Exception ex)
      {
        Cursor.Current = Cursors.Default;
        MessageBox.Show(ex.ToString());
      }
      Close();
    }

    private void btnCancel_Click(object sender, System.EventArgs e)
    {
      _project.CancelEdit();
      Close();
    }

    private void ProjectEdit_Load(object sender, System.EventArgs e)
    {
      this.Text = "Project " + _project.Name;

      foreach(string role in Assignment.Roles)
      {
        mnuRoles.MenuItems.Add(Assignment.Roles[role]);
        this.mnuRoles.MenuItems[mnuRoles.MenuItems.Count - 1].Click += 
          new System.EventHandler(this.mnuRoles_Click);
      }

      if(Thread.CurrentPrincipal.IsInRole("ProjectManager"))
      {
        // only project managers can save a project
        _project.BeginEdit();
        btnAddResource.Enabled = true;
        btnRemoveResource.Enabled = true;
      }
      else
      {
        btnAddResource.Enabled = false;
        btnRemoveResource.Enabled = false;
      }

      DataBind();
    }

    void DataBind()
    {
      if(Thread.CurrentPrincipal.IsInRole("ProjectManager"))
      {
        // only project managers can save a project
        Util.BindField(btnSave, "Enabled", _project, "IsValid");
      }
      else
        btnSave.Enabled = false;

      Util.BindField(chkIsDirty, "Checked", _project, "IsDirty");
      Util.BindField(txtID, "Text", _project, "ID");
      Util.BindField(txtName, "Text", _project, "Name");
      Util.BindField(txtStarted, "Text", _project, "Started");
      Util.BindField(txtEnded, "Text", _project, "Ended");
      Util.BindField(txtDescription, "Text", _project, "Description");
      
      lstRules.DataSource = _project.GetBrokenRulesCollection;
      lstRules.DisplayMember = "Description";

      dvResources.SuspendLayout();
      dvResources.Clear();
      dvResources.AutoDiscover = false;
      dvResources.Columns.Add("ID", "ResourceID", 0);
      dvResources.Columns.Add("Last name", "LastName", 100);
      dvResources.Columns.Add("First name", "FirstName", 100);
      dvResources.Columns.Add("Assigned", "Assigned", 100);
      dvResources.Columns.Add("Role", "Role", 150);
      dvResources.DataSource = _project.Resources;
      dvResources.ResumeLayout();
    }

    private void ProjectEdit_Closing(object sender, 
      System.ComponentModel.CancelEventArgs e)
    {
      _project.CancelEdit();
    }

    private void btnAddResource_Click(object sender, System.EventArgs e)
    {
      ResourceSelect dlg = new ResourceSelect();
      dlg.Text = "Assign resource";
      dlg.ShowDialog(this);
      string id = dlg.Result;

      if(id.Length > 0)
      {
        dvResources.SuspendLayout();
        dvResources.DataSource = null;
        try
        {
          _project.Resources.Assign(id);
        }
        catch(Exception ex)
        {
          MessageBox.Show(ex.Message);
        }
        finally
        {
          dvResources.DataSource = _project.Resources;
          dvResources.ResumeLayout();
        }
      }
    }

    private void btnRemoveResource_Click(object sender, System.EventArgs e)
    {
      string id = dvResources.SelectedItems[0].Text;

      if(MessageBox.Show("Remove resource " + id + " from project?", 
        "Remove resource",
        MessageBoxButtons.YesNo, 
        MessageBoxIcon.Question) == DialogResult.Yes)
      {
        dvResources.SuspendLayout();
        dvResources.DataSource = null;
        _project.Resources.Remove(id);
        dvResources.DataSource = _project.Resources;
        dvResources.ResumeLayout();
      }
    }

    private void dvResources_DoubleClick(object sender, System.EventArgs e)
    {
      string id = dvResources.SelectedItems[0].Text;
      Cursor.Current = Cursors.WaitCursor;
      ResourceEdit frm = new ResourceEdit();
      frm.MdiParent = this.MdiParent;
      frm.Resource = Resource.GetResource(id);
      Cursor.Current = Cursors.Default;
      frm.Show();
    }

    private void mnuRoles_Click(object sender, System.EventArgs e)
    {
      MenuItem item = (MenuItem)sender;
      if(dvResources.SelectedItems.Count > 0)
      {
        string id = dvResources.SelectedItems[0].Text;

        dvResources.SuspendLayout();
        dvResources.DataSource = null;
        _project.Resources[id].Role = item.Text;
        dvResources.DataSource = _project.Resources;
        dvResources.ResumeLayout();
      }
    }
  }
}
