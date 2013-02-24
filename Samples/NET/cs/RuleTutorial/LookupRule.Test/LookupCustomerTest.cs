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
using LookupRule.Test;
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
    public class TheCtor
    {
      private IBusinessRule Rule;
      [TestInitialize]
      public void InitTests()
      {
        Rule = new LookupCustomer(RootFake.CustomerIdProperty, RootFake.NameProperty);
      }

      [TestMethod]
      public void LookupCustomer_MustBeAsync()
      {
        Assert.IsFalse(Rule.IsAsync);
      }

      [TestMethod]
      public void LookupCustomer_MustHavePrimaryProperty()
      {
        Assert.IsNotNull(Rule.PrimaryProperty);
        Assert.AreEqual(RootFake.CustomerIdProperty, Rule.PrimaryProperty);
      }

      [TestMethod]
      public void LookupCustomer_MustHaveCustomerIdInInputProperties()
      {
        Assert.IsTrue(Rule.InputProperties.Contains(RootFake.CustomerIdProperty));
      }

      [TestMethod]
      public void LookupCustomer_MustHaveNameInAffectedProperties()
      {
        Assert.IsTrue(Rule.AffectedProperties.Contains(RootFake.NameProperty));
      }
    }

    [TestClass()]
    public class TheExecuteMethod : BusinessRuleTest
    {
      [TestInitialize]
      public void InitTests()
      {
        var rule = new LookupCustomer(RootFake.CustomerIdProperty, RootFake.NameProperty);
        InitializeTest(rule, null);
      }

      [TestMethod]
      public void LookupCustomer_ExecuteMustSetNameInOutput()
      {
        // run rule with supplied InputProperties 
        ExecuteRule(new Dictionary<IPropertyInfo, object>() { { RootFake.CustomerIdProperty, 21164 } });

        Assert.IsTrue(RuleContext.OutputPropertyValues.ContainsKey(RootFake.NameProperty));
      }
    }

    [TestClass()]
    public class TheExecuteMethodAlt : BusinessRuleTest
    {
      private RootFake _myBO;

      [TestInitialize]
      public void InitTests()
      {
        _myBO = new RootFake();
        var rule = new LookupCustomer(RootFake.CustomerIdProperty, RootFake.NameProperty);
        InitializeTest(rule, _myBO);
      }

      [TestMethod]
      public void LookupCustomer_ExecuteMustSetNameInOutputAlt()
      {
        // load values into BO
        LoadProperty(_myBO, RootFake.CustomerIdProperty, 21164);
        ExecuteRule(); // will add values into InputPropertyValues in RuleContext
        Assert.IsTrue(RuleContext.OutputPropertyValues.ContainsKey(RootFake.NameProperty));
      }
    }
  }
}
