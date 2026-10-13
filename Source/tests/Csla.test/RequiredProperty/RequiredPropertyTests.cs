//-----------------------------------------------------------------------
// <copyright file="RequiredPropertyTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for RelationshipTypes.Required</summary>
//-----------------------------------------------------------------------

using Csla.Testing;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.RequiredProperty
{
  [TestClass]
  public class RequiredPropertyTests
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
    public void Create_RequiredPropertySet_Succeeds()
    {
      var obj = _testHost.GetDataPortal<RequiredRoot>().Create(true);

      obj.Name.Should().Be("name");
      obj.Notes.Should().BeNull();
    }

    [TestMethod]
    public void Create_RequiredPropertyNull_Throws()
    {
      var dataPortal = _testHost.GetDataPortal<RequiredRoot>();

      var ex = dataPortal.Invoking(dp => dp.Create(false)).Should().Throw<DataPortalException>().Which;

      AssertRequiredPropertyNotSet(ex, typeof(RequiredRoot), nameof(RequiredRoot.Name));
    }

    [TestMethod]
    public async Task CreateAsync_RequiredPropertyNull_Throws()
    {
      var dataPortal = _testHost.GetDataPortal<RequiredRoot>();

      var ex = (await dataPortal.Awaiting(dp => dp.CreateAsync(false)).Should().ThrowAsync<DataPortalException>()).Which;

      AssertRequiredPropertyNotSet(ex, typeof(RequiredRoot), nameof(RequiredRoot.Name));
    }

    [TestMethod]
    public void Fetch_RequiredPropertySet_Succeeds()
    {
      var obj = _testHost.GetDataPortal<RequiredRoot>().Fetch(true, false);

      obj.Name.Should().Be("name");
    }

    [TestMethod]
    public void Fetch_RequiredPropertyNull_Throws()
    {
      var dataPortal = _testHost.GetDataPortal<RequiredRoot>();

      var ex = dataPortal.Invoking(dp => dp.Fetch(false, false)).Should().Throw<DataPortalException>().Which;

      AssertRequiredPropertyNotSet(ex, typeof(RequiredRoot), nameof(RequiredRoot.Name));
    }

    [TestMethod]
    public void Fetch_LazyLoadRequiredPropertyNotLoaded_Succeeds()
    {
      var dataPortal = _testHost.GetDataPortal<RequiredRoot>();

      dataPortal.Invoking(dp => dp.Fetch(true, false)).Should().NotThrow();
    }

    [TestMethod]
    public void Fetch_LazyLoadRequiredPropertyLoadedWithNull_Throws()
    {
      var dataPortal = _testHost.GetDataPortal<RequiredRoot>();

      var ex = dataPortal.Invoking(dp => dp.Fetch(true, true)).Should().Throw<DataPortalException>().Which;

      AssertRequiredPropertyNotSet(ex, typeof(RequiredRoot), nameof(RequiredRoot.LazyChild));
    }

    [TestMethod]
    public void Fetch_MultipleRequiredPropertiesNull_ListsAll()
    {
      var dataPortal = _testHost.GetDataPortal<RequiredRoot>();

      var ex = dataPortal.Invoking(dp => dp.Fetch(false, true)).Should().Throw<DataPortalException>().Which;

      AssertRequiredPropertyNotSet(ex, typeof(RequiredRoot), nameof(RequiredRoot.Name), nameof(RequiredRoot.LazyChild));
    }

    [TestMethod]
    public void Insert_RequiredPropertySetToNull_Throws()
    {
      var dataPortal = _testHost.GetDataPortal<RequiredRoot>();
      var obj = dataPortal.Create(true);
      obj.Data = null;

      var ex = dataPortal.Invoking(dp => dp.Update(obj)).Should().Throw<DataPortalException>().Which;

      AssertRequiredPropertyNotSet(ex, typeof(RequiredRoot), nameof(RequiredRoot.Data));
    }

    [TestMethod]
    public void Update_RequiredPropertySet_Succeeds()
    {
      var dataPortal = _testHost.GetDataPortal<RequiredRoot>();
      var obj = dataPortal.Fetch(true, false);
      obj.Name = "changed";

      var result = dataPortal.Update(obj);

      result.Name.Should().Be("changed");
    }

    [TestMethod]
    public void Update_RequiredPropertySetToNull_Throws()
    {
      var dataPortal = _testHost.GetDataPortal<RequiredRoot>();
      var obj = dataPortal.Fetch(true, false);
      obj.Data = null;

      var ex = dataPortal.Invoking(dp => dp.Update(obj)).Should().Throw<DataPortalException>().Which;

      AssertRequiredPropertyNotSet(ex, typeof(RequiredRoot), nameof(RequiredRoot.Data));
    }

    [TestMethod]
    public void Update_RequiredStringSetToNull_IsCoercedToEmptyAndSucceeds()
    {
      var dataPortal = _testHost.GetDataPortal<RequiredRoot>();
      var obj = dataPortal.Fetch(true, false);
      obj.Name = null;

      var result = dataPortal.Update(obj);

      result.Name.Should().BeEmpty();
    }

    [TestMethod]
    public void Delete_DoesNotCheckRequiredProperties()
    {
      var dataPortal = _testHost.GetDataPortal<RequiredRoot>();

      dataPortal.Invoking(dp => dp.Delete(true)).Should().NotThrow();
    }

    [TestMethod]
    public void Create_PrivateFieldRequiredPropertySet_Succeeds()
    {
      var obj = _testHost.GetDataPortal<RequiredPrivateFieldRoot>().Create(true);

      obj.Code.Should().Be("code");
    }

    [TestMethod]
    public void Create_PrivateFieldRequiredPropertyNull_Throws()
    {
      var dataPortal = _testHost.GetDataPortal<RequiredPrivateFieldRoot>();

      var ex = dataPortal.Invoking(dp => dp.Create(false)).Should().Throw<DataPortalException>().Which;

      AssertRequiredPropertyNotSet(ex, typeof(RequiredPrivateFieldRoot), nameof(RequiredPrivateFieldRoot.Code));
    }

    [TestMethod]
    public void ReadOnlyFetch_RequiredPropertySet_Succeeds()
    {
      var obj = _testHost.GetDataPortal<RequiredReadOnly>().Fetch(true);

      obj.Name.Should().Be("name");
    }

    [TestMethod]
    public void ReadOnlyFetch_RequiredPropertyNull_Throws()
    {
      var dataPortal = _testHost.GetDataPortal<RequiredReadOnly>();

      var ex = dataPortal.Invoking(dp => dp.Fetch(false)).Should().Throw<DataPortalException>().Which;

      AssertRequiredPropertyNotSet(ex, typeof(RequiredReadOnly), nameof(RequiredReadOnly.Name));
    }

    [TestMethod]
    public void CommandExecute_RequiredPropertySet_Succeeds()
    {
      var dataPortal = _testHost.GetDataPortal<RequiredCommand>();
      var cmd = dataPortal.Create(true);

      cmd = dataPortal.Execute(cmd);

      cmd.Result.Should().Be("result");
    }

    [TestMethod]
    public void CommandExecute_RequiredPropertyNull_Throws()
    {
      var dataPortal = _testHost.GetDataPortal<RequiredCommand>();
      var cmd = dataPortal.Create(false);

      var ex = dataPortal.Invoking(dp => dp.Execute(cmd)).Should().Throw<DataPortalException>().Which;

      AssertRequiredPropertyNotSet(ex, typeof(RequiredCommand), nameof(RequiredCommand.Result));
    }

    [TestMethod]
    public void ChildCreate_RequiredPropertySet_Succeeds()
    {
      var child = _testHost.GetChildDataPortal<RequiredChild>().CreateChild(true);

      child.Name.Should().Be("name");
    }

    [TestMethod]
    public void ChildCreate_RequiredPropertyNull_Throws()
    {
      var childPortal = _testHost.GetChildDataPortal<RequiredChild>();

      var ex = childPortal.Invoking(dp => dp.CreateChild(false)).Should().Throw<DataPortalException>().Which;

      AssertRequiredPropertyNotSet(ex, typeof(RequiredChild), nameof(RequiredChild.Name));
    }

    [TestMethod]
    public void ChildFetch_RequiredPropertyNull_Throws()
    {
      var childPortal = _testHost.GetChildDataPortal<RequiredChild>();

      var ex = childPortal.Invoking(dp => dp.FetchChild(false)).Should().Throw<DataPortalException>().Which;

      AssertRequiredPropertyNotSet(ex, typeof(RequiredChild), nameof(RequiredChild.Name));
    }

    [TestMethod]
    public void ChildUpdate_RequiredPropertySetToNull_Throws()
    {
      var childPortal = _testHost.GetChildDataPortal<RequiredChild>();
      var child = childPortal.FetchChild(true);
      child.Data = null;

      var ex = childPortal.Invoking(dp => dp.UpdateChild(child)).Should().Throw<DataPortalException>().Which;

      AssertRequiredPropertyNotSet(ex, typeof(RequiredChild), nameof(RequiredChild.Data));
    }

    [TestMethod]
    public void ObjectFactoryFetch_RequiredPropertySet_Succeeds()
    {
      var obj = _testHost.GetDataPortal<RequiredFactoryRoot>().Fetch(true);

      obj.Name.Should().Be("name");
    }

    [TestMethod]
    public void ObjectFactoryFetch_RequiredPropertyNull_Throws()
    {
      var dataPortal = _testHost.GetDataPortal<RequiredFactoryRoot>();

      var ex = dataPortal.Invoking(dp => dp.Fetch(false)).Should().Throw<DataPortalException>().Which;

      AssertRequiredPropertyNotSet(ex, typeof(RequiredFactoryRoot), nameof(RequiredFactoryRoot.Name));
    }

    [TestMethod]
    public void ExceptionConstructor_BuildsMessageFromTypeAndNames()
    {
      var ex = new RequiredPropertyNotSetException(typeof(RequiredRoot), ["A", "B"]);

      ex.ObjectType.Should().Be(typeof(RequiredRoot));
      ex.PropertyNames.Should().Equal("A", "B");
      ex.Message.Should().Contain(typeof(RequiredRoot).FullName).And.Contain("A, B");
    }

    private static void AssertRequiredPropertyNotSet(DataPortalException ex, Type objectType, params string[] propertyNames)
    {
      var inner = ex.BusinessException.Should().BeOfType<RequiredPropertyNotSetException>().Which;
      inner.ObjectType.Should().Be(objectType);
      inner.PropertyNames.Should().BeEquivalentTo(propertyNames);
      foreach (var name in propertyNames)
        inner.Message.Should().Contain(name);
    }
  }
}
