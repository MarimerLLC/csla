using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using cslalighttest.Engine;

namespace cslalighttest.ReadOnly
{
  [TestClass]
  public class ReadOnlyTests
  {
    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void AddItemToReadOnlyFail()
    {
      MockReadOnlyList list = new MockReadOnlyList();
      MockReadOnly mock = new MockReadOnly();
      list.Add(mock);
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void InsertItemToReadOnlyFail()
    {
      MockReadOnlyList list = new MockReadOnlyList();
      MockReadOnly mock = new MockReadOnly();
      list.Insert(0, mock);
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void IndexInsertItemToReadOnlyFail()
    {
      MockReadOnlyList list = new MockReadOnlyList(new MockReadOnly());
      list[0] = new MockReadOnly();
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void AddNewToReadOnlyFail()
    {
      MockReadOnlyList list = new MockReadOnlyList();
      list.AddNew();
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void ClearReadOnlyFail()
    {
      MockReadOnlyList list = new MockReadOnlyList();
      list.Clear();
    }
  }
}
