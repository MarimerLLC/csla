//-----------------------------------------------------------------------
// <copyright file="AuthTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Text;
using Csla.Serialization;
using Csla.Rules;
using Csla.Test.Security;
using System.Diagnostics;
using Csla.Test;
using Csla.Test.Authorization;

namespace Csla.Android.Test
{
#if TESTING
    [DebuggerNonUserCode]
    [DebuggerStepThrough]
#endif
  public class AuthTests
  {

    private Csla.Test.DataPortal.DpRoot root = Csla.Test.DataPortal.DpRoot.NewRoot();

    public void Cleanup()
    {
      ApplicationContext.RuleSet = ApplicationContext.DefaultRuleSet;
    }

    public void TestAuthCloneRules()
    {
      ApplicationContext.GlobalContext.Clear();

      Assert.Reset();
      Csla.Test.Security.TestPrincipal.SimulateLogin();

      Assert.AreEqual(true, Csla.ApplicationContext.User.IsInRole("Admin"));

      #region "Pre Cloning Tests"

      //Is it denying read properly?
      Assert.AreEqual("[DenyReadOnProperty] Can't read property", root.DenyReadOnProperty,
          "Read should have been denied 1");

      //Is it denying write properly?
      root.DenyWriteOnProperty = "DenyWriteOnProperty";

      Assert.AreEqual("[DenyWriteOnProperty] Can't write variable", root.Auth,
          "Write should have been denied 2");

      //Is it denying both read and write properly?
      Assert.AreEqual("[DenyReadWriteOnProperty] Can't read property", root.DenyReadWriteOnProperty,
          "Read should have been denied 3");

      root.DenyReadWriteOnProperty = "DenyReadWriteONproperty";


      Assert.AreEqual("[DenyReadWriteOnProperty] Can't write variable", root.Auth,
          "Write should have been denied 4");

      //Is it allowing both read and write properly?
      Assert.AreEqual(root.AllowReadWriteOnProperty, root.Auth,
          "Read should have been allowed 5");

      root.AllowReadWriteOnProperty = "No value";
      Assert.AreEqual("No value", root.Auth,
          "Write should have been allowed 6");

      #endregion

      #region "After Cloning Tests"

      //Do they work under cloning as well?
      Csla.Test.DataPortal.DpRoot NewRoot = root.Clone();

      ApplicationContext.GlobalContext.Clear();

      //Is it denying read properly?
      Assert.AreEqual("[DenyReadOnProperty] Can't read property", NewRoot.DenyReadOnProperty,
          "Read should have been denied 7");

      //Is it denying write properly?
      NewRoot.DenyWriteOnProperty = "DenyWriteOnProperty";

      Assert.AreEqual("[DenyWriteOnProperty] Can't write variable", NewRoot.Auth,
          "Write should have been denied 8");

      //Is it denying both read and write properly?
      Assert.AreEqual("[DenyReadWriteOnProperty] Can't read property", NewRoot.DenyReadWriteOnProperty,
          "Read should have been denied 9");

      NewRoot.DenyReadWriteOnProperty = "DenyReadWriteONproperty";

      Assert.AreEqual("[DenyReadWriteOnProperty] Can't write variable", NewRoot.Auth,
          "Write should have been denied 10");

      //Is it allowing both read and write properly?
      Assert.AreEqual(NewRoot.AllowReadWriteOnProperty, NewRoot.Auth,
          "Read should have been allowed 11");

      NewRoot.AllowReadWriteOnProperty = "AllowReadWriteOnProperty";
      Assert.AreEqual("AllowReadWriteOnProperty", NewRoot.Auth,
          "Write should have been allowed 12");

      #endregion

      Csla.Test.Security.TestPrincipal.SimulateLogout();
    }

    public void TestAuthBeginEditRules()
    {
      ApplicationContext.GlobalContext.Clear();

      Assert.Reset();
      Csla.Test.Security.TestPrincipal.SimulateLogin();

#if SILVERLIGHT
      Assert.AreEqual(true, Csla.ApplicationContext.User.IsInRole("Admin"));
#else
      Assert.AreEqual(true, System.Threading.Thread.CurrentPrincipal.IsInRole("Admin"));
#endif

      root.Data = "Something new";

      root.BeginEdit();

      #region "Pre-Testing"

      root.Data = "Something new 1";

      //Is it denying read properly?
      Assert.AreEqual("[DenyReadOnProperty] Can't read property", root.DenyReadOnProperty,
          "Read should have been denied");

      //Is it denying write properly?
      root.DenyWriteOnProperty = "DenyWriteOnProperty";

      Assert.AreEqual("[DenyWriteOnProperty] Can't write variable", root.Auth,
          "Write should have been denied");

      //Is it denying both read and write properly?
      Assert.AreEqual("[DenyReadWriteOnProperty] Can't read property", root.DenyReadWriteOnProperty,
          "Read should have been denied");

      root.DenyReadWriteOnProperty = "DenyReadWriteONproperty";

      Assert.AreEqual("[DenyReadWriteOnProperty] Can't write variable", root.Auth,
          "Write should have been denied");

      //Is it allowing both read and write properly?
      Assert.AreEqual(root.AllowReadWriteOnProperty, root.Auth,
          "Read should have been allowed");

      root.AllowReadWriteOnProperty = "No value";
      Assert.AreEqual("No value", root.Auth,
          "Write should have been allowed");

      #endregion

      #region "Cancel Edit"

      //Cancel the edit and see if the authorization rules still work
      root.CancelEdit();

      //Is it denying read properly?
      Assert.AreEqual("[DenyReadOnProperty] Can't read property", root.DenyReadOnProperty,
          "Read should have been denied");

      //Is it denying write properly?
      root.DenyWriteOnProperty = "DenyWriteOnProperty";

      Assert.AreEqual("[DenyWriteOnProperty] Can't write variable", root.Auth,
          "Write should have been denied");

      //Is it denying both read and write properly?
      Assert.AreEqual("[DenyReadWriteOnProperty] Can't read property", root.DenyReadWriteOnProperty,
          "Read should have been denied");

      root.DenyReadWriteOnProperty = "DenyReadWriteONproperty";

      Assert.AreEqual("[DenyReadWriteOnProperty] Can't write variable", root.Auth,
          "Write should have been denied");

      //Is it allowing both read and write properly?
      Assert.AreEqual(root.AllowReadWriteOnProperty, root.Auth,
          "Read should have been allowed");

      root.AllowReadWriteOnProperty = "No value";
      Assert.AreEqual("No value", root.Auth,
          "Write should have been allowed");

      #endregion

      #region "Apply Edit"

      //Apply this edit and see if the authorization rules still work
      //Is it denying read properly?
      Assert.AreEqual("[DenyReadOnProperty] Can't read property", root.DenyReadOnProperty,
          "Read should have been denied");

      //Is it denying write properly?
      root.DenyWriteOnProperty = "DenyWriteOnProperty";

      Assert.AreEqual("[DenyWriteOnProperty] Can't write variable", root.Auth,
          "Write should have been denied");

      //Is it denying both read and write properly?
      Assert.AreEqual("[DenyReadWriteOnProperty] Can't read property", root.DenyReadWriteOnProperty,
          "Read should have been denied");

      root.DenyReadWriteOnProperty = "DenyReadWriteONproperty";

      Assert.AreEqual("[DenyReadWriteOnProperty] Can't write variable", root.Auth,
          "Write should have been denied");

      //Is it allowing both read and write properly?
      Assert.AreEqual(root.AllowReadWriteOnProperty, root.Auth,
          "Read should have been allowed");

      root.AllowReadWriteOnProperty = "No value";
      Assert.AreEqual("No value", root.Auth,
          "Write should have been allowed");


      #endregion

      Csla.Test.Security.TestPrincipal.SimulateLogout();
    }

    public void TestAuthorizationAfterEditCycle()
    {
      Csla.ApplicationContext.GlobalContext.Clear();
      Csla.Test.Security.PermissionsRoot pr = Csla.Test.Security.PermissionsRoot.NewPermissionsRoot();

      Assert.Reset();
      Csla.Test.Security.TestPrincipal.SimulateLogin();
      pr.FirstName = "something";

      pr.BeginEdit();
      pr.FirstName = "ba";
      pr.CancelEdit();
      Csla.Test.Security.TestPrincipal.SimulateLogout();

      Csla.Test.Security.PermissionsRoot prClone = pr.Clone();
      Csla.Test.Security.TestPrincipal.SimulateLogin();
      prClone.FirstName = "somethiansdfasdf";
      Csla.Test.Security.TestPrincipal.SimulateLogout();
    }

    public void TestUnauthorizedAccessToGet()
    {
      Csla.ApplicationContext.GlobalContext.Clear();

      PermissionsRoot pr = PermissionsRoot.NewPermissionsRoot();

      //this should throw an exception, since only admins have access to this property
      string something = pr.FirstName;
    }

    public void TestUnauthorizedAccessToSet()
    {
      PermissionsRoot pr = PermissionsRoot.NewPermissionsRoot();

      //will cause an exception, because only admins can write to property
      pr.FirstName = "test";
    }

    public void TestAuthorizedAccess()
    {
      Assert.Reset();
      Csla.ApplicationContext.GlobalContext.Clear();
      Csla.Test.Security.TestPrincipal.SimulateLogin();

      PermissionsRoot pr = PermissionsRoot.NewPermissionsRoot();
      //should work, because we are now logged in as an admin
      pr.FirstName = "something";
      string something = pr.FirstName;

#if SILVERLIGHT
      Assert.AreEqual(true, Csla.ApplicationContext.User.IsInRole("Admin"));
#else
      Assert.AreEqual(true, System.Threading.Thread.CurrentPrincipal.IsInRole("Admin"));
#endif
      //set to null so the other testmethods continue to throw exceptions
      Csla.Test.Security.TestPrincipal.SimulateLogout();

#if SILVERLIGHT
      Assert.AreEqual(false, Csla.ApplicationContext.User.IsInRole("Admin"));
#else
      Assert.AreEqual(false, System.Threading.Thread.CurrentPrincipal.IsInRole("Admin"));
#endif
    }

    public void TestAuthExecute()
    {
      Assert.Reset();
      Csla.ApplicationContext.GlobalContext.Clear();
      Csla.Test.Security.TestPrincipal.SimulateLogin();

      PermissionsRoot pr = PermissionsRoot.NewPermissionsRoot();
      //should work, because we are now logged in as an admin
      pr.DoWork();

#if SILVERLIGHT
      Assert.AreEqual(true, Csla.ApplicationContext.User.IsInRole("Admin"));
#else
      Assert.AreEqual(true, System.Threading.Thread.CurrentPrincipal.IsInRole("Admin"));
#endif
      //set to null so the other testmethods continue to throw exceptions
      Csla.Test.Security.TestPrincipal.SimulateLogout();

#if SILVERLIGHT
      Assert.AreEqual(false, Csla.ApplicationContext.User.IsInRole("Admin"));
#else
      Assert.AreEqual(false, System.Threading.Thread.CurrentPrincipal.IsInRole("Admin"));
#endif
    }

    public void TestUnAuthExecute()
    {
      Assert.Reset();
      Csla.ApplicationContext.GlobalContext.Clear();

      Assert.AreEqual(false, Csla.ApplicationContext.User.IsInRole("Admin"));

      PermissionsRoot pr = PermissionsRoot.NewPermissionsRoot();
      //should fail, because we're not an admin
      pr.DoWork();

    }

    //[TestMethod()]
    //public void TestAuthorizationAfterClone()
    //{
    //    Csla.ApplicationContext.GlobalContext.Clear();
    //    Csla.Test.Security.TestPrincipal.SimulateLogin();

    //    PermissionsRoot pr = PermissionsRoot.NewPermissionsRoot();

    //    //should work because we are now logged in as an admin
    //    pr.FirstName = "something";
    //    string something = pr.FirstName;

    //    //The permissions should persist across a cloning
    //    PermissionsRoot prClone = pr.Clone();
    //    pr.FirstName = "something";
    //    something = pr.FirstName;
    //}

    public void TestAuthRuleSetsOnStaticHasPermissionMethodsWhenAddingAuthzRuleSetExplicitly()
    {
      Assert.Reset();
      var root = PermissionsRoot.NewPermissionsRoot();
      Csla.Test.Security.TestPrincipal.SimulateLogin();

#if SILVERLIGHT
      Assert.IsTrue(Csla.ApplicationContext.User.IsInRole("Admin"));
      Assert.IsFalse(Csla.ApplicationContext.User.IsInRole("User"));
#else
      Assert.IsTrue(System.Threading.Thread.CurrentPrincipal.IsInRole("Admin"));
      Assert.IsFalse(System.Threading.Thread.CurrentPrincipal.IsInRole("User"));
#endif

      //BusinessRules.AddRule(typeof(PermissionsRoot), new IsInRole(AuthorizationActions.DeleteObject, "User"), ApplicationContext.DefaultRuleSet);
      //BusinessRules.AddRule(typeof(PermissionsRoot), new IsInRole(AuthorizationActions.DeleteObject, "Admin"), "custom1");
      //BusinessRules.AddRule(typeof(PermissionsRoot), new IsInRole(AuthorizationActions.DeleteObject, "User", "Admin"), "custom2");

      // implicit usage of ApplicationContext.RuleSet
      ApplicationContext.RuleSet = ApplicationContext.DefaultRuleSet;
      Assert.IsFalse(BusinessRules.HasPermission(AuthorizationActions.DeleteObject, typeof(PermissionsRoot)));
      ApplicationContext.RuleSet = "custom1";
      Assert.IsTrue(BusinessRules.HasPermission(AuthorizationActions.DeleteObject, typeof(PermissionsRoot)));
      ApplicationContext.RuleSet = "custom2";
      Assert.IsTrue(BusinessRules.HasPermission(AuthorizationActions.DeleteObject, typeof(PermissionsRoot)));

      ApplicationContext.RuleSet = ApplicationContext.DefaultRuleSet;

      // directly specifying which ruleset to use
      Assert.IsFalse(BusinessRules.HasPermission(AuthorizationActions.DeleteObject, typeof(PermissionsRoot), ApplicationContext.DefaultRuleSet));
      Assert.IsTrue(BusinessRules.HasPermission(AuthorizationActions.DeleteObject, typeof(PermissionsRoot), "custom1"));
      Assert.IsTrue(BusinessRules.HasPermission(AuthorizationActions.DeleteObject, typeof(PermissionsRoot), "custom2"));

      Csla.Test.Security.TestPrincipal.SimulateLogout();
    }

    public void TestAuthRuleSetsOnStaticHasPermissionMethodsWhenAddingAuthzRuleSetUsingApplicationContextRuleSet()
    {
      Assert.Reset();
      var root = PermissionsRoot2.NewPermissionsRoot();
      Csla.Test.Security.TestPrincipal.SimulateLogin();

#if SILVERLIGHT
      Assert.IsTrue(Csla.ApplicationContext.User.IsInRole("Admin"));
      Assert.IsFalse(Csla.ApplicationContext.User.IsInRole("User"));
#else
      Assert.IsTrue(System.Threading.Thread.CurrentPrincipal.IsInRole("Admin"));
      Assert.IsFalse(System.Threading.Thread.CurrentPrincipal.IsInRole("User"));
#endif

      //BusinessRules.AddRule(typeof(PermissionsRoot), new IsInRole(AuthorizationActions.DeleteObject, "User"), ApplicationContext.DefaultRuleSet);
      //BusinessRules.AddRule(typeof(PermissionsRoot), new IsInRole(AuthorizationActions.DeleteObject, "Admin"), "custom1");
      //BusinessRules.AddRule(typeof(PermissionsRoot), new IsInRole(AuthorizationActions.DeleteObject, "User", "Admin"), "custom2");

      // implicit usage of ApplicationContext.RuleSet
      ApplicationContext.RuleSet = ApplicationContext.DefaultRuleSet;
      Assert.IsFalse(BusinessRules.HasPermission(AuthorizationActions.DeleteObject, typeof(PermissionsRoot2)));
      ApplicationContext.RuleSet = "custom1";
      Assert.IsTrue(BusinessRules.HasPermission(AuthorizationActions.DeleteObject, typeof(PermissionsRoot2)));
      ApplicationContext.RuleSet = "custom2";
      Assert.IsTrue(BusinessRules.HasPermission(AuthorizationActions.DeleteObject, typeof(PermissionsRoot2)));

      ApplicationContext.RuleSet = ApplicationContext.DefaultRuleSet;

      // directly specifying which ruleset to use
      Assert.IsFalse(BusinessRules.HasPermission(AuthorizationActions.DeleteObject, typeof(PermissionsRoot2), ApplicationContext.DefaultRuleSet));
      Assert.IsTrue(BusinessRules.HasPermission(AuthorizationActions.DeleteObject, typeof(PermissionsRoot2), "custom1"));
      Assert.IsTrue(BusinessRules.HasPermission(AuthorizationActions.DeleteObject, typeof(PermissionsRoot2), "custom2"));

      Csla.Test.Security.TestPrincipal.SimulateLogout();
    }

    public void TestAuthRulesCleanupAndAddAgainWhenExceptionIsThrownInAddObjectBusinessRules()
    {
      Assert.Reset();
      RootException.Counter = 0;

      ApplicationContext.RuleSet = ApplicationContext.DefaultRuleSet;
      // AddObjectAuthorizations should throw exception
      try
      {
        BusinessRules.HasPermission(AuthorizationActions.DeleteObject, typeof(RootException));
      }
      catch (Exception ex)
      {
#if !SILVERLIGHT
        Assert.IsInstanceOfType(ex, typeof(TargetInvocationException));
        Assert.IsInstanceOfType(ex.InnerException, typeof(ArgumentException));
#else
        using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
        {
          using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream("sl_testauth.txt", FileMode.OpenOrCreate, isf)) 
          { using (StreamWriter sw = new StreamWriter(isfs)) 
          { 
            sw.Write(ex.ToString()); 
            sw.Close(); } }
        }
        Assert.IsInstanceOfType(typeof(TargetInvocationException), ex);
        Assert.IsInstanceOfType(typeof(ArgumentException), ex.InnerException);
#endif
      }

      // AddObjectAuthorizations should be called again and
      // should throw exception again
      try
      {
        BusinessRules.HasPermission(AuthorizationActions.DeleteObject, typeof(RootException));
      }
      catch (Exception ex)
      {
#if !SILVERLIGHT
        Assert.IsInstanceOfType(ex, typeof(TargetInvocationException));
        Assert.IsInstanceOfType(ex.InnerException, typeof(ArgumentException));
#else
        Assert.IsInstanceOfType(typeof(TargetInvocationException), ex);
        Assert.IsInstanceOfType(typeof(ArgumentException), ex.InnerException);
#endif
      }

      Assert.IsTrue(RootException.Counter == 2);
    }

    public void AuthorizeRemoveFromList()
    {
      var root = new RootList();
      root.RemoveAt(0);
    }
  }

  [Serializable]
  public class RootList : BusinessListBase<RootList, ChildItem>
  {
    public RootList()
    {
      RaiseListChangedEvents = false;
      Add(Csla.DataPortal.CreateChild<ChildItem>());
      RaiseListChangedEvents = true;
    }
  }

  [Serializable]
  public class ChildItem : BusinessBase<ChildItem>
  {
    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new NoAuth(AuthorizationActions.DeleteObject));
    }

    private class NoAuth : Csla.Rules.AuthorizationRule
    {
      public NoAuth(AuthorizationActions action)
        : base(action)
      { }

      protected override void Execute(AuthorizationContext context)
      {
        context.HasPermission = false;
      }
    }
  }
}