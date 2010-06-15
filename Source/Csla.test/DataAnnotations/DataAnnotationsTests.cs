//-----------------------------------------------------------------------
// <copyright file="DataAnnotationsTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;
using UnitDriven;
using System.ComponentModel.DataAnnotations;

#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestSetup = NUnit.Framework.SetUpAttribute;
#elif MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Csla.Test.DataAnnotations
{
#if TESTING
  [System.Diagnostics.DebuggerNonUserCode]
#endif
  [TestClass]
  public class DataAnnotationsTests : TestBase
  {
#if SILVERLIGHT
    [TestInitialize]
    public void Setup()
    {
      Csla.DataPortal.ProxyTypeName = "Local";
    }
#endif

    [TestMethod]
    public void SingleAttribute()
    {
      var context = GetContext();

      var dp = new Csla.DataPortal<Single>();
      dp.CreateCompleted += (o, e) =>
        {
          var root = e.Object;
          var rules = root.GetRules();

          Assert.AreEqual(1, rules.Length, "Should be 1 rule");
          Assert.IsFalse(root.IsValid, "Obj shouldn't be valid");
          Assert.AreEqual(1, root.BrokenRulesCollection.Count, "Should be 1 broken rule");
          Assert.AreEqual("Name value required", root.BrokenRulesCollection[0].Description, "Desc should match");
          context.Assert.Success();
        };
      dp.BeginCreate();

      context.Complete();
    }

    [TestMethod]
    public void MultipleAttributes()
    {
      var context = GetContext();

      var dp = new Csla.DataPortal<Multiple>();
      dp.CreateCompleted += (o, e) =>
        {
          var root = e.Object;
          var rules = root.GetRules();

          Assert.AreEqual(3, rules.Length, "Should be 3 rules");
          Assert.IsFalse(root.IsValid, "Obj shouldn't be valid");
          Assert.AreEqual(1, root.BrokenRulesCollection.Count, "Should be 1 broken rule");
          root.Name = "xyz";
          Assert.AreEqual(2, root.BrokenRulesCollection.Count, "Should be 2 broken rules after edit");
          context.Assert.Success();
        };
      dp.BeginCreate();

      context.Complete();
    }

    [TestMethod]
    public void CustomAttribute()
    {
      var context = GetContext();

      var dp = new Csla.DataPortal<Custom>();
      dp.CreateCompleted += (o, e) =>
      {
        var root = e.Object;
        var rules = root.GetRules();

        Assert.AreEqual(1, rules.Length, "Should be 1 rule");
        Assert.IsFalse(root.IsValid, "Obj shouldn't be valid");
        Assert.AreEqual(1, root.BrokenRulesCollection.Count, "Should be 1 broken rule");
        Assert.AreEqual("Name must be abc", root.BrokenRulesCollection[0].Description, "Desc should match");
        context.Assert.Success();
      };
      dp.BeginCreate();

      context.Complete();
    }
  }

  [Serializable]
  public class Single : BusinessBase<Single>
  {
    private static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    [Required(ErrorMessage = "Name value required")]
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public string[] GetRules()
    {
      return BusinessRules.GetRuleDescriptions();
    }
  }

  [Serializable]
  public class Multiple : BusinessBase<Multiple>
  {
    private static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    [Required(ErrorMessage = "Name value required")]
    [RegularExpression("[0-9]")]
    [System.ComponentModel.DataAnnotations.Range(typeof(string), "0", "9")]
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public string[] GetRules()
    {
      return BusinessRules.GetRuleDescriptions();
    }
  }

  [Serializable]
  public class Custom : BusinessBase<Custom>
  {
    private static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    [TestRule]
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public string[] GetRules()
    {
      return BusinessRules.GetRuleDescriptions();
    }
  }

  public class TestRuleAttribute : ValidationAttribute
  {
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
      if (validationContext.ObjectInstance == null)
        return new ValidationResult("ObjectInstance is null");
      var obj = validationContext.ObjectInstance as Custom;
      if (obj == null)
        return new ValidationResult("ObjectInstance is not the Custom type");
      if (string.IsNullOrEmpty(obj.Name) || obj.Name != "abc")
        return new ValidationResult("Name must be abc");
      return null;
    }
  }
}