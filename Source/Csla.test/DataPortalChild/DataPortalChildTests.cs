//-----------------------------------------------------------------------
// <copyright file="DataPortalChildTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

#if !NUNIT
using Microsoft.VisualStudio.TestTools.UnitTesting;

#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#endif

namespace Csla.Test.DataPortalChild
{
  [TestClass()]
  public class DataPortalChildTests
  {
    [TestMethod]
    public void CreateAndSaveChild()
    {
      Root root = new Root();
      root.Data = "a";
      root.Child.Data = "b";

      Assert.IsTrue(root.IsDirty, "Root should be dirty");
      Assert.IsTrue(root.Child.IsNew, "Child should be new");
      Assert.IsTrue(root.Child.IsDirty, "Child should be dirty");
      Assert.AreEqual("Created", root.Child.Status, "Child status incorrect after create");
      
      root = root.Save();

      Assert.IsFalse(root.IsDirty, "Root should not be dirty");
      Assert.IsFalse(root.IsNew, "Root should not be new");
      Assert.IsFalse(root.Child.IsDirty, "Child should not be dirty");
      Assert.IsFalse(root.Child.IsNew, "Child should not be new");
      Assert.AreEqual("Inserted", root.Child.Status, "Child status incorrect after Save");
    }

    [TestMethod]
    public void CreateAndDeleteChild()
    {
      Root root = new Root();

      root.Child.DeleteChild();
      Assert.IsTrue(root.Child.IsDeleted, "Child should be marked for deletion");
      Assert.IsTrue(root.Child.IsDirty, "Child should be dirty");

      root = root.Save();

      Assert.IsTrue(root.IsDirty, "Root should be dirty after Save");
      Assert.IsFalse(root.IsNew, "Root should not be new");
      Assert.IsTrue(root.Child.IsDirty, "Child should be dirty");
      Assert.IsTrue(root.Child.IsNew, "Child should be new");
      Assert.AreEqual("Created", root.Child.Status, "Child status incorrect");
    }

    [TestMethod]
    public void FetchAndSaveChild()
    {
      Root root = new Root();
      root.FetchChild();

      Assert.IsFalse(root.Child.IsNew, "Child should not be new");
      Assert.IsFalse(root.Child.IsDirty, "Child should not be dirty");
      Assert.AreEqual("Fetched", root.Child.Status, "Child status incorrect after fetch");

      root.Child.Data = "b";

      Assert.IsTrue(root.Child.IsDirty, "Child should be dirty");

      root = root.Save();

      Assert.IsFalse(root.IsDirty, "Root should not be dirty");
      Assert.IsFalse(root.IsNew, "Root should not be new");
      Assert.IsFalse(root.Child.IsDirty, "Child should not be dirty");
      Assert.IsFalse(root.Child.IsNew, "Child should not be new");
      Assert.AreEqual("Updated", root.Child.Status, "Child status incorrect after Save");
    }

    [TestMethod]
    public void FetchAndDeleteChild()
    {
      Root root = new Root();
      root.FetchChild();

      Assert.IsFalse(root.Child.IsNew, "Child should not be new");
      Assert.IsFalse(root.Child.IsDirty, "Child should not be dirty");
      Assert.AreEqual("Fetched", root.Child.Status, "Child status incorrect after fetch");

      root.Child.DeleteChild();
      Assert.IsTrue(root.Child.IsDeleted, "Child should be marked for deletion");
      Assert.IsTrue(root.Child.IsDirty, "Child should be dirty");

      root = root.Save();

      Assert.IsTrue(root.IsDirty, "Root should be dirty after Save");
      Assert.IsFalse(root.IsNew, "Root should not be new");
      Assert.IsTrue(root.Child.IsDirty, "Child should be dirty after Save");
      Assert.IsTrue(root.Child.IsNew, "Child should be new after Save");
      Assert.AreEqual("Deleted", root.Child.Status, "Child status incorrect after Save");
    }

    [TestMethod]
    public void FetchAndSaveChildList()
    {
      Root root = new Root();
      var list = root.ChildList;
      Assert.IsFalse(root.ChildList.IsDirty, "Child list should not be dirty");
      Assert.AreEqual("Fetched", root.ChildList.Status, "Child list status incorrect after fetch");

      list.Add(Child.NewChild());

      Assert.IsTrue(root.ChildList.IsDirty, "Child list should be dirty after add");
      Assert.IsTrue(root.ChildList[0].IsDirty, "Child should be dirty after add");
      Assert.IsTrue(root.ChildList[0].IsNew, "Child should be new after add");

      root = root.Save();

      Assert.IsFalse(root.IsDirty, "Root should not be dirty after Save");
      Assert.IsFalse(root.IsNew, "Root should not be new");
      Assert.IsFalse(root.ChildList.IsDirty, "Child should not be dirty after Save");
      Assert.AreEqual("Updated", root.ChildList.Status, "Child list status incorrect after Save");
      Assert.IsFalse(root.ChildList[0].IsDirty, "Child should not be dirty after Save");
      Assert.IsFalse(root.ChildList[0].IsNew, "Child should not be new after Save");
      Assert.AreEqual("Inserted", root.ChildList[0].Status, "Child status incorrect after Save");
    }

    [TestMethod]
    public void FetchAndSaveChildListVerifyParent()
    {
      Root root = new Root();
      root.Data = "root";
      var oneChild = root.Child;
      oneChild.Data = "random";

      var list = root.ChildList;
      Assert.IsFalse(root.ChildList.IsDirty, "Child list should not be dirty");
      Assert.AreEqual("Fetched", root.ChildList.Status, "Child list status incorrect after fetch");

      

      list.Add(Child.NewChild());

      Assert.IsTrue(root.ChildList.IsDirty, "Child list should be dirty after add");
      Assert.IsTrue(root.ChildList[0].IsDirty, "Child should be dirty after add");
      Assert.IsTrue(root.ChildList[0].IsNew, "Child should be new after add");

      root = root.Save();

      Assert.IsFalse(root.IsDirty, "Root should not be dirty after Save");
      Assert.IsFalse(root.IsNew, "Root should not be new");
      Assert.IsFalse(root.ChildList.IsDirty, "Child should not be dirty after Save");
      Assert.AreEqual("Updated", root.ChildList.Status, "Child list status incorrect after Save");
      Assert.IsFalse(root.ChildList[0].IsDirty, "Child should not be dirty after Save");
      Assert.IsFalse(root.ChildList[0].IsNew, "Child should not be new after Save");
      Assert.AreEqual("Inserted", root.ChildList[0].Status, "Child status incorrect after Save");

      Assert.AreEqual("root", root.ChildList[0].RootData, "Parent data is not correct after Save in the list");
      Assert.AreEqual("root", root.Child.RootData, "Parent data is not correct after Save in one child");
    }
  }
}