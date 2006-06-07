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

namespace Csla.Test.Basic
{
  [TestClass]
  public class CollectionTests
  {
    [TestMethod]
    public void SetLast()
    {
      TestCollection list = new TestCollection();
      list.Add(new TestItem());
      list.Add(new TestItem());
      TestItem oldItem = new TestItem();
      list.Add(oldItem);
      TestItem newItem = new TestItem();
      list[2] = newItem;
      Assert.AreEqual(3, list.Count, "List should have 3 items");
      Assert.AreEqual(newItem, list[2], "Last item should be newItem");
      Assert.AreEqual(true, list.ContainsDeleted(oldItem), "Deleted list should have old item");
    }
  }

  [Serializable]
  public class TestCollection : BusinessListBase<TestCollection, TestItem>
  {
  }

  [Serializable]
  public class TestItem : BusinessBase<TestItem>
  {
    protected override object GetIdValue()
    {
      return 0;
    }

    public TestItem()
    {
      MarkAsChild();
    }
  }
}
