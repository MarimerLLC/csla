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
      this.DataContextChanged += new DependencyPropertyChangedEventHandler(ErrorDisplayContainer_DataContextChanged);
      this.Loaded += new RoutedEventHandler(ErrorDisplayContainer_Loaded);
    }

    private void ErrorDisplayContainer_Loaded(object sender, RoutedEventArgs e)
    {
      ((FrameworkElement)this).LostFocus += new RoutedEventHandler(ErrorDisplayContainer_LostFocus);
      _haveRecentChange = true;
      ErrorScan();
      _loaded = true;
    }

    void ErrorDisplayContainer_LostFocus(object sender, RoutedEventArgs e)
    {
      ErrorScan();
    }

    void ErrorDisplayContainer_GotFocus(object sender, RoutedEventArgs e)
    {
      ErrorScan();
    }

    private void DataContext_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      _haveRecentChange = true;
    }

    private void ErrorDisplayContainer_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
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
      _dataSource = e.NewValue as IDataErrorInfo;

      if (_loaded)
      {
        _haveRecentChange = true;
        ErrorScan();
      }
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
        if (childVisual.GetType().Equals(typeof(TextBox)))
        {
          TextBox ctl = (TextBox)childVisual;
          Binding bnd = BindingOperations.GetBinding(ctl, TextBox.TextProperty);
          _bindings.Add(new BindingInfo(bnd, (FrameworkElement)childVisual, TextBox.TextProperty));
          ((FrameworkElement)childVisual).GotFocus += new RoutedEventHandler(ErrorDisplayContainer_GotFocus);
        }
        FindBindings(childVisual);
      }
    }
  }

  internal class BindingInfo
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
}
