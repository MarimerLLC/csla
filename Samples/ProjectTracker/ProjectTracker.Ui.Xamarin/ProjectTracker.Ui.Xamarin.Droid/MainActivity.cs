using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using XamarinFormsUi;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Csla;

namespace ProjectTracker.Ui.Xamarin.Droid
{
  [Activity(Label = "ProjectTracker.Ui.Xamarin", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
  public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
  {
    protected override void OnCreate(Bundle bundle)
    {
      base.OnCreate(bundle);

      global::Xamarin.Forms.Forms.Init(this, bundle);
      LoadApplication(new App());
    }
  }

  [Serializable]
  public class foo : ReadOnlyBase<foo>
  {
    [Display(Name = "bar")]
    public static readonly PropertyInfo<string> MyPropertyProperty = RegisterProperty<string>(c => c.MyProperty);
    public string MyProperty
    {
      get { return GetProperty(MyPropertyProperty); }
      private set { LoadProperty(MyPropertyProperty, value); }
    }
  }


}

