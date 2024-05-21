//-----------------------------------------------------------------------
// <copyright file="AuthTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using System.Reflection;
using Csla.Rules;
using Csla.Test.Security;
using System.Diagnostics;
using System.Security.Claims;
using Csla.TestHelpers;
using Csla.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.Authorization
{
#if TESTING
  [DebuggerNonUserCode]
  [DebuggerStepThrough]
#endif
  [TestClass]
  public class AuthTests
  {
    private static TestDIContext _anonymousDIContext;
    private static TestDIContext _adminDIContext;

    private static ClaimsPrincipal GetPrincipal(params string[] roles)
    {
      var identity = new ClaimsIdentity();
      foreach (var item in roles)
        identity.AddClaim(new Claim(ClaimTypes.Role, item));
      return new ClaimsPrincipal(identity);
    }

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _anonymousDIContext = TestDIContextFactory.CreateContext(new ClaimsPrincipal());
      _adminDIContext = TestDIContextFactory.CreateContext(GetPrincipal("Admin"));
    }

    [TestInitialize]
    public void Initialize()
    {
      TestResults.Reinitialise();
    }

    [Ignore]
    [TestMethod]
    public void TestAuthCloneRules()
    {
      IDataPortal<DataPortal.DpRoot> dataPortal = _adminDIContext.CreateDataPortal<DataPortal.DpRoot>();
      ApplicationContext applicationContext = _adminDIContext.CreateTestApplicationContext();

      DataPortal.DpRoot root = dataPortal.Fetch(new DataPortal.DpRoot.Criteria());

      Assert.AreEqual(true, applicationContext.User.IsInRole("Admin"));

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
      DataPortal.DpRoot newRoot = root.Clone();

      //Is it denying read properly?
      Assert.AreEqual("[DenyReadOnProperty] Can't read property", newRoot.DenyReadOnProperty,
          "Read should have been denied 7");

      //Is it denying write properly?
      newRoot.DenyWriteOnProperty = "DenyWriteOnProperty";

      Assert.AreEqual("[DenyWriteOnProperty] Can't write variable", newRoot.Auth,
          "Write should have been denied 8");

      //Is it denying both read and write properly?
      Assert.AreEqual("[DenyReadWriteOnProperty] Can't read property", newRoot.DenyReadWriteOnProperty,
          "Read should have been denied 9");

      newRoot.DenyReadWriteOnProperty = "DenyReadWriteONproperty";

      Assert.AreEqual("[DenyReadWriteOnProperty] Can't write variable", newRoot.Auth,
          "Write should have been denied 10");

      //Is it allowing both read and write properly?
      Assert.AreEqual(newRoot.AllowReadWriteOnProperty, newRoot.Auth,
          "Read should have been allowed 11");

      newRoot.AllowReadWriteOnProperty = "AllowReadWriteOnProperty";
      Assert.AreEqual("AllowReadWriteOnProperty", newRoot.Auth,
          "Write should have been allowed 12");

      #endregion

    }

    [TestMethod]
    [TestCategory("SkipOnCIServer")]
    public void TestAuthBeginEditRules()
    {
      Guid managerInstanceId;
      TestDIContext customDIContext = TestDIContextFactory.CreateContext(GetPrincipal("Admin"));
      IDataPortal<DataPortal.DpRoot> dataPortal = customDIContext.CreateDataPortal<DataPortal.DpRoot>();
      ApplicationContext applicationContext = customDIContext.CreateTestApplicationContext();

      DataPortal.DpRoot root = dataPortal.Create(new DataPortal.DpRoot.Criteria());

      Assert.AreEqual(true, applicationContext.Principal.IsInRole("Admin"));

      root.Data = "Something new";

      root.BeginEdit();

      #region "Pre-Testing"

      root.Data = "Something new 1";

      //Is it denying read properly?
      managerInstanceId = ((ApplicationContextManagerUnitTests)applicationContext.ContextManager).InstanceId;
      Debug.WriteLine(managerInstanceId);
      string result = root.DenyReadOnProperty;
      //Assert.AreEqual(managerInstanceId.ToString(), result);
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

    }

    [TestMethod]
    [TestCategory("SkipOnCIServer")]
    public void TestAuthorizationAfterEditCycle()
    {
      TestDIContext customDIContext = TestDIContextFactory.CreateContext(GetPrincipal("Admin"));
      IDataPortal<PermissionsRoot> dataPortal = customDIContext.CreateDataPortal<PermissionsRoot>();
      ApplicationContext applicationContext = customDIContext.CreateTestApplicationContext();

      PermissionsRoot pr = dataPortal.Create();

      pr.FirstName = "something";

      pr.BeginEdit();
      pr.FirstName = "ba";
      pr.CancelEdit();

      applicationContext.User = new ClaimsPrincipal();

      PermissionsRoot prClone = pr.Clone();
      applicationContext.User = GetPrincipal("Admin");
      prClone.FirstName = "somethiansdfasdf";
    }

    [ExpectedException(typeof(Csla.Security.SecurityException))]
    [TestMethod]
    public void TestUnauthorizedAccessToGet()
    {
      IDataPortal<PermissionsRoot> dataPortal = _anonymousDIContext.CreateDataPortal<PermissionsRoot>();

      PermissionsRoot pr = dataPortal.Create();

      //this should throw an exception, since only admins have access to this property
      string something = pr.FirstName;
    }

    [ExpectedException(typeof(Csla.Security.SecurityException))]
    [TestMethod]
    public void TestUnauthorizedAccessToSet()
    {
      IDataPortal<PermissionsRoot> dataPortal = _anonymousDIContext.CreateDataPortal<PermissionsRoot>();

      PermissionsRoot pr = dataPortal.Create();

      //will cause an exception, because only admins can write to property
      pr.FirstName = "test";
    }

    [TestMethod]
    [TestCategory("SkipOnCIServer")]
    public void TestAuthorizedAccess()
    {
      TestDIContext customDIContext = TestDIContextFactory.CreateContext(GetPrincipal("Admin"));
      IDataPortal<PermissionsRoot> dataPortal = customDIContext.CreateDataPortal<PermissionsRoot>();
      ApplicationContext applicationContext = customDIContext.CreateTestApplicationContext();

      PermissionsRoot pr = dataPortal.Create();

      //should work, because we are now logged in as an admin
      pr.FirstName = "something";
      string something = pr.FirstName;

      Assert.AreEqual(true, applicationContext.Principal.IsInRole("Admin"));

      //set to null so the other testmethods continue to throw exceptions
      applicationContext.User = new ClaimsPrincipal();

      Assert.AreEqual(false, applicationContext.Principal.IsInRole("Admin"));
    }

    [TestMethod]
    [TestCategory("SkipOnCIServer")]
    public void TestAuthExecute()
    {
      TestDIContext customDIContext = TestDIContextFactory.CreateContext(GetPrincipal("Admin"));
      IDataPortal<PermissionsRoot> dataPortal = customDIContext.CreateDataPortal<PermissionsRoot>();
      ApplicationContext applicationContext = customDIContext.CreateTestApplicationContext();

      PermissionsRoot pr = dataPortal.Create();
      //should work, because we are now logged in as an admin
      pr.DoWork();

      Assert.AreEqual(true, applicationContext.Principal.IsInRole("Admin"));

      //set to null so the other testmethods continue to throw exceptions
      applicationContext.User = new ClaimsPrincipal();

      Assert.AreEqual(false, applicationContext.Principal.IsInRole("Admin"));
    }

    [TestMethod]
    [ExpectedException(typeof(Csla.Security.SecurityException))]
    public void TestUnAuthExecute()
    {
      IDataPortal<PermissionsRoot> dataPortal = _anonymousDIContext.CreateDataPortal<PermissionsRoot>();
      ApplicationContext applicationContext = _anonymousDIContext.CreateTestApplicationContext();

      Assert.AreEqual(false, applicationContext.User.IsInRole("Admin"));

      PermissionsRoot pr = dataPortal.Create();
      //should fail, because we're not an admin
      pr.DoWork();

    }

    [Ignore]
    [TestMethod]
    public void TestAuthRuleSetsOnStaticHasPermissionMethodsWhenAddingAuthzRuleSetExplicitly()
    {
      IDataPortal<PermissionsRoot> dataPortal = _adminDIContext.CreateDataPortal<PermissionsRoot>();
      ApplicationContext applicationContext = _adminDIContext.CreateTestApplicationContext();

      var root = dataPortal.Create();

      Assert.IsTrue(applicationContext.Principal.IsInRole("Admin"));
      Assert.IsFalse(applicationContext.Principal.IsInRole("User"));

      // implicit usage of ApplicationContext.RuleSet
      applicationContext.RuleSet = ApplicationContext.DefaultRuleSet;
      Assert.IsFalse(BusinessRules.HasPermission(applicationContext, AuthorizationActions.DeleteObject, typeof(PermissionsRoot)));
      applicationContext.RuleSet = "custom1";
      Assert.IsTrue(BusinessRules.HasPermission(applicationContext, AuthorizationActions.DeleteObject, typeof(PermissionsRoot)));
      applicationContext.RuleSet = "custom2";
      Assert.IsTrue(BusinessRules.HasPermission(applicationContext, AuthorizationActions.DeleteObject, typeof(PermissionsRoot)));

      applicationContext.RuleSet = ApplicationContext.DefaultRuleSet;

      // directly specifying which ruleset to use
      Assert.IsFalse(BusinessRules.HasPermission(applicationContext, AuthorizationActions.DeleteObject, typeof(PermissionsRoot), ApplicationContext.DefaultRuleSet));
      Assert.IsTrue(BusinessRules.HasPermission(applicationContext, AuthorizationActions.DeleteObject, typeof(PermissionsRoot), "custom1"));
      Assert.IsTrue(BusinessRules.HasPermission(applicationContext, AuthorizationActions.DeleteObject, typeof(PermissionsRoot), "custom2"));
    }

    [Ignore]
    [TestMethod]
    public void TestAuthRuleSetsOnStaticHasPermissionMethodsWhenAddingAuthzRuleSetUsingApplicationContextRuleSet()
    {
      IDataPortal<PermissionsRoot2> dataPortal = _adminDIContext.CreateDataPortal<PermissionsRoot2>();
      ApplicationContext applicationContext = _adminDIContext.CreateTestApplicationContext();

      var root = dataPortal.Create();

      Assert.IsTrue(applicationContext.Principal.IsInRole("Admin"));
      Assert.IsFalse(applicationContext.Principal.IsInRole("User"));

      //BusinessRules.AddRule(typeof(PermissionsRoot), new Csla.Rules.CommonRules.IsInRole(AuthorizationActions.DeleteObject, "User"), ApplicationContext.DefaultRuleSet);
      //BusinessRules.AddRule(typeof(PermissionsRoot), new Csla.Rules.CommonRules.IsInRole(AuthorizationActions.DeleteObject, "Admin"), "custom1");
      //BusinessRules.AddRule(typeof(PermissionsRoot), new Csla.Rules.CommonRules.IsInRole(AuthorizationActions.DeleteObject, "User", "Admin"), "custom2");

      // implicit usage of ApplicationContext.RuleSet
      applicationContext.RuleSet = ApplicationContext.DefaultRuleSet;
      Assert.IsFalse(BusinessRules.HasPermission(applicationContext, AuthorizationActions.DeleteObject, typeof(PermissionsRoot2)));
      applicationContext.RuleSet = "custom1";
      Assert.IsTrue(BusinessRules.HasPermission(applicationContext, AuthorizationActions.DeleteObject, typeof(PermissionsRoot2)));
      applicationContext.RuleSet = "custom2";
      Assert.IsTrue(BusinessRules.HasPermission(applicationContext, AuthorizationActions.DeleteObject, typeof(PermissionsRoot2)));

      applicationContext.RuleSet = ApplicationContext.DefaultRuleSet;

      // directly specifying which ruleset to use
      Assert.IsFalse(BusinessRules.HasPermission(applicationContext, AuthorizationActions.DeleteObject, typeof(PermissionsRoot2), ApplicationContext.DefaultRuleSet));
      Assert.IsTrue(BusinessRules.HasPermission(applicationContext, AuthorizationActions.DeleteObject, typeof(PermissionsRoot2), "custom1"));
      Assert.IsTrue(BusinessRules.HasPermission(applicationContext, AuthorizationActions.DeleteObject, typeof(PermissionsRoot2), "custom2"));
    }

    [TestMethod]

    public void TestAuthRulesCleanupAndAddAgainWhenExceptionIsThrownInAddObjectBusinessRules()
    {
      RootException.Counter = 0;
      ApplicationContext applicationContext = _anonymousDIContext.CreateTestApplicationContext();

      applicationContext.RuleSet = ApplicationContext.DefaultRuleSet;
      // AddObjectAuthorizations should throw exception
      try
      {
        BusinessRules.HasPermission(applicationContext, AuthorizationActions.DeleteObject, typeof(RootException));
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
        BusinessRules.HasPermission(applicationContext, AuthorizationActions.DeleteObject, typeof(RootException));
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
      IDataPortal<RootList> dataPortal = _anonymousDIContext.CreateDataPortal<RootList>();

      var root = dataPortal.Create();
      root.RemoveAt(0);
    }

    [TestMethod]
    public void PerTypeAuthEditObject()
    {
      TestDIContext testDIContext = TestDIContextFactory.CreateContext(
        options => options.DataPortal(
          dp => dp.AddServerSideDataPortal(
            cfg => cfg.RegisterActivator<PerTypeAuthDPActivator>())
        ));
      ApplicationContext applicationContext = testDIContext.CreateTestApplicationContext();

      Assert.IsFalse(BusinessRules.HasPermission(applicationContext, AuthorizationActions.EditObject, typeof(PerTypeAuthRoot)));
    }

    /// <summary>
    /// This test is temporarily not working because the ability to call
    /// the data portal with an interface and have that translate
    /// to a concrete type is missing. Should be resolved in #3557
    /// </summary>
    [TestMethod]
    public void PerTypeAuthEditObjectViaInterface()
    {
      TestDIContext customDIContext = TestDIContextFactory.CreateContext(
        options => options.DataPortal(
          dp => dp.AddServerSideDataPortal(cfg => cfg.RegisterActivator<PerTypeAuthDPActivator>())
      ));
      ApplicationContext applicationContext = customDIContext.CreateTestApplicationContext();

      Assert.IsFalse(BusinessRules.HasPermission(applicationContext, AuthorizationActions.EditObject, typeof(IPerTypeAuthRoot)));
    }

    [TestMethod]
    public void PerTypeAuthCreateWithCriteria()
    {
      ApplicationContext applicationContext = _anonymousDIContext.CreateTestApplicationContext();

      Assert.IsTrue(
        BusinessRules.HasPermission(
          applicationContext,
          AuthorizationActions.CreateObject,
          typeof(PermissionRootWithCriteria),
          [new PermissionRootWithCriteria.Criteria()]));

      Assert.IsFalse(
        BusinessRules.HasPermission(
          applicationContext,
          AuthorizationActions.CreateObject,
          typeof(PermissionRootWithCriteria),
          [new object()]));


      Assert.IsFalse(
        BusinessRules.HasPermission(
          applicationContext,
          AuthorizationActions.CreateObject,
          typeof(PermissionRootWithCriteria),
          (object[])null));
    }

    [TestMethod]
    public void PerTypeAuthFetchWithCriteria()
    {
      ApplicationContext applicationContext = _anonymousDIContext.CreateTestApplicationContext();

      Assert.IsTrue(
        BusinessRules.HasPermission(
          applicationContext,
          AuthorizationActions.GetObject,
          typeof(PermissionRootWithCriteria),
          [new PermissionRootWithCriteria.Criteria()]));

      Assert.IsFalse(
        BusinessRules.HasPermission(
          applicationContext,
          AuthorizationActions.GetObject,
          typeof(PermissionRootWithCriteria),
          [new object()]));


      Assert.IsFalse(
        BusinessRules.HasPermission(
          applicationContext,
          AuthorizationActions.GetObject,
          typeof(PermissionRootWithCriteria),
          (object[])null));
    }

    [TestMethod]
    public void PerTypeAuthDeleteWithCriteria()
    {
      ApplicationContext applicationContext = _anonymousDIContext.CreateTestApplicationContext();

      Assert.IsTrue(
        BusinessRules.HasPermission(
          applicationContext,
          AuthorizationActions.DeleteObject,
          typeof(PermissionRootWithCriteria),
          [new PermissionRootWithCriteria.Criteria()]));

      Assert.IsFalse(
        BusinessRules.HasPermission(
          applicationContext,
          AuthorizationActions.DeleteObject,
          typeof(PermissionRootWithCriteria),
          [new object()]));


      Assert.IsFalse(
        BusinessRules.HasPermission(
          applicationContext,
          AuthorizationActions.DeleteObject,
          typeof(PermissionRootWithCriteria),
          (object[])null));
    }
  }

  public class PerTypeAuthDPActivator(IServiceProvider serviceProvider) : Server.DefaultDataPortalActivator(serviceProvider)
  {
    public override Type ResolveType(Type requestedType)
    {
      if (requestedType.Equals(typeof(IPerTypeAuthRoot)))
        return typeof(PerTypeAuthRoot);
      else
        return base.ResolveType(requestedType);
    }
  }

  public interface IPerTypeAuthRoot;

  [Serializable]
  public class PerTypeAuthRoot : BusinessBase<PerTypeAuthRoot>, IPerTypeAuthRoot
  {
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static void AddObjectAuthorizationRules()
    {
      BusinessRules.AddRule(
        typeof(PerTypeAuthRoot),
        new Rules.CommonRules.IsInRole(AuthorizationActions.EditObject, "Test"));
    }
  }

  [Serializable]
  public class RootList : BusinessListBase<RootList, ChildItem>
  {
    public RootList()
    {
    }

    [Create]
    private void Create([Inject] IChildDataPortal<ChildItem> childDataPortal)
    {
      using (SuppressListChangedEvents)
      {
        Add(childDataPortal.CreateChild());
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

    private class NoAuth : AuthorizationRule
    {
      public NoAuth(AuthorizationActions action)
        : base(action)
      { }

      protected override void Execute(IAuthorizationContext context)
      {
        context.HasPermission = false;
      }
    }
  }
}