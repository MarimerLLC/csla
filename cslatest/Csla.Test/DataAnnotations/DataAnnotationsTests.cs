using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using System.ComponentModel.DataAnnotations;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;

#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif

namespace Csla.Test.DataAnnotations
{
  [TestClass]
  public class DataAnnotationsTests
  {
    [TestMethod]
    public void SingleAttribute()
    {
      var root = Csla.DataPortal.Create<Single>();
      var rules = root.GetRules();

      Assert.AreEqual(1, rules.Length, "Should be 1 rule");
      Assert.IsFalse(root.IsValid, "Obj shouldn't be valid");
      Assert.AreEqual(1, root.BrokenRulesCollection.Count, "Should be 1 broken rule");
      Assert.AreEqual("Name value required", root.BrokenRulesCollection[0].Description, "Desc should match");
    }

    [TestMethod]
    public void MultipleAttributes()
    {
      var root = Csla.DataPortal.Create<Multiple>();
      var rules = root.GetRules();

      Assert.AreEqual(3, rules.Length, "Should be 3 rules");
      Assert.IsFalse(root.IsValid, "Obj shouldn't be valid");
      Assert.AreEqual(1, root.BrokenRulesCollection.Count, "Should be 1 broken rule");
      root.Name = "xyz";
      Assert.AreEqual(2, root.BrokenRulesCollection.Count, "Should be 2 broken rules after edit");
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
      return ValidationRules.GetRuleDescriptions();
    }
  }

  [Serializable]
  public class Multiple : BusinessBase<Multiple>
  {
    private static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    [Required(ErrorMessage = "Name value required")]
    [RegularExpression("[0-9]")]
    [Range(typeof(string), "0", "9")]
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public string[] GetRules()
    {
      return ValidationRules.GetRuleDescriptions();
    }
  }
}
