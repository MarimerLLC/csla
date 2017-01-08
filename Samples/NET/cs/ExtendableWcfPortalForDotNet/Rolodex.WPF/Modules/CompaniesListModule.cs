using System;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Composite.Regions;
using Rolodex.Silverlight.ViewModels;
using System.Linq;
using Microsoft.Practices.Composite.Events;
using Rolodex.Silverlight.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using System.Windows;
using Microsoft.Practices.Unity;

namespace Rolodex.Silverlight.Modules
{
    public class CompaniesListModule : IModule
    {
        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;
        private UnityContainer _unityContainer;
        public CompaniesListModule(IRegionManager regionManager, IEventAggregator eventAggregator, UnityContainer unityContainer)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _unityContainer = unityContainer;
            _unityContainer.RegisterInstance<IEventAggregator>(eventAggregator);
        }

        #region IModule Members

        public void Initialize()
        {
            _eventAggregator.GetEvent<EditCompanyEvent>().Subscribe(HandleEditCompany, ThreadOption.UIThread, true);
            _eventAggregator.GetEvent<AddCompanyEvent>().Subscribe(HandleAddCompany, ThreadOption.UIThread, true);
            _eventAggregator.GetEvent<CloseEditViewEvent>().Subscribe(HandleCloseEdit, ThreadOption.UIThread, true);
            _eventAggregator.GetEvent<ShowCompaniesListEvent>().Subscribe(HandleShowCompaniesList, ThreadOption.UIThread, true);

        }
        #endregion

        public void HandleEditCompany(EditCompanyEventArgs e)
        {
            ClearRegion(Constants.LoginRegion);
            ClearRegion(Constants.MainRegion);
            AddViewToRegion((new CompanyEditModel(e.CompanyID, _eventAggregator)).View, Constants.MainRegion);
        }

        public void HandleAddCompany(EventArgs e)
        {
            ClearRegion(Constants.LoginRegion);
            ClearRegion(Constants.MainRegion);
            AddViewToRegion((new CompanyEditModel(_eventAggregator)).View, Constants.MainRegion);
        }

        public void HandleShowCompaniesList(EventArgs e)
        {
            ClearRegion(Constants.LoginRegion);
            ClearRegion(Constants.MainRegion);
            AddViewToRegion(_unityContainer.Resolve<CompaniesListModel>().View, Constants.MainRegion);
        }


        public void HandleCloseEdit(EventArgs e)
        {
            HandleShowCompaniesList(e);
        }

        private void ClearRegion(string regionName)
        {
            IRegion region = _regionManager.Regions[regionName];
            var currentView = (from oneView in region.Views
                               select oneView).FirstOrDefault();
            if (currentView != null)
            {
                region.Remove(currentView);
            }
        }

        private void AddViewToRegion(FrameworkElement view, string regionName)
        {
            IRegion region = _regionManager.Regions[regionName];
            region.Add(view);
        }
    }
}
