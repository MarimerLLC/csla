//-----------------------------------------------------------------------
// <copyright file="OperationNameFlowTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for operation name computation and flow</summary>
//-----------------------------------------------------------------------
using System.Reflection;
using Csla.Server;
using Csla.TestHelpers;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.DataPortal
{
  [TestClass]
  public class OperationNameFlowTests
  {
    private static TestDIContext _diContext = default!;

    [ClassInitialize]
    public static void ClassSetup(TestContext context)
    {
      _ = context;
      _diContext = TestDIContextFactory.CreateDefaultContext();
    }

    #region Operation Name Computation Tests

    [TestMethod]
    public void ComputeOperationName_NoCriteria_ReturnsBaseName()
    {
      // Arrange
      var method = typeof(TestRoot).GetMethod("DataPortal_Fetch", BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);

      // Act
      var operationName = DataPortalOperationNameHelper.ComputeOperationName<FetchAttribute>(method!);

      // Assert
      operationName.Should().Be("Fetch");
    }

    [TestMethod]
    public void ComputeOperationName_SingleIntParameter_ReturnsCorrectName()
    {
      // Arrange
      var method = typeof(TestRoot).GetMethod("DataPortal_Fetch", BindingFlags.NonPublic | BindingFlags.Instance, null, [typeof(int)], null);

      // Act
      var operationName = DataPortalOperationNameHelper.ComputeOperationName<FetchAttribute>(method!);

      // Assert
      operationName.Should().Be("Fetch__Int32");
    }

    [TestMethod]
    public void ComputeOperationName_SingleStringParameter_ReturnsCorrectName()
    {
      // Arrange
      var method = typeof(TestRoot).GetMethod("DataPortal_Fetch", BindingFlags.NonPublic | BindingFlags.Instance, null, [typeof(string)], null);

      // Act
      var operationName = DataPortalOperationNameHelper.ComputeOperationName<FetchAttribute>(method!);

      // Assert
      operationName.Should().Be("Fetch__String");
    }

    [TestMethod]
    public void ComputeOperationName_MultipleParameters_ReturnsCorrectName()
    {
      // Arrange
      var method = typeof(TestRoot).GetMethod("DataPortal_Fetch", BindingFlags.NonPublic | BindingFlags.Instance, null, [typeof(int), typeof(string)], null);

      // Act
      var operationName = DataPortalOperationNameHelper.ComputeOperationName<FetchAttribute>(method!);

      // Assert
      operationName.Should().Be("Fetch__Int32_String");
    }

    [TestMethod]
    public void ComputeOperationName_ArrayParameter_ReturnsCorrectName()
    {
      // Arrange
      var method = typeof(TestRoot).GetMethod("DataPortal_Fetch", BindingFlags.NonPublic | BindingFlags.Instance, null, [typeof(int[])], null);

      // Act
      var operationName = DataPortalOperationNameHelper.ComputeOperationName<FetchAttribute>(method!);

      // Assert
      operationName.Should().Be("Fetch__Int32Array");
    }

    [TestMethod]
    public void ComputeOperationName_GenericParameter_ReturnsCorrectName()
    {
      // Arrange
      var method = typeof(TestRoot).GetMethod("DataPortal_Fetch", BindingFlags.NonPublic | BindingFlags.Instance, null, [typeof(List<int>)], null);

      // Act
      var operationName = DataPortalOperationNameHelper.ComputeOperationName<FetchAttribute>(method!);

      // Assert
      operationName.Should().Be("Fetch__List_1_Int32");
    }

    [TestMethod]
    public void ComputeOperationName_WithInjectAttribute_IgnoresInjectedParameter()
    {
      // Arrange
      var method = typeof(TestRootWithDI).GetMethod("DataPortal_Fetch", BindingFlags.NonPublic | BindingFlags.Instance);

      // Act
      var operationName = DataPortalOperationNameHelper.ComputeOperationName<FetchAttribute>(method!);

      // Assert
      operationName.Should().Be("Fetch__Int32");
    }

    [TestMethod]
    public void ComputeOperationName_CreateOperation_ReturnsCorrectBaseName()
    {
      // Arrange
      var method = typeof(TestRoot).GetMethod("DataPortal_Create", BindingFlags.NonPublic | BindingFlags.Instance, null, [typeof(string)], null);

      // Act
      var operationName = DataPortalOperationNameHelper.ComputeOperationName<CreateAttribute>(method!);

      // Assert
      operationName.Should().Be("Create__String");
    }

    [TestMethod]
    public void ComputeOperationName_DeleteOperation_ReturnsCorrectBaseName()
    {
      // Arrange
      var method = typeof(TestRoot).GetMethod("DataPortal_Delete", BindingFlags.NonPublic | BindingFlags.Instance, null, [typeof(int)], null);

      // Act
      var operationName = DataPortalOperationNameHelper.ComputeOperationName<DeleteAttribute>(method!);

      // Assert
      operationName.Should().Be("Delete__Int32");
    }

    [TestMethod]
    public void ComputeOperationName_ExecuteOperation_ReturnsCorrectBaseName()
    {
      // Arrange
      var method = typeof(TestCommand).GetMethod("DataPortal_Execute", BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);

      // Act
      var operationName = DataPortalOperationNameHelper.ComputeOperationName<ExecuteAttribute>(method!);

      // Assert
      operationName.Should().Be("Execute");
    }

    #endregion

    #region Integration Tests

    [TestMethod]
    public async Task DataPortal_Fetch_FlowsOperationNameToServer()
    {
      // Arrange & Act
      var obj = await _diContext.CreateDataPortal<TestRootForFlow>().FetchAsync(123);

      // Assert
      obj.Should().NotBeNull();
      obj.OperationNameWasCalled.Should().BeTrue();
    }

    [TestMethod]
    public async Task DataPortal_Create_FlowsOperationNameToServer()
    {
      // Arrange & Act
      var obj = await _diContext.CreateDataPortal<TestRootForFlow>().CreateAsync("test");

      // Assert
      obj.Should().NotBeNull();
      obj.OperationNameWasCalled.Should().BeTrue();
    }

    [TestMethod]
    public async Task DataPortal_Execute_FlowsOperationNameToServer()
    {
      // Arrange
      var dp = _diContext.CreateDataPortal<TestCommandForFlow>();
      var cmd = await dp.CreateAsync();
      
      // Act
      cmd = await dp.ExecuteAsync(cmd);

      // Assert
      cmd.Should().NotBeNull();
      cmd.OperationNameWasCalled.Should().BeTrue();
    }

    [TestMethod]
    public async Task DataPortal_Update_FlowsInsertOperationName()
    {
      // Arrange
      var obj = await _diContext.CreateDataPortal<TestRootForFlow>().CreateAsync();

      // Act
      obj = await _diContext.CreateDataPortal<TestRootForFlow>().UpdateAsync(obj);

      // Assert
      obj.Should().NotBeNull();
      obj.OperationNameWasCalled.Should().BeTrue();
    }

    [TestMethod]
    public async Task DataPortal_Update_FlowsUpdateOperationName()
    {
      // Arrange
      var obj = await _diContext.CreateDataPortal<TestRootForFlow>().FetchAsync(123);

      // Act
      obj = await _diContext.CreateDataPortal<TestRootForFlow>().UpdateAsync(obj);

      // Assert
      obj.Should().NotBeNull();
      obj.OperationNameWasCalled.Should().BeTrue();
    }

    [TestMethod]
    public async Task DataPortal_Update_FlowsDeleteSelfOperationName()
    {
      // Arrange
      var obj = await _diContext.CreateDataPortal<TestRootForFlow>().FetchAsync(123);
      obj.Delete();

      // Act
      obj = await _diContext.CreateDataPortal<TestRootForFlow>().UpdateAsync(obj);

      // Assert
      obj.Should().NotBeNull();
      obj.OperationNameWasCalled.Should().BeTrue();
    }

    #endregion

    #region Test Classes

    [Serializable]
    public class TestRoot : BusinessBase<TestRoot>
    {
      [Fetch]
      private void DataPortal_Fetch()
      {
      }

      [Fetch]
      private void DataPortal_Fetch(int id)
      {
      }

      [Fetch]
      private void DataPortal_Fetch(string name)
      {
      }

      [Fetch]
      private void DataPortal_Fetch(int id, string name)
      {
      }

      [Fetch]
      private void DataPortal_Fetch(int[] ids)
      {
      }

      [Fetch]
      private void DataPortal_Fetch(List<int> ids)
      {
      }

      [Create]
      private void DataPortal_Create(string name)
      {
      }

      [Delete]
      private void DataPortal_Delete(int id)
      {
      }
    }

    [Serializable]
    public class TestRootWithDI : BusinessBase<TestRootWithDI>
    {
      [Fetch]
      private void DataPortal_Fetch(int id, [Inject] IServiceProvider serviceProvider)
      {
      }
    }

    [Serializable]
    public class TestCommand : CommandBase<TestCommand>
    {
      [Execute]
      private void DataPortal_Execute()
      {
      }
    }

    [Serializable]
    public class TestRootForFlow : BusinessBase<TestRootForFlow>, IDataPortalOperationNamedMapping
    {
      public static readonly PropertyInfo<bool> OperationNameWasCalledProperty = RegisterProperty<bool>(nameof(OperationNameWasCalled));
      public bool OperationNameWasCalled
      {
        get => GetProperty(OperationNameWasCalledProperty);
        private set => LoadProperty(OperationNameWasCalledProperty, value);
      }

      public Task InvokeNamedOperationAsync(string operationName, bool isSync, object?[]? criteria, IServiceProvider serviceProvider)
      {
        OperationNameWasCalled = true;
        
        // Dispatch based on operation name
        switch (operationName)
        {
          case "Create":
            DataPortal_Create();
            break;
          case "Create__String":
            if (criteria is { Length: 1 } && criteria[0] is string name)
              DataPortal_Create(name);
            break;
          case "Fetch__Int32":
            if (criteria is { Length: 1 } && criteria[0] is int id)
              DataPortal_Fetch(id);
            break;
          case "Insert":
            DataPortal_Insert();
            break;
          case "Update":
            DataPortal_Update();
            break;
          case "DeleteSelf":
            DataPortal_DeleteSelf();
            break;
          default:
            throw new DataPortalOperationNotSupportedException(operationName, criteria);
        }
        
        return Task.CompletedTask;
      }

      [Create]
      private void DataPortal_Create()
      {
      }

      [Create]
      private void DataPortal_Create(string name)
      {
      }

      [Fetch]
      private void DataPortal_Fetch(int id)
      {
        MarkOld();
      }

      [Insert]
      private void DataPortal_Insert()
      {
        MarkOld();
      }

      [Update]
      private void DataPortal_Update()
      {
      }

      [DeleteSelf]
      private void DataPortal_DeleteSelf()
      {
      }
    }

    [Serializable]
    public class TestCommandForFlow : CommandBase<TestCommandForFlow>, IDataPortalOperationNamedMapping
    {
      public static readonly PropertyInfo<bool> OperationNameWasCalledProperty = RegisterProperty<bool>(nameof(OperationNameWasCalled));
      public bool OperationNameWasCalled
      {
        get => ReadProperty(OperationNameWasCalledProperty);
        private set => LoadProperty(OperationNameWasCalledProperty, value);
      }

      public Task InvokeNamedOperationAsync(string operationName, bool isSync, object?[]? criteria, IServiceProvider serviceProvider)
      {
        OperationNameWasCalled = true;
        
        if (operationName == "Execute")
        {
          DataPortal_Execute();
          return Task.CompletedTask;
        }
        
        throw new DataPortalOperationNotSupportedException(operationName, criteria);
      }

      [Create]
      private void DataPortal_Create()
      {
      }

      [Execute]
      private void DataPortal_Execute()
      {
      }
    }

    #endregion
  }
}
