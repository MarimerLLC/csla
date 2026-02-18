//-----------------------------------------------------------------------
// <copyright file="HttpPortalTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Unit tests for HttpPortal class</summary>
//-----------------------------------------------------------------------

using System.Runtime.Serialization;
using System.Security.Principal;
using Csla.Core;
using Csla.Serialization;
using Csla.Server;
using Csla.Server.Hosts;
using Csla.Server.Hosts.DataPortalChannel;
using Csla.TestHelpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.ServerTest.Hosts;

/// <summary>
/// Unit tests for the HttpPortal class that validates
/// server-side DataPortal functionality over HTTP.
/// </summary>
[TestClass]
public class HttpPortalTests
{
  private TestDIContext _testDIContext;
  private ApplicationContext _applicationContext;
  private FakeDataPortalServer _fakeDataPortalServer;
  private HttpPortal _systemUnderTest;

  [TestInitialize]
  public void Setup()
  {
    _testDIContext = TestDIContextFactory.CreateDefaultContext(services =>
    {
      services.AddScoped<IContextDictionary, ContextDictionary>();
    });
    _applicationContext = _testDIContext.CreateTestApplicationContext();
    _fakeDataPortalServer = new FakeDataPortalServer();
    _systemUnderTest = new HttpPortal(_applicationContext, _fakeDataPortalServer);
  }

  #region Constructor Tests

  [TestMethod]
  public void Constructor_WithNullApplicationContext_ThrowsArgumentNullException()
  {
    // Arrange
    ApplicationContext nullApplicationContext = null;

    // Act & Assert
    Assert.ThrowsException<ArgumentNullException>(
      () => new HttpPortal(nullApplicationContext, _fakeDataPortalServer));
  }

  [TestMethod]
  public void Constructor_WithNullDataPortalServer_ThrowsArgumentNullException()
  {
    // Arrange
    IDataPortalServer nullDataPortalServer = null;

    // Act & Assert
    Assert.ThrowsException<ArgumentNullException>(
      () => new HttpPortal(_applicationContext, nullDataPortalServer));
  }

  [TestMethod]
  public void Constructor_WithValidParameters_CreatesInstance()
  {
    // Act & Assert
    _systemUnderTest.Should().NotBeNull();
  }

  #endregion Constructor Tests

  #region Create Method Tests

  [TestMethod]
  public async Task Create_WithNullRequest_ThrowsArgumentNullException()
  {
    // Arrange
    CriteriaRequest nullRequest = null;

    // Act & Assert
    await Assert.ThrowsExceptionAsync<ArgumentNullException>(
      () => _systemUnderTest.Create(nullRequest));
  }

  [TestMethod]
  public async Task Create_WithValidRequest_ReturnsDataPortalResponse()
  {
    // Arrange
    var serializer = _applicationContext.GetRequiredService<ISerializationFormatter>();
    var criteriaData = serializer.Serialize(new TestCslaObject());
    var principalData = serializer.Serialize((IPrincipal)null);
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

    _fakeDataPortalServer.SetReturnValue(new TestCslaObject());

    // Act
    var result = await _systemUnderTest.Create(request);

    // Assert
    result.Should().NotBeNull();
    result.ObjectData.Should().NotBeNull();
    result.ErrorData.Should().BeNull();
  }

  [TestMethod]
  public async Task Create_WithServerError_ReturnsErrorData()
  {
    // Arrange
    var serializer = _applicationContext.GetRequiredService<ISerializationFormatter>();
    var criteriaData = serializer.Serialize(new TestCslaObject());
    var principalData = serializer.Serialize((IPrincipal)null);
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

    var testException = new InvalidOperationException("Test error");
    _fakeDataPortalServer.SetErrorValue(testException);

    // Act
    var result = await _systemUnderTest.Create(request);

    // Assert
    result.Should().NotBeNull();
    result.ErrorData.Should().NotBeNull();
  }

  [TestMethod]
  public async Task Create_WithPrimitiveCriteria_UnpacksCorrectly()
  {
    // Arrange
    var serializer = _applicationContext.GetRequiredService<ISerializationFormatter>();
    var primitiveValue = 42;
    var primitiveCriteria = new DataPortalClient.PrimitiveCriteria(primitiveValue);
    var criteriaData = serializer.Serialize(primitiveCriteria);
    var principalData = serializer.Serialize((IPrincipal)null);
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

    var returnObject = new TestCslaObject();
    _fakeDataPortalServer.SetReturnValue(returnObject);

    // Act
    var result = await _systemUnderTest.Create(request);

    // Assert
    result.Should().NotBeNull();
    result.ObjectData.Should().NotBeNull();
  }

  #endregion Create Method Tests

  #region Fetch Method Tests

  [TestMethod]
  public async Task Fetch_WithNullRequest_ThrowsArgumentNullException()
  {
    // Arrange
    CriteriaRequest nullRequest = null;

    // Act & Assert
    await Assert.ThrowsExceptionAsync<ArgumentNullException>(
      () => _systemUnderTest.Fetch(nullRequest));
  }

  [TestMethod]
  public async Task Fetch_WithValidRequest_ReturnsDataPortalResponse()
  {
    // Arrange
    var serializer = _applicationContext.GetRequiredService<ISerializationFormatter>();
    var criteriaData = serializer.Serialize(new TestCslaObject());
    var principalData = serializer.Serialize((IPrincipal)null);
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

    var returnObject = new TestCslaObject();
    _fakeDataPortalServer.SetReturnValue(returnObject);

    // Act
    var result = await _systemUnderTest.Fetch(request);

    // Assert
    result.Should().NotBeNull();
    result.ObjectData.Should().NotBeNull();
    result.ErrorData.Should().BeNull();
  }

  [TestMethod]
  public async Task Fetch_WithServerError_ReturnsErrorData()
  {
    // Arrange
    var serializer = _applicationContext.GetRequiredService<ISerializationFormatter>();
    var criteriaData = serializer.Serialize(new TestCslaObject());
    var principalData = serializer.Serialize((IPrincipal)null);
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

    var testException = new InvalidOperationException("Fetch error");
    _fakeDataPortalServer.SetErrorValue(testException);

    // Act
    var result = await _systemUnderTest.Fetch(request);

    // Assert
    result.Should().NotBeNull();
    result.ErrorData.Should().NotBeNull();
  }

  #endregion Fetch Method Tests

  #region Update Method Tests

  [TestMethod]
  public async Task Update_WithNullRequest_ThrowsArgumentNullException()
  {
    // Arrange
    UpdateRequest nullRequest = null;

    // Act & Assert
    await Assert.ThrowsExceptionAsync<ArgumentNullException>(
      () => _systemUnderTest.Update(nullRequest));
  }

  [TestMethod]
  public async Task Update_WithNullObjectData_ThrowsInvalidOperationException()
  {
    // Arrange
    var principalData = Array.Empty<byte>();
    var contextData = Array.Empty<byte>();

    var request = new UpdateRequest(
      principalData,
      contextData,
      "en-US",
      "en-US",
      Array.Empty<byte>());

    // Act & Assert
    await Assert.ThrowsExceptionAsync<SerializationException>(
      () => _systemUnderTest.Update(request));
  }

  [TestMethod]
  public async Task Update_WithValidRequest_ReturnsDataPortalResponse()
  {
    // Arrange
    var serializer = _applicationContext.GetRequiredService<ISerializationFormatter>();

    // Create a minimal mock object that implements ICslaObject
    var businessObject = new TestCslaObject();
    var objectData = serializer.Serialize(businessObject);
    var principalData = serializer.Serialize((IPrincipal)null);
    var contextData = serializer.Serialize(_applicationContext.GetRequiredService<IContextDictionary>());

    var request = new UpdateRequest(
      principalData,
      contextData,
      "en-US",
      "en-US",
      objectData);

    _fakeDataPortalServer.SetReturnValue(businessObject);

    // Act
    var result = await _systemUnderTest.Update(request);

    // Assert
    result.Should().NotBeNull();
    result.ObjectData.Should().NotBeNull();
  }

  [TestMethod]
  public async Task Update_WithServerError_ReturnsErrorData()
  {
    // Arrange
    var serializer = _applicationContext.GetRequiredService<ISerializationFormatter>();
    var businessObject = new TestCslaObject();
    var objectData = serializer.Serialize(businessObject);
    var principalData = serializer.Serialize((IPrincipal)null);
    var contextData = serializer.Serialize(_applicationContext.GetRequiredService<IContextDictionary>());

    var request = new UpdateRequest(
      principalData,
      contextData,
      "en-US",
      "en-US",
      objectData);

    var testException = new InvalidOperationException("Update error");
    _fakeDataPortalServer.SetErrorValue(testException);

    // Act
    var result = await _systemUnderTest.Update(request);

    // Assert
    result.Should().NotBeNull();
    result.ErrorData.Should().NotBeNull();
  }

  #endregion Update Method Tests

  #region Delete Method Tests

  [TestMethod]
  public async Task Delete_WithNullRequest_ThrowsArgumentNullException()
  {
    // Arrange
    CriteriaRequest nullRequest = null;

    // Act & Assert
    await Assert.ThrowsExceptionAsync<ArgumentNullException>(
      () => _systemUnderTest.Delete(nullRequest));
  }

  [TestMethod]
  public async Task Delete_WithValidRequest_ReturnsDataPortalResponse()
  {
    // Arrange
    var serializer = _applicationContext.GetRequiredService<ISerializationFormatter>();
    var criteriaData = serializer.Serialize(new TestCslaObject());
    var principalData = serializer.Serialize((IPrincipal)null);
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

    _fakeDataPortalServer.SetReturnValue(new TestCslaObject());

    // Act
    var result = await _systemUnderTest.Delete(request);

    // Assert
    result.Should().NotBeNull();
  }

  [TestMethod]
  public async Task Delete_WithServerError_ReturnsErrorData()
  {
    // Arrange
    var serializer = _applicationContext.GetRequiredService<ISerializationFormatter>();
    var criteriaData = serializer.Serialize(new TestCslaObject());
    var principalData = serializer.Serialize((IPrincipal)null);
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

    var testException = new InvalidOperationException("Delete error");
    _fakeDataPortalServer.SetErrorValue(testException);

    // Act
    var result = await _systemUnderTest.Delete(request);

    // Assert
    result.Should().NotBeNull();
    result.ErrorData.Should().NotBeNull();
  }

  #endregion Delete Method Tests

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
