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

    private void ApplyAuthorizationRules()
    {
      // have the controls enable/disable/etc
      this.ReadWriteAuthorization1.ResetControlAuthorization();

      bool canEdit = 
        ProjectTracker.Library.Project.CanEditObject();

      // enable/disable appropriate buttons
      this.OKButton.Enabled = canEdit;
      this.ApplyButton.Enabled = canEdit;
      this.Cancel_Button.Enabled = canEdit;
      this.AssignButton.Enabled = canEdit;
      this.UnassignButton.Enabled = canEdit;

      // enable/disable role column in grid
      this.ResourcesDataGridView.Columns[2].ReadOnly = 
        !canEdit;
    }

    private void SaveProject(bool rebind)
    {
      using (StatusBusy busy = new StatusBusy("Saving..."))
      {
        this.projectBindingSource.RaiseListChangedEvents = false;
        this.resourcesBindingSource.RaiseListChangedEvents = false;
        // do the save
        this.projectBindingSource.EndEdit();
        this.resourcesBindingSource.EndEdit();
        try
        {
          Project temp = _project.Clone();
          _project = temp.Save();
          _project.BeginEdit();
          if (rebind)
          {
            // rebind the UI
            this.projectBindingSource.DataSource = null;
            this.resourcesBindingSource.DataSource = this.projectBindingSource;
            this.projectBindingSource.DataSource = _project;
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
          this.projectBindingSource.RaiseListChangedEvents = true;
          this.resourcesBindingSource.RaiseListChangedEvents = true;
        }
      }
    }

    public ProjectEdit(Project project)
    {
      InitializeComponent();

      _project = project;
      _project.BeginEdit();

      this.roleListBindingSource.DataSource = RoleList.GetList();
      this.projectBindingSource.DataSource = _project;

      ApplyAuthorizationRules();
    }

    private void OKButton_Click(object sender, EventArgs e)
    {
      SaveProject(false);
      this.Close();
    }

    private void ApplyButton_Click(object sender, EventArgs e)
    {
      SaveProject(true);
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
        try
        {
          _project.Resources.Assign(dlg.ResourceId);
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
      if (this.ResourcesDataGridView.SelectedRows.Count > 0)
      {
        int resourceId = int.Parse(
          this.ResourcesDataGridView.SelectedRows[0].
          Cells[0].Value.ToString());
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
  }
}
