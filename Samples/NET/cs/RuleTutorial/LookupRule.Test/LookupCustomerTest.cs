// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LookupCustomerTest.cs" company="Marimer LLC">
//   Copyright (c) Marimer LLC. All rights reserved. Website: http://www.lhotka.net/cslanet
// </copyright>
//  <summary>
//   Unit tests fot LookupCustomer rule.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Linq;
using LookupRule;
using LookupRule.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Csla.Core;
using Csla.Rules;
using RuleTutorial.Testing.Common;

namespace LooukupRule.Test
{
  /// <summary>
  ///This is a test class for LookupCustomerTest and is intended
  ///to contain all LookupCustomerTest Unit Tests
  ///</summary>

  public class LookupCustomerTest 
  {
    [TestClass()]
    public class TheCtor : BusinessRuleTest
    {
      private Root _myBO;

      [TestInitialize]
      public void InitTests()
      {
        _myBO = new Root();
        var rule = new LookupCustomer(Root.CustomerIdProperty, Root.NameProperty);
        InitializeTest(rule, _myBO);
      }

      [TestMethod]
      public void IsSync()
      {
        Assert.IsFalse(Rule.IsAsync);
      }

      [TestMethod]
      public void HasPrimaryProperty()
      {
        Assert.IsNotNull(Rule.PrimaryProperty);
        Assert.AreEqual(Root.CustomerIdProperty, Rule.PrimaryProperty);
      }


      [TestMethod]
      public void HasInputProperties()
      {
        Assert.IsTrue(Rule.InputProperties.Contains(Root.CustomerIdProperty));
      }

      [TestMethod]
      public void HasAffectedProperties()
      {
        Assert.IsTrue(Rule.AffectedProperties.Contains(Root.NameProperty));
      }
    }


    [TestClass()]
    public class TheExecuteMethod : BusinessRuleTest
    {
      private Root _myBO;

      [TestInitialize]
      public void InitTests()
      {
        _myBO = new Root();
        var rule = new LookupCustomer(Root.CustomerIdProperty, Root.NameProperty);
        InitializeTest(rule, _myBO);
      }

      [TestMethod]
      public void MustSetOutputPropertiesWithObjectAsTarget()
      {
        // load values into BO
        LoadProperty(_myBO, Root.CustomerIdProperty, 21164);

        ExecuteRule(); // will add values from Target into InputPropertyValues in RuleContext

        Assert.IsTrue(RuleContext.OutputPropertyValues.ContainsKey(Root.NameProperty));
        //Assert.AreEqual("Name (21164)", RuleContext.OutputPropertyValues[Root.NameProperty]);
      }

      [TestMethod]
      public void MustSetOutputPropertiesTestWithExplicitInputProperties()
      {
        // run rule with supplied InputProperties (no need for Target object) 
        ExecuteRule(new Dictionary<IPropertyInfo, object>() {{Root.CustomerIdProperty, 21164}});

        Assert.IsTrue(RuleContext.OutputPropertyValues.ContainsKey(Root.NameProperty));
        //Assert.AreEqual("Name (21164)", RuleContext.OutputPropertyValues[Root.NameProperty]);

        // in the samme manner I  could also test for
        //Assert.IsTrue(
        //    RuleContext.Results.Any(p => p.PrimaryProperty == Root.NameProperty && p.Severity == RuleSeverity.Error));
      }
    }
  }
}
