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
  /// <summary>
  /// Implements a datagrid text column that
  /// uses PropertyStatus to display validation messages.
  /// </summary>
  public class ValidatedDataGridTextColumn : ValidatedDataGridColumn
  {
    /// <summary>
    /// Prepares a cell for edit.
    /// </summary>
    /// <param name="editingElement">Editing element</param>
    /// <param name="editingEventArgs">Event args</param>
    /// <returns></returns>
    protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
    {
      StackPanel panel = editingElement as StackPanel;
      string text = null;
      if (panel != null)
      {
        TextBox box = panel.Children[0] as TextBox;
        if (box != null)
        {
          box.Focus();
          text = box.Text;
          int length = text.Length;
          KeyEventArgs args = editingEventArgs as KeyEventArgs;
          if ((args != null)) // && (args.Key == Key.F2))
          {
            args.Handled = true;
            box.Select(0, length);
          }
          else box.Select(length, length);
        }
      }
      return text;
    }

    /// <summary>
    /// Creates an element.
    /// </summary>
    protected override FrameworkElement CreateElement()
    {
      return new TextBlock();
    }

    /// <summary>
    /// Gets the Text property value.
    /// </summary>
    protected override DependencyProperty ElementProperty
    {
      get { return TextBlock.TextProperty; }
    }

    /// <summary>
    /// Creates the editing element.
    /// </summary>
    protected override FrameworkElement CreateEditingElement()
    {
      return new TextBox();
    }

    /// <summary>
    /// Gets the Text property value.
    /// </summary>
    protected override DependencyProperty EditingElementProperty
    {
      get { return TextBox.TextProperty; }
    }
  }
}
