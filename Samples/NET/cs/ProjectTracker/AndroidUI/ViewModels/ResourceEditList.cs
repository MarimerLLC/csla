using System.Threading.Tasks;

namespace ProjectTracker.AndroidUI.ViewModels
{
    public class ResourceEditList : Csla.Axml.ViewModel<Library.ResourceList>
    {
        public async Task LoadAsync()
        {
            this.IsBusy = true;
            this.Model = await Library.ResourceList.GetResourceListAsync();
            this.IsBusy = false;
        }

        public bool CanAdd
        {
            get { return Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.CreateObject, typeof(Library.ResourceEdit)); }
        }

        public bool CanEdit
        {
            get { return Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.EditObject, typeof(Library.ResourceEdit)); }
        }
    }
}