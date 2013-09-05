using System;
using System.Threading.Tasks;

namespace ProjectTracker.AndroidUI.ViewModels
{
    public class ResourceEdit : Csla.Axml.ViewModel<Library.ResourceEdit>
    {
        public bool EditMode { get; set; }
        public ResourceEdit()
        {
            EditMode = false;
            ManageObjectLifetime = true;
        }

        public async Task LoadProjectAsync(int resourceId = Constants.NewRecordId)
        {
            if (this.IsBusy)
                return;
            this.IsBusy = true;
            try
            {
                if (resourceId == Constants.NewRecordId)
                {
                    this.Model = new Library.ResourceEdit();
                }
                else
                {
                    this.Model = await Library.ResourceEdit.GetResourceEditAsync(resourceId);
                }
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        public async Task SaveResource()
        {
            try
            {
                this.IsBusy = true;
                this.Model.ApplyEdit();
                this.Model = await this.Model.SaveAsync();
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        public async Task DeleteResource()
        {
            try
            {
                this.IsBusy = true;
                this.Model.CancelEdit();
                this.Model.Delete();
                await this.Model.SaveAsync();
                this.Model = null;
            }
            finally
            {
                this.IsBusy = false;
            }
        }
    }
}