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
    public static Page GetMainPage()
    {
      var page = new MainPage();

      MainViewModel = new MainViewModel();
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
