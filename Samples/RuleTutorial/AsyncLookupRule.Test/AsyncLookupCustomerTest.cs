// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AsyncLookupCustomerTest.cs" company="Marimer LLC">
//   Copyright (c) Marimer LLC. All rights reserved. Website: http://www.lhotka.net/cslanet
// </copyright>
//  <summary>
//   Unit tests foa AsyncLookupCustomer rule.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
using System.Collections.Generic;
using System.Data;
using AsyncLookupRule.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Csla.Core;
using Csla.Rules;
using RuleTutorial.Testing.Common;

namespace AsyncLookupRule.Test
{
  /// <summary>
  ///This is a test class for AsyncLookupCustomerTest and is intended
  ///to contain all AsyncLookupCustomerTest Unit Tests
  ///</summary>
  public class AsyncLookupCustomerTest 
  {
    [TestClass()]
    public class TheCtor
    {
      private IBusinessRule Rule;
      [TestInitialize]
      public void InitTests()
      {
        Rule = new AsyncLookupCustomer(RootFake.CustomerIdProperty, RootFake.NameProperty);
      }

      [TestMethod]
      public void AsyncLookupCustomer_MustBeAsync()
      {
        Assert.IsTrue(Rule.IsAsync);
      }

      [TestMethod]
      public void AsyncLookupCustomer_MustHavePrimaryProperty()
      {
        Assert.IsNotNull(Rule.PrimaryProperty);
        Assert.AreEqual(RootFake.CustomerIdProperty, Rule.PrimaryProperty);
      }


      [TestMethod]
      public void AsyncLookupCustomer_MustHaveCustomerIdInInputProperties()
      {
        Assert.IsTrue(Rule.InputProperties.Contains(RootFake.CustomerIdProperty));
      }

      [TestMethod]
      public void AsyncLookupCustomer_MustHaveNameInAffectedProperties()
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
        var rule = new AsyncLookupCustomer(RootFake.CustomerIdProperty, RootFake.NameProperty);
        InitializeTest(rule, null);
      }

      [TestMethod]
      public void AsyncLookupCustomer_ExecuteMustSetNameInOutput()
      {
        // run rule with supplied InputProperties 
        ExecuteRule(new Dictionary<IPropertyInfo, object>() {{RootFake.CustomerIdProperty, 21164}});

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
        var rule = new AsyncLookupCustomer(RootFake.CustomerIdProperty, RootFake.NameProperty);
        InitializeTest(rule, _myBO);
      }

      [TestMethod]
      public void AsyncLookupCustomer_ExecuteMustSetNameInOutputAlt()
      {
        // load values into BO
        LoadProperty(_myBO, RootFake.CustomerIdProperty, 21164);
        ExecuteRule(); // will add values into InputPropertyValues in RuleContext
        Assert.IsTrue(RuleContext.OutputPropertyValues.ContainsKey(RootFake.NameProperty));
      }
    }
  }
}
