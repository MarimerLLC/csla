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
  [TestClass()]
  public class LookupCustomerTest : BusinessRuleTest
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
    public void Rule_MustBeSync()
    {
      Assert.IsFalse(Rule.IsAsync);
    }

    [TestMethod]
    public void Rule_MustHaveCustomerIdAsPrimaryProperty()
    {
      Assert.IsNotNull(Rule.PrimaryProperty);
      Assert.AreEqual(Root.CustomerIdProperty, Rule.PrimaryProperty);
    }


    [TestMethod]
    public void Rule_MustHaveInputProperties_CustomerNumber()
    {
      Assert.IsTrue(Rule.InputProperties.Contains(Root.CustomerIdProperty));
    }

    [TestMethod]
    public void Rule_MustHaveAffectedProperties_Name()
    {
      Assert.IsTrue(Rule.AffectedProperties.Contains(Root.NameProperty));
    }

    [TestMethod]
    public void Execute_MustSetOutputProperty()
    {
      // load values into BO
      LoadProperty(_myBO, Root.CustomerIdProperty, 21164);

      ExecuteRule();   // will add values into InputPropertyValues in RuleContext

      Assert.IsTrue(RuleContext.OutputPropertyValues.ContainsKey(Root.NameProperty));
      Assert.AreEqual("Name (21164)", RuleContext.OutputPropertyValues[Root.NameProperty]);
    }

    [TestMethod]
    public void Execute_MustSetOutputProperty2()
    {
      // run rule with supplied InputProperties 
      ExecuteRule(new Dictionary<IPropertyInfo, object>() { { Root.CustomerIdProperty, 21164 } });

      Assert.IsTrue(RuleContext.OutputPropertyValues.ContainsKey(Root.NameProperty));
      Assert.AreEqual("Name (21164)", RuleContext.OutputPropertyValues[Root.NameProperty]);

      // in the samme manner I  could also test for
      //Assert.IsTrue(
      //    RuleContext.Results.Any(p => p.PrimaryProperty == Root.NameProperty && p.Severity == RuleSeverity.Error));
    }

  }
}
