using System;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Composite.Regions;
using Rolodex.Silverlight.ViewModels;
using System.Linq;
using Microsoft.Practices.Composite.Events;
using Rolodex.Silverlight.Events;
using Microsoft.Practices.Composite.Presentation.Events;
using System.Windows;

namespace Rolodex.Silverlight.Modules
{
    public class CompaniesListModule : IModule
    {
        private IRegionManager _regionManager;
        private IEventAggregator _eventAggregator;
        public CompaniesListModule(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
        }

        #region IModule Members

        public void Initialize()
        {
            ClearRegion(Constants.LoginRegion);
            ClearRegion(Constants.MainRegion);
            AddViewToRegion((new CompaniesListModel(_eventAggregator)).View, Constants.MainRegion);
            _eventAggregator.GetEvent<EditCompanyEvent>().Subscribe(HandleEditCompany, ThreadOption.UIThread, true);
            _eventAggregator.GetEvent<AddCompanyEvent>().Subscribe(HandleAddCompany, ThreadOption.UIThread, true);
            _eventAggregator.GetEvent<CloseEditViewEvent>().Subscribe(HandleCloseEdit, ThreadOption.UIThread, true);
        }
        #endregion
        public void HandleEditCompany(EditCompanyEventArgs e)
        {
            ClearRegion(Constants.MainRegion);
            AddViewToRegion((new CompanyEditModel(e.CompanyID, _eventAggregator)).View, Constants.MainRegion);
        }

        public void HandleAddCompany(EventArgs e)
        {
            ClearRegion(Constants.MainRegion);
            AddViewToRegion((new CompanyEditModel(_eventAggregator)).View, Constants.MainRegion);
        }


        public void HandleCloseEdit(EventArgs e)
        {
            ClearRegion(Constants.MainRegion);
            AddViewToRegion((new CompaniesListModel(_eventAggregator)).View, Constants.MainRegion);
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
