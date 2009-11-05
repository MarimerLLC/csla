using System;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Composite.Regions;
using Rolodex.Silverlight.Views;
using Rolodex.Silverlight.ViewModels;

namespace Rolodex.Silverlight.Modules
{
    public class LoginModule : IModule
    {
        private readonly IRegionManager _regionManager;
        public LoginModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        #region IModule Members

        public void Initialize()
        {
            IRegion loginRegion = _regionManager.Regions[Constants.LoginRegion];
            loginRegion.Add((new LoginViewModel()).View);
        }

        #endregion

    }
}
