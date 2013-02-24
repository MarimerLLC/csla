using System;
using System.Collections.Generic;
using System.Text;
using UnitDriven;

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
  [TestClass()]
  public class ValidationTests : TestBase
  {
#if SILVERLIGHT
    [TestInitialize]
    public void Setup()
    {
      Csla.DataPortal.ProxyTypeName = "Local";
    }
#endif

    [TestMethod()]
    public void TestValidationRulesWithPrivateMember()
    {
      //works now because we are calling ValidationRules.CheckRules() in DataPortal_Create
      UnitTestContext context = GetContext();
      Csla.ApplicationContext.GlobalContext.Clear();
      HasRulesManager.NewHasRulesManager((o, e) =>
      {
        HasRulesManager root = e.Object;
        context.Assert.AreEqual("<new>", root.Name);
        context.Assert.AreEqual(true, root.IsValid, "should be valid on create");
        context.Assert.AreEqual(0, root.BrokenRulesCollection.Count);

        root.BeginEdit();
        root.Name = "";
        root.CancelEdit();

        context.Assert.AreEqual("<new>", root.Name);
        context.Assert.AreEqual(true, root.IsValid, "should be valid after CancelEdit");
        context.Assert.AreEqual(0, root.BrokenRulesCollection.Count);

        root.BeginEdit();
        root.Name = "";
        root.ApplyEdit();

        context.Assert.AreEqual("", root.Name);
        context.Assert.AreEqual(false, root.IsValid);
        context.Assert.AreEqual(1, root.BrokenRulesCollection.Count);
        context.Assert.AreEqual("Name required", root.BrokenRulesCollection[0].Description);
        context.Assert.Success();
      });

      context.Complete();
    }

    [TestMethod()]
    public void TestValidationRulesWithPublicProperty()
    {
      UnitTestContext context = GetContext();
      //should work since ValidationRules.CheckRules() is called in DataPortal_Create
      Csla.ApplicationContext.GlobalContext.Clear();
      HasRulesManager2.NewHasRulesManager2((o, e) =>
      {
        HasRulesManager2 root = e.Object;
        context.Assert.AreEqual("<new>", root.Name);
        context.Assert.AreEqual(true, root.IsValid, "should be valid on create");
        context.Assert.AreEqual(0, root.BrokenRulesCollection.Count);

        root.BeginEdit();
        root.Name = "";
        root.CancelEdit();

        context.Assert.AreEqual("<new>", root.Name);
        context.Assert.AreEqual(true, root.IsValid, "should be valid after CancelEdit");
        context.Assert.AreEqual(0, root.BrokenRulesCollection.Count);

        root.BeginEdit();
        root.Name = "";
        root.ApplyEdit();

        context.Assert.AreEqual("", root.Name);
        context.Assert.AreEqual(false, root.IsValid);
        context.Assert.AreEqual(1, root.BrokenRulesCollection.Count);
        context.Assert.AreEqual("Name required", root.BrokenRulesCollection[0].Description);
        context.Assert.Success();
      });

      context.Complete();
    }

    [TestMethod()]
    public void TestValidationAfterEditCycle()
    {
      //should work since ValidationRules.CheckRules() is called in DataPortal_Create
      Csla.ApplicationContext.GlobalContext.Clear();
      UnitTestContext context = GetContext();
      HasRulesManager.NewHasRulesManager((o, e) =>
      {
        HasRulesManager root = e.Object;
        context.Assert.AreEqual("<new>", root.Name);
        context.Assert.AreEqual(true, root.IsValid, "should be valid on create");
        context.Assert.AreEqual(0, root.BrokenRulesCollection.Count);

        bool validationComplete = false;
        root.ValidationComplete += (vo, ve) => { validationComplete = true; };

        root.BeginEdit();
        root.Name = "";
        context.Assert.AreEqual("", root.Name);
        context.Assert.AreEqual(false, root.IsValid);
        context.Assert.AreEqual(1, root.BrokenRulesCollection.Count);
        context.Assert.AreEqual("Name required", root.BrokenRulesCollection[0].Description);
        context.Assert.IsTrue(validationComplete, "ValidationComplete should have run");
        root.BeginEdit();
        root.Name = "Begin 1";
        context.Assert.AreEqual("Begin 1", root.Name);
        context.Assert.AreEqual(true, root.IsValid);
        context.Assert.AreEqual(0, root.BrokenRulesCollection.Count);
        root.BeginEdit();
        root.Name = "Begin 2";
        context.Assert.AreEqual("Begin 2", root.Name);
        context.Assert.AreEqual(true, root.IsValid);
        context.Assert.AreEqual(0, root.BrokenRulesCollection.Count);
        root.BeginEdit();
        root.Name = "Begin 3";
        context.Assert.AreEqual("Begin 3", root.Name);
        context.Assert.AreEqual(true, root.IsValid);
        context.Assert.AreEqual(0, root.BrokenRulesCollection.Count);

        HasRulesManager hrmClone = root.Clone();

        //Test validation rule cancels for both clone and cloned
        root.CancelEdit();
        context.Assert.AreEqual("Begin 2", root.Name);
        context.Assert.AreEqual(true, root.IsValid);
        context.Assert.AreEqual(0, root.BrokenRulesCollection.Count);
        hrmClone.CancelEdit();
        context.Assert.AreEqual("Begin 2", hrmClone.Name);
        context.Assert.AreEqual(true, hrmClone.IsValid);
        context.Assert.AreEqual(0, hrmClone.BrokenRulesCollection.Count);
        root.CancelEdit();
        context.Assert.AreEqual("Begin 1", root.Name);
        context.Assert.AreEqual(true, root.IsValid);
        context.Assert.AreEqual(0, root.BrokenRulesCollection.Count);
        hrmClone.CancelEdit();
        context.Assert.AreEqual("Begin 1", hrmClone.Name);
        context.Assert.AreEqual(true, hrmClone.IsValid);
        context.Assert.AreEqual(0, hrmClone.BrokenRulesCollection.Count);
        root.CancelEdit();
        context.Assert.AreEqual("", root.Name);
        context.Assert.AreEqual(false, root.IsValid);
        context.Assert.AreEqual(1, root.BrokenRulesCollection.Count);
        context.Assert.AreEqual("Name required", root.BrokenRulesCollection[0].Description);
        hrmClone.CancelEdit();
        context.Assert.AreEqual("", hrmClone.Name);
        context.Assert.AreEqual(false, hrmClone.IsValid);
        context.Assert.AreEqual(1, hrmClone.BrokenRulesCollection.Count);
        context.Assert.AreEqual("Name required", hrmClone.BrokenRulesCollection[0].Description);
        root.CancelEdit();
        context.Assert.AreEqual("<new>", root.Name);
        context.Assert.AreEqual(true, root.IsValid);
        context.Assert.AreEqual(0, root.BrokenRulesCollection.Count);
        hrmClone.CancelEdit();
        context.Assert.AreEqual("<new>", hrmClone.Name);
        context.Assert.AreEqual(true, hrmClone.IsValid);
        context.Assert.AreEqual(0, hrmClone.BrokenRulesCollection.Count);
        context.Assert.Success();
      });

      context.Complete();
    }

    [TestMethod()]
    public void TestValidationRulesAfterClone()
    {
      //this test uses HasRulesManager2, which assigns criteria._name to its public
      //property in DataPortal_Create.  If it used HasRulesManager, it would fail
      //the first assert, but pass the others
      Csla.ApplicationContext.GlobalContext.Clear();
      UnitTestContext context = GetContext();
      HasRulesManager2.NewHasRulesManager2((o, e) =>
      {
        HasRulesManager2 root = e.Object;
        context.Assert.AreEqual(true, root.IsValid);
        root.BeginEdit();
        root.Name = "";
        root.ApplyEdit();

        context.Assert.AreEqual(false, root.IsValid);
        HasRulesManager2 rootClone = root.Clone();
        context.Assert.AreEqual(false, rootClone.IsValid);

        rootClone.Name = "something";
        context.Assert.AreEqual(true, rootClone.IsValid);
        context.Assert.Success();
      });
      context.Complete();
    }

    [TestMethod()]
    public void BreakRequiredRule()
    {
      Csla.ApplicationContext.GlobalContext.Clear();
      UnitTestContext context = GetContext();
      HasRulesManager.NewHasRulesManager((o, e) =>
      {
        HasRulesManager root = e.Object;
        root.Name = "";

        context.Assert.AreEqual(false, root.IsValid, "should not be valid");
        context.Assert.AreEqual(1, root.BrokenRulesCollection.Count);
        context.Assert.AreEqual("Name required", root.BrokenRulesCollection[0].Description);

        context.Assert.Success();
      });

      context.Complete();
    }

    [TestMethod()]
    public void BreakLengthRule()
    {
      Csla.ApplicationContext.GlobalContext.Clear();
      UnitTestContext context = GetContext();
      HasRulesManager.NewHasRulesManager((o, e) =>
      {
        HasRulesManager root = e.Object;
        root.Name = "12345678901";
        context.Assert.AreEqual(false, root.IsValid, "should not be valid");
        context.Assert.AreEqual(1, root.BrokenRulesCollection.Count);
        //Assert.AreEqual("Name too long", root.GetBrokenRulesCollection[0].Description);
        Assert.AreEqual("The value for Name is too long", root.BrokenRulesCollection[0].Description);

        root.Name = "1234567890";
        context.Assert.AreEqual(true, root.IsValid, "should be valid");
        context.Assert.AreEqual(0, root.BrokenRulesCollection.Count);
        context.Assert.Success();
      });
      context.Complete();
    }

    [TestMethod()]
    public void BreakLengthRuleAndClone()
    {
      Csla.ApplicationContext.GlobalContext.Clear();
      UnitTestContext context = GetContext();
      HasRulesManager.NewHasRulesManager((o, e) =>
      {
        HasRulesManager root = e.Object;
        root.Name = "12345678901";
        context.Assert.AreEqual(false, root.IsValid, "should not be valid");
        context.Assert.AreEqual(1, root.BrokenRulesCollection.Count);
        //Assert.AreEqual("Name too long", root.GetBrokenRulesCollection[0].Description;
        Assert.AreEqual("The value for Name is too long", root.BrokenRulesCollection[0].Description);

        root = (HasRulesManager)(root.Clone());
        context.Assert.AreEqual(false, root.IsValid, "should not be valid");
        context.Assert.AreEqual(1, root.BrokenRulesCollection.Count);
        //Assert.AreEqual("Name too long", root.GetBrokenRulesCollection[0].Description;
        context.Assert.AreEqual("The value for Name is too long", root.BrokenRulesCollection[0].Description);

        root.Name = "1234567890";
        context.Assert.AreEqual(true, root.IsValid, "Should be valid");
        context.Assert.AreEqual(0, root.BrokenRulesCollection.Count);
        context.Assert.Success();
      });
      context.Complete();
    }

    [TestMethod()]
    public void RegExSSN()
    {
      Csla.ApplicationContext.GlobalContext.Clear();
      UnitTestContext context = GetContext();

      HasRegEx root = new HasRegEx();

      root.Ssn = "555-55-5555";
      root.Ssn2 = "555-55-5555";
      context.Assert.IsTrue(root.IsValid, "Ssn should be valid");

      root.Ssn = "555-55-5555d";
      context.Assert.IsFalse(root.IsValid, "Ssn should not be valid");

      root.Ssn = "555-55-5555";
      root.Ssn2 = "555-55-5555d";
      context.Assert.IsFalse(root.IsValid, "Ssn should not be valid");

      context.Assert.Success();

    }

    [TestMethod]
    public void MergeBrokenRules()
    {
      UnitTestContext context = GetContext();
      BrokenRulesMergeRoot root = new BrokenRulesMergeRoot();
      root.Validate();
      Csla.Validation.BrokenRulesCollection list = root.BrokenRulesCollection;
      context.Assert.AreEqual(2, list.Count, "Should have 2 broken rules");
      context.Assert.IsTrue(list[0].RuleName.Contains(@"rule://root."), "Rule should contain rule://root.");

      context.Assert.Success();
    }

    [TestMethod]
    public void VerifyUndoableStateStackOnClone()
    {
      Csla.ApplicationContext.GlobalContext.Clear();
      using (UnitTestContext context = GetContext())
      {
        HasRulesManager2.NewHasRulesManager2((o, e) =>
        {
          context.Assert.IsNull(e.Error);
          HasRulesManager2 root = e.Object;

          string expected = root.Name;
          root.BeginEdit();
          root.Name = "";
          HasRulesManager2 rootClone = root.Clone();
          rootClone.CancelEdit();

          string actual = rootClone.Name;
          context.Assert.AreEqual(expected, actual);
          context.Assert.Try(rootClone.ApplyEdit);

          context.Assert.Success();
        });
      }
    }

    [TestMethod()]
    public void ListChangedEventTrigger()
    {
      Csla.ApplicationContext.GlobalContext.Clear();
      UnitTestContext context = GetContext();
      HasChildren.NewObject((o, e) =>
      {
        try
        {
          HasChildren root = e.Object;
          context.Assert.AreEqual(false, root.IsValid);
          root.BeginEdit();
          root.ChildList.Add(new Child());
          context.Assert.AreEqual(true, root.IsValid);

          root.CancelEdit();
          context.Assert.AreEqual(false, root.IsValid);

          context.Assert.Success();
        }
        finally
        {
          context.Complete();
        }
      });
    }
  }
}
