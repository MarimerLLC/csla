using System.IO;
using System.Threading.Tasks;
using Javax.Security.Auth;
using ProjectTracker.Library;

namespace ProjectTracker.AndroidUI.ViewModels
{
    public class ProjectResourceAdd : Csla.Axml.ViewModel<ProjectResourceEditCreator>
    {
        public Library.ProjectEdit Root { get; private set; }
        private Library.ResourceList Resources;
        public int ResourceId { get; set; }

        public ProjectResourceAdd()
        {
            ManageObjectLifetime = false;
        }

        public void LoadProject(Library.ProjectEdit project)
        {
            this.Root = project;
            if (((Csla.Core.IUndoableObject)this.Root).EditLevel < 3)
                this.Root.BeginEdit();
        }

        public async Task<Library.ResourceList> GetResourcesAsync()
        {
            return this.Resources ?? (this.Resources = await Library.ResourceList.GetResourceListAsync());
        }

        public async Task<Library.ProjectResourceEdit> CreateProjectResourceAsync()
        {
            if (IsBusy)
                return null;
            try
            {
                this.IsBusy = true;
                var projectResourceEdit = await this.Root.Resources.AssignAsync(this.ResourceId);
                return projectResourceEdit;
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        private Task<ProjectResourceEditCreator> NewProjectResourceAsync()
        {
            var tcs = new TaskCompletionSource<ProjectResourceEditCreator>();
            ProjectResourceEditCreator.GetProjectResourceEditCreator(this.Root.Id, this.ResourceId, (o, e) =>
            {
                if (e.Error == null && e.Object != null)
                {
                    tcs.SetResult(e.Object);
                }
                else
                {
                    if (e.Error != null)
                        tcs.SetException(e.Error);
                    else
                        tcs.SetCanceled();
                }
            });
            return tcs.Task;
        }

        public void BeginEdit()
        {
            if (this.Root == null)
                throw new InvalidDataException("Root not loaded");
            this.Root.BeginEdit();
        }

        public void CancelEdit()
        {
            if (this.Root == null)
                throw new InvalidDataException("Root not loaded");
            this.Root.CancelEdit();
        }

        public void ApplyEdit()
        {
            if (this.Root == null)
                throw new InvalidDataException("Root not loaded");
            this.Root.ApplyEdit();
        }
    }
}