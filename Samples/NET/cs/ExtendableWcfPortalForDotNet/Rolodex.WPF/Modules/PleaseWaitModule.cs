using System;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Composite.Regions;
using Rolodex.Silverlight.Views;
using Rolodex.Silverlight.ViewModels;
using Microsoft.Practices.Composite.Events;

namespace Rolodex.Silverlight.Modules
{
    public class PleaseWaitModule : IModule
    {
         #region Private Members

        private IRegionManager regionManager;
        private IEventAggregator eventAggregator;
        #endregion

        #region Constructor
        public PleaseWaitModule(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
        }
        #endregion

        #region IModule Members

        public void Initialize()
        {
            //this is needed because the ViewModel must set a focus to the view
            WaitWindowView view = new WaitWindowView();
            view.DataContext = new WaitWindowViewModel(eventAggregator, view);
            regionManager.AddToRegion(Constants.PleaseWaitRegion, view);
        }

        #endregion
    }
}
