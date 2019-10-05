using System;
using Csla.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XamarinExample.Views;

namespace XamarinExample
{
  public partial class App : Application
  {

    public App()
    {
      InitializeComponent();

      CslaConfiguration.Configure().
        ContextManager(new Csla.Xaml.ApplicationContextManager());
      var services = new ServiceCollection();
      services.AddCsla();
      services.AddTransient(typeof(DataAccess.IPersonDal), typeof(DataAccess.PersonDal));

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
