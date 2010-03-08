using System;
using System.Windows;
using System.Windows.Interactivity;
using System.ComponentModel;

namespace Csla.Xaml
{
  /// <summary>
  /// Executes a method on the current DataContext.
  /// </summary>
  public class Execute : TriggerAction<FrameworkElement>
  {
    /// <summary>
    /// Gets or sets the name of the method
    /// to be invoked.
    /// </summary>
    public static readonly DependencyProperty MethodNameProperty =
      DependencyProperty.Register("MethodName", typeof(string),
      typeof(Execute), new PropertyMetadata(null));
    /// <summary>
    /// Gets or sets the name of the method
    /// to be invoked.
    /// </summary>
    [Category("Common")]
    public string MethodName
    {
      get
      {
        return (string)GetValue(MethodNameProperty);
      }
      set
      {
        SetValue(MethodNameProperty, value);
      }
    }

    /// <summary>
    /// Gets or sets the value of a parameter to
    /// be passed to the invoked method.
    /// </summary>
    public static readonly DependencyProperty MethodParameterProperty =
      DependencyProperty.Register("MethodParameter", typeof(object),
      typeof(Execute), new PropertyMetadata(null));
    /// <summary>
    /// Gets or sets the value of a parameter to
    /// be passed to the invoked method.
    /// </summary>
    [Category("Common")]
    public object MethodParameter
    {
      get
      {
        return (object)GetValue(MethodParameterProperty);
      }
      set
      {
        SetValue(MethodParameterProperty, value);
      }
    }

    /// <summary>
    /// Executes the specified method.
    /// </summary>
    /// <param name="parameter">
    /// EventArgs parameter from the event that
    /// triggered this invocation.
    /// </param>
    protected override void Invoke(object parameter)
    {
      var methodName = this.MethodName;
      if (!string.IsNullOrEmpty(methodName))
      {
        var element = this.AssociatedObject;
        var target = element.DataContext;
        if (target != null)
        {
          var icv = target as ICollectionView;
          if (icv != null)
            target = icv.CurrentItem;
          var methodInfo = target.GetType().GetMethod(methodName);
          if (methodInfo == null)
            throw new MissingMethodException(methodName);

          var pCount = methodInfo.GetParameters().Length;
          try
          {
            if (pCount == 0)
              methodInfo.Invoke(target, null);
            else if (pCount == 2)
              methodInfo.Invoke(target, new object[] { this, new ExecuteEventArgs
              {
                  TriggerSource = element,
                  MethodParameter = this.MethodParameter,
                  TriggerParameter = parameter
              }});
            else
              throw new NotSupportedException(Csla.Properties.Resources.ExecuteBadParams);
          }
          catch (System.Reflection.TargetInvocationException ex)
          {
            if (ex.InnerException != null)
              throw ex.InnerException;
            else
              throw;
          }
        }
      }
    }
  }
}
