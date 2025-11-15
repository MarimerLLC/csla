//-----------------------------------------------------------------------
// <copyright file="ReadOnlyListBaseMetastateTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for IMobileObjectMetastate interface implementation on ReadOnlyListBase.</summary>
//-----------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csla.Serialization.Mobile;
using Csla.TestHelpers;

namespace Csla.Test.Serialization
{
  [TestClass]
  public class ReadOnlyListBaseMetastateTests
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
    public void ReadOnlyListBase_GetSetMetastate_IsReadOnly_RoundTrip()
    {
      // Arrange
      var dataPortal = _testDIContext.CreateDataPortal<TestReadOnlyList>();
      var original = dataPortal.Fetch();
      
      // The list should be readonly by default
      Assert.IsTrue(original.IsReadOnly);

      // Act - Get the metastate
      var metastate = ((IMobileObjectMetastate)original).GetMetastate();
      
      // Create a new instance and restore the metastate
      var restored = dataPortal.Fetch();
      
      // Verify before setting metastate
      Assert.IsTrue(restored.IsReadOnly);
      
      // Set the metastate
      ((IMobileObjectMetastate)restored).SetMetastate(metastate);
      
      // Assert - The IsReadOnly property should be preserved
      Assert.AreEqual(original.IsReadOnly, restored.IsReadOnly);
    }

    [TestMethod]
    public void ReadOnlyListBase_GetSetMetastate_IsReadOnly_False()
    {
      // Arrange
      var dataPortal = _testDIContext.CreateDataPortal<TestReadOnlyList>();
      var original = dataPortal.Fetch();
      
      // Unlock the list to test IsReadOnly = false
      original.SetIsReadOnlyForTest(false);
      Assert.IsFalse(original.IsReadOnly);

      // Act - Get the metastate
      var metastate = ((IMobileObjectMetastate)original).GetMetastate();
      
      // Create a new instance (which will be readonly by default)
      var restored = dataPortal.Fetch();
      Assert.IsTrue(restored.IsReadOnly);
      
      // Set the metastate
      ((IMobileObjectMetastate)restored).SetMetastate(metastate);
      
      // Assert - The IsReadOnly property should be false after restore
      Assert.AreEqual(original.IsReadOnly, restored.IsReadOnly);
      Assert.IsFalse(restored.IsReadOnly);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void ReadOnlyListBase_SetMetastate_ThrowsOnNullMetastate()
    {
      // Arrange
      var dataPortal = _testDIContext.CreateDataPortal<TestReadOnlyList>();
      var list = dataPortal.Fetch();

      // Act
      ((IMobileObjectMetastate)list).SetMetastate(null);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void ReadOnlyListBase_SetMetastate_ThrowsOnEmptyMetastate()
    {
      // Arrange
      var dataPortal = _testDIContext.CreateDataPortal<TestReadOnlyList>();
      var list = dataPortal.Fetch();

      // Act
      ((IMobileObjectMetastate)list).SetMetastate(new byte[0]);
    }

    #region Test Helper Classes

    [Serializable]
    public class TestReadOnlyList : ReadOnlyListBase<TestReadOnlyList, string>
    {
      [Fetch]
      private void DataPortal_Fetch()
      {
        // Populate with some simple test data
        IsReadOnly = false;
        Add("Item1");
        Add("Item2");
        Add("Item3");
        IsReadOnly = true;
      }

      // Expose a method to set IsReadOnly for testing
      public void SetIsReadOnlyForTest(bool value)
      {
        IsReadOnly = value;
      }
    }

    #endregion
  }
}
