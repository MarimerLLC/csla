//-----------------------------------------------------------------------
// <copyright file="DataPortalChildTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

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
    [TestMethod]
    public void FetchAndSaveChild()
    {
      Root root = new Root();
      root.FetchChild();

      Assert.IsFalse(root.Child.IsDirty, "Child should not be dirty");
      Assert.AreEqual("Fetched", root.Child.Status, "Child status incorrect after fetch");

      root = root.Save();

      Assert.AreEqual("Fetched", root.Child.Status, "Child status incorrect after Save");
    }

    [TestMethod]
    public void FetchAndSaveAnyChild()
    {
      var root = new RootUpdateAllChildren();
      root.FetchChild();

      Assert.IsFalse(root.Child.IsDirty, "Child should not be dirty");
      Assert.AreEqual("Fetched", root.Child.Status, "Child status incorrect after fetch");

      root = root.Save();

      Assert.AreEqual("Updated", root.Child.Status, "Child status incorrect after Save");
    }

  }
}