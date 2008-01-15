using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProjectTracker.Library;
using System.Linq;

namespace PTWin
{
  public partial class ProjectSelect : Form
  {

    private Guid _projectId;

    public Guid ProjectId
    {
      get { return _projectId; }
    }



    public ProjectSelect()
    {
      InitializeComponent();
    }

    private void OK_Button_Click(object sender, EventArgs e)
    {
      _projectId = (Guid)this.ProjectListListBox.SelectedValue;
      this.Close();
    }

    private void Cancel_Button_Click(object sender, EventArgs e)
    {
      this.Close();
    }

    private void ProjectSelect_Load(object sender, EventArgs e)
    {
      DisplayList(ProjectList.GetProjectList());
    }

    private void DisplayList(ProjectList list)
    {
      var sortedList = from p in list orderby p.Name select p;
      this.projectListBindingSource.DataSource = sortedList;
    }

    private void GetListButton_Click(
      object sender, EventArgs e)
    {
      DisplayList(ProjectList.GetProjectList(NameTextBox.Text));
    }
  }
}