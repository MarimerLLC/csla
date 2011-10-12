using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using System.Windows;
using Rolodex.Silverlight.Regions;
using Rolodex.Silverlight.ViewModels;
using Rolodex.Silverlight.Views;
using Rolodex.Silverlight.Events;

namespace Rolodex.Silverlight.Services
{
    public class RolodexServiceLoader : IRolodexServiceLoader
    {
        private Dictionary<RolodexService, ServiceDefinition> services = new Dictionary<RolodexService, ServiceDefinition>();
        private readonly IRegionManager regionManager;
        private readonly IUnityContainer unityContainer;
        private readonly IEventAggregator eventAggregator;

        public RolodexServiceLoader(IRegionManager regionManager, IUnityContainer unityContainer, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.unityContainer = unityContainer;
            this.eventAggregator = eventAggregator;
            SubscribeToEvents();
        }

        private void SubscribeToEvents()
        {
            eventAggregator.GetEvent<LoginCompleted>().Subscribe(OnLogin);
            eventAggregator.GetEvent<ServiceSelected>().Subscribe(OnMenuSelected);
            eventAggregator.GetEvent<CloseRequested>().Subscribe(OnCloseRequested);
        }

        public void OnCloseRequested(IRolodexViewModel viewModel)
        {
            var rolodexView = (from oneRegion in regionManager.Regions
                                from oneView in oneRegion.Views
                                where
                                    oneView is IRolodexView &&
                                    ReferenceEquals(((IRolodexView) oneView).DataContext, viewModel)
                                select new {View = oneView, Region = oneRegion}).FirstOrDefault();
            if (rolodexView != null)
            {
                rolodexView.Region.Remove(rolodexView.View);
            }
        }

        public void OnMenuSelected(ServiceEventArgs args)
        {
            LoadService(args.Service, args.RegionName, args.Parameter);
        }

        public void OnLogin(bool success)
        {
            if (success)
            {
                var rolodexViews = (from oneRegion in regionManager.Regions
                                    from oneView in oneRegion.Views
                                    where oneView is IRolodexView
                                    select oneView).ToList();
                foreach (var rolodexView in rolodexViews)
                {
                    regionManager.RemoveView(rolodexView);
                }
                
                LoadService(RolodexService.Menu, Constants.MenuRegion);
            }
        }

        public void OnLogout(object payload)
        {

            var rolodexViews = (from oneRegion in regionManager.Regions
                                from oneView in oneRegion.Views
                                where oneView is IRolodexView
                                select oneView).ToList();

            if ((from IRolodexView view in rolodexViews where view.IsDirty select view).Any())
            {
                MessageBox.Show("You have pending changes", "Rolodex", MessageBoxButton.OK);
            }
            else
            {

                var nonRolodexViews = (from oneRegion in regionManager.Regions
                                       from oneView in oneRegion.Views
                                       where !(oneView is IRolodexView)
                                       select new { View = oneView, Region = oneRegion }).ToList();

                foreach (IRolodexView item in rolodexViews)
                {
                    if (item.DataContext != null && item.DataContext is IRolodexViewModel)
                    {
                        (item.DataContext as IRolodexViewModel).Cleanup();
                    }
                }
                foreach (var item in nonRolodexViews)
                {
                    item.Region.Remove(item.View);
                }
                LoadService(RolodexService.Login);
            }

        }

        public void RegisterService<TViewInterface, TView, TViewModelInterface, TViewModel>(RolodexService rolodexService)
            where TViewInterface : IRolodexView
            where TView : TViewInterface
            where TViewModelInterface : IRolodexViewModel
            where TViewModel : TViewModelInterface
        {
            services.Add(rolodexService, new ServiceDefinition(
                typeof(TViewInterface), typeof(TView), typeof(TViewModelInterface), typeof(TViewModel)));
            unityContainer.RegisterType(typeof(TViewInterface), typeof(TView));
            unityContainer.RegisterType(typeof(TViewModelInterface), typeof(TViewModel));
        }

        public void LoadService(RolodexService rolodexService)
        {
            LoadService(rolodexService, Constants.MainRegion, null);
        }

        public void LoadService(RolodexService rolodexService, object parameter)
        {
            LoadService(rolodexService, Constants.MainRegion, parameter);
        }

        public void LoadService(RolodexService rolodexService, string regionName)
        {
            LoadService(rolodexService, regionName, null);
        }


        public void LoadService(RolodexService rolodexService, string regionName, object parameter)
        {
            if (string.IsNullOrEmpty(regionName))
            {
                regionName = Constants.MainRegion;
            }
            ServiceDefinition serviceDefinition = services[rolodexService];
            if (!regionManager.ActivateViewIfExists(regionName, serviceDefinition.ViewInterfaceType))
            {
                regionManager.DeactivateViews(regionName);
                var viewModel = (IRolodexViewModel)unityContainer.Resolve(serviceDefinition.ViewModelInterfaceType, null);
                var view = (IRolodexView)unityContainer.Resolve(serviceDefinition.ViewInterfaceType, null);
                regionManager.AddViewToRegion(regionName, view);
                view.DataContext = viewModel;
                regionManager.ActivateViewIfExists(regionName, serviceDefinition.ViewInterfaceType);
                if (parameter == null)
                {
                    viewModel.Initialize();
                }
                else
                {
                    viewModel.Initialize(parameter);
                }
               
            }
        }


    }
}
