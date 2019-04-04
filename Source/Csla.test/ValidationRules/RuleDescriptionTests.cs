//-----------------------------------------------------------------------
// <copyright file="RuleDescriptionTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using UnitDriven;
using Csla.Serialization;

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

    [TestMethod]
    public void BasicParsing()
    {
      var uri = new Csla.Rules.RuleUri("rule://type/property");
      Assert.AreEqual("type", uri.RuleTypeName, "Rule type");
      Assert.AreEqual("property", uri.PropertyName, "Property name");

      uri = new Csla.Rules.RuleUri("rule://type/property?p1=a");
      Assert.AreEqual("type", uri.RuleTypeName, "Rule type");
      Assert.AreEqual("property", uri.PropertyName, "Property name");
    }

    [TestMethod]
    public void NameParsing()
    {
      var uri = new Csla.Rules.RuleUri("type", "property");
      Assert.AreEqual("type", uri.RuleTypeName, "Rule type");
      Assert.AreEqual("property", uri.PropertyName, "Property name");
    }

    [TestMethod]
    public void SpecialCharacterParsing()
    {
      var uri = new Csla.Rules.RuleUri("A+ []`,=%Ä", "P+ []`,=%Ä");
      Assert.AreEqual("a-----25-c3-84", uri.RuleTypeName, "Rule type");
      Assert.AreEqual("P-----25-C3-84", uri.PropertyName, "Property name");
    }

    [TestMethod]
    public void LongTypeName()
    {
      var hostName = "abcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghijabcdefghij";

      var uri = new Csla.Rules.RuleUri(hostName, "property");
      Assert.AreEqual(hostName.Replace("/", ""), uri.RuleTypeName, "Rule type");
      Assert.AreEqual("property", uri.PropertyName, "Property name");
    }

    [TestMethod]
    public void Arguments()
    {
      var uri = new Csla.Rules.RuleUri("rule://type/property?p1=v1");
      Assert.AreEqual(1, uri.Arguments.Count, "Count should be 1");
      Assert.AreEqual("v1", uri.Arguments["p1"], "Value shoudl be v1");

      uri = new Csla.Rules.RuleUri("rule://type/property?p1=v1&p2=v2");
      Assert.AreEqual(2, uri.Arguments.Count, "Count should be 2");
      Assert.AreEqual("v1", uri.Arguments["p1"], "Value shoudl be v1");
      Assert.AreEqual("v2", uri.Arguments["p2"], "Value shoudl be v2");
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
    protected override void Execute(Rules.IRuleContext context)
    {
      base.Execute(context);
    }
  }
}