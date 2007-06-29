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
using Csla.Security;

namespace Csla.Wpf
{
  /// <summary>
  /// Container for other UI controls that adds
  /// the ability for the contained controls
  /// to change state based on the authorization
  /// information provided by the data binding
  /// context.
  /// </summary>
  public class Authorizer : DataDecoratorBase
  {
    #region NotVisibleMode property

    // Define DependencyProperty
    private static readonly DependencyProperty NotVisibleModeProperty = 
      DependencyProperty.Register(
        "NotVisibleMode", 
        typeof(VisibilityMode), 
        typeof(Authorizer), 
        new FrameworkPropertyMetadata(VisibilityMode.Hidden), 
        new ValidateValueCallback(IsValidVisibilityMode));

    // Define method to validate the value
    private static bool IsValidVisibilityMode(object o)
    {
      return (o is VisibilityMode);
    }

    /// <summary>
    /// Gets or sets the value controlling how controls
    /// bound to non-readable properties will be rendered.
    /// </summary>
    public VisibilityMode NotVisibleMode
    {
      get
      {
        return (VisibilityMode)base.GetValue(NotVisibleModeProperty);
      }
      set
      {
        base.SetValue(NotVisibleModeProperty, value);
      }
    }

    #endregion

    #region ControlVisibility property

    private static readonly DependencyProperty ControlVisibilityProperty =
      DependencyProperty.RegisterAttached(
      "ControlVisibility", typeof(VisibilityMode), typeof(Authorizer));

    /// <summary>
    /// Gets a value specifying the visibility mode
    /// of the specified control. This value is
    /// used to override the NotVisibleMode property
    /// for a specific control.
    /// </summary>
    public static VisibilityMode GetControlVisibility(DependencyObject obj)
    {
      return (VisibilityMode)obj.GetValue(ControlVisibilityProperty);
    }

    /// <summary>
    /// Sets a value specifying the visibility mode
    /// of the specified control. This value is
    /// used to override the NotVisibleMode property
    /// for a specific control.
    /// </summary>
    public static void SetControlVisibility(DependencyObject obj, VisibilityMode visibility)
    {
      obj.SetValue(ControlVisibilityProperty, visibility);
    }

    #endregion

    IAuthorizeReadWrite _source;

    /// <summary>
    /// This method is called when the data
    /// object to which the control is bound
    /// has changed.
    /// </summary>
    protected override void DataObjectChanged()
    {
      Refresh();
    }

    /// <summary>
    /// Refresh authorization and update
    /// all controls.
    /// </summary>
    public void Refresh()
    {
      _source = DataObject as IAuthorizeReadWrite;
      if (_source != null)
        base.FindChildBindings();
    }

    /// <summary>
    /// Check the read and write status
    /// of the control based on the current
    /// user's authorization.
    /// </summary>
    /// <param name="bnd">The Binding object.</param>
    /// <param name="control">The control containing the binding.</param>
    /// <param name="prop">The data bound DependencyProperty.</param>
    protected override void FoundBinding(Binding bnd, FrameworkElement control, DependencyProperty prop)
    {
      SetRead(bnd, (UIElement)control, _source);
      SetWrite(bnd, (UIElement)control, _source);
    }

    private void SetWrite(Binding bnd, UIElement ctl, IAuthorizeReadWrite source)
    {
      bool canWrite = source.CanWriteProperty(bnd.Path.Path);

      // enable/disable writing of the value
      PropertyInfo propertyInfo =
        ctl.GetType().GetProperty("IsReadOnly",
        BindingFlags.FlattenHierarchy |
        BindingFlags.Instance |
        BindingFlags.Public);
      if (propertyInfo != null)
      {
        propertyInfo.SetValue(
          ctl, !canWrite, new object[] { });
      }
      else
      {
        ctl.IsEnabled = canWrite;
      }
    }

    private void SetRead(Binding bnd, UIElement ctl, IAuthorizeReadWrite source)
    {
      bool canRead = source.CanReadProperty(bnd.Path.Path);
      VisibilityMode visibility = NotVisibleMode;
      object controlVisibility = ctl.GetValue(Authorizer.ControlVisibilityProperty);
      if (controlVisibility != null)
        visibility = (VisibilityMode)controlVisibility;

      if (canRead)
        switch (visibility)
        {
          case VisibilityMode.Collapsed:
            if (ctl.Visibility == Visibility.Collapsed)
              ctl.Visibility = Visibility.Visible;
            break;
          case VisibilityMode.Hidden:
            if (ctl.Visibility == Visibility.Hidden)
              ctl.Visibility = Visibility.Visible;
            break;
          default:
            break;
        }
      else
        switch (visibility)
        {
          case VisibilityMode.Collapsed:
            ctl.Visibility = Visibility.Collapsed;
            break;
          case VisibilityMode.Hidden:
            ctl.Visibility = Visibility.Hidden;
            break;
          default:
            break;
        }
    }
  }
}
#endif