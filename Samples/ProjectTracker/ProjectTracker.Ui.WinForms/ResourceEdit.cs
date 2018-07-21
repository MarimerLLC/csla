using System;
using System.ComponentModel;
using System.Windows.Forms;
using Csla.Rules;
using ProjectTracker.Library;

namespace PTWin
{
  public partial class ResourceEdit : WinPart
  {
    #region Properties

    public ProjectTracker.Library.ResourceEdit Resource { get; private set; }

    protected internal override object GetIdValue()
    {
      return Resource;
    }

    public override string ToString()
    {
      return Resource.FullName;
    }

    #endregion

    #region Change event handlers

    private void ResourceEdit_CurrentPrincipalChanged(object sender, EventArgs e)
    {
      ApplyAuthorizationRules();
    }

    private void Resource_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "IsDirty")
      {
        this.ResourceBindingSource.ResetBindings(true);
        this.AssignmentsBindingSource.ResetBindings(true);
      }
    }

    #endregion

    #region Constructors

    private ResourceEdit()
    {
      // force to use parametrized constructor
    }

    public ResourceEdit(ProjectTracker.Library.ResourceEdit resource)
    {
      InitializeComponent();

      // store object reference
      Resource = resource;
    }

    #endregion

    #region Plumbing...

    private void ResourceEdit_Load(object sender, EventArgs e)
    {
      this.CurrentPrincipalChanged += new EventHandler(ResourceEdit_CurrentPrincipalChanged);
      Resource.PropertyChanged += new PropertyChangedEventHandler(Resource_PropertyChanged);

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
        typeof(ProjectTracker.Library.ResourceEdit));
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
      this.AssignmentsDataGridView.Columns[3].ReadOnly = !canEdit;
    }

    private void BindUI()
    {
      Resource.BeginEdit();
      this.ResourceBindingSource.DataSource = Resource;
    }

    private bool RebindUI(bool saveObject, bool rebind)
    {
      // disable events
      this.ResourceBindingSource.RaiseListChangedEvents = false;
      this.AssignmentsBindingSource.RaiseListChangedEvents = false;
      try
      {
        // unbind the UI
        UnbindBindingSource(this.AssignmentsBindingSource, saveObject, false);
        UnbindBindingSource(this.ResourceBindingSource, saveObject, true);
        this.AssignmentsBindingSource.DataSource = this.ResourceBindingSource;

        // save or cancel changes
        if (saveObject)
        {
          Resource.ApplyEdit();
          try
          {
            Resource = Resource.Save();
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
          Resource.CancelEdit();

        return true;
      }
      finally
      {
        // rebind UI if requested
        if (rebind)
          BindUI();

        // restore events
        this.ResourceBindingSource.RaiseListChangedEvents = true;
        this.AssignmentsBindingSource.RaiseListChangedEvents = true;

        if (rebind)
        {
          // refresh the UI if rebinding
          this.ResourceBindingSource.ResetBindings(false);
          this.AssignmentsBindingSource.ResetBindings(false);
        }
      }
    }

    #endregion

    #region Button event handlers

    private void OKButton_Click(object sender, EventArgs e)
    {
      if (IsSavable())
      {
        using (StatusBusy busy = new StatusBusy("Saving..."))
        {
          if (RebindUI(true, false))
          {
            this.Close();
          }
        }
      }
    }

    private void ApplyButton_Click(object sender, EventArgs e)
    {
      if (IsSavable())
      {
        using (StatusBusy busy = new StatusBusy("Saving..."))
        {
          RebindUI(true, true);
        }
      }
    }

    private bool IsSavable()
    {
      if (Resource.IsSavable)
        return true;

      if (!Resource.IsValid)
      {
        MessageBox.Show(GetErrorMessage(), "Saving Resource", MessageBoxButtons.OK, MessageBoxIcon.Error);
      }

      return false;
    }

    private string GetErrorMessage()
    {
      var message = "Resource is invalid and cannot be saved." + Environment.NewLine + Environment.NewLine;
      foreach (var rule in Resource.BrokenRulesCollection)
      {
        if (rule.Severity == RuleSeverity.Error)
          message += "- " + rule.Description + Environment.NewLine;
      }

      return message;
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
      if (CanRefresh())
      {
        using (StatusBusy busy = new StatusBusy("Refreshing..."))
        {
          if (RebindUI(false, false))
          {
            Resource = ProjectTracker.Library.ResourceEdit.GetResourceEdit(Resource.Id);
            RoleList.InvalidateCache();
            RoleList.CacheList();
            Setup();
          }
        }
      }
    }

    private bool CanRefresh()
    {
      if (!Resource.IsDirty)
        return true;

      var dlg = MessageBox.Show("Resource is not saved and all changes will be lost.\r\n\r\nDo you want to refresh?.",
        "Refreshing Resource",
        MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

      return dlg == DialogResult.OK;
    }

    private void AssignButton_Click(object sender, EventArgs e)
    {
      using (ProjectSelect dlg = new ProjectSelect())
      {
        if (dlg.ShowDialog() == DialogResult.OK)
        {
          try
          {
            Resource.Assignments.AssignTo(dlg.ProjectId);
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
      if (this.AssignmentsDataGridView.SelectedRows.Count > 0)
      {
        var projectId = (int) this.AssignmentsDataGridView.SelectedRows[0].Cells[0].Value;
        Resource.Assignments.Remove(projectId);
      }
    }

    private void AssignmentsDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
      if (e.ColumnIndex == 1 && e.RowIndex > -1)
      {
        var projectId = (int) this.AssignmentsDataGridView.Rows[e.RowIndex].Cells[0].Value;
        MainForm.Instance.ShowEditProject(projectId);
      }
    }

    #endregion
  }
}