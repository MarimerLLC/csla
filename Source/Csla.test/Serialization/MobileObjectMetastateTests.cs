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
    [ExpectedException(typeof(ArgumentException))]
    public void SetMetastate_ThrowsOnEmptyMetastate()
    {
      // Arrange
      var brokenRule = new BrokenRule();
   
      // Act
      ((IMobileObjectMetastate)brokenRule).SetMetastate(new byte[0]);
    }
  }
}
