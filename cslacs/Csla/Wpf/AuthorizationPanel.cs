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
  public class AuthorizationPanel : Decorator
  {
    private bool _loaded;
    private IAuthorizeReadWrite _dataSource;

    // Define DependencyProperty
    private static readonly DependencyProperty VisibilityModeProperty = 
      DependencyProperty.Register(
        "NotVisibleMode", 
        typeof(VisibilityMode), 
        typeof(AuthorizationPanel), 
        new FrameworkPropertyMetadata(VisibilityMode.Hidden), 
        new ValidateValueCallback(IsValidVisibilityMode));

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    public AuthorizationPanel()
    {
      this.DataContextChanged += new DependencyPropertyChangedEventHandler(AuthorizationPanel_DataContextChanged);
      this.Loaded += new RoutedEventHandler(AuthorizationPanel_Loaded);
    }

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
        return (VisibilityMode)base.GetValue(VisibilityModeProperty);
      }
      set
      {
        base.SetValue(VisibilityModeProperty, value);
      }
    }

    private void AuthorizationPanel_Loaded(object sender, RoutedEventArgs e)
    {
      Refresh();
      _loaded = true;
    }

    private void AuthorizationPanel_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
      // store a ref to the data source if it is IAuthorizeReadWrite
      _dataSource = e.NewValue as IAuthorizeReadWrite;

      if (_loaded)
        Refresh();
    }

    /// <summary>
    /// Refresh authorization and update
    /// all controls.
    /// </summary>
    public void Refresh()
    {
      if (_dataSource != null)
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
            if (bnd != null && bnd.RelativeSource == null)
            {
              SetRead(bnd, (UIElement)childVisual);
              SetWrite(bnd, (UIElement)childVisual);
            }
          }
        }
        FindBindings(childVisual);
      }
    }

    private void SetWrite(Binding bnd, UIElement ctl)
    {
      bool canWrite = _dataSource.CanWriteProperty(bnd.Path.Path);

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

    private void SetRead(Binding bnd, UIElement ctl)
    {
      bool canRead = _dataSource.CanReadProperty(bnd.Path.Path);

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
