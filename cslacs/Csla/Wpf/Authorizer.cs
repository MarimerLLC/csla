using System;
using System.Windows;
using System.Windows.Data;
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

    // Define attached DependencyProperty
    private static readonly DependencyProperty NotVisibleModeProperty = 
      DependencyProperty.RegisterAttached(
        "NotVisibleMode", 
        typeof(VisibilityMode), 
        typeof(Authorizer),
        new FrameworkPropertyMetadata(VisibilityMode.Collapsed), 
        new ValidateValueCallback(IsValidVisibilityMode));

    // Define method to validate the value
    private static bool IsValidVisibilityMode(object o)
    {
      return (o is VisibilityMode);
    }

    /// <summary>
    /// Gets the value controlling how controls
    /// bound to non-readable properties will be rendered.
    /// </summary>
    public static VisibilityMode GetNotVisibleMode(DependencyObject obj)
    {
      return (VisibilityMode)obj.GetValue(NotVisibleModeProperty);
    }

    /// <summary>
    /// Sets the value controlling how controls
    /// bound to non-readable properties will be rendered.
    /// </summary>
    public static void SetNotVisibleMode(DependencyObject obj, VisibilityMode mode)
    {
      obj.SetValue(NotVisibleModeProperty, mode);
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
      VisibilityMode visibilityMode = GetNotVisibleMode(ctl);

      if (canRead)
        switch (visibilityMode)
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
        switch (visibilityMode)
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