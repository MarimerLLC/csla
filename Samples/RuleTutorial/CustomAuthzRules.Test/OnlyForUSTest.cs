using CustomAuthzRules.Rules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Csla.Rules;
using RuleTutorial.Testing.Common;
using Csla;

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
        Root = DataPortal.Create<TestRoot>();
        var rule = new OnlyForUS(AuthorizationActions.WriteProperty, TestRoot.NameProperty, TestRoot.CountryProperty);
        InitializeTest(rule, Root, typeof(TestRoot));
      }

      [TestMethod]
      public void OnlyForUS_MustReturnFalse_WhenCountryIsNotUS()
      {
        Root.Country = "NO";
        ExecuteRule();
        Assert.IsFalse(AuthorizationContext.HasPermission);
      }

      [TestMethod]
      public void OnlyForUS_MustReturnTrue_WhenCountryIsUS()
      {
        Root.Country = "US";
        ExecuteRule();
        Assert.IsTrue(AuthorizationContext.HasPermission);
      }
    }


  }
}
