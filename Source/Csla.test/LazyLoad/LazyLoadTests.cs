//-----------------------------------------------------------------------
// <copyright file="LazyLoadTests.cs" company="Marimer LLC">
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

namespace Csla.Test.LazyLoad
{
  [TestClass]
  public class LazyLoadTests
  {
    [TestMethod]
    public void NullApplyEdit()
    {
      AParent parent = new AParent();
      Assert.IsNull(parent.GetChildList(), "GetChildList should be null");

      parent.BeginEdit();
      AChildList list = parent.ChildList;
      Assert.IsNotNull(list, "ChildList should not be null");
      Assert.IsNotNull(parent.GetChildList(), "GetChildList should not be null");

      parent.ApplyEdit();
      Assert.IsNotNull(parent.GetChildList(), "GetChildList should not be null after ApplyEdit");
    }

    [TestMethod]
    public void NullCancelEdit()
    {
      AParent parent = new AParent();
      Assert.IsNull(parent.GetChildList(), "GetChildList should be null");

      parent.BeginEdit();
      AChildList list = parent.ChildList;
      Assert.IsNotNull(list, "ChildList should not be null");
      Assert.IsNotNull(parent.GetChildList(), "GetChildList should not be null");

      parent.BeginEdit();
      list = parent.ChildList;
      Assert.IsNotNull(list, "ChildList should not be null");
      Assert.IsNotNull(parent.GetChildList(), "GetChildList should not be null after 2nd BeginEdit");

      parent.CancelEdit();
      Assert.IsNotNull(parent.GetChildList(), "GetChildList should not be null after 1st CancelEdit");

      parent.CancelEdit();
      Assert.IsNull(parent.GetChildList(), "GetChildList should be null after CancelEdit");
    }

    [TestMethod]
    public void NewChildEditLevel()
    {
      AParent parent = new AParent();
      Assert.IsNull(parent.GetChildList(), "GetChildList should be null");

      parent.BeginEdit();
      AChildList list = parent.ChildList;
      Assert.IsNotNull(list, "ChildList should not be null");
      Assert.IsNotNull(parent.GetChildList(), "GetChildList should not be null");

      Assert.AreEqual(1, parent.EditLevel, "Parent edit level should be 1");
      Assert.AreEqual(1, list.EditLevel, "Child list edit level should be 1");
      Assert.AreEqual(1, list[0].EditLevel, "Child edit level should be 1");

      parent.BeginEdit();
      Assert.AreEqual(2, parent.EditLevel, "Parent edit level should be 2");
      Assert.AreEqual(2, list.EditLevel, "Child list edit level should be 2");
      Assert.AreEqual(2, list[0].EditLevel, "Child edit level should be 2");
    }

    [TestMethod]
    public void NewChildEditLevelCancel()
    {
      AParent parent = new AParent();
      Assert.IsNull(parent.GetChildList(), "GetChildList should be null");

      parent.BeginEdit();
      AChildList list = parent.ChildList;
      Assert.IsNotNull(list, "ChildList should not be null");
      Assert.IsNotNull(parent.GetChildList(), "GetChildList should not be null");

      Assert.AreEqual(1, parent.EditLevel, "Parent edit level should be 1");
      Assert.AreEqual(1, list.EditLevel, "Child list edit level should be 1");
      Assert.AreEqual(1, list[0].EditLevel, "Child edit level should be 1");

      parent.CancelEdit();
      Assert.AreEqual(0, parent.EditLevel, "Parent edit level should be 0");
      Assert.IsNull(parent.GetChildList(), "GetChildList should be null after CancelEdit");
    }

    [TestMethod]
    public void NewChildEditLevelApply()
    {
      AParent parent = new AParent();
      Assert.IsNull(parent.GetChildList(), "GetChildList should be null");

      parent.BeginEdit();
      AChildList list = parent.ChildList;
      Assert.IsNotNull(list, "ChildList should not be null");
      Assert.IsNotNull(parent.GetChildList(), "GetChildList should not be null");

      Assert.AreEqual(1, parent.EditLevel, "Parent edit level should be 1");
      Assert.AreEqual(1, list.EditLevel, "Child list edit level should be 1");
      Assert.AreEqual(1, list[0].EditLevel, "Child edit level should be 1");

      parent.ApplyEdit();
      Assert.AreEqual(0, parent.EditLevel, "Parent edit level should be 0");
      list = parent.GetChildList();
      Assert.AreEqual(0, list.EditLevel, "Child list edit level should be 0");
      Assert.AreEqual(0, list[0].EditLevel, "Child edit level should be 0");
    }
  }
}