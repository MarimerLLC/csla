using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MobileUI
{
  public class App
  {
    public static OrderVm MainViewModel { get; private set; }

    public static Page GetMainPage()
    {
      Csla.ApplicationContext.DataPortalProxy = typeof(Csla.DataPortalClient.WcfProxy).AssemblyQualifiedName;
      Csla.ApplicationContext.DataPortalUrlString = "http://simplentier.azurewebsites.net/SlPortal.svc";

      var page = new MainPage();

      MainViewModel = new OrderVm();
      page.BindingContext = MainViewModel;
      return new NavigationPage(page);
    }

    public static async Task InitializeViewModel()
    {
      try
      {
        await MainViewModel.LoadData();
      }
      catch (Exception ex)
      {
        var tmp = ex;
      }
    }
  }
}
