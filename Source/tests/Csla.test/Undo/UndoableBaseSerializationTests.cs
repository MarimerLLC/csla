//-----------------------------------------------------------------------
// <copyright file="UndoableBaseSerializationTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests n-level undo of fields that hold serializable object graphs</summary>
//-----------------------------------------------------------------------

using Csla.Core;
using Csla.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.Undo
{
  [TestClass]
  public class UndoableBaseSerializationTests
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

    private static UndoRoot NewRoot() => _testHost.GetDataPortal<UndoRoot>().Create();

    [TestMethod]
    public void CancelEdit_RestoresMobileObjectFieldContents()
    {
      var root = NewRoot();
      root.Numbers = new MobileList<int> { 1, 2 };

      root.BeginEdit();
      root.Numbers.Add(3);
      root.CancelEdit();

      root.Numbers.Should().Equal(1, 2);
    }

    [TestMethod]
    public void CancelEdit_RestoresMobileObjectFieldSetToNull()
    {
      var root = NewRoot();
      root.Numbers = new MobileList<int> { 1, 2 };

      root.BeginEdit();
      root.Numbers = null;
      root.CancelEdit();

      root.Numbers.Should().Equal(1, 2);
    }

    [TestMethod]
    public void CancelEdit_RestoresNullMobileObjectField()
    {
      var root = NewRoot();

      root.BeginEdit();
      root.Numbers = new MobileList<int> { 1, 2 };
      root.CancelEdit();

      root.Numbers.Should().BeNull();
    }

    [TestMethod]
    public void CancelEdit_RestoresManagedProperties()
    {
      var root = NewRoot();
      root.Name = "before";
      root.Tags = new MobileList<string> { "a" };

      root.BeginEdit();
      root.Name = "after";
      root.Tags = new MobileList<string> { "b", "c" };
      root.CancelEdit();

      root.Name.Should().Be("before");
      root.Tags.Should().Equal("a");
    }

    [TestMethod]
    public void Clone_KeepsUndoStateAcrossSerialization()
    {
      var root = NewRoot();
      root.Name = "before";
      root.Tags = new MobileList<string> { "a" };

      root.BeginEdit();
      root.Name = "after";
      root.Tags = new MobileList<string> { "b" };

      var clone = root.Clone();
      clone.EditLevel.Should().Be(1);
      clone.Name.Should().Be("after");

      clone.CancelEdit();

      clone.EditLevel.Should().Be(0);
      clone.Name.Should().Be("before");
      clone.Tags.Should().Equal("a");
    }
  }

  [Serializable]
  public partial class UndoRoot : BusinessBase<UndoRoot>
  {
    private MobileList<int> _numbers;

    public MobileList<int> Numbers
    {
      get => _numbers;
      set => _numbers = value;
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get => GetProperty(NameProperty);
      set => SetProperty(NameProperty, value);
    }

    public static readonly PropertyInfo<MobileList<string>> TagsProperty = RegisterProperty<MobileList<string>>(nameof(Tags));
    public MobileList<string> Tags
    {
      get => GetProperty(TagsProperty);
      set => SetProperty(TagsProperty, value);
    }

    public new int EditLevel => base.EditLevel;

    [Create]
    private void Create()
    { }
  }
}
