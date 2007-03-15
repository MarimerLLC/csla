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
  public class ValidationPanel : DataPanelBase
  {
    private bool _haveRecentChange;

    /// <summary>
    /// Creates a new instance of the object.
    /// </summary>
    public ValidationPanel()
    {
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

    /// <summary>
    /// Reload all the binding information for the 
    /// controls contained within the
    /// ErrorDisplayContainer, and refresh
    /// the validation status.
    /// </summary>
    public void ReloadBindings()
    {
      _bindings.Clear();
      FindBindings(this);
      Refresh();
    }

    /// <summary>
    /// This method is called when a property
    /// of the data object to which the 
    /// control is bound has changed.
    /// </summary>
    protected override void DataPropertyChanged(PropertyChangedEventArgs e)
    {
      // note that there's been a change, so the 
      // next scan will perform validation
      _haveRecentChange = true;
    }

    /// <summary>
    /// This method is called when the data
    /// object to which the control is bound
    /// has changed.
    /// </summary>
    protected override void DataObjectChanged()
    {
      ReloadBindings();
    }

    #region Trigger Validation

    private void ValidationPanel_Loaded(object sender, RoutedEventArgs e)
    {
      ((FrameworkElement)this).LostFocus += new RoutedEventHandler(ValidationPanel_LostFocus);
      _haveRecentChange = true;
      ErrorScan();
    }

    private void ValidationPanel_LostFocus(object sender, RoutedEventArgs e)
    {
      ErrorScan();
    }

    private void ValidationPanel_GotFocus(object sender, RoutedEventArgs e)
    {
      ErrorScan();
    }

    #endregion

    #region Validation implementation

    private void ErrorScan()
    {
      IDataErrorInfo source = DataObject as IDataErrorInfo;
      if (_haveRecentChange && source != null)
      {
        _haveRecentChange = false;
        if (_bindings.Count == 0)
          ReloadBindings();

        if (source != null && _bindings.Count > 0)
          foreach (BindingInfo item in _bindings)
          {
            string text = source[item.BindingObject.Path.Path];
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

    #endregion
  }
}
