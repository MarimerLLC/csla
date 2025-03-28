//-----------------------------------------------------------------------
// <copyright file="ChildInterfaceTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.TestHelpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Csla.Test.ChildrenByInterface
{
  [TestClass]
  public class ChildInterfaceTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext context)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    [TestInitialize]
    public void Initialize()
    {
      TestResults.Reinitialise();
    }

    [TestMethod]
    public void AddItems()
    {
      var portal = _testDIContext.CreateDataPortal<ItemList>();
      var child1Portal = _testDIContext.CreateDataPortal<Item1>();
      var child2Portal = _testDIContext.CreateDataPortal<Item2>();
      var list = portal.Create();
      list.AddRange(
      [
        child1Portal.Create(),
        child2Portal.Create()

      ]);

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

    [Create]
    private void Create()
    {
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

    [Create]
    private void Create()
    {
    }
  }

  [Serializable]
  public class ItemList : BusinessBindingListBase<ItemList, IItem>
  {
    [Create]
    private void Create()
    {
      AllowNew = true;
      AllowEdit = true;
      AllowRemove = true;
    }
  }
}