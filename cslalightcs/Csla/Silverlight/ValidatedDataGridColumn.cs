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

namespace Csla.Silverlight
{

  public abstract class ValidatedDataGridColumn : DataGridBoundColumn
  {
    #region Abstract members

    protected abstract FrameworkElement CreateElement();
    protected abstract FrameworkElement CreateEditingElement();

    protected abstract DependencyProperty ElementProperty { get; }
    protected abstract DependencyProperty EditingElementProperty { get; }

    private FrameworkElement _editControl;
    #endregion
    
    #region Generate elements

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
        Binding binding = new Binding();
        binding.Source = Binding.Source;
        binding.Mode = BindingMode.OneWay;
        status.SetBinding(PropertyStatus.SourceProperty, binding);
        status.Property = Binding.Path.Path;
        status.Target = _editControl;

        panel.Children.Add(status);
      }

      return panel;
    }

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
        Binding binding = new Binding();
        binding.Source = Binding.Source;
        binding.Mode = BindingMode.OneWay;
        status.SetBinding(PropertyStatus.SourceProperty, binding);
        status.Property = Binding.Path.Path;
        status.Target = element;

        panel.Children.Add(status);
      }

      return panel;
    }

    protected override void CancelCellEdit(FrameworkElement editingElement, object uneditedValue)
    {
      _editControl.SetValue(EditingElementProperty, uneditedValue);

      base.CancelCellEdit(editingElement, uneditedValue);
    }

    #endregion
  }
}
