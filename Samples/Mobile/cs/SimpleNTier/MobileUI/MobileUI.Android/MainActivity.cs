using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using Xamarin.Forms.Platform.Android;

namespace MobileUI.Droid
{
  [Activity(Label = "MobileUI", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
  public class MainActivity : AndroidActivity
  {
    protected override async void OnCreate(Bundle bundle)
    {
      base.OnCreate(bundle);

      Xamarin.Forms.Forms.Init(this, bundle);

      SetPage(App.GetMainPage());

      await App.InitializeViewModel();
    }
  }
}

