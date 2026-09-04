//-----------------------------------------------------------------------
// <copyright file="DataAnnotationsTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for functionality around Data Annotations attributes</summary>
//-----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;
using Csla.Testing;
using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.ValidationRules
{
  [TestClass]
  public class DataAnnotationsTests
  {
    private static CslaTestHost _testHost;

    [ClassInitialize]
    public static void Initialize(TestContext testcontext)
    {
      _testHost = CslaTestHost.Create();
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
      _testHost?.Dispose();
    }

    [TestInitialize]
    public void Initialize()
    {
      TestResults.Reinitialise();
    }

    [TestMethod]
    public void BrokenRulesCollection_ValidTestObject_ReturnsZeroBrokenRules()
    {
      // Arrange
      IDataPortal<TestObject> dataPortal = _testHost.GetDataPortal<TestObject>();
      TestObject testObject = dataPortal.Create();
      int expected = 0;
      int actual;

      // Act
      actual = testObject.BrokenRulesCollection.Count;

      // Assert
      Assert.AreEqual(expected, actual);

    }

    #region Private Helper Classes

    private class TestObject : BusinessBase<TestObject>
    {
      public static PropertyInfo<string> TestPropertyProperty = RegisterProperty<string>(nameof(TestProperty));

      [Required]
      [DIBasedTest]
      public string TestProperty 
      { 
        get { return GetProperty(TestPropertyProperty); } 
        set { SetProperty(TestPropertyProperty, value); }
      }

      [Create]
      private void Create() 
      {
        using (BypassPropertyChecks)
        {
          LoadProperty(TestPropertyProperty, "Test");
        }
        BusinessRules.CheckRules();
      }
    }

    #endregion
  }
}
