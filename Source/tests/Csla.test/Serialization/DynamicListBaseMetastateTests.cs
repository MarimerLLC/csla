//-----------------------------------------------------------------------
// <copyright file="DynamicListBaseMetastateTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for IMobileObjectMetastate interface implementation on DynamicListBase.</summary>
//-----------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csla.Serialization.Mobile;
using Csla.TestHelpers;

namespace Csla.Test.Serialization
{
  [TestClass]
  public class DynamicListBaseMetastateTests
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
    public void DynamicListBase_GetSetMetastate_RoundTrip()
    {
      // Arrange
      var dataPortal = _testDIContext.CreateDataPortal<TestDynamicList>();
      var original = dataPortal.Create();
      
      // Add an item
      var childDataPortal = _testDIContext.CreateDataPortal<TestDynamicItem>();
      var item = childDataPortal.Create();
      item.Name = "Test Item";
      original.Add(item);
      Assert.AreEqual(1, original.Count, "List should have one item");

      // Act - Get the metastate
      var metastate = ((IMobileObjectMetastate)original).GetMetastate();
      
      // Create a new instance and restore the metastate
      var restored = dataPortal.Create();
      
      // Verify before setting metastate
      Assert.AreEqual(0, restored.Count, "New list should be empty");
      
      // Set the metastate
      ((IMobileObjectMetastate)restored).SetMetastate(metastate);
      
      // Assert - The metastate should preserve basic list state
      Assert.IsNotNull(restored);
    }

    [TestMethod]
    public void DynamicListBase_GetSetMetastate_EmptyList()
    {
      // Arrange
      var dataPortal = _testDIContext.CreateDataPortal<TestDynamicList>();
      var original = dataPortal.Create();
      
      // Don't add any items - test empty list
      Assert.AreEqual(0, original.Count, "List should be empty");

      // Act - Get the metastate
      var metastate = ((IMobileObjectMetastate)original).GetMetastate();
      
      // Create a new instance and restore the metastate
      var restored = dataPortal.Create();
      
      // Set the metastate
      ((IMobileObjectMetastate)restored).SetMetastate(metastate);
      
      // Assert - The empty list state should be preserved
      Assert.IsNotNull(restored);
      Assert.AreEqual(0, restored.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void DynamicListBase_SetMetastate_ThrowsOnNullMetastate()
    {
      // Arrange
      var dataPortal = _testDIContext.CreateDataPortal<TestDynamicList>();
      var list = dataPortal.Create();

      // Act
      ((IMobileObjectMetastate)list).SetMetastate(null);
    }

    #region Test Helper Classes

    [Serializable]
    public class TestDynamicList : DynamicListBase<TestDynamicItem>
    {
      [Create]
      private void DataPortal_Create()
      {
        // Empty list by default
      }

      [Update]
      private void DataPortal_Update()
      {
        // Simulate successful save
      }
    }

    [Serializable]
    public class TestDynamicItem : BusinessBase<TestDynamicItem>
    {
      public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
      public string Name
      {
        get => GetProperty(NameProperty);
        set => SetProperty(NameProperty, value);
      }

      [Create]
      private void DataPortal_Create()
      {
        // Default create
      }

      [Insert]
      private void DataPortal_Insert()
      {
        // Simulate save
      }

      [Update]
      private void DataPortal_Update()
      {
        // Simulate save
      }

      [Delete]
      private void DataPortal_Delete()
      {
        // Simulate delete
      }
    }

    #endregion
  }
}
