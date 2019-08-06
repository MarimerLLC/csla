using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using Csla.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.ValidationRules
{


  /// <summary>
  ///This is a test class for RootTest and is intended
  ///to contain all RootTest Unit Tests
  ///</summary>
  [TestClass()]
  public class RuleBaseClassesTests
  {


    private TestContext testContextInstance;

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext
    {
      get
      {
        return testContextInstance;
      }
      set
      {
        testContextInstance = value;
      }
    }

    [TestMethod()]
    public void LookupRuleDefaultCanXYZValues()
    {
      
      var rule = new LookupCustomer(RuleBaseClassesRoot.CustomerIdProperty, RuleBaseClassesRoot.NameProperty);
      Assert.IsTrue(rule.IsAsync);
      Assert.IsFalse(rule.CanRunOnServer);
      Assert.IsFalse(rule.CanRunInCheckRules);
      Assert.IsFalse(rule.CanRunAsAffectedProperty);
    }


    [TestMethod()]
    public void PropertyRuleDefaultCanXYZVaules()
    {
      var rule = new LessThan(RuleBaseClassesRoot.StartDateProperty, RuleBaseClassesRoot.EndDateProperty);
      Assert.IsFalse(rule.IsAsync);
      Assert.IsTrue(rule.CanRunOnServer);
      Assert.IsTrue(rule.CanRunInCheckRules);
      Assert.IsTrue(rule.CanRunAsAffectedProperty);
    }

    [TestMethod()]
    public void PropertyEditRuleDefaultCanXYZVaules()
    {
      var rule = new CalcSum(RuleBaseClassesRoot.NameProperty);
      Assert.IsFalse(rule.IsAsync);
      Assert.IsFalse(rule.CanRunOnServer);
      Assert.IsTrue(rule.CanRunInCheckRules);
      Assert.IsTrue(rule.CanRunAsAffectedProperty);
    }

    [TestMethod()]
    [ExpectedException(typeof(ArgumentException))]
    public void ObjectRuleThrowsExceptionIfPrimareyPropertyIsSet()
    {
      var rule = new ValidateRootObject();
      rule.PrimaryProperty = RuleBaseClassesRoot.NameProperty;
    }
    /// <summary>
    ///A test for NewEditableRoot
    ///</summary>
    [TestMethod()]
    
    public void LessThanSetsErrorOnBothFields()
    {
      // StartDate less than 
      string ruleSet = "Date";
      string err1, err2;
      var actual = RuleBaseClassesRoot.NewEditableRoot(ruleSet);

      Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
      actual.StartDate = "today";
      actual.EndDate = "yesterday";

      err1 = ((IDataErrorInfo)actual)[RuleBaseClassesRoot.StartDateProperty.Name];
      err2 = ((IDataErrorInfo)actual)[RuleBaseClassesRoot.EndDateProperty.Name];

      Assert.IsFalse(actual.IsSelfValid);   // object has broken rule 
      Assert.IsTrue(err1.Length > 1);       // both fields have error message
      Assert.IsTrue(err2.Length > 1);

      actual.EndDate = "tomorrow";
      err1 = ((IDataErrorInfo)actual)[RuleBaseClassesRoot.StartDateProperty.Name];
      err2 = ((IDataErrorInfo)actual)[RuleBaseClassesRoot.EndDateProperty.Name];

      Assert.IsTrue(actual.IsSelfValid);     // object has no broken rules
      Assert.AreEqual(string.Empty, err1);   // both fields are now OK
      Assert.AreEqual(string.Empty, err2);
    }

    [TestMethod()]
    public void AsyncLookupDoNotRunServerSide()
    {
      string ruleSet = "Lookup";
      var actual = RuleBaseClassesRoot.NewEditableRoot(ruleSet);

      // rule did not run on serverside (DAL) and no value is explicitly set 
      Assert.AreEqual(string.Empty, actual.Name);
    }

    [TestMethod()]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void AsyncLookupCustomerSetsCustomerName()
    {
      string ruleSet = "Lookup";
      var actual = RuleBaseClassesRoot.NewEditableRoot(ruleSet);

      var waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
      actual.ValidationComplete += (o, e) => waitHandle.Set();
      waitHandle.Reset();
      actual.CustomerId = 1;
      waitHandle.WaitOne();  // wait for async lookup to complete
      Assert.AreEqual("Rocky Lhotka", actual.Name);

      waitHandle.Reset();
      actual.CustomerId = 2;
      waitHandle.WaitOne();
      Assert.AreEqual("Customer_2", actual.Name);
    }


    [TestMethod()]
    public void NameRequiredIsBrokenOnNewRoot()
    {
      // test that Name has broken rule on new object
      string ruleSet = "LookupAndNameRequired";
      string err1;
      var actual = RuleBaseClassesRoot.NewEditableRoot(ruleSet);
      err1 = ((IDataErrorInfo)actual)[RuleBaseClassesRoot.NameProperty.Name];


      Assert.AreEqual(string.Empty, actual.Name);  // name is not set 
      Assert.IsFalse(actual.IsSelfValid);         // object has broken rules 
      Assert.IsTrue(err1.Length > 0);             // name has broken rule with message
    }



    [TestMethod()]
    [TestCategory("SkipWhenLiveUnitTesting")]
    public void NameRequiredIsNotBrokenAfterLookupCustomer()
    {
      // test that Name is revalidated as it is an affected property of LookupCustomer rule 
      // that runs when CustomerId is set.
      string ruleSet = "LookupAndNameRequired";
      string err1;
      var actual = RuleBaseClassesRoot.NewEditableRoot(ruleSet);
      err1 = ((IDataErrorInfo)actual)[RuleBaseClassesRoot.NameProperty.Name];

      Assert.IsFalse(actual.IsSelfValid);   // is broken before we set customerid

      var waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
      actual.ValidationComplete += (o, e) => waitHandle.Set();
      waitHandle.Reset();
      actual.CustomerId = 1;
      waitHandle.WaitOne();  // wait for async lookup to complete

      Assert.IsTrue(actual.IsSelfValid);    // is valid after 
    }

    [TestMethod()]
    public void NewObjectWithObjectRulesIsValid()
    {
      string ruleSet = "Object";
      var actual = RuleBaseClassesRoot.NewEditableRoot(ruleSet);
      Assert.IsTrue(actual.IsSelfValid);
    }

    [TestMethod()]
    public void NewObjectWithObjectRulesHas3ErrorForCustomerId4()
    {
      string ruleSet = "Object";
      var actual = RuleBaseClassesRoot.NewEditableRoot(ruleSet);

      actual.CustomerId = 4;
      Assert.IsFalse(actual.IsValid);
      Assert.AreEqual(3, actual.BrokenRulesCollection.Where(p => p.Severity == RuleSeverity.Error).Count());
    }

    [TestMethod()]
    public void NewObjectWithObjectRulesHas3WarningsForCustomerId5()
    {
      string ruleSet = "Object";
      var actual = RuleBaseClassesRoot.NewEditableRoot(ruleSet);

      actual.CustomerId = 5;
      Assert.IsTrue(actual.IsValid);
      Assert.AreEqual(3, actual.BrokenRulesCollection.Where(p => p.Severity == RuleSeverity.Warning).Count());
    }

    [TestMethod()]
    public void NewObjectWithObjectRulesHas3InfosForCustomerId6()
    {
      string ruleSet = "Object";
      var actual = RuleBaseClassesRoot.NewEditableRoot(ruleSet);

      actual.CustomerId = 6;
      Assert.IsTrue(actual.IsValid);
      Assert.AreEqual(3, actual.BrokenRulesCollection.Where(p => p.Severity == RuleSeverity.Information).Count());
    }


    [TestMethod()]
    public void MessageDelegateAndResources()
    {
      string ruleSet = "Required";
      var actual = RuleBaseClassesRoot.NewEditableRoot(ruleSet);
      var culture = Thread.CurrentThread.CurrentCulture;

      Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
      Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("en-US");
      actual.Name = "xyz";
      actual.Name = "";
      Assert.AreEqual("Name required", actual.BrokenRulesCollection[0].Description);
      Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("nb-NO");
      Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("nb-NO");
      actual.Name = "xyz";
      actual.Name = "";
      Assert.AreEqual("Name påkrevd", actual.BrokenRulesCollection[0].Description);
      Thread.CurrentThread.CurrentCulture = culture;
      Thread.CurrentThread.CurrentUICulture = culture;
    }
  }
}
