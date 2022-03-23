using System;
using Csla;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinExample.Views;

namespace XamarinExample
{
  public partial class App : Application
  {
    private IServiceProvider serviceProvider;

    public static ApplicationContext ApplicationContext { get; private set; }

    public App()
    {
      InitializeComponent();

      var services = new ServiceCollection();
      services.AddCsla();
      //services.AddCsla(o => o.RegisterContextManager<Csla.Xaml.ApplicationContextManager>());
      services.AddTransient(typeof(DataAccess.IPersonDal), typeof(DataAccess.PersonDal));
      serviceProvider = services.BuildServiceProvider();
      ApplicationContext = serviceProvider.GetRequiredService<ApplicationContext>();

      MainPage = new AppShell();
    }

    protected override void OnStart()
    {
      // Handle when your app starts
    }

    protected override void OnSleep()
    {
      // Handle when your app sleeps
    }

    protected override void OnResume()
    {
      // Handle when your app resumes
    }
  }
}
