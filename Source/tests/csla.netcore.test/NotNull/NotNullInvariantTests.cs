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
      Assert.ThrowsExactly<ArgumentNullException>(() => NotNullInvariant.GetUnsatisfiedProperties(null!));
      Assert.ThrowsExactly<ArgumentNullException>(() => NotNullInvariant.Verify(null!));
    }

    [TestMethod]
    public void NotNullInvariant_List_IsSatisfied()
    {
      var list = New<NotNullPortalList>();

      Assert.AreEqual(0, NotNullInvariant.GetUnsatisfiedProperties(list).Count);
      NotNullInvariant.Verify(list);
    }

    [TestMethod]
    public void NotNullInvariant_AllLoaded_IsSatisfied()
    {
      var obj = New<NotNullPortalRoot>();
      obj.LoadAll();

      Assert.AreEqual(0, NotNullInvariant.GetUnsatisfiedProperties(obj).Count);
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
      var ex = Assert.ThrowsExactly<InvalidOperationException>(() => NotNullInvariant.Verify(obj));

      Assert.AreEqual(nameof(NotNullPortalRoot.Data), failing.Single().Name);
      StringAssert.Contains(ex.Message, typeof(NotNullPortalRoot).FullName);
      StringAssert.Contains(ex.Message, nameof(NotNullPortalRoot.Data));
      Assert.IsFalse(((IManageProperties)obj).FieldExists(NotNullPortalRoot.CodeProperty), "The defaulted property was not materialised");
      Assert.AreEqual(wasDirty, obj.IsDirty);
    }

    [TestMethod]
    public void NotNullInvariant_ReadOnlyAndCommand_AreChecked()
    {
      Assert.AreEqual(nameof(NotNullPortalInfo.Name), NotNullInvariant.GetUnsatisfiedProperties(New<NotNullPortalInfo>()).Single().Name);
      Assert.AreEqual(nameof(NotNullPortalCommand.Result), NotNullInvariant.GetUnsatisfiedProperties(New<NotNullPortalCommand>()).Single().Name);
    }

    [TestMethod]
    public async Task ObjectFactory_VerifyNotNullInvariant_ChecksHandBuiltChild()
    {
      var portal = _testHost.GetDataPortal<NotNullFactoryParent>();

      var ex = await Assert.ThrowsAsync<DataPortalException>(() => portal.FetchAsync(false));

      InvalidOperationException? violation = null;
      for (Exception? e = ex; e != null; e = e.InnerException)
        violation ??= e as InvalidOperationException;
      Assert.IsNotNull(violation);
      StringAssert.Contains(violation.Message, typeof(NotNullPortalChild).FullName);

      var parent = await portal.FetchAsync(true);
      Assert.AreEqual("child", parent.Child.Name);
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
