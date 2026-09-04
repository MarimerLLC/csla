//-----------------------------------------------------------------------
// <copyright file="DataPortalChildTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.Testing;
using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Csla.Test.FieldManager.Async
{
  [TestClass]
  public class ChildUpdateTests
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
    public async Task CreateAndSaveChildAsync()
    {
      IChildDataPortal<Child> childDataPortal = _testHost.GetChildDataPortal<Child>();
      IDataPortal<Root> dataPortal = _testHost.GetDataPortal<Root>();

      Root root = await dataPortal.CreateAsync();
      await root.FetchChildAsync(childDataPortal);

      Assert.IsFalse(root.Child.IsDirty, "Child should not be dirty");
      Assert.AreEqual("Fetched", root.Child.Status, "Child status incorrect after fetch");

      root = await root.SaveAsync();

      Assert.AreEqual("Fetched", root.Child.Status, "Child status incorrect after Save");
    }

    [TestMethod]
    public async Task CreateAndSaveAnyChildAsync()
    {
      IChildDataPortal<Child> childDataPortal = _testHost.GetChildDataPortal<Child>();
      IDataPortal<RootUpdateAllChildren> dataPortal = _testHost.GetDataPortal<RootUpdateAllChildren>();

      var root = await dataPortal.CreateAsync();
      await root.FetchChildAsync(childDataPortal);

      Assert.IsFalse(root.Child.IsDirty, "Child should not be dirty");
      Assert.AreEqual("Fetched", root.Child.Status, "Child status incorrect after fetch");

      root = await root.SaveAsync();

      Assert.AreEqual("Updated", root.Child.Status, "Child status incorrect after Save");
    }

  }
}