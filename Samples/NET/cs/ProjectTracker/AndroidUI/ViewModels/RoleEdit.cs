using System.Linq;
using System.Threading.Tasks;

namespace ProjectTracker.AndroidUI.ViewModels
{
    public class RoleEdit : Csla.Axml.ViewModel<Library.Admin.RoleEdit>
    {
        public bool EditMode { get; set; }
        public Library.Admin.RoleEditList ParentList { get; set; }

        public RoleEdit(Library.Admin.RoleEditList parent)
        {
            ParentList = parent;
            EditMode = false;
            ManageObjectLifetime = true;
            Model = new Library.Admin.RoleEdit();
        }

        public RoleEdit(Library.Admin.RoleEdit role)
        {
            ParentList = (Library.Admin.RoleEditList)role.Parent;
            EditMode = true;
            ManageObjectLifetime = true;
            Model = role;
        }

        public async Task SaveParentListAsync()
        {
            if (this.ParentList != null)
            {
                try
                {
                    this.IsBusy = true;
                    var id = this.Model.IsNew ? -1 : this.Model.Id;
                    if (this.Model.IsNew)
                    {
                        this.ParentList.Add(this.Model);
                    }
                    this.ParentList = await this.ParentList.SaveAsync();
                    if (id > 0)
                    {
                        this.Model = this.ParentList.GetRoleById(id);
                    }
                    else
                    {
                        // There should always at leave be one, even on an add
                        // Deletes should not use this method
                        this.Model = this.ParentList.Last();
                    }
                }
                finally
                {
                    this.IsBusy = false;
                }
            }
        }

        public void DeleteFromParentList()
        {
            if (this.ParentList != null)
            {
                try
                {
                    this.IsBusy = true;
                    this.ParentList.Remove(this.Model);
                    this.Model = null;
                }
                finally
                {
                    this.IsBusy = false;
                }
            }
        }
    }
}