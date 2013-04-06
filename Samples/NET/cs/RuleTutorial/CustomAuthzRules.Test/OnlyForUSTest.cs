using CustomAuthzRules.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Csla.Rules;
using RuleTutorial.Testing.Common;

namespace CustomAuthzRules.Test
{
    
    
    /// <summary>
    ///This is a test class for OnlyForUSTest and is intended
    ///to contain all OnlyForUSTest Unit Tests
    ///</summary>
  public class OnlyForUSTest 
  {
    [TestClass()]
    public class TheExecuteMethod : AuthorizationRuleTest
    {
      private TestRoot Root { get; set; }

      [TestInitialize]
      public void Setup()
      {
        Root = new TestRoot();
        var rule = new OnlyForUS(AuthorizationActions.WriteProperty, TestRoot.NameProperty, TestRoot.CountryProperty);
        InitializeTest(rule, Root, typeof(TestRoot));
      }

      [TestMethod]
      public void MustReturnFalseWhenCountryIsNotUS()
      {
        Root.Country = "NO";
        ExecuteRule();
        Assert.IsFalse(AuthorizationContext.HasPermission);
      }

      [TestMethod]
      public void MustReturnTrueWhenCountryIsUS()
      {
        Root.Country = "US";
        ExecuteRule();
        Assert.IsTrue(AuthorizationContext.HasPermission);
      }
    }


  }
}
