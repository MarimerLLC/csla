using System.Threading.Tasks;

namespace ProjectTracker.AndroidUI.ViewModels
{
    public class RoleEditList : Csla.Axml.ViewModel<Library.Admin.RoleEditList>
    {
        public Task LoadAsync()
        {
            var tcs = new TaskCompletionSource<Library.Admin.RoleEditList>();
            this.IsBusy = true;

            Library.Admin.RoleEditList.GetRoles((o, e) =>
            {
                this.IsBusy = false;
                if (e.Error == null && e.Object != null)
                {
                    this.Model = e.Object;
                    tcs.SetResult(null);
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

        public void LoadFromExisting(Library.Admin.RoleEditList roleEditList)
        {
            this.Model = roleEditList;
        }

        public async Task SaveRoles()
        {
            if (!this.IsBusy && this.CanEditObject)
            {
                try
                {
                    this.IsBusy = true;
                    this.Model = await this.Model.SaveAsync();
                }
                finally
                {
                    this.IsBusy = false;
                }
            }
        }
    }
}