//-----------------------------------------------------------------------
// <copyright file="ReadOnlyBaseMetastateTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for IMobileObjectMetastate interface implementation on ReadOnlyBase.</summary>
//-----------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csla.Serialization.Mobile;
using Csla.TestHelpers;

namespace Csla.Test.Serialization
{
  [TestClass]
  public class ReadOnlyBaseMetastateTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestInitialize]
    public void Initialize()
    {
      TestResults.Reinitialise();
    }

    [TestMethod]
    public void ReadOnlyBase_GetSetMetastate_PropertyValues_RoundTrip()
    {
      // Arrange
      var dataPortal = _testDIContext.CreateDataPortal<TestReadOnly>();
      var original = dataPortal.Create();
      
      // Verify original values
      Assert.AreEqual("Test Name", original.Name);
      Assert.AreEqual(42, original.Number);

      // Act - Get the metastate
      var metastate = ((IMobileObjectMetastate)original).GetMetastate();
      
      // Create a new instance and restore the metastate
      var restored = dataPortal.Create();
      
      // Verify before setting metastate (should have default values)
      Assert.AreEqual("Test Name", restored.Name);
      
      // Set the metastate
      ((IMobileObjectMetastate)restored).SetMetastate(metastate);
      
      // Assert - The property values should be preserved
      Assert.AreEqual(original.Name, restored.Name);
      Assert.AreEqual(original.Number, restored.Number);
    }

    [TestMethod]
    public void ReadOnlyBase_GetSetMetastate_WithNullPropertyValues()
    {
      // Arrange
      var dataPortal = _testDIContext.CreateDataPortal<TestReadOnlyWithNulls>();
      var original = dataPortal.Create();
      
      // Verify original values
      Assert.IsNull(original.NullableValue);

      // Act - Get the metastate
      var metastate = ((IMobileObjectMetastate)original).GetMetastate();
      
      // Create a new instance and restore the metastate
      var restored = dataPortal.Create();
      
      // Set the metastate
      ((IMobileObjectMetastate)restored).SetMetastate(metastate);
      
      // Assert - Null values should be preserved
      Assert.IsNull(restored.NullableValue);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ReadOnlyBase_SetMetastate_ThrowsOnNullMetastate()
    {
      // Arrange
      var dataPortal = _testDIContext.CreateDataPortal<TestReadOnly>();
      var obj = dataPortal.Create();

      // Act
      ((IMobileObjectMetastate)obj).SetMetastate(null);
    }

    [TestMethod]
    public void ReadOnlyBase_SetMetastate_AcceptsEmptyMetastate()
    {
      // Arrange
      var dataPortal = _testDIContext.CreateDataPortal<TestReadOnly>();
      var obj = dataPortal.Create();

      // Act - Setting empty metastate should not throw
      ((IMobileObjectMetastate)obj).SetMetastate(new byte[0]);
      
      // Assert - The object should still be valid
      Assert.IsNotNull(obj);
    }

    #region Test Helper Classes

    [Serializable]
    public class TestReadOnly : ReadOnlyBase<TestReadOnly>
    {
      public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
      public string Name
      {
        get => GetProperty(NameProperty);
        private set => LoadProperty(NameProperty, value);
      }

      public static readonly PropertyInfo<int> NumberProperty = RegisterProperty<int>(nameof(Number));
      public int Number
      {
        get => GetProperty(NumberProperty);
        private set => LoadProperty(NumberProperty, value);
      }

      [Create]
      private void DataPortal_Create()
      {
        LoadProperty(NameProperty, "Test Name");
        LoadProperty(NumberProperty, 42);
      }
    }

    [Serializable]
    public class TestReadOnlyWithNulls : ReadOnlyBase<TestReadOnlyWithNulls>
    {
      public static readonly PropertyInfo<string> NullableValueProperty = RegisterProperty<string>(nameof(NullableValue));
      public string NullableValue
      {
        get => GetProperty(NullableValueProperty);
        private set => LoadProperty(NullableValueProperty, value);
      }

      [Create]
      private void DataPortal_Create()
      {
        LoadProperty(NullableValueProperty, null);
      }
    }

    #endregion
  }
}
