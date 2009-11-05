using System;
using System.Collections.Generic;
using System.Text;
using UnitDriven;

#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#elif MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Csla.Test.ValidationRules
{
  [TestClass]
  public class RuleDescriptionTests
  {
    [TestMethod]
    public void CheckDescription()
    {
      var root = new RuleTestClass();
      foreach (var item in root.Rules)
      {
        var desc = new Csla.Validation.RuleDescription(item);
        Assert.AreEqual("MyRule", desc.MethodName, "Wrong method name");
      }
    }
  }

  [Serializable]
  public class RuleTestClass : BusinessBase<RuleTestClass>
  {
    private static PropertyInfo<string> NameProperty = RegisterProperty(new PropertyInfo<string>("Name", "Name"));
    public string Name
    {
      get { return GetProperty<string>(NameProperty); }
      set { SetProperty<string>(NameProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule(MyRule, NameProperty);
      ValidationRules.AddRule<RuleTestClass>(MyRule, NameProperty);
    }

    public static bool MyRule(object target, Csla.Validation.RuleArgs e)
    {
      e.Description = "My rule broken";
      return false;
    }

    public string[] Rules
    {
      get { return this.ValidationRules.GetRuleDescriptions(); }
    }
  }
}
