using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Rolodex.Silverlight.Services;

namespace Rolodex.Silverlight.Modules
{
    public abstract class RolodexModule : IModule
    {
        #region Priave Members

        private readonly IRegionManager _regionManager;
        private readonly IEventAggregator _eventAggregator;
        private readonly IUnityContainer _unityContainer;
        private readonly IModuleManager _moduleManager;
        private readonly IModuleCatalog _moduleCatalog;
        private static IRolodexServiceLoader _serviceLoader;
        private static object _lock = new object();

        #endregion

        #region Constructor

        public RolodexModule(
                   IRegionManager regionManager,
                   IEventAggregator eventAggregator,
                   IUnityContainer unityContainer,
                   IModuleManager moduleManager,
                   IModuleCatalog moduleCatalog)
        {
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _unityContainer = unityContainer;
            _moduleManager = moduleManager;
            _moduleCatalog = moduleCatalog;
            lock (_lock)
            {
                if (_serviceLoader == null)
                {
                    _serviceLoader = UnityContainer.Resolve<IRolodexServiceLoader>();
                }
            }
        }

        #endregion

        #region Properties

        protected IEventAggregator EventAggregator { get { return _eventAggregator; } }
        protected IRegionManager RegionManager { get { return _regionManager; } }
        protected IUnityContainer UnityContainer { get { return _unityContainer; } }
        protected IModuleManager ModuleManager { get { return _moduleManager; } }
        protected IModuleCatalog ModuleCatalog { get { return _moduleCatalog; } }
        protected IRolodexServiceLoader ServiceLoader { get { return _serviceLoader; } }

        #endregion

        #region Initialization

        public virtual void Initialize()
        {
            RegisterViews();
            SubscribeToEvents();
        }


        protected virtual void RegisterViews()
        {

        }

        protected virtual void SubscribeToEvents()
        {

        }

        #endregion

        #region Methods


        #endregion
    }
}
