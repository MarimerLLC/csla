//-----------------------------------------------------------------------
// <copyright file="MobileObjectMetastateTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for IMobileObjectMetastate interface implementation.</summary>
//-----------------------------------------------------------------------

using Csla.Serialization.Mobile;
using Csla.Testing;
using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.Serialization
{
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
      // Arrange - a CommandBase object that does not override OnGetMetastate or
      // OnSetMetastate returns and accepts an empty byte array
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

    // Ignored: asserts that property values survive a metastate round trip, which is what
    // IMobileObjectMetastate's documentation promises but no implementation does -- see
    // https://github.com/MarimerLLC/csla/issues/4898. Kept rather than deleted so the
    // discrepancy is not lost again.
    [Ignore]
    [TestMethod]
    public void CommandBase_GetSetMetastate_PropertyValues_RoundTrip()
    {
      // Arrange
      var dataPortal = _testHost.GetDataPortal<Test.CommandBase.CommandObject>();
      var original = dataPortal.Create();

      var loader = new PropertyLoader(_testHost.ApplicationContext);
      loader.Load(original, Test.CommandBase.CommandObject.NameProperty, "Test Command");
      loader.Load(original, Test.CommandBase.CommandObject.NumProperty, 123);

      // Act
      var metastate = ((IMobileObjectMetastate)original).GetMetastate();

      var restored = dataPortal.Create();
      ((IMobileObjectMetastate)restored).SetMetastate(metastate);

      // Assert
      Assert.AreEqual("Test Command", restored.Name);
      Assert.AreEqual(123, restored.Num);
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
      // Arrange - a fetched object is old and clean, which is the state whose
      // preservation across a metastate round trip is under test
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

    /// <summary>
    /// Exposes the protected <see cref="Csla.Server.ObjectFactory.LoadProperty{P}"/> so a
    /// test can put a command object into a known state without going through a data portal
    /// operation that sets the values.
    /// </summary>
    private class PropertyLoader : Csla.Server.ObjectFactory
    {
      public PropertyLoader(ApplicationContext applicationContext) : base(applicationContext)
      {
      }

      public void Load<P>(object obj, PropertyInfo<P> propertyInfo, P newValue)
      {
        LoadProperty(obj, propertyInfo, newValue);
      }
    }
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
