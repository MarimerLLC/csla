using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Csla;
using Csla.Core;

namespace Csla.Android.Test
{
  public class TestClass : ReadOnlyBase<TestClass>
  {

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public static readonly PropertyInfo<MobileList<string>> TestProperty = RegisterProperty<MobileList<string>>(c => c.TestProp);
    public static readonly PropertyInfo<string> ModelNumberProperty = RegisterProperty<string>(c => c.ModelNumber);


    public TestClass()
    {
      this.Name = string.Empty;
    }

    public string ModelNumber
    {
      get { return GetProperty<string>(ModelNumberProperty); }
      protected set { LoadProperty<string>(ModelNumberProperty, value); }
    }

    public string Name
    {
      get { return GetProperty<string>(NameProperty); }
      protected set { LoadProperty<string>(NameProperty, value); }
    }

    public MobileList<string> TestProp
    {
      get { return GetProperty<MobileList<string>>(TestProperty); }
      set { LoadProperty<MobileList<string>>(TestProperty, value); }
    }

    public bool IsInTest(string test)
    {
      var roles = ReadProperty<MobileList<string>>(TestProperty);
      if (roles != null)
        return roles.Contains(test);
      else
        return false;
    }
  }

}