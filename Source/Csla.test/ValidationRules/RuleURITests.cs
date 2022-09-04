using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.Linq;

using Csla.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csla.TestHelpers;

namespace Csla.Test.ValidationRules
{
  public class AddressEdit : Csla.BusinessBase<AddressEdit>
  {
    
  }

  public class GenericRule<T> : ObjectRule
  {
    
  }


  [TestClass]
  public class RuleURITests
  {

    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestInitialize]
    public void Initialize()
    {
      TestResults.Reinitialise();
    }

    [TestMethod]
    public void RuleURIWithGenericObjectRule()
    {
      var rule = new GenericRule<AddressEdit>();
      Assert.AreEqual("rule://csla.test.validationrules.genericrule-csla.test.validationrules/.AddressEdit-/(object)", rule.RuleName);
    }

    [TestMethod]
    public void RuleURIWithNonGenericType()
    {
      var property = new PropertyInfo<string>("test1");
      var rule = new Csla.Rules.CommonRules.MaxLength(property, 30);
      Assert.AreEqual("rule://csla.rules.commonrules.maxlength/test1?max=30", rule.RuleName);
    }

    [TestMethod]
    public void RuleURIWithGenericType()
    {
      var property = new PropertyInfo<int>("test1");
      var rule = new Csla.Rules.CommonRules.MaxValue<int>(property, 30);
      Assert.AreEqual("rule://csla.rules.commonrules.maxvalue-system.int32-/test1?max=30", rule.RuleName);
    }

    [TestMethod]
    public void RuleURIWithObjectRule()
    {
      var rule = new Csla.Rules.CommonRules.Required(null);
      Assert.AreEqual("rule://csla.rules.commonrules.required/(object)", rule.RuleName);
    }

    [TestMethod]
    public void LambdaRuleExtensionsAddUniueURIs()
    {
      IDataPortal<HasLambdaRules> dataPortal = _testDIContext.CreateDataPortal<HasLambdaRules>();

      var root = HasLambdaRules.New(dataPortal);
      var ruleUris = root.GetRuleDescriptions();

      var distinctUris = ruleUris.Distinct().ToArray();
      // must have same length
      Assert.AreEqual(ruleUris.Length, distinctUris.Length);
    }

    [TestMethod]
    public void RuleWithCyrillicNameMustHaveValidURI()
    {
      var prop = new PropertyInfo<string>("Изилдр");
      var rule = new ИзилдрRule(prop);
      Assert.IsTrue(true);
    }
  }
}
