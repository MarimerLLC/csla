//-----------------------------------------------------------------------
// <copyright file="AuthorizeDataPortalTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Set of integration tests for IAUthorizeDataPortal</summary>
//-----------------------------------------------------------------------
using Csla.Testing.Business.DataPortal;
using UnitDriven;
using Csla.DataPortalClient;

using System;
#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
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
            dp.BeginFetch(1);
          });

      context.Complete();
    }

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
            dp.BeginDelete(1);
          });

      context.Complete();
    }

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

    #endregion


  }
}