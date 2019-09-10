using System.Collections.Generic;
using Csla.Core;
using Csla.Rules;
using RuleTutorial.Testing.Common;
using TransformationRules.Rules;
using Xunit;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace TransformationRules.Test
{

  
  public class ToUpperTest
  {

     public class TheCtor
    {
      private IBusinessRule Rule;

      public TheCtor()
      {
        Rule = new ToUpper(RootFake.NameProperty);
      }

      [Fact]
      public void ToUpper_IsSync()
      {
        Assert.IsFalse(Rule.IsAsync);
      }

      [Fact]
      public void ToUpper_HasPrimaryProperty()
      {
        Assert.IsNotNull(Rule.PrimaryProperty);
        Assert.AreEqual(RootFake.NameProperty, Rule.PrimaryProperty);
      }


      [Fact]
      public void ToUpper_HasInputProperties()
      {
        Assert.IsTrue(Rule.InputProperties.Contains(RootFake.NameProperty));
      }

      [Fact]
      public void ToUpper_HasAffectedProperties()
      {
        Assert.IsTrue(Rule.AffectedProperties.Contains(RootFake.NameProperty));
      }
    }
  

    public class TheExecuteMethod : BusinessRuleTest
    {
      public TheExecuteMethod()
      {
        var rule = new ToUpper(RootFake.NameProperty);
        InitializeTest(rule, null);
      }
      
      [Fact]
      public void ToUpper_MustSetOutputPropertyToUpper()
      {
        var expected = "CSLA ROCKS";
        ExecuteRule(new Dictionary<IPropertyInfo, object>() {{RootFake.NameProperty, "csla rocks"}});
        Assert.IsTrue(GetOutputPropertyValue(RootFake.NameProperty) == expected);
      }
    }

    public class TheExecuteMethodAlt : BusinessRuleTest
    {
      public TheExecuteMethodAlt()
      {
        var rule = new ToUpper(RootFake.NameProperty);
        var root = new RootFake();
        root.Name = "csla rocks";
        InitializeTest(rule, root);
      }


      [Fact]
      public void ToUpper_MustSetOutputPropertyToUpper()
      {
        var expected = "CSLA ROCKS";
        ExecuteRule();
        Assert.IsTrue(GetOutputPropertyValue(RootFake.NameProperty) == expected);
      }
    }
  }
}
