//-----------------------------------------------------------------------
// <copyright file="DataPortalChildTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

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

namespace Csla.Test.FieldManager
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
    public void FetchAndSaveChild()
    {
      IChildDataPortal<Child> childDataPortal = _testDIContext.CreateChildDataPortal<Child>();

      Root root = new Root();
      root.FetchChild(childDataPortal);

      Assert.IsFalse(root.Child.IsDirty, "Child should not be dirty");
      Assert.AreEqual("Fetched", root.Child.Status, "Child status incorrect after fetch");

      root = root.Save();

      Assert.AreEqual("Fetched", root.Child.Status, "Child status incorrect after Save");
    }

    [TestMethod]
    public void FetchAndSaveAnyChild()
    {
      IChildDataPortal<Child> childDataPortal = _testDIContext.CreateChildDataPortal<Child>();

      var root = new RootUpdateAllChildren();
      root.FetchChild(childDataPortal);

      Assert.IsFalse(root.Child.IsDirty, "Child should not be dirty");
      Assert.AreEqual("Fetched", root.Child.Status, "Child status incorrect after fetch");

      root = root.Save();

      Assert.AreEqual("Updated", root.Child.Status, "Child status incorrect after Save");
    }

  }
}