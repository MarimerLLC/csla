using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Reflection;

namespace Csla.Wpf
{
  /// <summary>
  /// Container for other UI controls that adds
  /// the ability for the contained controls
  /// to change appearance based on the error
  /// information provided by the data binding
  /// context.
  /// </summary>
  public class ValidationPanel : Decorator
  {
    private bool _loaded;
    private IDataErrorInfo _dataSource;
    private bool _haveRecentChange;

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    public ValidationPanel()
    {
      this.DataContextChanged += new DependencyPropertyChangedEventHandler(ValidationPanel_DataContextChanged);
      this.Loaded += new RoutedEventHandler(ValidationPanel_Loaded);
    }

    /// <summary>
    /// Force the panel to refresh all validation
    /// error status information for all controls
    /// it contains.
    /// </summary>
    public void Refresh()
    {
      _haveRecentChange = true;
      ErrorScan();
    }

    private void ValidationPanel_Loaded(object sender, RoutedEventArgs e)
    {
      ((FrameworkElement)this).LostFocus += new RoutedEventHandler(ValidationPanel_LostFocus);
      _haveRecentChange = true;
      ErrorScan();
      _loaded = true;
    }

    private void ValidationPanel_LostFocus(object sender, RoutedEventArgs e)
    {
      ErrorScan();
    }

    private void ValidationPanel_GotFocus(object sender, RoutedEventArgs e)
    {
      ErrorScan();
    }

    private void DataContext_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      // note that there's been a change, so the 
      // next scan will perform validation
      _haveRecentChange = true;
    }

    private void ValidationPanel_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      INotifyPropertyChanged oldContext = e.OldValue as INotifyPropertyChanged;
      INotifyPropertyChanged newContext = e.NewValue as INotifyPropertyChanged;

      // unhook any old event handling
      if (oldContext != null)
        oldContext.PropertyChanged -= new PropertyChangedEventHandler(DataContext_PropertyChanged);

      // hook any new event
      if (newContext != null)
        newContext.PropertyChanged += new PropertyChangedEventHandler(DataContext_PropertyChanged);

      // store a ref to the data source if it is IDataErrorInfo
      if (e.NewValue is DataSourceProvider)
        _dataSource = ((DataSourceProvider)e.NewValue).Data as IDataErrorInfo;
      else
        _dataSource = e.NewValue as IDataErrorInfo;

      if (_loaded)
        Refresh();
    }

    private void ErrorScan()
    {
      if (_haveRecentChange && _dataSource != null)
      {
        _haveRecentChange = false;
        if (_bindings.Count == 0)
          ReloadBindings();

        if (_dataSource != null && _bindings.Count > 0)
          foreach (BindingInfo item in _bindings)
          {
            string text = _dataSource[item.BindingObject.Path.Path];
            BindingExpression expression = item.Element.GetBindingExpression(item.Property);
            ValidationError error = new ValidationError(new ExceptionValidationRule(), expression, text, null);
            if (string.IsNullOrEmpty(text))
              System.Windows.Controls.Validation.ClearInvalid(expression);
            else
              System.Windows.Controls.Validation.MarkInvalid(expression, error);
          }
      }
    }

    private List<BindingInfo> _bindings = new List<BindingInfo>();

    /// <summary>
    /// Reload all the binding information for the 
    /// controls contained within the
    /// ErrorDisplayContainer.
    /// </summary>
    public void ReloadBindings()
    {
      _bindings.Clear();
      FindBindings(this);
    }

    private void FindBindings(Visual visual)
    {
      for (int i = 0; i < VisualTreeHelper.GetChildrenCount(visual); i++)
      {
        Visual childVisual = (Visual)VisualTreeHelper.GetChild(visual, i);
        MemberInfo[] sharedMembers = childVisual.GetType().GetMembers(
          BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        foreach (MemberInfo member in sharedMembers)
        {
          DependencyProperty prop = null;
          if (member.MemberType == MemberTypes.Field)
            prop = ((FieldInfo)member).GetValue(childVisual) as DependencyProperty;
          else if (member.MemberType == MemberTypes.Property)
            prop = ((PropertyInfo)member).GetValue(childVisual, null) as DependencyProperty;

          if (prop != null)
          {
            Binding bnd = BindingOperations.GetBinding(childVisual, prop);
            if (bnd != null)
            {
              _bindings.Add(new BindingInfo(bnd, (FrameworkElement)childVisual, prop));
              ((FrameworkElement)childVisual).GotFocus += new RoutedEventHandler(ValidationPanel_GotFocus);
            }
          }
        }
        FindBindings(childVisual);
      }
    }

    #region BindingInfo Class

    /// <summary>
    /// Contains details about each binding that
    /// are required to handle the validation
    /// processing.
    /// </summary>
    private class BindingInfo
    {
      private Binding _bindingObject;

      public Binding BindingObject
      {
        get { return _bindingObject; }
        set { _bindingObject = value; }
      }

      private FrameworkElement _element;

      public FrameworkElement Element
      {
        get { return _element; }
        set { _element = value; }
      }

      private DependencyProperty _property;

      public DependencyProperty Property
      {
        get { return _property; }
        set { _property = value; }
      }

      public BindingInfo(Binding binding, FrameworkElement element, DependencyProperty property)
      {
        _bindingObject = binding;
        _element = element;
        _property = property;
      }
    }

    #endregion
  }
}
