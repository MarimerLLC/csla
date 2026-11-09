//-----------------------------------------------------------------------
// <copyright file="CalculatedPropertyTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for CalculatedPropertyAttribute</summary>
//-----------------------------------------------------------------------

using System.ComponentModel;
using Csla.Core;
using Csla.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.CalculatedProperties
{
  [TestClass]
  public class CalculatedPropertyTests
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
    public void IsCalculatedReflectsAttribute()
    {
      Assert.IsTrue(CalculatedRoot.TotalProperty.IsCalculated);
      Assert.IsTrue(CalculatedRoot.LabelProperty.IsCalculated);
      Assert.IsTrue(CalculatedRoot.SummaryProperty.IsCalculated);
      Assert.IsFalse(CalculatedRoot.Num1Property.IsCalculated);
      Assert.IsFalse(CalculatedRoot.DoubledProperty.IsCalculated);
      Assert.IsFalse(CalculatedRoot.TagsProperty.IsCalculated);
    }

    [TestMethod]
    public void CalculatedPropertyIsNotSerializable()
    {
      Assert.IsFalse(CalculatedRoot.TotalProperty.IsSerializable);
      Assert.IsTrue(CalculatedRoot.Num1Property.IsSerializable);
      Assert.IsTrue(CalculatedRoot.DoubledProperty.IsSerializable);
    }

    [TestMethod]
    public void ExplicitIsSerializableOverridesCalculatedAttribute()
    {
      var property = new PropertyInfo<int>(nameof(CalculatedRoot.Total), null, typeof(CalculatedRoot), 0, RelationshipTypes.None, isSerializable: true);

      Assert.IsTrue(property.IsCalculated);
      Assert.IsTrue(property.IsSerializable);
    }

    [TestMethod]
    public async Task RuleOutputToCalculatedPropertyDoesNotMarkDirty()
    {
      var obj = await FetchAsync(1, 2, runRules: false);
      Assert.AreEqual(0, obj.Total);
      Assert.IsFalse(obj.IsDirty);

      obj.RecalculateTotal();

      Assert.AreEqual(3, obj.Total);
      Assert.IsFalse(obj.IsSelfDirty);
      Assert.IsFalse(obj.IsDirty);
    }

    [TestMethod]
    public async Task RuleOutputToNormalPropertyStillMarksDirty()
    {
      var obj = await FetchAsync(1, 2, runRules: false);
      Assert.IsFalse(obj.IsDirty);

      obj.RecalculateDoubled();

      Assert.AreEqual(2, obj.Doubled);
      Assert.IsTrue(obj.IsDirty);
    }

    [TestMethod]
    public async Task SetCalculatedManagedPropertyDoesNotMarkDirty()
    {
      var obj = await FetchAsync(1, 2, runRules: true);
      var changed = new List<string>();
      obj.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

      obj.SetLabel("abc");

      Assert.AreEqual("abc", obj.Label);
      Assert.IsFalse(obj.IsSelfDirty);
      Assert.IsFalse(obj.IsDirty);
      CollectionAssert.Contains(changed, nameof(CalculatedRoot.Label));
    }

    [TestMethod]
    public async Task SetCalculatedPrivateFieldPropertyDoesNotMarkDirty()
    {
      var obj = await FetchAsync(1, 2, runRules: true);
      var changed = new List<string>();
      obj.PropertyChanged += (_, e) => changed.Add(e.PropertyName);

      obj.SetSummary("xyz");

      Assert.AreEqual("xyz", obj.Summary);
      Assert.IsFalse(obj.IsSelfDirty);
      Assert.IsFalse(obj.IsDirty);
      CollectionAssert.Contains(changed, nameof(CalculatedRoot.Summary));
    }

    [TestMethod]
    public async Task ChangingInputMarksDirtyAndRecalculates()
    {
      var obj = await FetchAsync(1, 2, runRules: true);
      Assert.AreEqual(3, obj.Total);
      Assert.IsFalse(obj.IsDirty);

      obj.Num1 = 10;

      Assert.AreEqual(12, obj.Total);
      Assert.IsTrue(obj.IsDirty);
    }

    [TestMethod]
    public async Task CloneRecalculatesCalculatedPropertyAndStaysClean()
    {
      var obj = await FetchAsync(1, 2, runRules: true);
      Assert.AreEqual(3, obj.Total);
      Assert.IsFalse(obj.IsDirty);

      var clone = obj.Clone();

      Assert.AreEqual(1, clone.Num1);
      Assert.AreEqual(2, clone.Num2);
      Assert.AreEqual(3, clone.Total);
      Assert.IsFalse(clone.IsSelfDirty);
      Assert.IsFalse(clone.IsDirty);
    }

    [TestMethod]
    public async Task CloneDoesNotSerializeCalculatedPropertySetDirectly()
    {
      var obj = await FetchAsync(1, 2, runRules: true);
      obj.SetLabel("abc");

      var clone = obj.Clone();

      Assert.AreEqual(string.Empty, clone.Label);
      Assert.IsFalse(clone.IsDirty);
    }

    [TestMethod]
    public async Task DoNotSerializeFieldWithMobileObjectValueIsNotSerialized()
    {
      var obj = await FetchAsync(1, 2, runRules: true);
      obj.Tags = ["a", "b"];

      var clone = obj.Clone();

      Assert.IsNotNull(obj.Tags);
      Assert.IsNull(clone.Tags);
    }

    private static Task<CalculatedRoot> FetchAsync(int num1, int num2, bool runRules)
    {
      var dataPortal = _testHost.GetDataPortal<CalculatedRoot>();
      return dataPortal.FetchAsync(num1, num2, runRules);
    }
  }
}
