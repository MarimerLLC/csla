using System;
using System.Windows.Forms;

namespace ProjectsVendors.WinForms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void selectProject_Click(object sender, EventArgs e)
        {
            using (ProjectSelect dlg = new ProjectSelect())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    CleanWorkspace();
                    workspace.Controls.Add(new ProjectEditor(dlg.ProjectId));
                }
            }
        }

        private void newProject_Click(object sender, EventArgs e)
        {
            CleanWorkspace();
            workspace.Controls.Add(new ProjectEditor(-1));
        }

        private void CleanWorkspace()
        {
            foreach (UserControl workspaceControl in workspace.Controls)
            {
                workspaceControl.Dispose();
            }

            workspace.Controls.Clear();
        }
    }
}