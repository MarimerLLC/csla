using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using Csla;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PropertyStatus
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    public App()
    {
      var services = new ServiceCollection();
      services.AddCsla();
      var provider = services.BuildServiceProvider();
      ApplicationContext = provider.GetService<ApplicationContext>();
    }

    public static ApplicationContext ApplicationContext { get; private set; }
  }
}
