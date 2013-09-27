using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ProjectTracker.Library;

namespace ProjectTracker.AndroidUI.ViewModels
{
    public class ProjectResourceEdit : Csla.Axml.ViewModel<Library.ProjectResourceEdit>
    {
        public bool EditMode { get; set; }
        public Library.ProjectEdit Root { get; private set; }
        private RoleList Roles;

        public ProjectResourceEdit()
        {
            EditMode = false;
            ManageObjectLifetime = false;
        }

        public void LoadProject(Library.ProjectEdit project, int projectResourceId = Constants.NewRecordId)
        {
            this.Root = project;
            if (projectResourceId == Constants.NewRecordId)
            {
                throw new NotImplementedException();
            }
            this.Model = this.Root.Resources.Single(r => r.ResourceId == projectResourceId);
            if (((Csla.Core.IUndoableObject)this.Root).EditLevel < 3)
                this.BeginEdit();
            this.EditMode = true;
        }

        public async Task<RoleList> GetRolesAsync()
        {
            return this.Roles ?? (this.Roles = await RoleList.GetListAsync());
        }

        public void DeleteResource()
        {
            if (!this.EditMode)
            {
                return;
            }
            this.IsBusy = true;
            this.Root.Resources.Remove(this.Model);
            this.Model = null;
            this.IsBusy = false;
        }

        public void BeginEdit()
        {
            if (this.Model == null)
                throw new InvalidDataException("Model not loaded");
            this.Root.BeginEdit();
        }

        public void CancelEdit()
        {
            if (this.Model == null)
                throw new InvalidDataException("Model not loaded");
            this.Root.CancelEdit();
        }

        public void ApplyEdit()
        {
            if (this.Model == null)
                throw new InvalidDataException("Model not loaded");
            this.Root.ApplyEdit();
        }
    }
}