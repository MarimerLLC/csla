using System.Linq;
using Csla.Rules;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Rolodex.Silverlight.Events;
using Rolodex.Silverlight.Services;
using Rolodex.Silverlight.ViewModels;
using Rolodex.Silverlight.Main.Events;

namespace Rolodex.Silverlight.Main.ViewModels
{
    public partial class CompaniesViewModel : RolodexSimpleViewModel<CompanyInfoList>, ICompaniesViewModel
    {
        #region Constructor

        public CompaniesViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IUnityContainer unityContainer)
            : base(regionManager, eventAggregator, unityContainer)
        {

        }


        protected override void CreateCommands()
        {
            base.CreateCommands();
            SearchCommand = new DelegateCommand<object>(Search);
            SelectCommand = new DelegateCommand<CompanyInfo>(Select);
            AddCompanyCommand = new DelegateCommand<CompanyInfo>(AddCompany, CanAddCompany);
            Aggregator.GetEvent<RefreshCompaniesEvent>().Subscribe(OnRefreshCompanies);
        }

        #endregion

        #region Properties

        private string partialName = string.Empty;

        public string PartialName
        {
            get { return partialName; }
            set { partialName = value; OnPropertyChanged("PartialName"); }
        }


        #endregion

        #region Commands

        private DelegateCommand<object> searchCommand;
        public DelegateCommand<object> SearchCommand
        {
            get
            {
                return searchCommand;
            }
            set
            {
                searchCommand = value;
                OnPropertyChanged("SearchCommand");
            }
        }


        public void Search(object parameter)
        {
            IsBusy = true;
            CompanyInfoList.GetCompanyInfoList(partialName, (o, e) =>
            {
                IsBusy = false;
                if (base.HasNoException(e))
                {
                    Model = e.Object;
                }
            });
        }


        private DelegateCommand<CompanyInfo> addCompanyCommand;
        public DelegateCommand<CompanyInfo> AddCompanyCommand
        {
            get
            {
                return addCompanyCommand;
            }
            set
            {
                addCompanyCommand = value;
                OnPropertyChanged("AddCompanyCommand");
            }
        }

        public bool CanAddCompany(object parameter)
        {
            return BusinessRules.HasPermission(AuthorizationActions.CreateObject, typeof (CompanyEdit));
        }

        public void AddCompany(object paramter)
        {
            Aggregator.GetEvent<ServiceSelected>().Publish(new ServiceEventArgs(RolodexService.CompanyEdit, 0));
        }

        private DelegateCommand<CompanyInfo> selectCommand;
        public DelegateCommand<CompanyInfo> SelectCommand
        {
            get
            {
                return selectCommand;
            }
            set
            {
                selectCommand = value;
                OnPropertyChanged("SelectCommand");
            }
        }

        public void Select(CompanyInfo companyInfo)
        {
            Aggregator.GetEvent<ServiceSelected>().Publish(new ServiceEventArgs(RolodexService.CompanyEdit, companyInfo.CompanyID));
        }

        #endregion

        #region Method

        public void OnRefreshCompanies(int companyID)
        {
            if (Model != null && Model.Any(one => one.CompanyID == companyID))
            {
                Search(null);
            }
        }

        #endregion
    }
}