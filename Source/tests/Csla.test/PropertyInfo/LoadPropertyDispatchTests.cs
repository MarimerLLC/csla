//-----------------------------------------------------------------------
// <copyright file="LoadPropertyDispatchTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests loading properties through a non-generic IPropertyInfo</summary>
//-----------------------------------------------------------------------

using Csla.Core;
using Csla.Testing;
using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.PropertyInfo
{
  [TestClass]
  public class LoadPropertyDispatchTests
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

    [TestMethod]
    public void BusinessBase_LoadProperty_ReferenceValueNullableAndEnumTypes()
    {
      var obj = _testHost.GetDataPortal<LoadPropertyRoot>().Create();

      obj.LoadUntyped(LoadPropertyRoot.NameProperty, "name");
      obj.LoadUntyped(LoadPropertyRoot.CountProperty, 42);
      obj.LoadUntyped(LoadPropertyRoot.OptionalProperty, 7);
      obj.LoadUntyped(LoadPropertyRoot.ColorProperty, LoadPropertyColor.Green);

      Assert.AreEqual("name", obj.Name);
      Assert.AreEqual(42, obj.Count);
      Assert.AreEqual(7, obj.Optional);
      Assert.AreEqual(LoadPropertyColor.Green, obj.Color);
    }

    [TestMethod]
    public void BusinessBase_LoadProperty_NullLoadsDefault()
    {
      var obj = _testHost.GetDataPortal<LoadPropertyRoot>().Create();
      obj.LoadUntyped(LoadPropertyRoot.CountProperty, 42);
      obj.LoadUntyped(LoadPropertyRoot.OptionalProperty, 7);

      obj.LoadUntyped(LoadPropertyRoot.CountProperty, null);
      obj.LoadUntyped(LoadPropertyRoot.OptionalProperty, null);

      Assert.AreEqual(0, obj.Count);
      Assert.IsNull(obj.Optional);
    }

    [TestMethod]
    public void BusinessBase_LoadProperty_WideningConversionUsesFallback()
    {
      var obj = _testHost.GetDataPortal<LoadPropertyRoot>().Create();

      obj.LoadUntyped(LoadPropertyRoot.TotalProperty, 5);

      Assert.AreEqual(5L, obj.Total);
    }

    [TestMethod]
    public void BusinessBase_LoadProperty_IncompatibleValueThrows()
    {
      var obj = _testHost.GetDataPortal<LoadPropertyRoot>().Create();

      Assert.ThrowsException<ArgumentException>(() => obj.LoadUntyped(LoadPropertyRoot.CountProperty, "not a number"));
    }

    [TestMethod]
    public void BusinessBase_LoadPropertyMarkDirty_ReportsChange()
    {
      var obj = _testHost.GetDataPortal<LoadPropertyRoot>().Create();
      obj.LoadUntyped(LoadPropertyRoot.CountProperty, 1);

      Assert.IsTrue(obj.LoadUntypedMarkDirty(LoadPropertyRoot.CountProperty, 2));
      Assert.IsFalse(obj.LoadUntypedMarkDirty(LoadPropertyRoot.CountProperty, 2));
      Assert.AreEqual(2, obj.Count);
    }

    [TestMethod]
    public void ReadOnlyBase_LoadProperty_ReferenceAndNullableTypes()
    {
      var obj = _testHost.GetDataPortal<LoadPropertyReadOnly>().Fetch();

      obj.LoadUntyped(LoadPropertyReadOnly.NameProperty, "name");
      obj.LoadUntyped(LoadPropertyReadOnly.OptionalProperty, 3);

      Assert.AreEqual("name", obj.Name);
      Assert.AreEqual(3, obj.Optional);
    }

    [TestMethod]
    public void CommandBase_LoadPropertyAndMarkDirty()
    {
      var obj = _testHost.GetDataPortal<LoadPropertyCommand>().Create();

      obj.LoadUntyped(LoadPropertyCommand.CountProperty, 1);
      Assert.AreEqual(1, obj.Count);

      // command objects don't track dirty state, so this loads the value and reports no change
      Assert.IsFalse(obj.LoadUntypedMarkDirty(LoadPropertyCommand.CountProperty, 2));
      Assert.AreEqual(2, obj.Count);
    }
    [TestMethod]
    public void CommandBase_LoadPropertyMarkDirty_WideningConversionUsesFallback()
    {
      var obj = _testHost.GetDataPortal<LoadPropertyCommand>().Create();

      Assert.IsFalse(obj.LoadUntypedMarkDirty(LoadPropertyCommand.TotalProperty, 5));
      Assert.AreEqual(5L, obj.Total);
    }

    [TestMethod]
    public void CommandBase_LoadProperty_WideningConversionUsesFallback()
    {
      var obj = _testHost.GetDataPortal<LoadPropertyCommand>().Create();

      obj.LoadUntyped(LoadPropertyCommand.TotalProperty, 5);

      Assert.AreEqual(5L, obj.Total);
    }

    [TestMethod]
    public void BusinessBase_LoadPropertyMarkDirty_WideningConversionUsesFallback()
    {
      var obj = _testHost.GetDataPortal<LoadPropertyRoot>().Create();

      Assert.IsTrue(obj.LoadUntypedMarkDirty(LoadPropertyRoot.TotalProperty, 5));
      Assert.AreEqual(5L, obj.Total);
    }
  }

  public enum LoadPropertyColor
  {
    Red,
    Green
  }

  [Serializable]
  public class LoadPropertyRoot : BusinessBase<LoadPropertyRoot>
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name => ReadProperty(NameProperty);

    public static readonly PropertyInfo<int> CountProperty = RegisterProperty<int>(c => c.Count);
    public int Count => ReadProperty(CountProperty);

    public static readonly PropertyInfo<int?> OptionalProperty = RegisterProperty<int?>(c => c.Optional);
    public int? Optional => ReadProperty(OptionalProperty);

    public static readonly PropertyInfo<long> TotalProperty = RegisterProperty<long>(c => c.Total);
    public long Total => ReadProperty(TotalProperty);

    public static readonly PropertyInfo<LoadPropertyColor> ColorProperty = RegisterProperty<LoadPropertyColor>(c => c.Color);
    public LoadPropertyColor Color => ReadProperty(ColorProperty);

    public void LoadUntyped(IPropertyInfo propertyInfo, object value) => LoadProperty(propertyInfo, value);

    public bool LoadUntypedMarkDirty(IPropertyInfo propertyInfo, object value) => LoadPropertyMarkDirty(propertyInfo, value);

    [Create]
    private void Create()
    { }
  }

  [Serializable]
  public class LoadPropertyReadOnly : ReadOnlyBase<LoadPropertyReadOnly>
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name => ReadProperty(NameProperty);

    public static readonly PropertyInfo<int?> OptionalProperty = RegisterProperty<int?>(c => c.Optional);
    public int? Optional => ReadProperty(OptionalProperty);

    public void LoadUntyped(IPropertyInfo propertyInfo, object value) => LoadProperty(propertyInfo, value);

    [Fetch]
    private void Fetch()
    { }
  }

  [Serializable]
  public class LoadPropertyCommand : CommandBase<LoadPropertyCommand>
  {
    public static readonly PropertyInfo<int> CountProperty = RegisterProperty<int>(c => c.Count);
    public int Count => ReadProperty(CountProperty);

    public static readonly PropertyInfo<long> TotalProperty = RegisterProperty<long>(c => c.Total);
    public long Total => ReadProperty(TotalProperty);

    public void LoadUntyped(IPropertyInfo propertyInfo, object value) => LoadProperty(propertyInfo, value);

    public bool LoadUntypedMarkDirty(IPropertyInfo propertyInfo, object value) => LoadPropertyMarkDirty(propertyInfo, value);

    [Create]
    private void Create()
    { }
  }
}
