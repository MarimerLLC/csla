using System.Windows;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;
using Rolodex.Silverlight.Services;

namespace Rolodex.Silverlight
{
    public class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            Shell shell = Container.Resolve<Shell>();
            Application.Current.RootVisual = shell;
            return shell;
        }

        protected override void InitializeModules()
        {
            base.InitializeModules();
            Container.RegisterType<IRolodexServiceLoader, RolodexServiceLoader>(new ContainerControlledLifetimeManager(), new InjectionMember[] { });
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();
            var catalog = (ModuleCatalog)ModuleCatalog;
            catalog.AddModule(new ModuleInfo("MainModule", "Rolodex.Silverlight.Main.MainModule, Rolodex.Silverlight.Main, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null") { Ref = "Rolodex.Silverlight.Main.xap" });
        }
    }
}
