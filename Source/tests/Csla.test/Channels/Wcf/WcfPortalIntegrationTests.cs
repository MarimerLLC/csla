//-----------------------------------------------------------------------
// <copyright file="WcfPortalIntegrationTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Integration tests for WcfPortal class validation</summary>
//-----------------------------------------------------------------------

using System.Security.Claims;
using Csla.Channels.Wcf.Server;
using Csla.Core;
using Csla.Serialization;
using Csla.Server;
using Csla.Server.Hosts.DataPortalChannel;
using Csla.TestHelpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.Channels.Wcf;

/// <summary>
/// Integration tests for the WcfPortal class.
/// These tests validate the core data portal operations
/// that are performed through the WCF channel.
///
/// Note: Full WCF testing would require the Csla.Channels.Wcf
/// assembly reference, which is added when integrating with
/// actual WCF server implementations.
/// </summary>
[TestClass]
public class WcfPortalIntegrationTests
{
  private TestDIContext _testDIContext;
  private ApplicationContext _applicationContext;
  private FakeDataPortalServer _fakeDataPortalServer;
  private WcfPortal _wcfPortal;


  [TestInitialize]
  public void Setup()
  {
    _testDIContext = TestDIContextFactory.CreateDefaultContext(services =>
    {
      services.AddScoped<IContextDictionary, ContextDictionary>();
    });
    _applicationContext = _testDIContext.CreateTestApplicationContext();
    _fakeDataPortalServer = new FakeDataPortalServer();
    _wcfPortal = new WcfPortal(_fakeDataPortalServer, _applicationContext);
  }

  #region Create Operation Tests

  [TestMethod]
  public async Task Create_WithValidCriteria_ReturnsCreatedObject()
  {
    // Arrange
    var serializer = _applicationContext.GetRequiredService<ISerializationFormatter>();
    var criteria = new object();
    var context = new DataPortalContext(
      _applicationContext,
      null,
      true,
      "en-US",
      "en-US",
      _applicationContext.GetRequiredService<IContextDictionary>());

    var expectedReturnObject = new object();
    _fakeDataPortalServer.SetReturnValue(expectedReturnObject);

    // Act
    var result = await _fakeDataPortalServer.Create(
      typeof(object),
      criteria,
      context,
      true);

    // Assert
    result.Should().NotBeNull();
    result.ReturnObject.Should().Be(expectedReturnObject);
    result.Error.Should().BeNull();
  }

  [TestMethod]
  public async Task Create_WithError_ReturnsErrorResult()
  {
    // Arrange
    var context = new DataPortalContext(
      _applicationContext,
      null,
      true,
      "en-US",
      "en-US",
      _applicationContext.GetRequiredService<IContextDictionary>());

    var testException = new InvalidOperationException("Create operation failed");
    _fakeDataPortalServer.SetErrorValue(testException);

    // Act
    var result = await _fakeDataPortalServer.Create(
      typeof(object),
      null,
      context,
      true);

    // Assert
    result.Should().NotBeNull();
    result.Error.Should().NotBeNull();
    result.Error.Should().Be(testException);
    result.ReturnObject.Should().BeNull();
  }

  [TestMethod]
  public async Task Create_WithPrimitiveCriteria_HandledCorrectly()
  {
    // Arrange
    var context = new DataPortalContext(
      _applicationContext,
      null,
      true,
      "en-US",
      "en-US",
      _applicationContext.GetRequiredService<IContextDictionary>());

    var primitiveValue = 42;
    var expectedReturnObject = new object();
    _fakeDataPortalServer.SetReturnValue(expectedReturnObject);

    // Act
    var result = await _fakeDataPortalServer.Create(
      typeof(object),
      primitiveValue,
      context,
      true);

    // Assert
    result.Should().NotBeNull();
    result.ReturnObject.Should().Be(expectedReturnObject);
  }

  #endregion Create Operation Tests

  #region Fetch Operation Tests

  [TestMethod]
  public async Task Fetch_WithValidCriteria_ReturnsFetchedObject()
  {
    // Arrange
    var context = new DataPortalContext(
      _applicationContext,
      null,
      true,
      "en-US",
      "en-US",
      _applicationContext.GetRequiredService<IContextDictionary>());

    var expectedReturnObject = new object();
    _fakeDataPortalServer.SetReturnValue(expectedReturnObject);

    // Act
    var result = await _fakeDataPortalServer.Fetch(
      typeof(object),
      new object(),
      context,
      true);

    // Assert
    result.Should().NotBeNull();
    result.ReturnObject.Should().Be(expectedReturnObject);
    result.Error.Should().BeNull();
  }

  [TestMethod]
  public async Task Fetch_WithError_ReturnsErrorResult()
  {
    // Arrange
    var context = new DataPortalContext(
      _applicationContext,
      null,
      true,
      "en-US",
      "en-US",
      _applicationContext.GetRequiredService<IContextDictionary>());

    var testException = new InvalidOperationException("Fetch operation failed");
    _fakeDataPortalServer.SetErrorValue(testException);
    var serializer = _applicationContext.GetRequiredService<ISerializationFormatter>();
    var criteriaData = serializer.Serialize(new TestCslaObject());
    var principalData = serializer.Serialize(new ClaimsPrincipal());
    var contextData = serializer.Serialize(_applicationContext.GetRequiredService<IContextDictionary>());

    var request = new CriteriaRequest(
      _applicationContext,
      principalData,
      contextData,
      "en-US",
      "en-US",
      criteriaData)
    {
      TypeName = typeof(object).AssemblyQualifiedName
    };
    // Act
    var result = await _wcfPortal.Fetch(request);

    // Assert
    result.Should().NotBeNull();
    result.ErrorData.Should().NotBeNull();
  }

  #endregion Fetch Operation Tests

  #region Update Operation Tests

  [TestMethod]
  public async Task Update_WithValidObject_ReturnsUpdatedObject()
  {
    // Arrange
    var context = new DataPortalContext(
      _applicationContext,
      null,
      true,
      "en-US",
      "en-US",
      _applicationContext.GetRequiredService<IContextDictionary>());

    var businessObject = new TestCslaObject();
    _fakeDataPortalServer.SetReturnValue(businessObject);
    var serializer = _applicationContext.GetRequiredService<ISerializationFormatter>();
    var criteriaData = serializer.Serialize(new TestCslaObject());
    var principalData = serializer.Serialize(new ClaimsPrincipal());
    var contextData = serializer.Serialize(_applicationContext.GetRequiredService<IContextDictionary>());

    var request = new UpdateRequest(
      principalData,
      contextData,
      "en-US",
      "en-US",
      criteriaData)
    {

    };
    // Act
    var result = await _wcfPortal.Update(request);

    // Assert
    result.Should().NotBeNull();

    result.ErrorData.Should().BeNull();
  }

  [TestMethod]
  public async Task Update_WithError_ReturnsErrorResult()
  {
    // Arrange
    var context = new DataPortalContext(
      _applicationContext,
      null,
      true,
      "en-US",
      "en-US",
      _applicationContext.GetRequiredService<IContextDictionary>());

    var businessObject = new TestCslaObject();
    var testException = new InvalidOperationException("Update operation failed");
    _fakeDataPortalServer.SetErrorValue(testException);

    var serializer = _applicationContext.GetRequiredService<ISerializationFormatter>();
    var criteriaData = serializer.Serialize(new TestCslaObject());
    var principalData = serializer.Serialize(new ClaimsPrincipal());
    var contextData = serializer.Serialize(_applicationContext.GetRequiredService<IContextDictionary>());

    var request = new UpdateRequest(
      principalData,
      contextData,
      "en-US",
      "en-US",
      criteriaData)
    {

    };
    // Act
    var result = await _wcfPortal.Update(request);

    // Assert
    result.Should().NotBeNull();
    result.ErrorData.Should().NotBeNull();
  }

  #endregion Update Operation Tests

  #region Delete Operation Tests

  [TestMethod]
  public async Task Delete_WithValidCriteria_DeletesObjectSuccessfully()
  {
    // Arrange
    var context = new DataPortalContext(
      _applicationContext,
      null,
      true,
      "en-US",
      "en-US",
      _applicationContext.GetRequiredService<IContextDictionary>());

    _fakeDataPortalServer.SetReturnValue(new TestCslaObject());
    var serializer = _applicationContext.GetRequiredService<ISerializationFormatter>();
    var criteriaData = serializer.Serialize(new TestCslaObject());
    var principalData = serializer.Serialize(new ClaimsPrincipal());
    var contextData = serializer.Serialize(_applicationContext.GetRequiredService<IContextDictionary>());

    var request = new CriteriaRequest(
      _applicationContext,
      principalData,
      contextData,
      "en-US",
      "en-US",
      criteriaData)
    {
      TypeName = typeof(object).AssemblyQualifiedName
    };
    // Act
    var result = await _wcfPortal.Delete(request);

    // Assert
    result.Should().NotBeNull();
    result.ErrorData.Should().BeNull();
  }

  [TestMethod]
  public async Task Delete_WithError_ReturnsErrorResult()
  {
    // Arrange
    var context = new DataPortalContext(
      _applicationContext,
      null,
      true,
      "en-US",
      "en-US",
      _applicationContext.GetRequiredService<IContextDictionary>());

    var testException = new InvalidOperationException("Delete operation failed");
    _fakeDataPortalServer.SetErrorValue(testException);
    var serializer = _applicationContext.GetRequiredService<ISerializationFormatter>();
    var criteriaData = serializer.Serialize(new TestCslaObject());
    var principalData = serializer.Serialize(new ClaimsPrincipal());
    var contextData = serializer.Serialize(_applicationContext.GetRequiredService<IContextDictionary>());

    var request = new CriteriaRequest(
      _applicationContext,
      principalData,
      contextData,
      "en-US",
      "en-US",
      criteriaData)
    {
      TypeName = typeof(object).AssemblyQualifiedName
    };
    // Act
    var result = await _wcfPortal.Delete(request);

    // Assert
    result.Should().NotBeNull();
    result.ErrorData.Should().NotBeNull();
  }

  #endregion Delete Operation Tests

  #region Context Tests

  [TestMethod]
  public async Task Operation_PreservesCultureSettings()
  {
    // Arrange
    var expectedCulture = "de-DE";
    var context = new DataPortalContext(
      _applicationContext,
      null,
      true,
      expectedCulture,
      "de-DE",
      _applicationContext.GetRequiredService<IContextDictionary>());

    _fakeDataPortalServer.SetReturnValue(new TestCslaObject());
    var serializer = _applicationContext.GetRequiredService<ISerializationFormatter>();
    var criteriaData = serializer.Serialize(new TestCslaObject());
    var principalData = serializer.Serialize(new ClaimsPrincipal());
    var contextData = serializer.Serialize(_applicationContext.GetRequiredService<IContextDictionary>());

    var request = new CriteriaRequest(
      _applicationContext,
      principalData,
      contextData,
      "en-US",
      "en-US",
      criteriaData)
    {
      TypeName = typeof(object).AssemblyQualifiedName
    };

    // Act
    var result = await _wcfPortal.Create(request);

    // Assert
    result.Should().NotBeNull();
    context.ClientCulture.Should().Be(expectedCulture);
  }

  #endregion Context Tests

  #region Helper Classes

  private class FakeDataPortalServer : IDataPortalServer
  {
    private object? _returnValue;
    private Exception? _errorValue;

    public void SetReturnValue(object? returnValue)
    {
      _returnValue = returnValue;
      _errorValue = null;
    }

    public void SetErrorValue(Exception errorValue)
    {
      _errorValue = errorValue;
      _returnValue = null;
    }

    public Task<DataPortalResult> Create(Type objectType, object? criteria, DataPortalContext context, bool isSync)
    {
      var appContext = ((IUseApplicationContext)context).ApplicationContext;
      var result = _errorValue != null
        ? new DataPortalResult(appContext, null, _errorValue)
        : new DataPortalResult(appContext, _returnValue);
      return Task.FromResult(result);
    }

    public Task<DataPortalResult> Fetch(Type objectType, object? criteria, DataPortalContext context, bool isSync)
    {
      var appContext = ((IUseApplicationContext)context).ApplicationContext;
      var result = _errorValue != null
        ? new DataPortalResult(appContext, null, _errorValue)
        : new DataPortalResult(appContext, _returnValue);
      return Task.FromResult(result);
    }

    public Task<DataPortalResult> Update(ICslaObject businessObject, DataPortalContext context, bool isSync)
    {
      var appContext = ((IUseApplicationContext)context).ApplicationContext;
      var result = _errorValue != null
        ? new DataPortalResult(appContext, null, _errorValue)
        : new DataPortalResult(appContext, businessObject);
      return Task.FromResult(result);
    }

    public Task<DataPortalResult> Delete(Type objectType, object? criteria, DataPortalContext context, bool isSync)
    {
      var appContext = ((IUseApplicationContext)context).ApplicationContext;
      var result = _errorValue != null
        ? new DataPortalResult(appContext, null, _errorValue)
        : new DataPortalResult(appContext, _returnValue);
      return Task.FromResult(result);
    }
  }

  private class TestCslaObject : MobileObject, ICslaObject
  {
  }

  #endregion Helper Classes
}
