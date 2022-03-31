//-----------------------------------------------------------------------
// <copyright file="AuthorizeDataPortalTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Configuration;
using Csla.DataPortalClient;
using Csla.Configuration;
using Csla.Testing.Business.DataPortal;
using Csla.Server;
using Csla.TestHelpers;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

#if !NUNIT && !ANDROID
using Microsoft.VisualStudio.TestTools.UnitTesting;
#elif !ANDROID
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif 


namespace Csla.Test.DataPortal
{
  [TestClass]
  public class AuthorizeDataPortalTests
  {
    private static TestDIContext _testDIContext;

    #region SetUp

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateContext(options =>
      {
        options.Services.AddTransient<TestableDataPortal>();
        options.DataPortal(
          dp => dp.AddServerSideDataPortal(
            config => config.RegisterAuthorizerProvider<AuthorizeDataPortalStub>())
          );
      });
    }
    
    [TestInitialize]
    public void Setup()
    {
      TestableDataPortal.Setup();

      //Csla.Configuration.ConfigurationManager.AppSettings["CslaAuthorizationProvider"] =
      //  "Csla.Testing.Business.DataPortal.AuthorizeDataPortalStub, Csla.Testing.Business";

    }

    #endregion

    #region constructor(Type authProviderType) tests

    // TODO: I don't think the new config mechanism supports setting this to an invalid value?
    //[TestMethod]
    //[ExpectedException(typeof(ArgumentNullException))]
    //public void IfAuthProviderTypeIsNull_ThenThrow_ArgumentNullException()
    //{
    //  Type authProviderType = null;
    //  new TestableDataPortal(authProviderType);
    //}

    // TODO: I don't think the new config mechanism supports setting this to an invalid value?
    //[TestMethod]
    //[ExpectedException(typeof(ArgumentException))]
    //public void IfAuthProviderTypeDoesNotImplement_IAuthorizeDataPortal_ThenThrow_ArgumentException()
    //{
    //  Type authProviderTypeNotImplementing_IAUthorizeDataPortal = typeof(object);
    //  new TestableDataPortal(authProviderTypeNotImplementing_IAUthorizeDataPortal);
    //}

    [TestMethod]
    public void IfAuthProviderTypeImplements_IAuthorizeDataPortal_Then_authorizerFieldShouldBeAnInstanceOfThatType()
    {
      var dp = _testDIContext.ServiceProvider.GetRequiredService<TestableDataPortal>();

      Assert.IsTrue(dp.AuthProviderType == typeof(AuthorizeDataPortalStub));//_authorizer field is set to correct value;
    }

    #endregion

    #region constructur(string cslaAuthorizationProvider) tests

    // TODO: I don't think this is a valid test now that config method has changed?
    //[TestMethod]
    //[ExpectedException(typeof(ArgumentNullException))]
    //public void IfCslaAuthorizationProviderAppSettingName_IsNull_ThenThrow_ArgumentNullException()
    //{
    //  const string appSettingName = null;
    //  new TestableDataPortal(appSettingName);
    //}

    // TODO: I don't think this is a valid test now that config method has changed?
    //[TestMethod]
    //public void IfCslaAuthorizationProviderAppSetting_DoesNotExist_ThenUse_NullAuthorizationProvider()
    //{
    //  var dp = new TestableDataPortal("NonExistentAppSetting");
    //  Assert.IsTrue(dp.NullAuthorizerUsed);
    //}

    // TODO: I don't think this is a valid test now that config method has changed?
    //[TestMethod]
    //[ExpectedException(typeof(TypeLoadException))]
    //public void IfCslaAuthorizationProviderAppSetting_HoldsInvalidType_ThenThrow_TypeLoadException()
    //{
    //  ConfigurationManager.AppSettings["InvalidTypeName"] ="InvalidTypeName";
    //  var dp = new TestableDataPortal("InvalidTypeName");
    //}

    // TODO: I don't think this is a valid test now that config method has changed?
    //[TestMethod]
    //public void IfCslaAuthorizationProviderAppSetting_HoldsEmptyString_ThenUse_NullAuthorizationProvider()
    //{
    //  ConfigurationManager.AppSettings["EmptyTypeName"] = string.Empty;
    //  var dp = new TestableDataPortal("EmptyTypeName");
    //
    //  Assert.IsTrue(dp.NullAuthorizerUsed);
    //}

    // TODO: I don't think this is a valid test now that config method has changed?
    //[TestMethod]
    //public void IfCslaAuthorizationProviderAppSetting_HoldsValidType_Then_authorizerFieldShouldHoldThatType()
    //{
    //  ConfigurationManager.AppSettings["ValidTypeNameSetting"] = "Csla.Testing.Business.DataPortal.AuthorizeDataPortalStub, Csla.Testing.Business";
    //  var dp = new TestableDataPortal("ValidTypeNameSetting");
    //
    //  Assert.IsTrue(typeof(AuthorizeDataPortalStub)==dp.AuthProviderType);
    //}

    #endregion

    #region default constructor() tests

    // TODO: I don't think this is a valid test now that config method has changed?
    //[TestMethod]
    //public void IfAuthorizationProvider_SetInConfigFile_DataPortal_Instantiates_AuthorizationProviderType()
    //{
    //  //Following is set in App.Config
    //  //ConfigurationManager.AppSettings["CslaAuthorizationProvider"] = "Csla.Test.Silverlight.DataPortal.AuthorizeDataPortalStub, Csla.Test";
    //  TestableDataPortal dp = new TestableDataPortal();
    //
    //  Assert.IsTrue(typeof(AuthorizeDataPortalStub) == dp.AuthProviderType);
    //}

    #endregion

    #region Authorize() tests

    [TestMethod]
    public async Task DataPortal_Create_Calls_IAuthorizeDataPortal_Authorize_WithCorrectParameters()
    {
      var applicationContext = _testDIContext.CreateTestApplicationContext();
      var dp = _testDIContext.ServiceProvider.GetRequiredService<TestableDataPortal>();
      await dp.Create(typeof(TestBO), null, new DataPortalContext(applicationContext, applicationContext.Principal, false), true);

      var result = (AuthorizeDataPortalStub)dp.AuthProvider;

      Assert.IsNotNull(result, "AuthorizeDataPortalStub not accessible");
      Assert.AreEqual(typeof(TestBO), result.ClientRequest?.ObjectType);
      Assert.AreEqual(DataPortalOperations.Create, result.ClientRequest.Operation);
    }

    [TestMethod]
    public async Task DataPortal_Fetch_Calls_IAuthorizeDataPortal_Authorize_WithCorrectParameters()
    {
      var applicationContext = _testDIContext.CreateTestApplicationContext();
      var dp = _testDIContext.ServiceProvider.GetRequiredService<TestableDataPortal>();
      await dp.Fetch(typeof(TestBO), null, new DataPortalContext(applicationContext, applicationContext.Principal, false), true);

      var result = (AuthorizeDataPortalStub)dp.AuthProvider;

      Assert.IsNotNull(result, "AuthorizeDataPortalStub not accessible");
      Assert.AreEqual(typeof(TestBO), result.ClientRequest?.ObjectType);
      Assert.AreEqual(DataPortalOperations.Fetch, result.ClientRequest.Operation);
    }

    [TestMethod]
    public async Task DataPortal_Update_Calls_IAuthorizeDataPortal_Authorize_WithCorrectParameters()
    {
      var applicationContext = _testDIContext.CreateTestApplicationContext();
      var dp = _testDIContext.ServiceProvider.GetRequiredService<TestableDataPortal>();
      await dp.Update(new TestBO(), new DataPortalContext(applicationContext, applicationContext.Principal, false), true);


      var result = (AuthorizeDataPortalStub)dp.AuthProvider;

      Assert.IsNotNull(result, "AuthorizeDataPortalStub not accessible");
      Assert.AreEqual(typeof(TestBO), result.ClientRequest?.ObjectType);
      Assert.AreEqual(DataPortalOperations.Update, result.ClientRequest.Operation);
    }

    [TestMethod]
    public async Task DataPortal_Delete_Calls_IAuthorizeDataPortal_Authorize_WithCorrectParameters()
    {
      var applicationContext = _testDIContext.CreateTestApplicationContext();
      var dp = _testDIContext.ServiceProvider.GetRequiredService<TestableDataPortal>();
      await dp.Delete(typeof(TestBO), new object(), new DataPortalContext(applicationContext, applicationContext.Principal, false), true);

      var result = (AuthorizeDataPortalStub)dp.AuthProvider;

      Assert.IsNotNull(result, "AuthorizeDataPortalStub not accessible");
      Assert.AreEqual(typeof(TestBO), result.ClientRequest?.ObjectType);
      Assert.AreEqual(DataPortalOperations.Delete, result.ClientRequest.Operation);
    }

    #endregion
  }


}