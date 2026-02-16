//-----------------------------------------------------------------------
// <copyright file="MobileObjectMetastateTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for IMobileObjectMetastate interface implementation.</summary>
//-----------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csla.Rules;
using Csla.Serialization.Mobile;

namespace Csla.Test.Serialization
{
  [TestClass]
  public class MobileObjectMetastateTests
  {
    [TestMethod]
    public void BrokenRule_GetSetMetastate_RoundTrip()
    {
      // Arrange
      var original = new BrokenRule("TestRule", "Test Description", 
        "TestProperty", RuleSeverity.Error, "OriginProperty", 5, 10);

      // Act
      var metastate = ((IMobileObjectMetastate)original).GetMetastate();
      var restored = new BrokenRule();
      ((IMobileObjectMetastate)restored).SetMetastate(metastate);
      
      // Assert
      Assert.AreEqual(original.RuleName, restored.RuleName);
      Assert.AreEqual(original.Description, restored.Description);
      Assert.AreEqual(original.Property, restored.Property);
      Assert.AreEqual(original.Severity, restored.Severity);
      Assert.AreEqual(original.OriginProperty, restored.OriginProperty);
      Assert.AreEqual(original.Priority, restored.Priority);
      Assert.AreEqual(original.DisplayIndex, restored.DisplayIndex);
    }

    [TestMethod]
    public void BrokenRule_GetSetMetastate_WithNullValues()
    {
      // Arrange
      var original = new BrokenRule("TestRule", "Test Description", 
        null, RuleSeverity.Warning, null, 1, 0);

      // Act
      var metastate = ((IMobileObjectMetastate)original).GetMetastate();
      var restored = new BrokenRule();
      ((IMobileObjectMetastate)restored).SetMetastate(metastate);
      
      // Assert
      Assert.AreEqual(original.RuleName, restored.RuleName);
      Assert.AreEqual(original.Description, restored.Description);
      Assert.IsNull(restored.Property);
      Assert.AreEqual(original.Severity, restored.Severity);
      Assert.IsNull(restored.OriginProperty);
      Assert.AreEqual(original.Priority, restored.Priority);
      Assert.AreEqual(original.DisplayIndex, restored.DisplayIndex);
    }

    [TestMethod]
    public void BrokenRule_TestAllSeverities()
    {
      // Test all enum values
      var severities = new[] { RuleSeverity.Error, RuleSeverity.Warning, RuleSeverity.Information, RuleSeverity.Success };
      
      foreach (var severity in severities)
      {
        // Arrange
        var original = new BrokenRule($"Rule_{severity}", $"Description for {severity}", 
          "Property", severity, "Origin", 1, 0);

        // Act
        var metastate = ((IMobileObjectMetastate)original).GetMetastate();
        var restored = new BrokenRule();
        ((IMobileObjectMetastate)restored).SetMetastate(metastate);
        
        // Assert
        Assert.AreEqual(severity, restored.Severity, $"Severity {severity} not preserved");
        Assert.AreEqual(original.RuleName, restored.RuleName);
      }
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void SetMetastate_ThrowsOnNullMetastate()
    {
      // Arrange
      var brokenRule = new BrokenRule();

      // Act
      ((IMobileObjectMetastate)brokenRule).SetMetastate(null);
    }

    [TestMethod]
    public void SetMetastate_AcceptsEmptyMetastate()
    {
      // Arrange - An object with no metastate will produce an empty byte array
      var brokenRule = new BrokenRule();
      var emptyMetastate = new byte[0];
   
      // Act - Setting empty metastate should not throw
      ((IMobileObjectMetastate)brokenRule).SetMetastate(emptyMetastate);
      
      // Assert - The object should still be valid
      Assert.IsNotNull(brokenRule);
    }

    [TestMethod]
    public void CommandBase_GetSetMetastate_EmptyMetastate_RoundTrip()
    {
      // Arrange - Create a CommandBase-derived object with no custom metastate
      // CommandBase objects that don't override OnGetMetastate/OnSetMetastate
      // will return/accept empty byte arrays
      var testDIContext = TestHelpers.TestDIContextFactory.CreateDefaultContext();
      var dataPortal = testDIContext.CreateDataPortal<Test.CommandBase.CommandObject>();
      var original = dataPortal.Create();
      
      // Act - Get the metastate from an object with no field state
      var metastate = ((IMobileObjectMetastate)original).GetMetastate();
      
      // Create a new instance and restore the empty metastate
      var restored = dataPortal.Create();
      ((IMobileObjectMetastate)restored).SetMetastate(metastate);
      
      // Assert - Should complete without exception
      // The empty metastate should have been successfully set
      Assert.IsNotNull(restored);
      Assert.AreEqual(original.Name, restored.Name);
      Assert.AreEqual(original.Num, restored.Num);
    }

    [TestMethod]
    public void CommandBase_GetSetMetastate_PropertyValues_RoundTrip()
    {
      // Arrange - Create a CommandBase-derived object with property values
      var testDIContext = TestHelpers.TestDIContextFactory.CreateDefaultContext();
      var dataPortal = testDIContext.CreateDataPortal<Test.CommandBase.CommandObject>();
      var original = dataPortal.Create();
      
      // Set some property values using ObjectFactory pattern
      var factory = new Test.CommandBase.CommandBaseTest(testDIContext.CreateTestApplicationContext());
      factory.LoadProperty(original, Test.CommandBase.CommandObject.NameProperty, "Test Command");
      factory.LoadProperty(original, Test.CommandBase.CommandObject.NumProperty, 123);

      // Act - Get the metastate
      var metastate = ((IMobileObjectMetastate)original).GetMetastate();
      
      // Create a new instance and restore the metastate
      var restored = dataPortal.Create();
      ((IMobileObjectMetastate)restored).SetMetastate(metastate);
      
      // Assert - Property values should be preserved
      Assert.AreEqual("Test Command", restored.Name);
      Assert.AreEqual(123, restored.Num);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CommandBase_SetMetastate_ThrowsOnNullMetastate()
    {
      // Arrange
      var testDIContext = TestHelpers.TestDIContextFactory.CreateDefaultContext();
      var dataPortal = testDIContext.CreateDataPortal<Test.CommandBase.CommandObject>();
      var command = dataPortal.Create();

      // Act
      ((IMobileObjectMetastate)command).SetMetastate(null);
    }

    [TestMethod]
    public void BusinessBase_GetSetMetastate_FetchedObject_FlagPreservation()
    {
      // Arrange - Create a simple test object that derives from BusinessBase
      var original = new Csla.Test.BasicModern.Root();
      
      // Simulate a fetched object by marking it as old (not new, not dirty)
      original.MarkOld();
      
      // Verify initial state
      Assert.IsFalse(original.IsNew, "After MarkOld, object should not be new");
      Assert.IsFalse(original.IsDirty, "After MarkOld, object should not be dirty");
      
      // Act - Get the metastate from the fetched object
      var metastate = ((IMobileObjectMetastate)original).GetMetastate();
      
      // Create a new instance and restore the metastate
      var restored = new Csla.Test.BasicModern.Root();
      ((IMobileObjectMetastate)restored).SetMetastate(metastate);
      
      // Assert - The flag properties should be preserved after deserialization
      Assert.IsFalse(restored.IsNew, "Deserialized object should preserve IsNew=false");
      Assert.IsFalse(restored.IsDirty, "Deserialized object should preserve IsDirty=false");
    }
  }
}
