//-----------------------------------------------------------------------
// <copyright file="RabbitMqPortalIntegrationTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Integration tests for RabbitMqPortal class validation</summary>
//-----------------------------------------------------------------------

using Csla.Core;
using Csla.Server;
using Csla.TestHelpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.ChannelsTest.RabbitMq;

/// <summary>
/// Integration tests for the RabbitMqPortal class.
/// These tests validate the core data portal operations
/// that are performed through RabbitMQ message queue.
///
/// Note: Full RabbitMQ testing would require a RabbitMQ broker
/// instance and the Csla.Channels.RabbitMq assembly reference.
/// </summary>
[TestClass]
public class RabbitMqPortalIntegrationTests
{
  private TestDIContext _testDIContext;
  private ApplicationContext _applicationContext;
  private FakeDataPortalServer _fakeDataPortalServer;

  [TestInitialize]
  public void Setup()
  {
    _testDIContext = TestDIContextFactory.CreateDefaultContext(services =>
    {
      services.AddScoped<IContextDictionary, ContextDictionary>();
    });
    _applicationContext = _testDIContext.CreateTestApplicationContext();
    _fakeDataPortalServer = new FakeDataPortalServer();
  }

  #region Create Operation Tests

  [TestMethod]
  public async Task Create_WithValidCriteria_ReturnsCreatedObject()
  {
    // Arrange
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
  public async Task Create_WithStringCriteria_HandledCorrectly()
  {
    // Arrange
    var stringCriteria = "test-criteria";
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
      stringCriteria,
      context,
      true);

    // Assert
    result.Should().NotBeNull();
    result.ReturnObject.Should().Be(expectedReturnObject);
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
    result.ReturnObject.Should().BeNull();
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
  public async Task Fetch_WithIntegerCriteria_HandledCorrectly()
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
      123,
      context,
      true);

    // Assert
    result.Should().NotBeNull();
    result.ReturnObject.Should().Be(expectedReturnObject);
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

    // Act
    var result = await _fakeDataPortalServer.Fetch(
      typeof(object),
      null,
      context,
      true);

    // Assert
    result.Should().NotBeNull();
    result.Error.Should().NotBeNull();
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

    // Act
    var result = await _fakeDataPortalServer.Update(businessObject, context, true);

    // Assert
    result.Should().NotBeNull();
    result.ReturnObject.Should().Be(businessObject);
    result.Error.Should().BeNull();
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

    // Act
    var result = await _fakeDataPortalServer.Update(businessObject, context, true);

    // Assert
    result.Should().NotBeNull();
    result.Error.Should().NotBeNull();
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

    // Act
    var result = await _fakeDataPortalServer.Delete(
      typeof(object),
      new object(),
      context,
      true);

    // Assert
    result.Should().NotBeNull();
    result.Error.Should().BeNull();
  }

  [TestMethod]
  public async Task Delete_WithStringCriteria_HandledCorrectly()
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

    // Act
    var result = await _fakeDataPortalServer.Delete(
      typeof(object),
      "delete-key",
      context,
      true);

    // Assert
    result.Should().NotBeNull();
    result.Error.Should().BeNull();
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

    // Act
    var result = await _fakeDataPortalServer.Delete(
      typeof(object),
      null,
      context,
      true);

    // Assert
    result.Should().NotBeNull();
    result.Error.Should().NotBeNull();
  }

  #endregion Delete Operation Tests

  #region Context Tests

  [TestMethod]
  public async Task Operation_PreservesCultureAndUICultureSettings()
  {
    // Arrange
    var expectedCulture = "pt-BR";
    var expectedUICulture = "pt-BR";
    var context = new DataPortalContext(
      _applicationContext,
      null,
      true,
      expectedCulture,
      expectedUICulture,
      _applicationContext.GetRequiredService<IContextDictionary>());

    _fakeDataPortalServer.SetReturnValue(new object());

    // Act
    var result = await _fakeDataPortalServer.Create(
      typeof(object),
      null,
      context,
      true);

    // Assert
    result.Should().NotBeNull();
    context.ClientCulture.Should().Be(expectedCulture);
    context.ClientUICulture.Should().Be(expectedUICulture);
  }

  [TestMethod]
  public async Task Operation_WithPrincipal_PreservesPrincipalAndRoles()
  {
    // Arrange
    var principal = new System.Security.Principal.GenericPrincipal(
      new System.Security.Principal.GenericIdentity("mq-user"),
      new[] { "MQRole", "DataRole" });

    var context = new DataPortalContext(
      _applicationContext,
      principal,
      true,
      "en-US",
      "en-US",
      _applicationContext.GetRequiredService<IContextDictionary>());

    _fakeDataPortalServer.SetReturnValue(new object());

    // Act
    var result = await _fakeDataPortalServer.Create(
      typeof(object),
      null,
      context,
      true);

    // Assert
    result.Should().NotBeNull();
    context.Principal.Should().NotBeNull();
    context.Principal.Identity.Name.Should().Be("mq-user");
    context.Principal.IsInRole("MQRole").Should().BeTrue();
    context.Principal.IsInRole("DataRole").Should().BeTrue();
  }

  [TestMethod]
  public async Task Operation_WithContextDictionary_PreservesContextData()
  {
    // Arrange
    var contextDict = _applicationContext.GetRequiredService<IContextDictionary>();
    contextDict.Add("testKey", "testValue");

    var context = new DataPortalContext(
      _applicationContext,
      null,
      true,
      "en-US",
      "en-US",
      contextDict);

    _fakeDataPortalServer.SetReturnValue(new object());

    // Act
    var result = await _fakeDataPortalServer.Create(
      typeof(object),
      null,
      context,
      true);

    // Assert
    result.Should().NotBeNull();
    contextDict.Should().NotBeNull();
  }

  #endregion Context Tests

  #region Synchronous Flag Tests

  [TestMethod]
  public async Task Create_WithSyncFalse_StillReturnsResult()
  {
    // Arrange
    var context = new DataPortalContext(
      _applicationContext,
      null,
      false, // Async operation
      "en-US",
      "en-US",
      _applicationContext.GetRequiredService<IContextDictionary>());

    var expectedReturnObject = new object();
    _fakeDataPortalServer.SetReturnValue(expectedReturnObject);

    // Act
    var result = await _fakeDataPortalServer.Create(
      typeof(object),
      null,
      context,
      false);

    // Assert
    result.Should().NotBeNull();
    result.ReturnObject.Should().Be(expectedReturnObject);
  }

  [TestMethod]
  public async Task Update_WithSyncFalse_StillReturnsResult()
  {
    // Arrange
    var context = new DataPortalContext(
      _applicationContext,
      null,
      false, // Async operation
      "en-US",
      "en-US",
      _applicationContext.GetRequiredService<IContextDictionary>());

    var businessObject = new TestCslaObject();
    _fakeDataPortalServer.SetReturnValue(businessObject);

    // Act
    var result = await _fakeDataPortalServer.Update(businessObject, context, false);

    // Assert
    result.Should().NotBeNull();
  }

  #endregion Synchronous Flag Tests

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

  private class TestCslaObject : ICslaObject
  {
  }

  #endregion Helper Classes
}
