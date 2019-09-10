using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MVVMexperiment
{
  public class DetailModel : Csla.Xaml.ViewModel<Data>
  {
    public static readonly DependencyProperty SelectedItemsProperty =
        DependencyProperty.Register("SelectedItems", typeof(List<Data>), typeof(DetailModel), null);

    public List<Data> SelectedItems
    {
      get { return (List<Data>)GetValue(SelectedItemsProperty); }
      set { SetValue(SelectedItemsProperty, value); }
    }

    public void Home()
    {
      MainPageModel.ShowForm(new ListPage());
    }
  }
}
