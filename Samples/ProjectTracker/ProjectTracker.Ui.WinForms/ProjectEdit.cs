using System;
using System.Windows.Forms;

namespace PTWin
{
  public partial class ProjectEdit : WinPart
  {
    private ProjectTracker.Library.ProjectEdit _project;

    public ProjectTracker.Library.ProjectEdit Project
    {
      get { return _project; }
    }

    #region WinPart Code

    public override string ToString()
    {
      return _project.Name;
    }

    protected internal override object GetIdValue()
    {
      return _project;
    }

    private void ProjectEdit_CurrentPrincipalChanged(
      object sender, EventArgs e)
    {
      ApplyAuthorizationRules();
    }

    #endregion

    public ProjectEdit(ProjectTracker.Library.ProjectEdit project)
    {
      InitializeComponent();

      // store object reference
      _project = project;
    }

    private void ProjectEdit_Load(object sender, EventArgs e)
    {
      // set up binding
      this.roleListBindingSource.DataSource = ProjectTracker.Library.RoleList.GetCachedList();

      BindUI();

      // check authorization
      ApplyAuthorizationRules();
    }

    private void ApplyAuthorizationRules()
    {
      bool canEdit = Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.EditObject,
        typeof(ProjectTracker.Library.ProjectEdit));
      if (!canEdit)
        RebindUI(false, true);

      // have the controls enable/disable/etc
      this.ReadWriteAuthorization1.ResetControlAuthorization();

      // enable/disable appropriate buttons
      this.OKButton.Enabled = canEdit;
      this.ApplyButton.Enabled = canEdit;
      this.Cancel_Button.Enabled = canEdit;
      this.AssignButton.Enabled = canEdit;
      this.UnassignButton.Enabled = canEdit;

      // enable/disable role column in grid
      this.ResourcesDataGridView.Columns[2].ReadOnly = !canEdit;
    }

    private void OKButton_Click(object sender, EventArgs e)
    {
      using (StatusBusy busy = new StatusBusy("Saving..."))
      {
        if (RebindUI(true, false))
        {
          this.Close();
        }
      }
    }

    private void ApplyButton_Click(object sender, EventArgs e)
    {
      using (StatusBusy busy = new StatusBusy("Saving..."))
      {
        RebindUI(true, true);
      }
    }

    private void Cancel_Button_Click(object sender, EventArgs e)
    {
      RebindUI(false, true);
    }

    private void CloseButton_Click(object sender, EventArgs e)
    {
      RebindUI(false, false);
      this.Close();
    }

    private void BindUI()
    {
      _project.BeginEdit();
      this.projectBindingSource.DataSource = _project;
    }

    private bool RebindUI(bool saveObject, bool rebind)
    {
      // disable events
      this.projectBindingSource.RaiseListChangedEvents = false;
      this.resourcesBindingSource.RaiseListChangedEvents = false;
      try
      {
        // unbind the UI
        UnbindBindingSource(this.resourcesBindingSource, saveObject, false);
        UnbindBindingSource(this.projectBindingSource, saveObject, true);
        this.resourcesBindingSource.DataSource = this.projectBindingSource;

        // save or cancel changes
        if (saveObject)
        {
          _project.ApplyEdit();
          try
          {
            _project = _project.Save();
          }
          catch (Csla.DataPortalException ex)
          {
            MessageBox.Show(ex.BusinessException.ToString(),
              "Error saving", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
          }
          catch (Exception ex)
          {
            MessageBox.Show(ex.ToString(),
              "Error Saving", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
          }
        }
        else
          _project.CancelEdit();

        return true;
      }
      finally
      {
        // rebind UI if requested
        if (rebind)
          BindUI();

        // restore events
        this.projectBindingSource.RaiseListChangedEvents = true;
        this.resourcesBindingSource.RaiseListChangedEvents = true;

        if (rebind)
        {
          // refresh the UI if rebinding
          this.projectBindingSource.ResetBindings(false);
          this.resourcesBindingSource.ResetBindings(false);
        }
      }
    }

    private void AssignButton_Click(object sender, EventArgs e)
    {
      ResourceSelect dlg = new ResourceSelect();
      if (dlg.ShowDialog() == DialogResult.OK)
        try
        {
          _project.Resources.Assign(dlg.ResourceId);
        }
        catch (InvalidOperationException ex)
        {
          MessageBox.Show(ex.Message,
            "Error Assigning", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.ToString(),
            "Error Assigning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
    }

    private void UnassignButton_Click(object sender, EventArgs e)
    {
      if (this.ResourcesDataGridView.SelectedRows.Count > 0)
      {
        int resourceId = int.Parse(
          this.ResourcesDataGridView.SelectedRows[0].Cells[0].Value.ToString());
        _project.Resources.Remove(resourceId);
      }
    }

    private void ResourcesDataGridView_CellContentClick(
      object sender, DataGridViewCellEventArgs e)
    {
      if (e.ColumnIndex == 1 && e.RowIndex > -1)
      {
        int resourceId = int.Parse(
          this.ResourcesDataGridView.Rows[
            e.RowIndex].Cells[0].Value.ToString());
        MainForm.Instance.ShowEditResource(resourceId);
      }
    }

    private void RefreshButton_Click(object sender, EventArgs e)
    {
      using (StatusBusy busy = new StatusBusy("Refreshing..."))
      {
        if (RebindUI(false, false))
        {
          _project = ProjectTracker.Library.ProjectEdit.GetProject(_project.Id);
          ProjectEdit_Load(sender, e);
        }
      }
    }
  }
}