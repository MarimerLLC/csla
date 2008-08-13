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
    Style _propertyStatusStyle;

    public Style PropertyStatusStyle
    {
      get { return _propertyStatusStyle;  }
      set { _propertyStatusStyle = value; }
    }

    //public static readonly DependencyProperty PropertyStatusStyleProperty = DependencyProperty.Register(
    //  "PropertyStatusStyle", 
    //  typeof(Style), 
    //  typeof(ValidatedDataGridColumn), 
    //  new PropertyMetadata((o, e) => ((ValidatedDataGridColumn)o).PropertyStatusStyle = (Style)e.NewValue));



    #region Abstract members

    protected abstract FrameworkElement CreateElement();
    protected abstract FrameworkElement CreateEditingElement();

    protected abstract DependencyProperty ElementProperty { get; }
    protected abstract DependencyProperty EditingElementProperty { get; } 

    #endregion
    
    #region Generate elements

    protected override FrameworkElement GenerateEditingElement(object dataItem)
    {
      StackPanel panel = new StackPanel();
      panel.Orientation = Orientation.Horizontal;

      if (dataItem != null && DisplayMemberBinding != null)
      {
        FrameworkElement element = CreateEditingElement();
        element.SetBinding(EditingElementProperty, DisplayMemberBinding);
        panel.Children.Add(element);

        PropertyStatus status = new PropertyStatus();
        Binding binding = new Binding();
        binding.Source = DisplayMemberBinding.Source;
        binding.Mode = BindingMode.OneWay;
        status.Style = PropertyStatusStyle;
        status.SetBinding(PropertyStatus.SourceProperty, binding);
        status.Property = DisplayMemberBinding.Path.Path;
        status.Target = element;

        panel.Children.Add(status);
      }

      return panel;
    }

    protected override FrameworkElement GenerateElement(object dataItem)
    {
      StackPanel panel = new StackPanel();
      panel.Orientation = Orientation.Horizontal;

      if (dataItem != null && DisplayMemberBinding != null)
      {
        FrameworkElement element = CreateElement();
        element.SetBinding(ElementProperty, DisplayMemberBinding);
        panel.Children.Add(element);

        PropertyStatus status = new PropertyStatus();
        Binding binding = new Binding();
        binding.Source = DisplayMemberBinding.Source;
        binding.Mode = BindingMode.OneWay;
        status.Style = PropertyStatusStyle;
        status.SetBinding(PropertyStatus.SourceProperty, binding);
        status.Property = DisplayMemberBinding.Path.Path;
        status.Target = element;

        panel.Children.Add(status);
      }

      return panel;
    } 

    #endregion
  }
}
