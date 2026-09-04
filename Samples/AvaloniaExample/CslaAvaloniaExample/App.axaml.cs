using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Csla.Configuration;
using CslaAvaloniaExample.ViewModels;
using CslaAvaloniaExample.Views;
using DataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace CslaAvaloniaExample;

public partial class App : Application
{
  public IServiceProvider Services { get; private set; } = null!;

  public override void Initialize()
  {
    AvaloniaXamlLoader.Load(this);
  }

  public override void OnFrameworkInitializationCompleted()
  {
    var services = new ServiceCollection();

    services.AddCsla(options => options
      .AddXaml());

    services.AddSingleton<IPersonDal, PersonDal>();

    services.AddTransient<PersonEditViewModel>();
    services.AddTransient<PersonListViewModel>();
    services.AddTransient<PersonEditPage>();
    services.AddTransient<PersonListPage>();
    services.AddTransient<MainWindow>();

    Services = services.BuildServiceProvider();

    // Important: force creation of the CSLA ApplicationContext.
    // This allows Csla.Xaml.ApplicationContextManager to capture it
    // for ViewModelBase and the other XAML helpers.
    _ = Services.GetRequiredService<Csla.ApplicationContext>();

    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
      desktop.MainWindow = Services.GetRequiredService<MainWindow>();

    base.OnFrameworkInitializationCompleted();
  }
}
