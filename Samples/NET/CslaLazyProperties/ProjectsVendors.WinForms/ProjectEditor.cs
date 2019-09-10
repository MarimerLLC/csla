using System;
using System.ComponentModel;
using System.Windows.Forms;
using Csla;
using ProjectsVendors.Business;

namespace ProjectsVendors.WinForms
{
    public partial class ProjectEditor : UserControl
    {
        public ProjectEdit Project { get; private set; }

        private ProjectEditor()
        {
            InitializeComponent();
        }

        public ProjectEditor(int projectId) :
            this()
        {
            if (projectId == -1)
                Project = ProjectEdit.NewProjectEdit();
            else
                Project = ProjectEdit.GetProjectEdit(projectId);
        }

        private void ProjectEdit_Load(object sender, EventArgs e)
        {
            Project.PropertyChanged += Project_PropertyChanged;

            VendorsBindingSource.DataMember = null;
            VendorsBindingSource.DataSource = null;

            BindUI();
        }

        private void Project_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsDirty")
            {
                ProjectBindingSource.ResetBindings(true);
            }
        }

        #region Plumbing...

        private void BindUI()
        {
            Project.BeginEdit();

            ProjectBindingSource.DataSource = Project;
        }

        private bool RebindUI(bool saveObject, bool rebind)
        {
            // disable events
            ProjectBindingSource.RaiseListChangedEvents = false;
            VendorsBindingSource.RaiseListChangedEvents = false;
            try
            {
                // unbind the UI
                UnbindBindingSource(VendorsBindingSource, saveObject, false);
                UnbindBindingSource(ProjectBindingSource, saveObject, true);
                VendorsBindingSource.DataSource = ProjectBindingSource;

                // save or cancel changes
                if (saveObject)
                {
                    Project.ApplyEdit();
                    try
                    {
                        Project = Project.Save();
                    }
                    catch (DataPortalException ex)
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
                    Project.CancelEdit();

                return true;
            }
            finally
            {
                // rebind UI if requested
                if (rebind)
                    BindUI();

                // restore events
                ProjectBindingSource.RaiseListChangedEvents = true;
                VendorsBindingSource.RaiseListChangedEvents = true;

                if (rebind)
                {
                    // refresh the UI if rebinding
                    ProjectBindingSource.ResetBindings(false);
                    VendorsBindingSource.ResetBindings(false);
                }
            }
        }

        protected void UnbindBindingSource(BindingSource source, bool apply, bool isRoot)
        {
            IEditableObject current = source.Current as IEditableObject;
            if (isRoot)
                source.DataSource = null;
            if (current != null)
                if (apply)
                    current.EndEdit();
                else
                    current.CancelEdit();
        }

        #endregion

        private void LazyLoadVendors()
        {
            VendorsBindingSource.DataMember = "Vendors";
            VendorsBindingSource.DataSource = ProjectBindingSource;
        }

        private bool Save()
        {
            return RebindUI(true, true);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            if (!Project.IsDirty)
                return;

            RebindUI(false, true);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (!Project.IsSavable)
                return;

            if (Save())
                MessageBox.Show("Project saved.", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedTab == vendorsTabPage)
            {
                LazyLoadVendors();
            }
        }

        private void vendorsDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
        }
    }
}