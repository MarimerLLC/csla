using System.Windows;
using Csla;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;

namespace Rolodex.Silverlight.ViewModels
{
    public abstract class RolodexSimpleViewModel<TModel> : Csla.Xaml.ViewModelBase<TModel>, IRolodexViewModel
        where TModel : class
    {

        #region Properties

        protected IEventAggregator Aggregator { get; private set; }
        protected IUnityContainer Container { get; private set; }
        protected IRegionManager RegionManager { get; private set; }

        public virtual bool IsDirty
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Constructors


        protected RolodexSimpleViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IUnityContainer unityContainer)
        {
            Aggregator = eventAggregator;
            Container = unityContainer;
            RegionManager = regionManager;
            CreateCommands();
        }

        protected virtual void CreateCommands()
        {
        }

        ~RolodexSimpleViewModel()
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Disposed of view model of type " + GetType().Name);
#endif
        }

        #endregion

        #region Error Handling

        protected virtual bool HasNoException(IDataPortalResult result)
        {
            bool returnValue = (result.Error == null);

            if (!returnValue)
            {
                //TODO: log errors
                MessageBox.Show(result.Error.Message, "Rolodex", MessageBoxButton.OK);
            }
            return returnValue;
        }

        #endregion

        #region Bases Methods

        public virtual void Activated()
        {

        }

        public virtual void Initialize()
        {

        }

        public virtual void Initialize(object parameter)
        {

        }

        #endregion



        public void Cleanup()
        {

        }
    }
}
