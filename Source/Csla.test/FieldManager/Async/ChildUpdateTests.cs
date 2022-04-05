//-----------------------------------------------------------------------
// <copyright file="DataPortalChildTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using System.Threading.Tasks;
using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
#if !NUNIT

#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif

namespace Csla.Test.FieldManager.Async
{
  [TestClass()]
  public class ChildUpdateTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestMethod]
    public async Task CreateAndSaveChildAsync()
    {
      IChildDataPortal<Child> childDataPortal = _testDIContext.CreateChildDataPortal<Child>();
      IDataPortal<Root> dataPortal = _testDIContext.CreateDataPortal<Root>();

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
      IChildDataPortal<Child> childDataPortal = _testDIContext.CreateChildDataPortal<Child>();
      IDataPortal<RootUpdateAllChildren> dataPortal = _testDIContext.CreateDataPortal<RootUpdateAllChildren>();

      var root = await dataPortal.CreateAsync();
      await root.FetchChildAsync(childDataPortal);

      Assert.IsFalse(root.Child.IsDirty, "Child should not be dirty");
      Assert.AreEqual("Fetched", root.Child.Status, "Child status incorrect after fetch");

      root = await root.SaveAsync();

      Assert.AreEqual("Updated", root.Child.Status, "Child status incorrect after Save");
    }

  }
}