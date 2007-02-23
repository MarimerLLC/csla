using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using PTServiceClient;

namespace PTServiceClient
{
  public partial class MainForm : Form
  {
    public MainForm()
    {
      InitializeComponent();
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
      using (PTService.PTService svc = new PTService.PTService())
      {
        this.ProjectDataBindingSource.DataSource = 
          svc.GetProjectList();
        this.ResourceDataBindingSource.DataSource = 
          svc.GetResourceList();
        this.RoleDataBindingSource.DataSource = 
          svc.GetRoles();
      }
    }

    private void ProjectDataDataGridView_SelectionChanged(object sender, EventArgs e)
    {
      if (this.ProjectDataDataGridView.SelectedRows.Count > 0)
      {
        string projectId = this.ProjectDataDataGridView.SelectedRows[0].Cells[0].Value.ToString();
        if (projectId != this.IdLabel1.Text)
        {
          using (PTService.PTService svc = new PTService.PTService())
          {
            PTService.ProjectRequest request = new PTServiceClient.PTService.ProjectRequest();
            request.Id = new Guid(projectId);
            this.ProjectDetailBindingSource.DataSource = svc.GetProject(request);
          }
        }
      }
    }

    private void SaveProjectButton_Click(object sender, EventArgs e)
    {
      using (PTService.PTService svc = new PTService.PTService())
      {
        SetCredentials(svc);
        Guid id = new Guid(this.IdLabel1.Text);
        if (Guid.Empty.Equals(id))
        {
          // adding
          this.ProjectDetailBindingSource.DataSource =
            svc.AddProject(
            this.NameTextBox.Text,
            this.StartedTextBox.Text,
            this.EndedTextBox.Text,
            this.DescriptionTextBox.Text);
        }
        else
        {
          // updating
          this.ProjectDetailBindingSource.DataSource =
            svc.EditProject(new Guid(this.IdLabel1.Text),
            this.NameTextBox.Text,
            this.StartedTextBox.Text,
            this.EndedTextBox.Text,
            this.DescriptionTextBox.Text);
        }
        // refresh project list
        this.ProjectDataBindingSource.DataSource = svc.GetProjectList();
      }
    }

    private void ClearProjectButton_Click(object sender, EventArgs e)
    {
      this.ProjectDetailBindingSource.Clear();
      this.ProjectDetailBindingSource.AddNew();
    }

    private void AssignToProjectButton_Click(
      object sender, EventArgs e)
    {
      if (this.ResourceIdLabel.Text.Trim().Length == 0)
        MessageBox.Show(
          "You must select a resource first", "Assign resource",
          MessageBoxButtons.OK, MessageBoxIcon.Information);
      if (this.ProjectIdLabel.Text.Trim().Length == 0 || 
        Guid.Empty.Equals(new Guid(this.ProjectIdLabel.Text)))
        MessageBox.Show(
          "You must select a project first", "Assign resource",
          MessageBoxButtons.OK, MessageBoxIcon.Information);
      using (PTService.PTService svc = new PTService.PTService())
      {
        SetCredentials(svc);
        try
        {
          // do the assignment
          svc.AssignResource(
            int.Parse(this.ResourceIdLabel.Text), 
            new Guid(this.ProjectIdLabel.Text));
          // refresh the detail view
          PTService.ProjectRequest request = 
            new PTServiceClient.PTService.ProjectRequest();
          request.Id = new Guid(this.ProjectIdLabel.Text);
          this.ProjectDetailBindingSource.DataSource = 
            svc.GetProject(request);
        }
        catch (Exception ex)
        {
          MessageBox.Show(ex.Message, "Assign resource",
            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }
      }
    }

    private void ResourceDataDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
      ResourceName dlg = new ResourceName(this.ResourceDataDataGridView.SelectedRows[0].Cells[0].Value.ToString(),
        this.ResourceDataDataGridView.SelectedRows[0].Cells[1].Value.ToString());
      if (dlg.ShowDialog() == DialogResult.OK)
      {
        using (PTService.PTService svc = new PTService.PTService())
        {
          SetCredentials(svc);
          // save the changes
          int resourceId = int.Parse(dlg.IdLabel1.Text);
          string firstName = dlg.FirstNameTextBox.Text;
          string lastName = dlg.LastNameTextBox.Text;
          svc.ChangeResourceName(resourceId, firstName, lastName);
          // refresh the resource list.
          this.ResourceDataBindingSource.DataSource = svc.GetResourceList();
        }
      }
    }

    private void SetCredentials(PTService.PTService svc)
    {
      PTService.CslaCredentials credentials = 
        new PTService.CslaCredentials();
      credentials.Username = usernameTextBox.Text;
      credentials.Password = passwordTextBox.Text;
      svc.CslaCredentialsValue = credentials;
    }
  }
}