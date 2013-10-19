
using System.IO;
namespace ProjectTracker.AndroidUI.ViewModels
{
    public class ProjectResourceEditList : Csla.Axml.ViewModel<Library.ProjectResources>
    {
        public Library.ProjectEdit Root { get; set; }

        public ProjectResourceEditList()
        {
            this.ManageObjectLifetime = false;
        }
        
        public void LoadFromExisting(Library.ProjectResources projectResources)
        {
            this.Model = projectResources;
            this.Root = (Library.ProjectEdit)projectResources.Parent;
            if (((Csla.Core.IUndoableObject)this.Root).EditLevel < 2)
                this.BeginEdit();
        }

        public bool CanAdd
        {
            get { return Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.CreateObject, typeof(Library.ProjectResourceEdit)); }
        }

        public bool CanEdit
        {
            get { return Csla.Rules.BusinessRules.HasPermission(Csla.Rules.AuthorizationActions.EditObject, typeof(Library.ProjectResourceEdit)); }
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