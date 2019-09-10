using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Csla.Configuration;

namespace WpfUI
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);

      CslaConfiguration.Configure()
        .DataPortal().DefaultProxy(typeof(Csla.DataPortalClient.HttpProxy), "https://localhost:44332/api/dataportal");
    }
  }
}
