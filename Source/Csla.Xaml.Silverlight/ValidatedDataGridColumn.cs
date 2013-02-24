//-----------------------------------------------------------------------
// <copyright file="ValidatedDataGridColumn.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Implements a column for the datagrid</summary>
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
using System.Windows.Data;
using Csla.Xaml;

namespace Csla.Xaml
{
  /// <summary>
  /// Implements a column for the datagrid
  /// that uses PropertyStatus for validation display.
  /// </summary>
  public abstract class ValidatedDataGridColumn : DataGridBoundColumn
  {
    #region Abstract members

    /// <summary>
    /// Creates the UI element.
    /// </summary>
    protected abstract FrameworkElement CreateElement();
    /// <summary>
    /// Creates the UI editing element.
    /// </summary>
    protected abstract FrameworkElement CreateEditingElement();
    /// <summary>
    /// Gets the element property value.
    /// </summary>
    protected abstract DependencyProperty ElementProperty { get; }
    /// <summary>
    /// Gets the element editing property value.
    /// </summary>
    protected abstract DependencyProperty EditingElementProperty { get; }

    private FrameworkElement _editControl;
    #endregion
    
    #region Generate elements

    /// <summary>
    /// Generates the editing element.
    /// </summary>
    /// <param name="cell">Cell for content</param>
    /// <param name="dataItem">Data item for binding</param>
    protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
    {
      StackPanel panel = new StackPanel();
      panel.Orientation = Orientation.Horizontal;

      if (dataItem != null && Binding != null)
      {
        _editControl = CreateEditingElement();
        _editControl.SetBinding(EditingElementProperty, Binding);
        panel.Children.Add(_editControl);

        PropertyStatus status = new PropertyStatus();
        status.SetBinding(PropertyStatus.PropertyProperty, Binding);

        panel.Children.Add(status);
      }

      return panel;
    }

    /// <summary>
    /// Generates the display element.
    /// </summary>
    /// <param name="cell">Cell for content</param>
    /// <param name="dataItem">Data item for binding</param>
    protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
    {
      StackPanel panel = new StackPanel();
      panel.Orientation = Orientation.Horizontal;

      if (dataItem != null && Binding != null)
      {
        FrameworkElement element = CreateElement();
        element.SetBinding(ElementProperty, Binding);
        panel.Children.Add(element);

        PropertyStatus status = new PropertyStatus();
        status.SetBinding(PropertyStatus.PropertyProperty, Binding);

        panel.Children.Add(status);
      }

      return panel;
    }

    /// <summary>
    /// Cancels edits on the cell.
    /// </summary>
    /// <param name="editingElement">Editing element</param>
    /// <param name="uneditedValue">Unedited value</param>
    protected override void CancelCellEdit(FrameworkElement editingElement, object uneditedValue)
    {
      _editControl.SetValue(EditingElementProperty, uneditedValue);

      base.CancelCellEdit(editingElement, uneditedValue);
    }

    #endregion
  }
}