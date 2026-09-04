//-----------------------------------------------------------------------
// <copyright file="ChildInterfaceTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.ChildrenByInterface
{
  [TestClass]
  public class ChildInterfaceTests
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
    public void AddItems()
    {
      ItemList list = _testHost.GetDataPortal<ItemList>().Create();
      list.AddRange([new Item1(), new Item2()]);

      Assert.IsTrue(list[0] is Item1, "First element should be Item1");
      Assert.IsTrue(list[1] is Item2, "Second element should be Item2");
    }
  }

  public interface IItem : Core.IEditableBusinessObject;

  [Serializable]
  public class Item1 : BusinessBase<Item1>, IItem
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
  public class Item2 : BusinessBase<Item2>, IItem
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
  public class ItemList : BusinessBindingListBase<ItemList, IItem>
  {
    [Create, RunLocal]
    private void Create()
    {

    }
  }
}