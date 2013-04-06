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

namespace cslalighttest.Serialization
{
  [Serializable]
  public class MockReadOnly : BusinessBase<MockReadOnly>
  {
    /// <summary>
    /// For constructing test mockreadonly objects
    /// </summary>
    /// <param name="id"></param>
    public MockReadOnly(int id)
    {
      SetProperty<int>(IdProperty, id);
    }

    public MockReadOnly() { }

    private static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(
      typeof(MockReadOnly),
      new PropertyInfo<int>("Id"));

    public int Id
    {
      get { return GetProperty<int>(IdProperty); }
    }
  }
}
