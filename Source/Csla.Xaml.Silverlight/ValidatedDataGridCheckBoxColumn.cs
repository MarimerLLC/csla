//-----------------------------------------------------------------------
// <copyright file="ValidatedDataGridCheckBoxColumn.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Implements a checkbox column for the datagrid</summary>
//-----------------------------------------------------------------------
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

namespace Csla.Xaml
{
  /// <summary>
  /// Implements a checkbox column for the datagrid
  /// that uses PropertyStatus for validation display.
  /// </summary>
  public class ValidatedDataGridCheckBoxColumn : ValidatedDataGridColumn
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

    /// <summary>
    /// Creates an element.
    /// </summary>
    /// <returns></returns>
    protected override FrameworkElement CreateElement()
    {
      return new CheckBox { IsEnabled = false };
    }

    /// <summary>
    /// Gets the IsChecked property value.
    /// </summary>
    protected override DependencyProperty ElementProperty
    {
      get { return CheckBox.IsCheckedProperty; }
    }

    /// <summary>
    /// Creates the editing element.
    /// </summary>
    /// <returns></returns>
    protected override FrameworkElement CreateEditingElement()
    {
      return new CheckBox();
    }

    /// <summary>
    /// Gets the IsChecked property value.
    /// </summary>
    protected override DependencyProperty EditingElementProperty
    {
      get { return CheckBox.IsCheckedProperty; }
    }
  }
}