//-----------------------------------------------------------------------
// <copyright file="AddNewAsyncTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.Testing;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.BusinessListBase
{
  [TestClass]
  public class AddNewAsyncTests
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
    public async Task RootAddNewCoreAsync()
    {
      bool changed = false;
      var obj = CreateRootList();
      obj.CollectionChanged += (_, _) =>
        {
          changed = true;
        };
      var child = await obj.AddNewAsync();
      Assert.IsTrue(changed);
      Assert.AreEqual(child, obj[0]);
    }

    [TestMethod]
    public async Task ChildAddNewCoreAsync()
    {
      bool childChanged = false;
      bool changed = false;
      var obj = CreateRoot();
      obj.ChildChanged += (_, _) =>
        {
          childChanged = true;
        };
      obj.Children.CollectionChanged += (_, _) =>
      {
        changed = true;
      };
      var child = await obj.Children.AddNewAsync();
      Assert.IsTrue(childChanged, "ChildChanged should be true");
      Assert.IsTrue(changed, "Collection changed should be true");
      Assert.AreEqual(child, obj.Children[0]);
    }

    private Root CreateRoot()
    {
      IDataPortal<Root> dataPortal = _testHost.GetDataPortal<Root>();

      return dataPortal.Create();
    }

    private RootList CreateRootList()
    {
      IDataPortal<RootList> dataPortal = _testHost.GetDataPortal<RootList>();

      return dataPortal.Create();
    }
  }
}