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
using System.Threading.Tasks;
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
    [TestMethod]
    public async Task IAuthorizeDataPortal_Implementation_Allows_Creation()
    {
      //setup for the test
      var command = new SetAppSettingValueCmd(
        "CslaAuthorizationProvider",
        "Csla.Testing.Business.DataPortal.AuthorizeDataPortalStub, Csla.Testing.Business");
      await Csla.DataPortal.ExecuteAsync(command);
      //actual test
      var dp = new DataPortal<TestBO>();
      await dp.CreateAsync();

      //Setting CslaAuthorizationnProvider to "" will force NullAuthorizer - we can not set it here as it is protected class
      command = new SetAppSettingValueCmd("CslaAuthorizationProvider", "");
      await Csla.DataPortal.ExecuteAsync(command);
    }

    [TestMethod]
    public async Task IAuthorizeDataPortal_Implementation_Allows_Fetch()
    {
      //setup for the test
      var command = new SetAppSettingValueCmd(
        "CslaAuthorizationProvider",
        "Csla.Testing.Business.DataPortal.AuthorizeDataPortalStub, Csla.Testing.Business");
      await Csla.DataPortal.ExecuteAsync(command);
      //actual test
      var dp = new DataPortal<TestBO>();
      await dp.FetchAsync();

      //Setting CslaAuthorizationnProvider to "" will force NullAuthorizer - we can not set it here as it is protected class
      command = new SetAppSettingValueCmd("CslaAuthorizationProvider", "");
      await Csla.DataPortal.ExecuteAsync(command);
    }

    [TestMethod]
    public async Task IAuthorizeDataPortal_Implementation_Allows_Delete()
    {
      var cmd = new SetAppSettingValueCmd(
        "CslaAuthorizationProvider",
        "Csla.Testing.Business.DataPortal.AuthorizeDataPortalStub, Csla.Testing.Business");
      await Csla.DataPortal.ExecuteAsync(cmd);

      //Setting CslaAuthorizationnProvider to "" will force NullAuthorizer - we can not set it here as it is protected class
      var command = new SetAppSettingValueCmd("CslaAuthorizationProvider", "");
      await Csla.DataPortal.ExecuteAsync(command);

    }

    [TestMethod]
    public async Task IAuthorizeDataPortal_Implementation_Allows_Update()
    {
      //setup for the test
      var cmd = new SetAppSettingValueCmd(
        "CslaAuthorizationProvider",
        "Csla.Testing.Business.DataPortal.AuthorizeDataPortalStub, Csla.Testing.Business");
      await Csla.DataPortal.ExecuteAsync(cmd);
      //actual test
      var dp = new DataPortal<TestBO>();
      var obj = await dp.CreateAsync();
      await dp.UpdateAsync(obj);

      //Setting CslaAuthorizationnProvider to "" will force NullAuthorizer - we can not set it here as it is protected class
      var command = new SetAppSettingValueCmd("CslaAuthorizationProvider", "");
      await Csla.DataPortal.ExecuteAsync(command);

    }
  }
}