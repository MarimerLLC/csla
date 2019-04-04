//-----------------------------------------------------------------------
// <copyright file="XamlPropertyInfoTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using Csla;
using Csla.Serialization;
#if !NUNIT
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Data;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif

namespace Csla.Test.PropertyInfo
{
  [TestClass]
  public class XamlPropertyInfoTests
  {
    [TestMethod]
    public void NewObject()
    {
      var obj = new TestRoot();

      var pi = new Csla.Xaml.PropertyInfo(true);
      pi.DataContext = obj;
      var binding = new System.Windows.Data.Binding("Name");
      binding.Source = obj;
      binding.Mode = System.Windows.Data.BindingMode.OneWay;
      pi.SetBinding(Csla.Xaml.PropertyInfo.PropertyProperty, binding);

      Assert.IsTrue(obj.IsValid, "object valid");
      Assert.IsTrue(pi.IsValid, "pi valid");

      obj.CheckRules();
      Assert.IsFalse(obj.IsValid, "object invalid");
      Assert.IsFalse(pi.IsValid, "pi invalid");

      obj.Name = "Rocky";
      Assert.IsTrue(obj.IsValid, "object valid after set");
      Assert.IsTrue(pi.IsValid, "pi valid after set");
    }
  }

  [Serializable]
  public class TestRoot : BusinessBase<TestRoot>
  {
    public static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(NameProperty));
    }

    public void CheckRules()
    {
      BusinessRules.CheckRules();
      OnUnknownPropertyChanged();
    }
  }
}
