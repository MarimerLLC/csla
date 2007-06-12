#if !NET20
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
      ReloadBindings(true);
    }

    private void ReloadBindings(bool refreshAfter)
    {
      _bindings.Clear();
      base.FindChildBindings();
      if (refreshAfter)
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
    /// This method is called if the data
    /// object is an IBindingList, and the 
    /// ListChanged event was raised by
    /// the data object.
    /// </summary>
    protected override void DataBindingListChanged(ListChangedEventArgs e)
    {
      Refresh();
    }

    /// <summary>
    /// This method is called if the data
    /// object is an INotifyCollectionChanged, 
    /// and the CollectionChanged event was 
    /// raised by the data object.
    /// </summary>
    protected override void DataObservableCollectionChanged(
      System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      Refresh();
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
      //ErrorScan();
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
          ReloadBindings(false);

        if (source != null && _bindings.Count > 0)
          foreach (BindingInfo item in _bindings)
          {
            BindingExpression expression = item.Element.GetBindingExpression(item.Property);
            if (expression != null)
            {
              string text = source[item.BindingObject.Path.Path];
              if (string.IsNullOrEmpty(text))
                System.Windows.Controls.Validation.ClearInvalid(expression);
              else
              {
                ValidationError error = 
                  new ValidationError(new ExceptionValidationRule(), expression, text, null);
                System.Windows.Controls.Validation.MarkInvalid(expression, error);
              }
            }
          }
      }
    }

    private List<BindingInfo> _bindings = new List<BindingInfo>();

    /// <summary>
    /// Store the binding for use in
    /// validation processing.
    /// </summary>
    /// <param name="bnd">The Binding object.</param>
    /// <param name="control">The control containing the binding.</param>
    /// <param name="prop">The data bound DependencyProperty.</param>
    protected override void FoundBinding(Binding bnd, FrameworkElement control, DependencyProperty prop)
    {
      _bindings.Add(new BindingInfo(bnd, control, prop));
      control.GotFocus += new RoutedEventHandler(ValidationPanel_GotFocus);
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
#endif