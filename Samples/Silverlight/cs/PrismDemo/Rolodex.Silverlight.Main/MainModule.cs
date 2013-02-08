using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Rolodex.Silverlight.Modules;
using Microsoft.Practices.Unity;
using Rolodex.Silverlight.Services;
using Rolodex.Silverlight.Main.Views;
using Rolodex.Silverlight.Main.ViewModels;

namespace Rolodex.Silverlight.Main
{
    public class MainModule : RolodexModule
    {
        public MainModule(
            IRegionManager regionManager, 
            IEventAggregator eventAggregator, 
            IUnityContainer unityContainer, 
            IModuleManager moduleManager, 
            IModuleCatalog moduleCatalog) :
            base(regionManager, eventAggregator, unityContainer, moduleManager, moduleCatalog)
        {
        }

        protected override void RegisterViews()
        {
            base.RegisterViews();
            ServiceLoader.RegisterService<IMenuView, MenuView, IMenuViewModel, MenuViewModel>(RolodexService.Menu);
            ServiceLoader.RegisterService<ILoginView, LoginView, ILoginViewModel, LoginViewModel>(RolodexService.Login);
            ServiceLoader.RegisterService<IStatusesView, StatusesView, IStatusesViewModel, StatusesViewModel>(RolodexService.Statuses);
            ServiceLoader.RegisterService<ICompaniesView, CompaniesView, ICompaniesViewModel, CompaniesViewModel>(RolodexService.Companies);
            ServiceLoader.RegisterService<ICompanyEditView, CompanyEditView, ICompanyEditViewModel, CompanyEditViewModel>(RolodexService.CompanyEdit);
            ServiceLoader.LoadService(RolodexService.Login);
        }
    }
}
