using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

using UnitDriven;

namespace Csla.Android.Test
{
  [Activity(Label = "Csla.Android.Test", MainLauncher = true)]
  public class StartActivity : MainActivity
  {
    //int count = 1;
    //static readonly string[] Tests = new String[] {   
    //"Add Object Authorization Rules", "Test Auth Begin Edit Rules"};

    protected override void OnCreate(Bundle bundle)
    {

      base.OnCreate(bundle);

      //Intent myIntent = new Intent(this.BaseContext, (Java.Lang.Class)new UnitDriven.MainActivity().Class); 
      //myIntent.AddFlags(ActivityFlags.NewTask); 
      //StartActivity(myIntent);

      Csla.DataPortal.ProxyTypeName = "Local";

      TestClass a = new TestClass();

      ////Csla.ApplicationContext.User = new System.Security.Principal.GenericPrincipal(new System.Security.Principal.GenericIdentity(string.Empty), new string[] { });
      // Get our button from the layout resource,
      // and attach an event to it
      //btnAddObjectAuthorizationRules = FindViewById<Button>(Resource.Id.btnAddObjectAuthorizationRules);
      //lblAddObjectAuthorizationRules = FindViewById<TextView>(Resource.Id.lblAddObjectAuthorizationRules);

      //btnAddObjectAuthorizationRules.Click += delegate { CheckAddObjectAuthorizationRules(); };

      ////System.Security.Principal.GenericPrincipal dontErase = new System.Security.Principal.GenericPrincipal(new System.Security.Principal.GenericIdentity(string.Empty), new string[] { });
      ////Csla.Security.UnauthenticatedPrincipal _principal = new Csla.Security.UnauthenticatedPrincipal();

      LoadTestItems(System.Reflection.Assembly.GetExecutingAssembly());
    }

    //void ListClick(object sender, ItemEventArgs args)
    //{
    //  if (((TextView)args.View).Text == "Add Object Authorization Rules")
    //  {

    //    try
    //    {
    //      AuthTests myTests = new AuthTests();

    //      myTests.TestAuthCloneRules();
    //    }
    //    catch (Exception ex)
    //    {
    //      Assert.AreEqual("1", "2", ex.Message);
    //    }
    //    if (Assert.IsValid)
    //    {
    //      Toast.MakeText(Application, "Success", ToastLength.Short).Show();
    //    }
    //    else
    //    {
    //      Toast.MakeText(Application, Assert.ErrorMessage, ToastLength.Short).Show();
    //    }
    //  }
    //  else if (((TextView)args.View).Text == "Test Auth Begin Edit Rules")
    //  {
    //    try
    //    {
    //      AuthTests myTests = new AuthTests();

    //      myTests.TestAuthBeginEditRules();
    //    }
    //    catch (Exception ex)
    //    {
    //      Assert.AreEqual("1", "2", ex.Message);
    //    }
    //    if (Assert.IsValid)
    //    {
    //      Toast.MakeText(Application, "Success", ToastLength.Short).Show();
    //    }
    //    else
    //    {
    //      Toast.MakeText(Application, Assert.ErrorMessage, ToastLength.Short).Show();
    //    }
    //  }
    //}
  }
}

