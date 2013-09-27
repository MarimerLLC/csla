using System;
using System.IO;
using System.Threading.Tasks;

namespace ProjectTracker.AndroidUI.ViewModels
{
    public class ProjectEdit : Csla.Axml.ViewModel<Library.ProjectEdit>
    {
        public bool EditMode { get; set; }
        public ProjectEdit()
        {
            EditMode = false;
            ManageObjectLifetime = false;
        }

        public async Task LoadProjectAsync(int projectId = Constants.NewRecordId)
        {
            if (this.IsBusy)
                return;
            this.IsBusy = true;
            try
            {
                if (projectId == Constants.NewRecordId)
                {
                    this.Model = new Library.ProjectEdit();                
                }
                else
                {
                    this.Model = await Library.ProjectEdit.GetProjectAsync(projectId);
                }
                this.BeginEdit();
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        public void LoadFromExisting(Library.ProjectEdit project)
        {
            this.Model = project;
            if (((Csla.Core.IUndoableObject)this.Model).EditLevel < 1)
                this.BeginEdit();
        }

        public async Task SaveProject()
        {
            try
            {
                this.IsBusy = true;
                this.ApplyEdit();
                this.Model = await this.Model.SaveAsync();
                this.BeginEdit();
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        public async Task DeleteProject()
        {
            try
            {
                this.IsBusy = true;
                this.CancelEdit();
                this.Model.Delete();
                await this.Model.SaveAsync();
                this.Model = null;
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        public string Started
        {
            get
            {
                return this.Model == null ? string.Empty : String.Format("{0:MM/dd/yyyy}", this.Model.Started);
            }
            set
            {
                DateTime returnValue;
                DateTime.TryParse(value, out returnValue);
                this.Model.Started = returnValue != DateTime.MinValue ? (DateTime?)returnValue : null;
                this.OnPropertyChanged("Started");
            }
        }

        public string Ended
        {
            get
            {
                return this.Model == null ? string.Empty : String.Format("{0:MM/dd/yyyy}", this.Model.Ended);
            }
            set
            {
                DateTime returnValue;
                DateTime.TryParse(value, out returnValue);
                this.Model.Ended = returnValue != DateTime.MinValue ? (DateTime?)returnValue : null;
                this.OnPropertyChanged("Ended");
            }
        }

        public void BeginEdit()
        {
            if (this.Model == null)
                throw new InvalidDataException("Model not loaded");
            this.Model.BeginEdit();
        }

        public void CancelEdit()
        {
            if (this.Model == null)
                throw new InvalidDataException("Model not loaded");
            this.Model.CancelEdit();
        }

        public void ApplyEdit()
        {
            if (this.Model == null)
                throw new InvalidDataException("Model not loaded");
            this.Model.ApplyEdit();
        }
    }
}