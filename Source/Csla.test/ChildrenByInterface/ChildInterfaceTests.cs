//-----------------------------------------------------------------------
// <copyright file="ChildInterfaceTests.cs" company="Marimer LLC">
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

namespace Csla.Test.ChildrenByInterface
{
  [TestClass]
  public class ChildInterfaceTests
  {
    [TestMethod]
    public void AddItems()
    {
      ItemList list = new ItemList();
      list.Add(new Item1());
      list.Add(new Item2());
      
      Assert.IsTrue(list[0] is Item1, "First element should be Item1");
      Assert.IsTrue(list[1] is Item2, "Second element should be Item2");
    }
  }

  public interface IItem : Csla.Core.IEditableBusinessObject
  { }

  [Serializable]
  public class Item1 : Csla.BusinessBase<Item1>, IItem
  {
    public Item1()
    {
      MarkAsChild();
    }

    protected override object GetIdValue()
    {
      return 0;
    }
  }

  [Serializable]
  public class Item2 : Csla.BusinessBase<Item2>, IItem
  {
    public Item2()
    {
      MarkAsChild();
    }

    protected override object GetIdValue()
    {
      return 0;
    }
  }

  [Serializable]
  public class ItemList : Csla.BusinessBindingListBase<ItemList, IItem>
  {
  }
}