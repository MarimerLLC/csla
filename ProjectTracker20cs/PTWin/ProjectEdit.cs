using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ProjectTracker.Library;

namespace PTWin
{
  public partial class ProjectEdit : WinPart
  {

    private Project _project;

    public Project Project
    {
      get { return _project; }
    }

    #region WinPart Code

    public override string Title
    {
      get { return _project.Name; }
      set { _project.Name = value; }
    }

    public override bool Equals(object obj)
    {
      if ((obj is ProjectEdit)&&(_project != null))
        return ((ProjectEdit)obj).Project.Equals(_project);
      else
        return false;
    }

    public override int GetHashCode()
    {
      if (_project != null)
        return _project.GetHashCode();
      else
        return base.GetHashCode();
    }

    private void ProjectEdit_CurrentPrincipalChanged(object sender, EventArgs e)
    {
      ApplyAuthorizationRules();
    }

    #endregion

    private void ApplyAuthorizationRules()
    {
      // have the controls enable/disable/etc
      this.ReadWriteAuthorization1.ResetControlAuthorization();

      bool canSave = ProjectTracker.Library.Project.CanSaveObject();

      // enable/disable appropriate buttons
      this.OKButton.Enabled = canSave;
      this.ApplyButton.Enabled = canSave;
      this.Cancel_Button.Enabled = canSave;
    }

    private void SaveProject()
    {
      using (StatusBusy busy = new StatusBusy("Saving..."))
      {
        // stop the flow of events
        this.ProjectBindingSource.RaiseListChangedEvents = false;
        this.ResourcesBindingSource.RaiseListChangedEvents = false;

        // do the save
        Project old = _project.Clone();
        _project.ApplyEdit();
        try
        {
          _project = _project.Save();
          _project.BeginEdit();
        }
        catch (Exception ex)
        {
          _project = old;
          MessageBox.Show(ex.ToString(), "Save error",
            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        // rebind the UI
        this.ProjectBindingSource.DataSource = null;
        this.ResourcesBindingSource.DataSource = null;
        this.ProjectBindingSource.RaiseListChangedEvents = true;
        this.ResourcesBindingSource.RaiseListChangedEvents = true;
        this.ProjectBindingSource.DataSource = _project;
        this.ResourcesBindingSource.DataSource = _project.Resources;
        ApplyAuthorizationRules();
      }
    }

    public ProjectEdit(Project project)
    {
      InitializeComponent();

      this.CurrentPrincipalChanged += new EventHandler(ProjectEdit_CurrentPrincipalChanged);

      _project = project;
      _project.BeginEdit();
      _project.PropertyChanged += new PropertyChangedEventHandler(mProject_PropertyChanged);

      this.RoleListBindingSource.DataSource = RoleList.GetList();
      this.ProjectBindingSource.DataSource = _project;
      this.ResourcesBindingSource.DataSource = _project.Resources;

      ApplyAuthorizationRules();
    }

    private void OKButton_Click(object sender, EventArgs e)
    {
      SaveProject();
      this.Close();
    }

    private void ApplyButton_Click(object sender, EventArgs e)
    {
      SaveProject();
    }

    private void Cancel_Button_Click(object sender, EventArgs e)
    {
      _project.CancelEdit();
    }

    private void CloseButton_Click(object sender, EventArgs e)
    {
      _project.CancelEdit();
      this.Close();
    }

    private void AssignButton_Click(object sender, EventArgs e)
    {
      ResourceSelect dlg = new ResourceSelect();
      if (dlg.ShowDialog() == DialogResult.OK)
        _project.Resources.Assign(dlg.ResourceId);
    }

    private void UnassignButton_Click(object sender, EventArgs e)
    {
      if (this.ResourcesDataGridView.SelectedRows.Count > 0)
      {
        int resourceId = int.Parse(this.ResourcesDataGridView.SelectedRows[0].Cells[0].Value.ToString());
        _project.Resources.Remove(resourceId);
      }
    }

    private void mProject_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "IsDirty")
      {
        this.ProjectBindingSource.ResetBindings(false);
        this.ResourcesBindingSource.ResetBindings(false);
      }
    }

    private void ResourcesDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.ColumnIndex == 1)
      {
        int resourceId = int.Parse(this.ResourcesDataGridView.Rows[e.RowIndex].Cells[0].Value.ToString());
        MainForm.Instance.ShowEditResource(resourceId);
      }
    }
  }
}
