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
  public class AuthorizationPanel : DataPanelBase
  {
    #region NotVisibleMode property

    // Define DependencyProperty
    private static readonly DependencyProperty NotVisibleModeProperty = 
      DependencyProperty.Register(
        "NotVisibleMode", 
        typeof(VisibilityMode), 
        typeof(AuthorizationPanel), 
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
      IAuthorizeReadWrite source = DataObject as IAuthorizeReadWrite;
      if (source != null)
        FindBindings(this, source);
    }

    private void FindBindings(Visual visual, IAuthorizeReadWrite source)
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
            if (bnd != null && bnd.RelativeSource == null)
            {
              SetRead(bnd, (UIElement)childVisual, source);
              SetWrite(bnd, (UIElement)childVisual, source);
            }
          }
        }
        FindBindings(childVisual, source);
      }
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

      if (canRead)
        switch (NotVisibleMode)
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
        switch (NotVisibleMode)
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
