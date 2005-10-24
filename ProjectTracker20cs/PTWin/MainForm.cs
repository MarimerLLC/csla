using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PTWin
{
  public partial class MainForm : Form
  {
    public MainForm()
    {
      InitializeComponent();
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
      this.projectListBindingSource.DataSource = ProjectTracker.Library.ProjectList.GetProjectList();
    }

    private void projectListDataGridView_SelectionChanged(object sender, EventArgs e)
    {
      if (this.projectListDataGridView.SelectedRows.Count > 0)
      {
        Guid id;
        id = new Guid(this.projectListDataGridView.SelectedRows[0].Cells[0].Value.ToString());
        ProjectTracker.Library.Project project = ProjectTracker.Library.Project.GetProject(id);
        this.projectBindingSource.DataSource = project;
      }
    }
  }
}