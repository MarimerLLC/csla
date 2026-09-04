//-----------------------------------------------------------------------
// <copyright file="MobileObjectMetastateTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for IMobileObjectMetastate interface implementation.</summary>
//-----------------------------------------------------------------------

using Csla.Rules;
using Csla.Serialization.Mobile;
using Csla.Testing;
using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.Serialization
{
  /// <summary>
  /// Tests for <see cref="IMobileObjectMetastate"/>, the channel an external serializer
  /// uses to round trip the non-public state of a business object -- its IsNew, IsDirty
  /// and related bookkeeping -- without reaching for private fields by reflection.
  /// </summary>
  /// <remarks>
  /// The byte array is opaque: a type with no non-public state to carry returns an empty
  /// one, and an empty array is valid input. See issues #4263 and #4767.
  /// </remarks>
  [TestClass]
  public class MobileObjectMetastateTests
  {
    private static CslaTestHost _testHost;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
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
    public void CommandBase_GetSetMetastate_EmptyMetastate_RoundTrip()
    {
      // Arrange - a CommandBase object has no non-public state to carry, so its
      // metastate is an empty byte array, which must round trip without complaint
      var dataPortal = _testHost.GetDataPortal<Test.CommandBase.CommandObject>();
      var original = dataPortal.Create();

      // Act
      var metastate = ((IMobileObjectMetastate)original).GetMetastate();

      var restored = dataPortal.Create();
      ((IMobileObjectMetastate)restored).SetMetastate(metastate);

      // Assert
      Assert.IsNotNull(restored);
      Assert.AreEqual(original.Name, restored.Name);
      Assert.AreEqual(original.Num, restored.Num);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void CommandBase_SetMetastate_ThrowsOnNullMetastate()
    {
      // Arrange
      var dataPortal = _testHost.GetDataPortal<Test.CommandBase.CommandObject>();
      var command = dataPortal.Create();

      // Act
      ((IMobileObjectMetastate)command).SetMetastate(null);
    }

    [TestMethod]
    public void BusinessBase_GetSetMetastate_FetchedObject_FlagPreservation()
    {
      // Arrange - a fetched object is old and clean, and preserving those flags across
      // a round trip is the whole point of the interface
      var dataPortal = _testHost.GetDataPortal<MetastateRoot>();
      var original = dataPortal.Fetch(1);

      Assert.IsFalse(original.IsNew, "A fetched object should not be new");
      Assert.IsFalse(original.IsDirty, "A fetched object should not be dirty");

      // Act
      var metastate = ((IMobileObjectMetastate)original).GetMetastate();

      var restored = dataPortal.Create();
      ((IMobileObjectMetastate)restored).SetMetastate(metastate);

      // Assert
      Assert.IsFalse(restored.IsNew, "Deserialized object should preserve IsNew=false");
      Assert.IsFalse(restored.IsDirty, "Deserialized object should preserve IsDirty=false");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void BrokenRule_SetMetastate_ThrowsOnNullMetastate()
    {
      // Arrange
      var brokenRule = CreateEmptyBrokenRule();

      // Act
      ((IMobileObjectMetastate)brokenRule).SetMetastate(null);
    }

    [TestMethod]
    public void BrokenRule_SetMetastate_AcceptsEmptyMetastate()
    {
      // Arrange - a BrokenRule carries no non-public state, so its metastate is empty
      var brokenRule = CreateEmptyBrokenRule();
      var emptyMetastate = Array.Empty<byte>();

      // Act - setting an empty metastate must not throw
      ((IMobileObjectMetastate)brokenRule).SetMetastate(emptyMetastate);

      // Assert
      Assert.IsNotNull(brokenRule);
    }

    /// <summary>
    /// Creates an empty rule the way MobileFormatter does. The parameterless constructor
    /// is obsolete-as-error, which is a hard compiler error no pragma can suppress, so the
    /// deserialization path can only be reached by reflection as the formatter reaches it.
    /// </summary>
    private static BrokenRule CreateEmptyBrokenRule()
      => (BrokenRule)Activator.CreateInstance(typeof(BrokenRule), nonPublic: true);
  }

  /// <summary>
  /// Business object with a fetch operation, so a test can obtain an object in the
  /// old and clean state a fetch produces.
  /// </summary>
  [Serializable]
  public class MetastateRoot : BusinessBase<MetastateRoot>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
    public int Id
    {
      get => GetProperty(IdProperty);
      set => SetProperty(IdProperty, value);
    }

    [Create]
    private void Create()
    { }

    [Fetch]
    private void Fetch(int id)
    {
      using (BypassPropertyChecks)
        Id = id;
    }
  }
}
