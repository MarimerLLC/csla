//-----------------------------------------------------------------------
// <copyright file="AuthTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Text;
using Csla;
using Csla.Serialization;
using Csla.Rules;
using Csla.Test.Security;
using UnitDriven;
using System.Diagnostics;
using System.Security.Claims;

#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#elif MSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
#endif

namespace Csla.Test.Authorization
{
#if TESTING
    [DebuggerNonUserCode]
    [DebuggerStepThrough]
#endif
  [TestClass()]
  public class AuthTests
  {
    private static ClaimsPrincipal GetPrincipal(params string[] roles)
    {
      var identity = new ClaimsIdentity();
      foreach (var item in roles)
        identity.AddClaim(new Claim(ClaimTypes.Role, item));
      return new ClaimsPrincipal(identity);
    }

    private DataPortal.DpRoot root = DataPortal.DpRoot.NewRoot();

    [TestCleanup]
    public void Cleanup()
    {
      ApplicationContext.RuleSet = ApplicationContext.DefaultRuleSet;
    }

    [TestMethod()]
    public void TestAuthCloneRules()
    {
      ApplicationContext.GlobalContext.Clear();

      Csla.ApplicationContext.User = GetPrincipal("Admin");

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
      DataPortal.DpRoot NewRoot = root.Clone();

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

      Csla.ApplicationContext.User = new ClaimsPrincipal();
    }

    [TestMethod()]
    public void TestAuthBeginEditRules()
    {
      ApplicationContext.GlobalContext.Clear();

      Csla.ApplicationContext.User = GetPrincipal("Admin");

      Assert.AreEqual(true, System.Threading.Thread.CurrentPrincipal.IsInRole("Admin"));

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

      Csla.ApplicationContext.User = new ClaimsPrincipal();
    }

    [TestMethod()]
    public void TestAuthorizationAfterEditCycle()
    {
      Csla.ApplicationContext.GlobalContext.Clear();
      Csla.Test.Security.PermissionsRoot pr = Csla.Test.Security.PermissionsRoot.NewPermissionsRoot();

      Csla.ApplicationContext.User = GetPrincipal("Admin");
      pr.FirstName = "something";

      pr.BeginEdit();
      pr.FirstName = "ba";
      pr.CancelEdit();
      Csla.ApplicationContext.User = new ClaimsPrincipal();

      Csla.Test.Security.PermissionsRoot prClone = pr.Clone();
      Csla.ApplicationContext.User = GetPrincipal("Admin");
      prClone.FirstName = "somethiansdfasdf";
      Csla.ApplicationContext.User = new ClaimsPrincipal();
    }

    [ExpectedException(typeof(Csla.Security.SecurityException))]
    [TestMethod]
    public void TestUnauthorizedAccessToGet()
    {
      Csla.ApplicationContext.GlobalContext.Clear();

      PermissionsRoot pr = PermissionsRoot.NewPermissionsRoot();

      //this should throw an exception, since only admins have access to this property
      string something = pr.FirstName;
    }

    [ExpectedException(typeof(Csla.Security.SecurityException))]
    [TestMethod]
    public void TestUnauthorizedAccessToSet()
    {
      PermissionsRoot pr = PermissionsRoot.NewPermissionsRoot();

      //will cause an exception, because only admins can write to property
      pr.FirstName = "test";
    }

    [TestMethod]
    public void TestAuthorizedAccess()
    {
      Csla.ApplicationContext.GlobalContext.Clear();
      Csla.ApplicationContext.User = GetPrincipal("Admin");

      PermissionsRoot pr = PermissionsRoot.NewPermissionsRoot();
      //should work, because we are now logged in as an admin
      pr.FirstName = "something";
      string something = pr.FirstName;

            Assert.AreEqual(true, System.Threading.Thread.CurrentPrincipal.IsInRole("Admin"));
      //set to null so the other testmethods continue to throw exceptions
      Csla.ApplicationContext.User = new ClaimsPrincipal();

      Assert.AreEqual(false, System.Threading.Thread.CurrentPrincipal.IsInRole("Admin"));
    }

    [TestMethod]
    public void TestAuthExecute()
    {
      Csla.ApplicationContext.GlobalContext.Clear();
      Csla.ApplicationContext.User = GetPrincipal("Admin");

      PermissionsRoot pr = PermissionsRoot.NewPermissionsRoot();
      //should work, because we are now logged in as an admin
      pr.DoWork();

          Assert.AreEqual(true, System.Threading.Thread.CurrentPrincipal.IsInRole("Admin"));
      //set to null so the other testmethods continue to throw exceptions
      Csla.ApplicationContext.User = new ClaimsPrincipal();

      Assert.AreEqual(false, System.Threading.Thread.CurrentPrincipal.IsInRole("Admin"));
    }

    [TestMethod]
    [ExpectedException(typeof(Csla.Security.SecurityException))]
    public void TestUnAuthExecute()
    {
      Csla.ApplicationContext.GlobalContext.Clear();

      Assert.AreEqual(false, Csla.ApplicationContext.User.IsInRole("Admin"));

      PermissionsRoot pr = PermissionsRoot.NewPermissionsRoot();
      //should fail, because we're not an admin
      pr.DoWork();

    }

    [TestMethod]
    public void TestAuthRuleSetsOnStaticHasPermissionMethodsWhenAddingAuthzRuleSetExplicitly()
    {
      var root = PermissionsRoot.NewPermissionsRoot();
      Csla.ApplicationContext.User = GetPrincipal("Admin");

      Assert.IsTrue(System.Threading.Thread.CurrentPrincipal.IsInRole("Admin"));
      Assert.IsFalse(System.Threading.Thread.CurrentPrincipal.IsInRole("User"));

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

      Csla.ApplicationContext.User = new ClaimsPrincipal();
    }

    [TestMethod]
    public void TestAuthRuleSetsOnStaticHasPermissionMethodsWhenAddingAuthzRuleSetUsingApplicationContextRuleSet()
    {
      var root = PermissionsRoot2.NewPermissionsRoot();
      Csla.ApplicationContext.User = GetPrincipal("Admin");

      Assert.IsTrue(System.Threading.Thread.CurrentPrincipal.IsInRole("Admin"));
      Assert.IsFalse(System.Threading.Thread.CurrentPrincipal.IsInRole("User"));

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

      Csla.ApplicationContext.User = new ClaimsPrincipal();
    }

    [TestMethod]

    public void TestAuthRulesCleanupAndAddAgainWhenExceptionIsThrownInAddObjectBusinessRules()
    {
      RootException.Counter = 0;

      ApplicationContext.RuleSet = ApplicationContext.DefaultRuleSet;
      // AddObjectAuthorizations should throw exception
      try
      {
        BusinessRules.HasPermission(AuthorizationActions.DeleteObject, typeof(RootException));
      }
      catch (Exception ex)
      {
        Assert.IsInstanceOfType(ex, typeof(TargetInvocationException));
        Assert.IsInstanceOfType(ex.InnerException, typeof(ArgumentException));
      }

      // AddObjectAuthorizations should be called again and
      // should throw exception again
      try
      {
        BusinessRules.HasPermission(AuthorizationActions.DeleteObject, typeof(RootException));
      }
      catch (Exception ex)
      {
        Assert.IsInstanceOfType(ex, typeof(TargetInvocationException));
        Assert.IsInstanceOfType(ex.InnerException, typeof(ArgumentException));
      }

      Assert.IsTrue(RootException.Counter == 2);
    }

    [TestMethod]
    public void AuthorizeRemoveFromList()
    {
      var root = new RootList();
      root.RemoveAt(0);
    }

    [TestMethod]
    public void PerTypeAuthEditObject()
    {
      ApplicationContext.DataPortalActivator = new PerTypeAuthDPActivator();
      try
      {
        Assert.IsFalse(BusinessRules.HasPermission(AuthorizationActions.EditObject, typeof(PerTypeAuthRoot)));
      }
      finally
      {
        Csla.ApplicationContext.DataPortalActivator = null;
      }
    }

    [TestMethod]
    public void PerTypeAuthEditObjectViaInterface()
    {
      ApplicationContext.DataPortalActivator = new PerTypeAuthDPActivator();
      try
      {
        Assert.IsFalse(BusinessRules.HasPermission(AuthorizationActions.EditObject, typeof(IPerTypeAuthRoot)));
      }
      finally
      {
        Csla.ApplicationContext.DataPortalActivator = null;
      }
    }

    [TestMethod]
    public void PerTypeAuthCreateWithCriteria() {
      Assert.IsTrue(
        BusinessRules.HasPermission(
          AuthorizationActions.CreateObject,
          typeof(PermissionRootWithCriteria),
          new object[] { new PermissionRootWithCriteria.Criteria() }));

      Assert.IsFalse(
        BusinessRules.HasPermission(
          AuthorizationActions.CreateObject,
          typeof(PermissionRootWithCriteria),
          new[] { new object() }));

    
      Assert.IsFalse(
        BusinessRules.HasPermission(
          AuthorizationActions.CreateObject,
          typeof(PermissionRootWithCriteria),
          (object[])null));
    }

    [TestMethod]
    public void PerTypeAuthFetchWithCriteria() 
    {
      Assert.IsTrue(
        BusinessRules.HasPermission(
          AuthorizationActions.GetObject,
          typeof(PermissionRootWithCriteria),
          new object[] { new PermissionRootWithCriteria.Criteria() }));

      Assert.IsFalse(
        BusinessRules.HasPermission(
          AuthorizationActions.GetObject,
          typeof(PermissionRootWithCriteria),
          new[] { new object() }));

    
      Assert.IsFalse(
        BusinessRules.HasPermission(
          AuthorizationActions.GetObject,
          typeof(PermissionRootWithCriteria),
          (object[])null));
    }
  
    [TestMethod]
    public void PerTypeAuthDeleteWithCriteria() 
    {
      Assert.IsTrue(
        BusinessRules.HasPermission(
          AuthorizationActions.DeleteObject,
          typeof(PermissionRootWithCriteria),
          new object[] { new PermissionRootWithCriteria.Criteria() }));

      Assert.IsFalse(
        BusinessRules.HasPermission(
          AuthorizationActions.DeleteObject,
          typeof(PermissionRootWithCriteria),
          new[] { new object() }));

    
      Assert.IsFalse(
        BusinessRules.HasPermission(
          AuthorizationActions.DeleteObject,
          typeof(PermissionRootWithCriteria),
          (object[])null));
    }
  }

  public class PerTypeAuthDPActivator : Server.IDataPortalActivator
  {
    public object CreateInstance(Type requestedType)
    {
      return Activator.CreateInstance(ResolveType(requestedType));
    }

    public void FinalizeInstance(object obj)
    {
    }

    public void InitializeInstance(object obj)
    {
    }

    public Type ResolveType(Type requestedType)
    {
      if (requestedType.Equals(typeof(IPerTypeAuthRoot)))
        return typeof(PerTypeAuthRoot);
      else
        return requestedType;
    }
  }

  public interface IPerTypeAuthRoot
  { }

  [Serializable]
  public class PerTypeAuthRoot : BusinessBase<PerTypeAuthRoot>, IPerTypeAuthRoot
  {
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static void AddObjectAuthorizationRules()
    {
      Csla.Rules.BusinessRules.AddRule(
        typeof(PerTypeAuthRoot), 
        new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.EditObject, "Test"));
    }
  }

  [Serializable]
  public class RootList : BusinessListBase<RootList, ChildItem>
  {
    public RootList()
    {
      using (SuppressListChangedEvents)
      {
        Add(Csla.DataPortal.CreateChild<ChildItem>());
      }
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
      {}

      protected override void Execute(IAuthorizationContext context)
      {
        context.HasPermission = false;
      }
    }
  }
}