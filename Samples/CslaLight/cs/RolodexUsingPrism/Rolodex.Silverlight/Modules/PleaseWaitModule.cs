using System;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Composite.Regions;
using Rolodex.Silverlight.Views;

namespace Rolodex.Silverlight.Modules
{
    public class PleaseWaitModule : IModule
    {
        private static PleaseWaitModule _self;
        private PleaseWaitView _view;
        private IRegionManager _regionManager;
        public PleaseWaitModule(IRegionManager regionManager)
        {
            _self = this;
            _regionManager = regionManager;
        }

        #region IModule Members

        public void Initialize()
        {
            _self = this;
            IRegion pleaseWaitRegion = _regionManager.Regions[Constants.PleaseWaitRegion];
            _view = new PleaseWaitView();
            pleaseWaitRegion.Add(_view, Constants.PleaseWaitViewName);
            _view.Visibility = System.Windows.Visibility.Collapsed;
        }

        #endregion

        public static void ShowPleaseWaitMessage()
        {
            _self._view.Visibility = System.Windows.Visibility.Visible;
#if SILVERLIGHT
            _self._view.Dispatcher.BeginInvoke(() => { _self._view.Focus(); });
#else
            _self._view.Dispatcher.BeginInvoke(new Action(() => _self._view.Focus()), null);
#endif
        }


        public static void HidePleaseWaitMessage()
        {
            _self._view.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
