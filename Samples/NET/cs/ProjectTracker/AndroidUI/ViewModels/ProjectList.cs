using System.Threading.Tasks;

namespace ProjectTracker.AndroidUI.ViewModels
{
    public class ProjectEditList : Csla.Axml.ViewModel<Library.ProjectList>
    {
        public async Task LoadAsync()
        {
            this.IsBusy = true;
            this.Model = await Library.ProjectList.GetProjectListAsync();
            this.IsBusy = false;
        }

        public bool CanAdd
        {
            get { return Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.CreateObject, typeof(Library.ProjectEdit)); }
        }

        public bool CanEdit
        {
            get { return Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.EditObject, typeof(Library.ProjectEdit)); }
        }
    }
}