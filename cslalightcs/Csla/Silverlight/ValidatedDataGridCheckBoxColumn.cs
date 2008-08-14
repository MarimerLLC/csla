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

namespace Csla.Silverlight
{

  public class ValidatedDataGridCheckBoxColumn : ValidatedDataGridColumn
  {
    protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
    {
      StackPanel panel = editingElement as StackPanel;
      bool? isChecked = null;
      if (panel != null)
      {
        CheckBox box = panel.Children[0] as CheckBox;
        if (box != null)
        {
          isChecked = box.IsChecked;
        }
      }
      return isChecked;
    }

    protected override FrameworkElement CreateElement()
    {
      return new CheckBox { IsEnabled = false };
    }

    protected override DependencyProperty ElementProperty
    {
      get { return CheckBox.IsCheckedProperty; }
    }

    protected override FrameworkElement CreateEditingElement()
    {
      return new CheckBox();
    }

    protected override DependencyProperty EditingElementProperty
    {
      get { return CheckBox.IsCheckedProperty; }
    }
  }
}
