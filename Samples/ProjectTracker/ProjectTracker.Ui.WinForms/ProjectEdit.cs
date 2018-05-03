using System;
using System.ComponentModel;
using System.Windows.Forms;
using ProjectTracker.Library;

namespace PTWin
{
  public partial class ProjectEdit : WinPart
  {
    #region Properties

    public ProjectTracker.Library.ProjectEdit Project { get; private set; }

    public override string ToString()
    {
      return Project.Name;
    }

    protected internal override object GetIdValue()
    {
      return Project;
    }

    #endregion

    #region Change event handlers

    private void ProjectEdit_CurrentPrincipalChanged(object sender, EventArgs e)
    {
      ApplyAuthorizationRules();
    }

    private void Project_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "IsDirty")
      {
        this.ProjectBindingSource.ResetBindings(true);
        this.ResourcesBindingSource.ResetBindings(true);
      }
    }

    #endregion

    public ProjectEdit(ProjectTracker.Library.ProjectEdit project)
    {
      InitializeComponent();

      // store object reference
      Project = project;
    }

    private void ProjectEdit_Load(object sender, EventArgs e)
    {
      this.CurrentPrincipalChanged += new EventHandler(ProjectEdit_CurrentPrincipalChanged);
      Project.PropertyChanged += new PropertyChangedEventHandler(Project_PropertyChanged);

      Setup();
    }

    private void Setup()
    {
      // set up binding
      this.RoleListBindingSource.DataSource = ProjectTracker.Library.RoleList.GetCachedList();

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

    private void BindUI()
    {
      Project.BeginEdit();
      this.ProjectBindingSource.DataSource = Project;
    }

    private bool RebindUI(bool saveObject, bool rebind)
    {
      // disable events
      this.ProjectBindingSource.RaiseListChangedEvents = false;
      this.ResourcesBindingSource.RaiseListChangedEvents = false;
      try
      {
        // unbind the UI
        UnbindBindingSource(this.ResourcesBindingSource, saveObject, false);
        UnbindBindingSource(this.ProjectBindingSource, saveObject, true);
        this.ResourcesBindingSource.DataSource = this.ProjectBindingSource;

        // save or cancel changes
        if (saveObject)
        {
          Project.ApplyEdit();
          try
          {
            Project = Project.Save();
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
          Project.CancelEdit();

        return true;
      }
      finally
      {
        // rebind UI if requested
        if (rebind)
          BindUI();

        // restore events
        this.ProjectBindingSource.RaiseListChangedEvents = true;
        this.ResourcesBindingSource.RaiseListChangedEvents = true;

        if (rebind)
        {
          // refresh the UI if rebinding
          this.ProjectBindingSource.ResetBindings(false);
          this.ResourcesBindingSource.ResetBindings(false);
        }
      }
    }

    #region Button event handlers

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

    private void RefreshButton_Click(object sender, EventArgs e)
    {
      using (StatusBusy busy = new StatusBusy("Refreshing..."))
      {
        if (RebindUI(false, false))
        {
          Project = ProjectTracker.Library.ProjectEdit.GetProject(Project.Id);
          RoleList.InvalidateCache();
          RoleList.CacheList();
          Setup();
        }
      }
    }

    private void AssignButton_Click(object sender, EventArgs e)
    {
      using (ResourceSelect dlg = new ResourceSelect())
      {
        if (dlg.ShowDialog() == DialogResult.OK)
        {
          try
          {
            Project.Resources.Assign(dlg.ResourceId);
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
      }
    }

    private void UnassignButton_Click(object sender, EventArgs e)
    {
      if (this.ResourcesDataGridView.SelectedRows.Count > 0)
      {
        int resourceId = int.Parse(this.ResourcesDataGridView.SelectedRows[0].Cells[0].Value.ToString());
        Project.Resources.Remove(resourceId);
      }
    }

    private void ResourcesDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.ColumnIndex == 1 && e.RowIndex > -1)
      {
        int resourceId = int.Parse(this.ResourcesDataGridView.Rows[e.RowIndex].Cells[0].Value.ToString());
        MainForm.Instance.ShowEditResource(resourceId);
      }
    }

    #endregion
  }
}