using System;
using System.Windows;
using Microsoft.Practices.Composite.UnityExtensions;
using Microsoft.Practices.Composite.Modularity;
using Rolodex.Silverlight.Modules;



namespace Rolodex.Silverlight
{
    public class Bootstrapper : UnityBootstrapper
    {
        private static Bootstrapper _bootstrapper;

        private static T ResolveModule<T>() where T: IModule
        {
            return (T)_bootstrapper.Container.Resolve<T>();
        }

        protected override DependencyObject CreateShell()
        {
            _bootstrapper = this;
            Shell shell = Container.Resolve<Shell>();
#if SILVERLIGHT
            Application.Current.RootVisual = shell;
#else
            shell.Show();
#endif
            return shell;
        }


        protected override void InitializeModules()
        {
            IModule loginModule = Container.Resolve<LoginModule>();
            loginModule.Initialize();

            IModule pleaseWaitModule = Container.Resolve<PleaseWaitModule>();
            pleaseWaitModule.Initialize();
        }

        public static void ShowCompanyList()
        {
            IModule companyModule = _bootstrapper.Container.Resolve<CompaniesListModule>();
            companyModule.Initialize();
        }
    }
}
