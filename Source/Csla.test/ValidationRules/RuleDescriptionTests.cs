//-----------------------------------------------------------------------
// <copyright file="RuleDescriptionTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
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
        var desc = new Csla.Rules.RuleUri(item);
        Assert.AreEqual("csla.test.validationrules.myrule", desc.RuleTypeName, "Wrong rule type name");
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
      BusinessRules.AddRule(new MyRule { PrimaryProperty = NameProperty });
    }

    public string[] Rules
    {
      get { return this.BusinessRules.GetRuleDescriptions(); }
    }
  }

  public class MyRule : Rules.BusinessRule
  {
    protected override void Execute(Rules.RuleContext context)
    {
      base.Execute(context);
    }
  }
}