using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Xaml;
using System.Windows;

namespace MVVMexperiment
{
  public class DataViewModel : ViewModel<Data>
  {
    /// <summary>
    /// Gets or sets the Test object.
    /// </summary>
    public static readonly DependencyProperty TestProperty =
        DependencyProperty.Register("Test", typeof(object), typeof(DataViewModel),
        new PropertyMetadata((o, e) =>
        {
          ((DataViewModel)o).Model = (Data)e.NewValue;
        }));
    /// <summary>
    /// Gets or sets the Test object.
    /// </summary>
    public object Test
    {
      get { return GetValue(TestProperty); }
      set { SetValue(TestProperty, value); }
    }
  }
}
