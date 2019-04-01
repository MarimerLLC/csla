using System;
using System.Linq;
using System.Windows.Forms;
using ProjectsVendors.Business;

namespace ProjectsVendors.WinForms
{
    public partial class ProjectSelect : Form
    {
        private int _projectId;

        public int ProjectId
        {
            get { return _projectId; }
        }

        public ProjectSelect()
        {
            InitializeComponent();
        }

        private void OK_Button_Click(object sender, EventArgs e)
        {
            _projectId = ((ProjectInfo) ProjectListListBox.SelectedValue).ProjectId;
            Close();
        }

        private void Cancel_Button_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void ProjectSelect_Load(object sender, EventArgs e)
        {
            DisplayList(ProjectList.GetProjectList());
        }

        private void DisplayList(ProjectList list)
        {
            var sortedList = from p in list orderby p.ProjectName select p;
            projectListBindingSource.DataSource = sortedList;
        }

        private void ProjectListListBox_DoubleClick(object sender, EventArgs e)
        {
            _projectId = ((ProjectInfo) ProjectListListBox.SelectedValue).ProjectId;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}