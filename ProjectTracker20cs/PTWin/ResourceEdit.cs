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

    public ResourceEdit(Resource resource)
    {
      InitializeComponent();

      _resource = resource;

      this.CurrentPrincipalChanged += new EventHandler(ResourceEdit_CurrentPrincipalChanged);
      _resource.PropertyChanged += new PropertyChangedEventHandler(mResource_PropertyChanged);

      this.RoleListBindingSource.DataSource = RoleList.GetList();
      this.ResourceBindingSource.DataSource = _resource;

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
      // have the controls enable/disable/etc
      this.ReadWriteAuthorization1.ResetControlAuthorization();

      bool canEdit = ProjectTracker.Library.Project.CanEditObject();

      // enable/disable appropriate buttons
      this.OKButton.Enabled = canEdit;
      this.ApplyButton.Enabled = canEdit;
      this.Cancel_Button.Enabled = canEdit;

      // enable/disable role column in grid
      this.AssignmentsDataGridView.Columns[3].ReadOnly = !canEdit;
    }

    private void SaveResource(bool rebind)
    {
      using (StatusBusy busy = new StatusBusy("Saving..."))
      {
        // stop the flow of events
        this.ResourceBindingSource.RaiseListChangedEvents = false;
        this.AssignmentsBindingSource.RaiseListChangedEvents = false;
        // do the save
        this.ResourceBindingSource.EndEdit();
        this.AssignmentsBindingSource.EndEdit();
        try
        {
          Resource temp = Resource.Clone();
          _resource = temp.Save();
          if (rebind)
          {
            // rebind the UI
            this.ResourceBindingSource.DataSource = null;
            this.AssignmentsBindingSource.DataSource = this.ResourceBindingSource;
            this.ResourceBindingSource.DataSource = _resource;
            ApplyAuthorizationRules();
          }
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
        finally
        {
          this.ResourceBindingSource.RaiseListChangedEvents = true;
          this.AssignmentsBindingSource.RaiseListChangedEvents = true;
          this.ResourceBindingSource.ResetBindings(false);
          this.AssignmentsBindingSource.ResetBindings(false);
        }
      }
    }

    private void OKButton_Click(object sender, EventArgs e)
    {
      SaveResource(false);
      this.Close();
    }

    private void ApplyButton_Click(object sender, EventArgs e)
    {
      SaveResource(true);
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
      if (e.ColumnIndex == 1 && e.RowIndex > -1)
      {
        Guid projectId = (Guid)this.AssignmentsDataGridView.Rows[e.RowIndex].Cells[0].Value;
        MainForm.Instance.ShowEditProject(projectId);
      }
    }
  }
}
