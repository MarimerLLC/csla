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

    private void ProjectEdit_CurrentPrincipalChanged(object sender, EventArgs e)
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
      this.ResourcesDataGridView.Columns[2].ReadOnly = !canEdit;
    }

    private void SaveProject()
    {
      using (StatusBusy busy = new StatusBusy("Saving..."))
      {
        this.ProjectBindingSource.RaiseListChangedEvents = false;
        this.ResourcesBindingSource.RaiseListChangedEvents = false;
        // do the save
        Project temp = _project.Clone();
        temp.ApplyEdit();
        try
        {
          _project = temp.Save();
          _project.BeginEdit();
          // rebind the UI
          this.ProjectBindingSource.DataSource = null;
          this.ProjectBindingSource.DataSource = _project;
          this.ResourcesBindingSource.DataSource = null;
          this.ResourcesBindingSource.DataSource = _project.Resources;
          ApplyAuthorizationRules();
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
          this.ProjectBindingSource.RaiseListChangedEvents = true;
          this.ResourcesBindingSource.RaiseListChangedEvents = true;
        }
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
      if (e.ColumnIndex == 1 && e.RowIndex > -1)
      {
        int resourceId = int.Parse(this.ResourcesDataGridView.Rows[e.RowIndex].Cells[0].Value.ToString());
        MainForm.Instance.ShowEditResource(resourceId);
      }
    }
  }
}
