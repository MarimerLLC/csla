using Csla.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ClientApp
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    public App()
    {
      CslaConfiguration.Configure()
        .DataPortal().DefaultProxy(typeof(CustomProxy), "");

      Csla.ApplicationContext.User = new Csla.Security.UnauthenticatedPrincipal();
    }
  }
}
