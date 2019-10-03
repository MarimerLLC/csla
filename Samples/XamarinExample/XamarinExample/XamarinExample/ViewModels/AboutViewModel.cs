using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace XamarinExample.ViewModels
{
  public class AboutViewModel 
  {
    public string Title { get; set; }

    public AboutViewModel()
    {
      Title = "About";

      OpenWebCommand = 
        new Command(() => Device.OpenUri(new Uri("https://xamarin.com/platform")));
    }

    public ICommand OpenWebCommand { get; }
  }
}