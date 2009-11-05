using Csla.Testing.Business.DataPortal;
using UnitDriven;
using Csla.DataPortalClient;

#if SILVERLIGHT

#else
using System;
#endif

#if NUNIT
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
#endif


namespace Csla.Test.Silverlight.DataPortal
{
  /// <summary>
  /// Set of integration tests for IAUthorizeDataPortal
  /// Notice 2 roundtrips to the server in each test, first being Execute Command,
  /// and second being a Data Portal call being tested. (Create, Fatch, Delete, and Update).
  /// The reason for the first roundtrip is to set the appSetting in App.Config to value
  /// needed by the test
  /// </summary>
  [TestClass]
  public class AuthorizeDataPortalTests : TestBase
  {
    [TestInitialize]
    public void Setup()
    {
      #if SILVERLIGHT
      Csla.DataPortal.ProxyTypeName = typeof(SynchronizedWcfProxy<>).AssemblyQualifiedName;
      #endif
    }

    [TestCleanup]
    public void Teardown()
    {
      //Setting CslaAuthorizationnProvider to "" will force NullAuthorizer - we can not set it here as it is protected class
      SetAppSettingValueCmd.ExecuteCommand(
        "CslaAuthorizationProvider", "",
        (o, e) =>
        { });
    }

    #region DataPortal.Create() Tests

    [TestMethod]
    public void IAuthorizeDataPortal_Implementation_Allows_Creation()
    {
      var context = GetContext();

      //setup for the test
      SetAppSettingValueCmd
        .ExecuteCommand(
        "CslaAuthorizationProvider",
        "Csla.Testing.Business.DataPortal.AuthorizeDataPortalStub, Csla.Testing.Business",
        (o, e) =>
          {
            //actual test
            var dp = new DataPortal<TestBO>();
            dp.CreateCompleted += ((sender, e1) =>
             {
               context.Assert.IsNull(e1.Error);
               context.Assert.Success();
             });
            dp.BeginCreate();
          });

      context.Complete();
    }

#if SILVERLIGHT
    [TestMethod]
    public void IAuthorizeDataPortal_Implementation_DoesNotAllow_Creation()
    {
      var context = GetContext();

      //setup for the test
      SetAppSettingValueCmd
        .ExecuteCommand(
        "CslaAuthorizationProvider",
        "Csla.Testing.Business.DataPortal.DontAuthorizeDataPortalStub, Csla.Testing.Business",
        (o, e) =>
          {
            //actual test
            var dp = new DataPortal<TestBO>();
            dp.CreateCompleted += ((sender, e1) =>
             {
               context.Assert.IsNotNull(e1.Error);
               //TODO: perhaps check to assure that exception type is System.Security.SecurityException
               //context.Assert.IsTrue(((Csla.DataPortalException)(e1.Error)).ErrorInfo.InnerError.ExceptionTypeName=="System.Security.SecurityException");
               context.Assert.Success();
             });
            dp.BeginCreate();
          });

      context.Complete();
    }
#else
#endif

    #endregion

    #region DataPortal.Fetch() Tests

    [TestMethod]
    public void IAuthorizeDataPortal_Implementation_Allows_Fetch()
    {
      var context = GetContext();

      //setup for the test
      SetAppSettingValueCmd
        .ExecuteCommand(
        "CslaAuthorizationProvider",
        "Csla.Testing.Business.DataPortal.AuthorizeDataPortalStub, Csla.Testing.Business",
        (o, e) =>
          {
            //actual test
            var dp = new DataPortal<TestBO>();
            dp.FetchCompleted += ((sender, e1) =>
              {
                context.Assert.IsNull(e1.Error);
                context.Assert.Success();
              });
            dp.BeginFetch(new SingleCriteria<TestBO,int>(1));
          });

      context.Complete();
    }

#if SILVERLIGHT
    [TestMethod]
    public void IAuthorizeDataPortal_Implementation_DoesNotAllow_Fetch()
    {
      var context = GetContext();

      //setup for the test
      SetAppSettingValueCmd
        .ExecuteCommand(
        "CslaAuthorizationProvider",
        "Csla.Testing.Business.DataPortal.DontAuthorizeDataPortalStub, Csla.Testing.Business",
        (o, e) =>
          {
            //actual test
            var dp = new DataPortal<TestBO>();
            dp.FetchCompleted += ((sender, e1) =>
            {
              context.Assert.IsNotNull(e1.Error);
              //TODO: perhaps check to assure that exception type is System.Security.SecurityException
              //context.Assert.IsTrue(((Csla.DataPortalException)(e1.Error)).ErrorInfo.InnerError.ExceptionTypeName=="System.Security.SecurityException");
              context.Assert.Success();
            });
            dp.BeginFetch();
          });

      context.Complete();
    }
#else
#endif

    #endregion

    #region DataPortal.Delete() Tests

    [TestMethod]
    public void IAuthorizeDataPortal_Implementation_Allows_Delete()
    {
      var context = GetContext();

      //setup for the test
      SetAppSettingValueCmd
        .ExecuteCommand(
        "CslaAuthorizationProvider",
        "Csla.Testing.Business.DataPortal.AuthorizeDataPortalStub, Csla.Testing.Business",
        (o, e) =>
          {
            //actual test
            var dp = new DataPortal<TestBO>();
            dp.DeleteCompleted += ((sender, e1) =>
             {
               context.Assert.IsNull(e1.Error);
               context.Assert.Success();
             });
            dp.BeginDelete(new SingleCriteria<TestBO,int>(1));
          });

      context.Complete();
    }

#if SILVERLIGHT
    [TestMethod]
    public void IAuthorizeDataPortal_Implementation_DoesNotAllow_Delete()
    {
      var context = GetContext();

      //setup for the test
      SetAppSettingValueCmd
        .ExecuteCommand(
        "CslaAuthorizationProvider",
        "Csla.Testing.Business.DataPortal.DontAuthorizeDataPortalStub, Csla.Testing.Business",
        (o, e) =>
          {
            //actual test
            var dp = new DataPortal<TestBO>();
            dp.DeleteCompleted += ((sender, e1) =>
             {
               context.Assert.IsNotNull(e1.Error);
               //TODO: perhaps check to assure that exception type is System.Security.SecurityException
               //context.Assert.IsTrue(((Csla.DataPortalException)(e1.Error)).ErrorInfo.InnerError.ExceptionTypeName=="System.Security.SecurityException");
               context.Assert.Success();
             });
            dp.BeginDelete(new SingleCriteria<TestBO, int>(1));
          });

      context.Complete();
    }
#else
#endif

    #endregion

    #region DataPortal.Update() Tests

    [TestMethod]
    public void IAuthorizeDataPortal_Implementation_Allows_Update()
    {
      var context = GetContext();

      //setup for the test
      SetAppSettingValueCmd
        .ExecuteCommand(
        "CslaAuthorizationProvider",
        "Csla.Testing.Business.DataPortal.AuthorizeDataPortalStub, Csla.Testing.Business",
        (o, e) =>
          {
            //actual test
            var dp = new DataPortal<TestBO>();
            dp.UpdateCompleted += ((sender, e1) =>
             {
               context.Assert.IsNull(e1.Error);
               context.Assert.Success();
             });
            dp.BeginUpdate(new TestBO());
          });

      context.Complete();
    }

#if SILVERLIGHT
    [TestMethod]
    public void IAuthorizeDataPortal_Implementation_DoesNotAllow_Uodate()
    {
      var context = GetContext();

      //setup for the test
      SetAppSettingValueCmd
        .ExecuteCommand(
        "CslaAuthorizationProvider",
        "Csla.Testing.Business.DataPortal.DontAuthorizeDataPortalStub, Csla.Testing.Business",
        (o, e) =>
          {
            //actual test
            var dp = new DataPortal<TestBO>();
            dp.UpdateCompleted += ((sender, e1) =>
             {
               context.Assert.IsNotNull(e1.Error);
               //TODO: perhaps check to assure that exception type is System.Security.SecurityException
               //context.Assert.IsTrue(((Csla.DataPortalException)(e1.Error)).ErrorInfo.InnerError.ExceptionTypeName=="System.Security.SecurityException");
               context.Assert.Success();
             });
            dp.BeginUpdate(new TestBO());
          });

      context.Complete();
    }

#else
#endif

    #endregion


  }
}
