using System;
using System.Threading;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using ProjectTracker.Library;

namespace PTWin
{
  /// <summary>
  /// Summary description for ResourceEdit.
  /// </summary>
  public class ResourceEdit : System.Windows.Forms.Form
  {
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
      this.txtID = new System.Windows.Forms.TextBox();
      this.colAssigned = new System.Windows.Forms.DataGridTextBoxColumn();
      this.txtFirstname = new System.Windows.Forms.TextBox();
      this.colName = new System.Windows.Forms.DataGridTextBoxColumn();
      this.chkIsDirty = new System.Windows.Forms.CheckBox();
      this.GroupBox1 = new System.Windows.Forms.GroupBox();
      this.dvProjects = new MSDN.DataListView.DataListView();
      this.btnRemoveProject = new System.Windows.Forms.Button();
      this.btnAssignProject = new System.Windows.Forms.Button();
      this.colRole = new System.Windows.Forms.DataGridTextBoxColumn();
      this.txtLastname = new System.Windows.Forms.TextBox();
      this.mnuRoles = new System.Windows.Forms.ContextMenu();
      this.Label5 = new System.Windows.Forms.Label();
      this.Label1 = new System.Windows.Forms.Label();
      this.btnSave = new System.Windows.Forms.Button();
      this.Label2 = new System.Windows.Forms.Label();
      this.btnCancel = new System.Windows.Forms.Button();
      this.colProjectID = new System.Windows.Forms.DataGridTextBoxColumn();
      this.GroupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // txtID
      // 
      this.txtID.Location = new System.Drawing.Point(89, 8);
      this.txtID.Name = "txtID";
      this.txtID.ReadOnly = true;
      this.txtID.TabIndex = 1;
      this.txtID.TabStop = false;
      this.txtID.Text = "";
      // 
      // colAssigned
      // 
      this.colAssigned.Format = "";
      this.colAssigned.FormatInfo = null;
      this.colAssigned.HeaderText = "Assigned";
      this.colAssigned.MappingName = "Assigned";
      this.colAssigned.ReadOnly = true;
      this.colAssigned.Width = 75;
      // 
      // txtFirstname
      // 
      this.txtFirstname.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right);
      this.txtFirstname.Location = new System.Drawing.Point(89, 40);
      this.txtFirstname.Name = "txtFirstname";
      this.txtFirstname.Size = new System.Drawing.Size(464, 20);
      this.txtFirstname.TabIndex = 3;
      this.txtFirstname.Text = "";
      // 
      // colName
      // 
      this.colName.Format = "";
      this.colName.FormatInfo = null;
      this.colName.HeaderText = "Project";
      this.colName.MappingName = "ProjectName";
      this.colName.ReadOnly = true;
      this.colName.Width = 140;
      // 
      // chkIsDirty
      // 
      this.chkIsDirty.Location = new System.Drawing.Point(537, 8);
      this.chkIsDirty.Name = "chkIsDirty";
      this.chkIsDirty.Size = new System.Drawing.Size(0, 0);
      this.chkIsDirty.TabIndex = 34;
      this.chkIsDirty.Text = "CheckBox1";
      // 
      // GroupBox1
      // 
      this.GroupBox1.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right);
      this.GroupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                            this.dvProjects,
                                                                            this.btnRemoveProject,
                                                                            this.btnAssignProject});
      this.GroupBox1.Location = new System.Drawing.Point(9, 104);
      this.GroupBox1.Name = "GroupBox1";
      this.GroupBox1.Size = new System.Drawing.Size(632, 208);
      this.GroupBox1.TabIndex = 6;
      this.GroupBox1.TabStop = false;
      this.GroupBox1.Text = "Assigned to";
      // 
      // dvProjects
      // 
      this.dvProjects.AutoDiscover = false;
      this.dvProjects.ContextMenu = this.mnuRoles;
      this.dvProjects.DataSource = null;
      this.dvProjects.DisplayMember = "";
      this.dvProjects.FullRowSelect = true;
      this.dvProjects.Location = new System.Drawing.Point(16, 24);
      this.dvProjects.MultiSelect = false;
      this.dvProjects.Name = "dvProjects";
      this.dvProjects.Size = new System.Drawing.Size(512, 168);
      this.dvProjects.TabIndex = 0;
      this.dvProjects.View = System.Windows.Forms.View.Details;
      this.dvProjects.DoubleClick += new System.EventHandler(this.dvProjects_DoubleClick);
      // 
      // btnRemoveProject
      // 
      this.btnRemoveProject.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
      this.btnRemoveProject.Location = new System.Drawing.Point(544, 56);
      this.btnRemoveProject.Name = "btnRemoveProject";
      this.btnRemoveProject.TabIndex = 2;
      this.btnRemoveProject.Text = "&Remove";
      this.btnRemoveProject.Click += new System.EventHandler(this.btnRemoveProject_Click);
      // 
      // btnAssignProject
      // 
      this.btnAssignProject.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
      this.btnAssignProject.Location = new System.Drawing.Point(544, 24);
      this.btnAssignProject.Name = "btnAssignProject";
      this.btnAssignProject.TabIndex = 1;
      this.btnAssignProject.Text = "&Assign to";
      this.btnAssignProject.Click += new System.EventHandler(this.btnAssignProject_Click);
      // 
      // colRole
      // 
      this.colRole.Format = "";
      this.colRole.FormatInfo = null;
      this.colRole.HeaderText = "Role";
      this.colRole.MappingName = "Role";
      this.colRole.Width = 140;
      // 
      // txtLastname
      // 
      this.txtLastname.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right);
      this.txtLastname.Location = new System.Drawing.Point(89, 72);
      this.txtLastname.Name = "txtLastname";
      this.txtLastname.Size = new System.Drawing.Size(464, 20);
      this.txtLastname.TabIndex = 5;
      this.txtLastname.Text = "";
      // 
      // Label5
      // 
      this.Label5.Location = new System.Drawing.Point(9, 8);
      this.Label5.Name = "Label5";
      this.Label5.Size = new System.Drawing.Size(69, 23);
      this.Label5.TabIndex = 0;
      this.Label5.Text = "ID";
      // 
      // Label1
      // 
      this.Label1.Location = new System.Drawing.Point(9, 72);
      this.Label1.Name = "Label1";
      this.Label1.TabIndex = 4;
      this.Label1.Text = "Last name";
      // 
      // btnSave
      // 
      this.btnSave.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
      this.btnSave.Location = new System.Drawing.Point(569, 8);
      this.btnSave.Name = "btnSave";
      this.btnSave.TabIndex = 7;
      this.btnSave.Text = "&Save";
      this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
      // 
      // Label2
      // 
      this.Label2.Location = new System.Drawing.Point(9, 40);
      this.Label2.Name = "Label2";
      this.Label2.TabIndex = 2;
      this.Label2.Text = "First name";
      // 
      // btnCancel
      // 
      this.btnCancel.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Location = new System.Drawing.Point(569, 40);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.TabIndex = 8;
      this.btnCancel.Text = "&Cancel";
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // colProjectID
      // 
      this.colProjectID.Format = "";
      this.colProjectID.FormatInfo = null;
      this.colProjectID.MappingName = "ProjectID";
      this.colProjectID.ReadOnly = true;
      this.colProjectID.Width = 0;
      // 
      // ResourceEdit
      // 
      this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
      this.ClientSize = new System.Drawing.Size(656, 326);
      this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                  this.txtFirstname,
                                                                  this.chkIsDirty,
                                                                  this.GroupBox1,
                                                                  this.txtLastname,
                                                                  this.Label5,
                                                                  this.Label1,
                                                                  this.btnSave,
                                                                  this.Label2,
                                                                  this.btnCancel,
                                                                  this.txtID});
      this.Name = "ResourceEdit";
      this.Text = "ResourceEdit";
      this.Closing += new System.ComponentModel.CancelEventHandler(this.ResourceEdit_Closing);
      this.GroupBox1.ResumeLayout(false);
      this.ResumeLayout(false);

    }
		#endregion

    internal System.Windows.Forms.TextBox txtID;
    internal System.Windows.Forms.DataGridTextBoxColumn colAssigned;
    internal System.Windows.Forms.TextBox txtFirstname;
    internal System.Windows.Forms.DataGridTextBoxColumn colName;
    internal System.Windows.Forms.CheckBox chkIsDirty;
    internal System.Windows.Forms.GroupBox GroupBox1;
    internal System.Windows.Forms.Button btnRemoveProject;
    internal System.Windows.Forms.Button btnAssignProject;
    internal System.Windows.Forms.DataGridTextBoxColumn colRole;
    internal System.Windows.Forms.TextBox txtLastname;
    internal System.Windows.Forms.ContextMenu mnuRoles;
    internal System.Windows.Forms.Label Label5;
    internal System.Windows.Forms.Label Label1;
    internal System.Windows.Forms.Button btnSave;
    internal System.Windows.Forms.Label Label2;
    internal System.Windows.Forms.Button btnCancel;
    internal System.Windows.Forms.DataGridTextBoxColumn colProjectID;
    private MSDN.DataListView.DataListView dvProjects;

    Resource _resource;

    public Resource Resource
    {
      get
      {
        return _resource;
      }
      set
      {
        _resource = value;
      }
    }

    public ResourceEdit(Resource resource)
    {
      InitializeComponent();
      _resource = resource;
      this.Text = "Resource " + _resource.LastName + ", " + _resource.FirstName;

      foreach(string role in Assignment.Roles)
      {
        MenuItem item = new MenuItem(Assignment.Roles[role]);
        mnuRoles.MenuItems.Add(item);
        item.Click += new System.EventHandler(mnuRoles_Click);
      }

      if(Thread.CurrentPrincipal.IsInRole("ProjectManager") ||
        Thread.CurrentPrincipal.IsInRole("Supervisor"))
      {
        // only project managers or supervisors can edit a resource
        _resource.BeginEdit();
        btnAssignProject.Enabled = true;
        btnRemoveProject.Enabled = true;
      }
      else
      {
        btnAssignProject.Enabled = false;
        btnRemoveProject.Enabled = false;
      }
      DataBind();
    }

    void DataBind()
    {
      if(Thread.CurrentPrincipal.IsInRole("ProjectManager") ||
         Thread.CurrentPrincipal.IsInRole("Supervisor"))
      {
        // only project managers or supervisors can save a resource
        Util.BindField(btnSave, "Enabled", _resource, "IsValid");
      }
      else
        btnSave.Enabled = false;

      Util.BindField(chkIsDirty, "Checked", _resource, "IsDirty");
      Util.BindField(txtID, "Text", _resource, "ID");
      Util.BindField(txtLastname, "Text", _resource, "LastName");
      Util.BindField(txtFirstname, "Text", _resource, "FirstName");

      dvProjects.SuspendLayout();
      dvProjects.Clear();
      dvProjects.AutoDiscover = false;
      dvProjects.Columns.Add("ID", "ProjectID", 0);
      dvProjects.Columns.Add("Project", "ProjectName", 200);
      dvProjects.Columns.Add("Assigned", "Assigned", 100);
      dvProjects.Columns.Add("Role", "Role", 150);
      dvProjects.DataSource = _resource.Assignments;
      dvProjects.ResumeLayout();
    }

    private void btnCancel_Click(object sender, System.EventArgs e)
    {
      Close();
    }

    private void btnSave_Click(object sender, System.EventArgs e)
    {
      try
      {
        Cursor.Current = Cursors.WaitCursor;
        _resource.ApplyEdit();
        _resource = (Resource)_resource.Save();
        Cursor.Current = Cursors.Default;
      }
      catch(Exception ex)
      {
        Cursor.Current = Cursors.Default;
        MessageBox.Show(this, ex.ToString());
      }
      Close();
    }

    private void ResourceEdit_Closing(object sender, 
      System.ComponentModel.CancelEventArgs e)
    {
      _resource.CancelEdit();
    }

    private void btnAssignProject_Click(object sender, System.EventArgs e)
    {
      ProjectSelect dlg = new ProjectSelect("Assign to project");
      dlg.ShowDialog(this);
      string result = dlg.Result;

      if(result.Length > 0)
      {
        dvProjects.SuspendLayout();
        dvProjects.DataSource = null;
        try
        {
          Guid id = new Guid(result);
          _resource.Assignments.AssignTo(id);
        }
        catch(Exception ex)
        {
          MessageBox.Show(this, ex.Message);
        }
        finally
        {
          dvProjects.DataSource = _resource.Assignments;
          dvProjects.ResumeLayout();
        }
      }
    }

    private void btnRemoveProject_Click(object sender, System.EventArgs e)
    {
      Guid id = new Guid(dvProjects.SelectedItems[0].Text);
      string name = dvProjects.SelectedItems[0].SubItems[0].Text;

      if(MessageBox.Show(this, "Remove from project " + Name + "?", 
        "Remove assignment",
        MessageBoxButtons.YesNo) == DialogResult.Yes)
      {
        dvProjects.SuspendLayout();
        dvProjects.DataSource = null;
        _resource.Assignments.Remove(id);
        dvProjects.DataSource = _resource.Assignments;
        dvProjects.ResumeLayout();
      }
    }

    private void mnuRoles_Click(object sender, System.EventArgs e)
    {
      MenuItem item = (MenuItem)sender;
      if(dvProjects.SelectedItems.Count > 0)
      {
        Guid id = new Guid(dvProjects.SelectedItems[0].Text);

        dvProjects.SuspendLayout();
        dvProjects.DataSource = null;
        _resource.Assignments[id].Role = item.Text;
        dvProjects.DataSource = _resource.Assignments;
        dvProjects.ResumeLayout();
      }
    }

    private void dvProjects_DoubleClick(object sender, System.EventArgs e)
    {
      Guid id = new Guid(dvProjects.SelectedItems[0].Text);
      Cursor.Current = Cursors.WaitCursor;
      ProjectEdit frm = new ProjectEdit(Project.GetProject(id));
      frm.MdiParent = this.MdiParent;
      Cursor.Current = Cursors.Default;
      frm.Show();
    }
  }
}
