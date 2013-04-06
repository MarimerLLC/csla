using System;
using System.Configuration;
using Csla.DataPortalClient;
using Csla.Testing.Business.DataPortal;
using NUnit.Framework;
using Csla.Server;


namespace Csla.Test.DataPortal
{
  [TestFixture]
  public class AuthorizeDataPortalTests
  {

    #region SetUp

    [SetUp]
    public void Setup()
    {
      TestableDataPortal.Setup();

      ConfigurationManager.AppSettings["CslaAuthorizationProvider"] =
        "Csla.Testing.Business.DataPortal.AuthorizeDataPortalStub, Csla.Testing.Business";

    }

    #endregion

    #region constructor(Type authProviderType) tests

    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void IfAuthProviderTypeIsNull_ThenThrow_ArgumentNullException()
    {
      Type authProviderType = null;
      new TestableDataPortal(authProviderType);
    }

    [Test]
    [ExpectedException(typeof(ArgumentException))]
    public void IfAuthProviderTypeDoesNotImplement_IAuthorizeDataPortal_ThenThrow_ArgumentException()
    {
      Type authProviderTypeNotImplementing_IAUthorizeDataPortal = typeof(object);
      new TestableDataPortal(authProviderTypeNotImplementing_IAUthorizeDataPortal);
    }

    [Test]
    public void IfAuthProviderTypeImplements_IAuthorizeDataPortal_Then_authorizerFieldShouldBeAnInstanceOfThatType()
    {
      Type validAuthProvider = typeof (AuthorizeDataPortalStub);
      var dp = new TestableDataPortal(validAuthProvider);

      Assert.IsTrue(validAuthProvider == dp.AuthProviderType);//_authorizer field is set to correct value;
    }

    #endregion

    #region constructur(string cslaAuthorizationProvider) tests

    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void IfCslaAuthorizationProviderAppSettingName_IsNull_ThenThrow_ArgumentNullException()
    {
      const string appSettingName = null;
      new TestableDataPortal(appSettingName);
    }

    [Test]
    public void IfCslaAuthorizationProviderAppSetting_DoesNotExist_ThenUse_NullAuthorizationProvider()
    {
      var dp = new TestableDataPortal("NonExistentAppSetting");
      Assert.IsTrue(dp.NullAuthorizerUsed);
    }

    [Test]
    [ExpectedException(typeof(TypeLoadException))]
    public void IfCslaAuthorizationProviderAppSetting_HoldsInvalidType_ThenThrow_TypeLoadException()
    {
      ConfigurationManager.AppSettings["InvalidTypeName"] ="InvalidTypeName";
      var dp = new TestableDataPortal("InvalidTypeName");
    }

    [Test]
    public void IfCslaAuthorizationProviderAppSetting_HoldsEmptyString_ThenUse_NullAuthorizationProvider()
    {
      ConfigurationManager.AppSettings["EmptyTypeName"] = string.Empty;
      var dp = new TestableDataPortal("EmptyTypeName");

      Assert.IsTrue(dp.NullAuthorizerUsed);
    }

    [Test]
    public void IfCslaAuthorizationProviderAppSetting_HoldsValidType_Then_authorizerFieldShouldHoldThatType()
    {
      ConfigurationManager.AppSettings["ValidTypeNameSetting"] = "Csla.Testing.Business.DataPortal.AuthorizeDataPortalStub, Csla.Testing.Business";
      var dp = new TestableDataPortal("ValidTypeNameSetting");

      Assert.IsTrue(typeof(AuthorizeDataPortalStub)==dp.AuthProviderType);
    }

    #endregion

    #region defaul constructor() tests

    [Test]
    public void IfAuthorizationProvider_SetInConfigFile_DataPortal_Instantiates_AuthorizationProviderType()
    {
      //Following is set in App.Config
      //ConfigurationManager.AppSettings["CslaAuthorizationProvider"] = "Csla.Test.Silverlight.DataPortal.AuthorizeDataPortalStub, Csla.Test";
      TestableDataPortal dp = new TestableDataPortal();

      Assert.IsTrue(typeof(AuthorizeDataPortalStub) == dp.AuthProviderType);
    }

    #endregion

    #region Authorize() tests

    [Test]
    public void DataPortal_Create_Calls_IAuthorizeDataPortal_Authorize_WithCorrectParameters()
    {
      var dp = new TestableDataPortal();
      dp.Create(typeof(TestBO), null, new DataPortalContext(null,false));

      var result = (AuthorizeDataPortalStub)dp.AuthProvider;//This comes from App.Config

      Assert.AreEqual(typeof(TestBO), result.ClientRequest.ObjectType);
      Assert.AreEqual(DataPortalOperations.Create, result.ClientRequest.Operation);
    }

    [Test]
    public void DataPortal_Fetch_Calls_IAuthorizeDataPortal_Authorize_WithCorrectParameters()
    {
      var dp = new TestableDataPortal();
      dp.Fetch(typeof(TestBO), null, new DataPortalContext(null, false));


      var result = (AuthorizeDataPortalStub)dp.AuthProvider;//This comes from App.Config

      Assert.AreEqual(typeof(TestBO), result.ClientRequest.ObjectType);
      Assert.AreEqual(DataPortalOperations.Fetch, result.ClientRequest.Operation);
    }

    [Test]
    public void DataPortal_Update_Calls_IAuthorizeDataPortal_Authorize_WithCorrectParameters()
    {
      var dp = new TestableDataPortal();
      dp.Update(new TestBO(), new DataPortalContext(null, false));


      var result = (AuthorizeDataPortalStub)dp.AuthProvider;//This comes from App.Config

      Assert.AreEqual(typeof(TestBO), result.ClientRequest.ObjectType);
      Assert.AreEqual(DataPortalOperations.Update, result.ClientRequest.Operation);
    }

    [Test]
    public void DataPortal_Delete_Calls_IAuthorizeDataPortal_Authorize_WithCorrectParameters()
    {
      var dp = new TestableDataPortal();
      dp.Delete(typeof(TestBO), null, new DataPortalContext(null, false));

      var result = (AuthorizeDataPortalStub)dp.AuthProvider;//This comes from App.Config

      Assert.AreEqual(typeof(TestBO), result.ClientRequest.ObjectType);
      Assert.AreEqual(DataPortalOperations.Delete, result.ClientRequest.Operation);
    }


    #endregion
  }


}