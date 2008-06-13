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
using Csla.Silverlight;

namespace cslalighttest.Mock.Serialization
{
  [Serializable]
  public class MockReadOnlyList : ReadOnlyListBase<MockReadOnlyList, MockReadOnly>
  {
    public MockReadOnlyList() { }

    public MockReadOnlyList(params MockReadOnly[] items)
    {
      IsReadOnly = false;
      AddRange(items);
      IsReadOnly = true;
    }

    protected override void AddNewCore()
    {
      DataPortal<MockReadOnly> dp = new DataPortal<MockReadOnly>();
      dp.CreateCompleted += OnCoreAdded;
      dp.SendCreate();
    }
  }
}
