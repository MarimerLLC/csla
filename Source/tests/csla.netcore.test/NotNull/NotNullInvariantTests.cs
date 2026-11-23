//-----------------------------------------------------------------------
// <copyright file="NotNullInvariantTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests for the public not-null invariant validator</summary>
//-----------------------------------------------------------------------

#nullable enable

using Csla.Core;
using Csla.Server;
using Csla.Testing;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.NotNull
{
  [TestClass]
  public class NotNullInvariantTests
  {
    private static CslaTestHost _testHost = default!;

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

    private static T New<T>() => _testHost.ApplicationContext.CreateInstanceDI<T>();

    [TestMethod]
    public void NotNullInvariant_Null_ThrowsArgumentNullException()
    {
      using (new AssertionScope())
      {
        FluentActions.Invoking(() => NotNullInvariant.GetUnsatisfiedProperties(null!)).Should().ThrowExactly<ArgumentNullException>();
        FluentActions.Invoking(() => NotNullInvariant.Verify(null!)).Should().ThrowExactly<ArgumentNullException>();
      }
    }

    [TestMethod]
    public void NotNullInvariant_List_IsSatisfied()
    {
      var list = New<NotNullPortalList>();

      NotNullInvariant.GetUnsatisfiedProperties(list).Should().BeEmpty();
      NotNullInvariant.Verify(list);
    }

    [TestMethod]
    public void NotNullInvariant_AllLoaded_IsSatisfied()
    {
      var obj = New<NotNullPortalRoot>();
      obj.LoadAll();

      NotNullInvariant.GetUnsatisfiedProperties(obj).Should().BeEmpty();
      NotNullInvariant.Verify(obj);
    }

    [TestMethod]
    public void NotNullInvariant_NullField_IsReportedAndObjectIsNotChanged()
    {
      var obj = New<NotNullPortalRoot>();
      obj.LoadAll();
      obj.PlantNull(NotNullPortalRoot.DataProperty);
      var wasDirty = obj.IsDirty;

      var failing = NotNullInvariant.GetUnsatisfiedProperties(obj);
      var ex = FluentActions.Invoking(() => NotNullInvariant.Verify(obj)).Should().ThrowExactly<InvalidOperationException>().Which;

      using (new AssertionScope())
      {
        failing.Single().Name.Should().Be(nameof(NotNullPortalRoot.Data));
        ex.Message.Should().Contain(typeof(NotNullPortalRoot).FullName!);
        ex.Message.Should().Contain(nameof(NotNullPortalRoot.Data));
        ((IManageProperties)obj).FieldExists(NotNullPortalRoot.CodeProperty).Should().BeFalse("the defaulted property was not materialised");
        obj.IsDirty.Should().Be(wasDirty);
      }
    }

    [TestMethod]
    public void NotNullInvariant_ReadOnlyAndCommand_AreChecked()
    {
      using (new AssertionScope())
      {
        NotNullInvariant.GetUnsatisfiedProperties(New<NotNullPortalInfo>()).Single().Name.Should().Be(nameof(NotNullPortalInfo.Name));
        NotNullInvariant.GetUnsatisfiedProperties(New<NotNullPortalCommand>()).Single().Name.Should().Be(nameof(NotNullPortalCommand.Result));
      }
    }

    [TestMethod]
    public async Task ObjectFactory_VerifyNotNullInvariant_ChecksHandBuiltChild()
    {
      var portal = _testHost.GetDataPortal<NotNullFactoryParent>();

      var ex = (await FluentActions.Awaiting(() => portal.FetchAsync(false)).Should().ThrowAsync<DataPortalException>()).Which;

      InvalidOperationException? violation = null;
      for (Exception? e = ex; e != null; e = e.InnerException)
        violation ??= e as InvalidOperationException;
      violation.Should().NotBeNull();
      violation!.Message.Should().Contain(typeof(NotNullPortalChild).FullName!);

      var parent = await portal.FetchAsync(true);
      parent.Child.Name.Should().Be("child");
    }
  }

  [Serializable]
  [ObjectFactory(typeof(NotNullParentFactory))]
  public class NotNullFactoryParent : BusinessBase<NotNullFactoryParent>
  {
    public static readonly NotNullPropertyInfo<NotNullPortalChild> ChildProperty = RegisterNotNullProperty<NotNullPortalChild>(nameof(Child));
    public NotNullPortalChild Child => GetProperty(ChildProperty);
  }

  public class NotNullParentFactory : ObjectFactory
  {
    public NotNullParentFactory(ApplicationContext applicationContext) : base(applicationContext)
    { }

    [Fetch]
    public NotNullFactoryParent Fetch(bool loadChild)
    {
      var child = ApplicationContext.CreateInstanceDI<NotNullPortalChild>();
      if (loadChild)
        LoadProperty(child, NotNullPortalChild.NameProperty, "child");
      MarkAsChild(child);
      MarkOld(child);
      VerifyNotNullInvariant(child);

      var parent = ApplicationContext.CreateInstanceDI<NotNullFactoryParent>();
      LoadProperty(parent, NotNullFactoryParent.ChildProperty, child);
      MarkOld(parent);
      return parent;
    }
  }
}
