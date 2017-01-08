using System;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Composite.Regions;
using Rolodex.Silverlight.Views;
using Rolodex.Silverlight.ViewModels;
using Microsoft.Practices.Unity;

namespace Rolodex.Silverlight.Modules
{
    public class LoginModule : IModule
    {
        private readonly IRegionManager _regionManager;
        private readonly IUnityContainer _container;
        public LoginModule(IUnityContainer container, IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _container = container;
        }

        #region IModule Members

        public void Initialize()
        {
            _regionManager.Regions[Constants.LoginRegion].Add(_container.Resolve<LoginViewModel>().View);
        }

        #endregion

    }
}
