using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace PTWin
{
  public partial class ResourceEdit : WinPart
  {

    private ProjectTracker.Library.ResourceEdit _resource;

    public ProjectTracker.Library.ResourceEdit Resource
    {
      get { return _resource; }
    }

    public ResourceEdit(ProjectTracker.Library.ResourceEdit resource)
    {
      InitializeComponent();

      // store object reference
      _resource = resource;
    }

    private void ResourceEdit_Load(object sender, EventArgs e)
    {
      this.CurrentPrincipalChanged += new EventHandler(ResourceEdit_CurrentPrincipalChanged);
      _resource.PropertyChanged += new PropertyChangedEventHandler(mResource_PropertyChanged);

      this.RoleListBindingSource.DataSource = ProjectTracker.Library.RoleList.GetList();

      BindUI();

      ApplyAuthorizationRules();
    }

    #region WinPart Code

    protected internal override object GetIdValue()
    {
      return _resource;
    }

    public override string ToString()
    {
      return _resource.FullName;
    }

    private void ResourceEdit_CurrentPrincipalChanged(object sender, EventArgs e)
    {
      ApplyAuthorizationRules();
    }

    #endregion

    private void ApplyAuthorizationRules()
    {
      bool canEdit = Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.EditObject, typeof(ProjectTracker.Library.ResourceEdit));
      if (!canEdit)
        RebindUI(false, true);

      // have the controls enable/disable/etc
      this.ReadWriteAuthorization1.ResetControlAuthorization();

      // enable/disable appropriate buttons
      this.OKButton.Enabled = canEdit;
      this.ApplyButton.Enabled = canEdit;
      this.Cancel_Button.Enabled = canEdit;

      // enable/disable role column in grid
      this.AssignmentsDataGridView.Columns[3].ReadOnly = !canEdit;
    }

    private void OKButton_Click(object sender, EventArgs e)
    {
      using (StatusBusy busy = new StatusBusy("Saving..."))
      {
        RebindUI(true, false);
      }
      this.Close();
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
      _resource.BeginEdit();
      this.ResourceBindingSource.DataSource = _resource;
    }

    private void RebindUI(bool saveObject, bool rebind)
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
          _resource.ApplyEdit();
          try
          {
            _resource = _resource.Save();
          }
          catch (Csla.DataPortalException ex)
          {
            MessageBox.Show(ex.BusinessException.ToString(),
              "Error saving", MessageBoxButtons.OK,
              MessageBoxIcon.Exclamation);
          }
          catch (Exception ex)
          {
            MessageBox.Show(ex.ToString(),
              "Error Saving", MessageBoxButtons.OK,
              MessageBoxIcon.Exclamation);
          }
        }
        else
          _resource.CancelEdit();
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

    private void AssignButton_Click(object sender, EventArgs e)
    {
      ProjectSelect dlg = new ProjectSelect();
      if (dlg.ShowDialog() == DialogResult.OK)
        try
        {
          _resource.Assignments.AssignTo(dlg.ProjectId);
        }
        catch (InvalidOperationException ex)
        {
          MessageBox.Show(ex.ToString(),
            "Error Assigning", MessageBoxButtons.OK,
            MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.ToString(),
            "Error Assigning", MessageBoxButtons.OK,
            MessageBoxIcon.Exclamation);
        }
    }

    private void UnassignButton_Click(object sender, EventArgs e)
    {
      if (this.AssignmentsDataGridView.SelectedRows.Count > 0)
      {
        var projectId = (int)this.AssignmentsDataGridView.SelectedRows[0].Cells[0].Value;
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
      if (e.ColumnIndex == 1 && e.RowIndex > -1)
      {
        var projectId = (int)this.AssignmentsDataGridView.Rows[e.RowIndex].Cells[0].Value;
        MainForm.Instance.ShowEditProject(projectId);
      }
    }
  }
}
