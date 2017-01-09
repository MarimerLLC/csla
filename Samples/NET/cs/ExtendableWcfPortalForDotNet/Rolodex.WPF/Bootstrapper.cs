using System.Windows;
using Microsoft.Practices.Composite.Modularity;
using Microsoft.Practices.Composite.UnityExtensions;
using Rolodex.Silverlight.Modules;

namespace Rolodex.Silverlight
{
  public class Bootstrapper : UnityBootstrapper
  {
    protected override DependencyObject CreateShell()
    {
      Shell shell = Container.Resolve<Shell>();
      shell.Show();

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