using Csla.Rules;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Rolodex.Silverlight.Main.Events;
using Rolodex.Silverlight.ViewModels;
using Rolodex.Silverlight.Events;

namespace Rolodex.Silverlight.Main.ViewModels
{
    public partial class CompanyEditViewModel : RolodexViewModel<CompanyEdit>, ICompanyEditViewModel
    {
        #region Constructor

        public CompanyEditViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IUnityContainer unityContainer)
            : base(regionManager, eventAggregator, unityContainer)
        {

        }

        #endregion

        #region methods

        protected override void CreateCommands()
        {
            base.CreateCommands();
            AddEmployeeCommand=new DelegateCommand<object>(OnAddEMployee, CanAddEMployee);
            DeleteEmployeeCommand = new DelegateCommand<EmlpoyeeEdit>(OnDeleteEMployee, CanDeleteEMployee);
        }

        public override void Initialize(object parameter)
        {
            base.Initialize();
            var companyID = (int)parameter;
            IsBusy = true;
            CompanyEditUoW.GetCompanyEdit(companyID,
                           (o, e) =>
                           {
                               IsBusy = false;
                               if (HasNoException(e))
                               {
                                   Statuses = e.Object.Statuses;
                                   Model = e.Object.Company;
                                   AddEmployeeCommand.RaiseCanExecuteChanged();
                               }
                           });
        }

        private EmployeeStatusInfoList statuses;
        public EmployeeStatusInfoList Statuses
        {
            get
            {
                return statuses;
            }
            set
            {
                statuses = value;
                OnPropertyChanged("Statuses");
            }
        }

        private DelegateCommand<object> addEmployeeCommand;

        public DelegateCommand<object> AddEmployeeCommand
        {
            get { return addEmployeeCommand; }
            set { addEmployeeCommand = value; OnPropertyChanged("AddEmployeeCommand"); }
        }


        public void OnAddEMployee(object parameter)
        {
            Model.Employees.NewEmlpoyeeEdit(Model.CompanyID);
        }

        public bool CanAddEMployee(object parameter)
        {
            return Model != null && BusinessRules.HasPermission(AuthorizationActions.CreateObject, typeof (EmlpoyeeEdit));
        }

        private DelegateCommand<EmlpoyeeEdit> deleteEmployeeCommand;

        public DelegateCommand<EmlpoyeeEdit> DeleteEmployeeCommand
        {
            get { return deleteEmployeeCommand; }
            set { deleteEmployeeCommand = value;OnPropertyChanged("DeleteEmployeeCommand"); }
        }

        public void OnDeleteEMployee(EmlpoyeeEdit parameter)
        {
            parameter.DeleteInList();
        }

        public bool CanDeleteEMployee(EmlpoyeeEdit parameter)
        {
            return Model != null && BusinessRules.HasPermission(AuthorizationActions.DeleteObject, typeof(EmlpoyeeEdit));
        }
        

        protected override void OnAfterDeleteSave()
        {
            base.OnAfterDeleteSave();
            Aggregator.GetEvent<CloseRequested>().Publish(this);
        }

        public override void AfterSave()
        {
            base.AfterSave();
            Aggregator.GetEvent<RefreshCompaniesEvent>().Publish(Model.CompanyID);
        }

        #endregion
    }
}