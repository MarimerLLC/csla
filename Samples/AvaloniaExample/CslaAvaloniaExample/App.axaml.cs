using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Csla.Configuration;
using CslaAvaloniaExample.ViewModels;
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

    // Local data portal: deliberately simple for exercising the Avalonia UI layer.
    services.AddCsla();
    // Same DAL abstraction used by the Maui-style BusinessLibrary.
    services.AddSingleton<IPersonDal, PersonDal>();

    services.AddScoped<PersonEditViewModel>();
    services.AddScoped<PersonListViewModel>();
    
    Services = services.BuildServiceProvider();

    // Make CSLA's ApplicationContext available to code resolved through DI.
    // UI/view-model code can resolve IDataPortal<T> from Services.
    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
      desktop.MainWindow = new MainWindow();

    base.OnFrameworkInitializationCompleted();
  }
}
