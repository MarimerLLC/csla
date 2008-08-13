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

  public class ValidatedDataGridTextColumn : ValidatedDataGridColumn
  {
    protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
    {
      StackPanel panel = editingElement as StackPanel;
      string text = null;
      if (panel != null)
      {
        TextBox box = panel.Children[0] as TextBox;
        if (box != null)
        {
          text = box.Text;
          int length = text.Length;
          KeyEventArgs args = editingEventArgs as KeyEventArgs;
          if ((args != null) && (args.Key == Key.F2))
          {
            box.Select(length, length);
            return text;
          }
          box.Select(0, length);
        }
      }
      return text;
    }

    protected override FrameworkElement CreateElement()
    {
      return new TextBlock();
    }

    protected override DependencyProperty ElementProperty
    {
      get { return TextBlock.TextProperty; }
    }

    protected override FrameworkElement CreateEditingElement()
    {
      return new TextBox();
    }

    protected override DependencyProperty EditingElementProperty
    {
      get { return TextBox.TextProperty; }
    }
  }
}
