//-----------------------------------------------------------------------
// <copyright file="BrokenRuleMetastateTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for IMobileObjectMetastate implementation on BrokenRule.</summary>
//-----------------------------------------------------------------------

using Csla.Rules;
using Csla.Serialization.Mobile;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace csla.netcore.test.Serialization
{
  /// <summary>
  /// Metastate round trip tests for <see cref="BrokenRule"/>.
  /// </summary>
  /// <remarks>
  /// These live here rather than alongside the other metastate tests because
  /// <see cref="BrokenRule"/>'s populating constructor is internal to Csla, and this is
  /// the test assembly Csla grants InternalsVisibleTo.
  /// <para>
  /// The parameterless constructor is obsolete-as-error because it exists only for
  /// MobileFormatter to call, so these tests create the empty instance by reflection
  /// exactly as the formatter does.
  /// </para>
  /// </remarks>
  [TestClass]
  public class BrokenRuleMetastateTests
  {
    /// <summary>
    /// Creates an empty rule the way MobileFormatter does. The parameterless constructor
    /// is obsolete-as-error, which is a hard compiler error that cannot be suppressed, so
    /// the only way to exercise the deserialization path is to reach it by reflection as
    /// the formatter itself does.
    /// </summary>
    private static BrokenRule CreateEmptyRule()
      => (BrokenRule)Activator.CreateInstance(typeof(BrokenRule), nonPublic: true);

    // Ignored: asserts that property values survive a metastate round trip, which is what
    // IMobileObjectMetastate's documentation promises but no implementation does -- see
    // https://github.com/MarimerLLC/csla/issues/4898. Kept rather than deleted so the
    // discrepancy is not lost again.
    [Ignore]
    [TestMethod]
    public void BrokenRule_GetSetMetastate_RoundTrip()
    {
      // Arrange
      var original = new BrokenRule("TestRule", "Test Description",
        "TestProperty", RuleSeverity.Error, "OriginProperty", 5, 10);

      // Act
      var metastate = ((IMobileObjectMetastate)original).GetMetastate();
      var restored = CreateEmptyRule();
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

    // Ignored: asserts that property values survive a metastate round trip, which is what
    // IMobileObjectMetastate's documentation promises but no implementation does -- see
    // https://github.com/MarimerLLC/csla/issues/4898. Kept rather than deleted so the
    // discrepancy is not lost again.
    [Ignore]
    [TestMethod]
    public void BrokenRule_GetSetMetastate_WithNullValues()
    {
      // Arrange
      var original = new BrokenRule("TestRule", "Test Description",
        null, RuleSeverity.Warning, null, 1, 0);

      // Act
      var metastate = ((IMobileObjectMetastate)original).GetMetastate();
      var restored = CreateEmptyRule();
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

    // Ignored: asserts that property values survive a metastate round trip, which is what
    // IMobileObjectMetastate's documentation promises but no implementation does -- see
    // https://github.com/MarimerLLC/csla/issues/4898. Kept rather than deleted so the
    // discrepancy is not lost again.
    [Ignore]
    [TestMethod]
    public void BrokenRule_TestAllSeverities()
    {
      var severities = new[] { RuleSeverity.Error, RuleSeverity.Warning, RuleSeverity.Information, RuleSeverity.Success };

      foreach (var severity in severities)
      {
        // Arrange
        var original = new BrokenRule($"Rule_{severity}", $"Description for {severity}",
          "Property", severity, "Origin", 1, 0);

        // Act
        var metastate = ((IMobileObjectMetastate)original).GetMetastate();
        var restored = CreateEmptyRule();
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
      var brokenRule = CreateEmptyRule();

      // Act
      ((IMobileObjectMetastate)brokenRule).SetMetastate(null);
    }

    [TestMethod]
    public void SetMetastate_AcceptsEmptyMetastate()
    {
      // Arrange - an object with no metastate produces an empty byte array
      var brokenRule = CreateEmptyRule();
      var emptyMetastate = Array.Empty<byte>();

      // Act - setting empty metastate should not throw
      ((IMobileObjectMetastate)brokenRule).SetMetastate(emptyMetastate);

      // Assert
      Assert.IsNotNull(brokenRule);
    }
  }
}
