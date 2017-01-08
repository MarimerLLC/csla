using System;
using System.Windows;
using Microsoft.Practices.Composite.UnityExtensions;
using Microsoft.Practices.Composite.Modularity;
using Rolodex.Silverlight.Modules;



namespace Rolodex.Silverlight
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            Shell shell = Container.Resolve<Shell>();
#if SILVERLIGHT
            Application.Current.RootVisual = shell;
#else
            shell.Show();
#endif
            return shell;
        }

        protected override IModuleCatalog GetModuleCatalog()
        {
            ModuleCatalog catalog = new ModuleCatalog()
                .AddModule(typeof(LoginModule))
                .AddModule(typeof(PleaseWaitModule))
                .AddModule(typeof(CompaniesListModule));

            return catalog;
        }
    }
}
