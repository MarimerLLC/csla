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
using Csla;
using Csla.Serialization;
using Csla.Xaml;

namespace cslalighttest.ReadOnly
{
  [Serializable]
  public class MockReadOnlyList : ReadOnlyBindingListBase<MockReadOnlyList, MockReadOnly>
  {
    public MockReadOnlyList() { }

    public MockReadOnlyList(MockReadOnly mock)
    {
      IsReadOnly = false;
      Add(mock);
      IsReadOnly = true;
    }
  }
}
