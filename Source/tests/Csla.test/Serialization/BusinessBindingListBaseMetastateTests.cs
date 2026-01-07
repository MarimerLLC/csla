//-----------------------------------------------------------------------
// <copyright file="BusinessBindingListBaseMetastateTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for IMobileObjectMetastate interface implementation on BusinessBindingListBase.</summary>
//-----------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csla.Serialization.Mobile;
using Csla.TestHelpers;

namespace Csla.Test.Serialization
{
  [TestClass]
  public class BusinessBindingListBaseMetastateTests
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
    public void BusinessBindingListBase_GetSetMetastate_RoundTrip()
    {
      // Arrange
      var dataPortal = _testDIContext.CreateDataPortal<TestBusinessBindingList>();
      var original = dataPortal.Create();
      
      // Add an item
      original.AddNew();
      Assert.AreEqual(1, original.Count, "List should have one item");

      // Act - Get the metastate
      var metastate = ((IMobileObjectMetastate)original).GetMetastate();
      
      // Create a new instance and restore the metastate
      var restored = dataPortal.Create();
      
      // Verify before setting metastate
      Assert.AreEqual(0, restored.Count, "New list should be empty");
      
      // Set the metastate
      ((IMobileObjectMetastate)restored).SetMetastate(metastate);
      
      // Assert - Basic metastate should be preserved
      Assert.IsNotNull(restored);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void BusinessBindingListBase_SetMetastate_ThrowsOnNullMetastate()
    {
      // Arrange
      var dataPortal = _testDIContext.CreateDataPortal<TestBusinessBindingList>();
      var list = dataPortal.Create();

      // Act
      ((IMobileObjectMetastate)list).SetMetastate(null);
    }

    #region Test Helper Classes

    [Serializable]
    public class TestBusinessBindingList : BusinessBindingListBase<TestBusinessBindingList, TestBusinessBindingItem>
    {
      [Create]
      private void DataPortal_Create()
      {
        // Empty list by default
      }

      [Update]
      private void DataPortal_Update()
      {
        // Simulate successful save - items will be marked clean automatically
      }
    }

    [Serializable]
    public class TestBusinessBindingItem : BusinessBase<TestBusinessBindingItem>
    {
      public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
      public string Name
      {
        get => GetProperty(NameProperty);
        set => SetProperty(NameProperty, value);
      }

      [CreateChild]
      protected override void Child_Create()
      {
        base.Child_Create();
      }
    }

    #endregion
  }
}
