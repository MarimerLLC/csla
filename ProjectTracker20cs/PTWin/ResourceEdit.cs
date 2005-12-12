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
  public partial class ResourceEdit : WinPart
  {

    private Resource _resource;

    public Resource Resource
    {
      get { return _resource; }
    }

    #region WinPart Code

    public override string Title
    {
      get { return _resource.LastName; }
      set { _resource.LastName = value; }
    }

    public override bool Equals(object obj)
    {
      if ((obj is ResourceEdit)&&(_resource != null))
        return ((ResourceEdit)obj).Resource.Equals(_resource);
      else
        return false;
    }

    public override int GetHashCode()
    {
      if (_resource != null)
        return _resource.GetHashCode();
      else
        return base.GetHashCode();
    }

    private void ResourceEdit_CurrentPrincipalChanged(object sender, EventArgs e)
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

    private void SaveResource()
    {
      using (StatusBusy busy = new StatusBusy("Saving..."))
      {
        // stop the flow of events
        this.ResourceBindingSource.RaiseListChangedEvents = false;
        this.AssignmentsBindingSource.RaiseListChangedEvents = false;

        // do the save
        Resource old = Resource.Clone();
        _resource.ApplyEdit();
        try
        {
          _resource = _resource.Save();
          _resource.BeginEdit();
        }
        catch (Exception ex)
        {
          _resource = old;
          MessageBox.Show(ex.ToString(), "Save error",
            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        // rebind the UI
        this.ResourceBindingSource.DataSource = null;
        this.AssignmentsBindingSource.DataSource = null;
        this.ResourceBindingSource.RaiseListChangedEvents = true;
        this.AssignmentsBindingSource.RaiseListChangedEvents = true;
        this.ResourceBindingSource.DataSource = _resource;
        this.AssignmentsBindingSource.DataSource = _resource.Assignments;
        ApplyAuthorizationRules();
      }
    }

    public ResourceEdit(Resource resource)
    {
      InitializeComponent();

      _resource = resource;
      _resource.BeginEdit();

      this.CurrentPrincipalChanged += new EventHandler(ResourceEdit_CurrentPrincipalChanged);
      _resource.PropertyChanged += new PropertyChangedEventHandler(mResource_PropertyChanged);

      this.RoleListBindingSource.DataSource = RoleList.GetList();
      this.ResourceBindingSource.DataSource = _resource;
      this.AssignmentsBindingSource.DataSource = _resource.Assignments;

      ApplyAuthorizationRules();
    }

    private void OKButton_Click(object sender, EventArgs e)
    {
      SaveResource();
      this.Close();
    }

    private void ApplyButton_Click(object sender, EventArgs e)
    {
      SaveResource();
    }

    private void Cancel_Button_Click(object sender, EventArgs e)
    {
      _resource.CancelEdit();
    }

    private void CloseButton_Click(object sender, EventArgs e)
    {
      _resource.CancelEdit();
      this.Close();
    }

    private void AssignButton_Click(object sender, EventArgs e)
    {
      ProjectSelect dlg = new ProjectSelect();
      if (dlg.ShowDialog() == DialogResult.OK)
        _resource.Assignments.AssignTo(dlg.ProjectId);
    }

    private void UnassignButton_Click(object sender, EventArgs e)
    {
      if (this.AssignmentsDataGridView.SelectedRows.Count > 0)
      {
        Guid projectId = (Guid)this.AssignmentsDataGridView.SelectedRows[0].Cells[0].Value;
        _resource.Assignments.Remove(projectId);
      }
    }

    private void mResource_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "IsDirty")
      {
        this.ResourceBindingSource.ResetBindings(true);
        this.AssignmentsBindingSource.ResetBindings(true);
      }
    }

    private void AssignmentsDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.ColumnIndex == 1)
      {
        Guid projectId = (Guid)this.AssignmentsDataGridView.Rows[e.RowIndex].Cells[0].Value;
        MainForm.Instance.ShowEditProject(projectId);
      }
    }
  }
}
