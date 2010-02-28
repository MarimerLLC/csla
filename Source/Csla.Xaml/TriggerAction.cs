using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;

namespace Csla.Xaml
{
  /// <summary>
  /// Control used to invoke a method on the DataContext
  /// based on an event being raised by a UI control.
  /// </summary>
  public class TriggerAction : FrameworkElement
  {
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    public TriggerAction()
    {
      Visibility = System.Windows.Visibility.Collapsed;
      Height = 20;
      Width = 20;
    }

    private void HookEvent(FrameworkElement oldTarget, FrameworkElement newTarget)
    {
      if (!string.IsNullOrEmpty(TriggerEvent) && !ReferenceEquals(oldTarget, newTarget))
      {
        if (oldTarget != null)
        {
          var eventRef = oldTarget.GetType().GetEvent(TriggerEvent);
          if (eventRef != null)
          {
            var invoke = eventRef.EventHandlerType.GetMethod("Invoke");
            var p = invoke.GetParameters();
            if (p.Length == 2)
            {
              var p1Type = p[1].ParameterType;
              if (typeof(EventArgs).IsAssignableFrom(p1Type))
              {
                var del = Delegate.CreateDelegate(eventRef.EventHandlerType,
                  this,
                  this.GetType().GetMethod("CallMethod", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic));
                eventRef.RemoveEventHandler(oldTarget, del);
              }
              else
              {
                throw new NotSupportedException(Csla.Properties.Resources.ExecuteBadTriggerEvent);
              }
            }
            else
              throw new NotSupportedException(Csla.Properties.Resources.ExecuteBadTriggerEvent);
          }
        }

        if (newTarget != null)
        {
          var eventRef = newTarget.GetType().GetEvent(TriggerEvent);
          if (eventRef != null)
          {
            var invoke = eventRef.EventHandlerType.GetMethod("Invoke");
            var p = invoke.GetParameters();
            if (p.Length == 2)
            {
              var p1Type = p[1].ParameterType;
              if (typeof(EventArgs).IsAssignableFrom(p1Type))
              {
                var del = Delegate.CreateDelegate(eventRef.EventHandlerType,
                  this,
                  this.GetType().GetMethod("CallMethod", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic));
                eventRef.AddEventHandler(newTarget, del);
              }
              else
              {
                throw new NotSupportedException(Csla.Properties.Resources.ExecuteBadTriggerEvent);
              }
            }
            else
              throw new NotSupportedException(Csla.Properties.Resources.ExecuteBadTriggerEvent);
          }
        }
      }
    }

    private void CallMethod(object sender, EventArgs e)
    {
      object target = this.DataContext;
      var icv = target as ICollectionView;
      if (icv != null)
        target = icv.CurrentItem;
      var targetMethod = target.GetType().GetMethod(MethodName);
      if (targetMethod == null)
        throw new MissingMethodException(MethodName);

      object p = MethodParameter;
      var pCount = targetMethod.GetParameters().Length;
      try
      {
        if (pCount == 0)
          targetMethod.Invoke(target, null);
        else if (pCount == 2)
          targetMethod.Invoke(target, new object[] { this, new ExecuteEventArgs
        {
          MethodParameter = p,
          TriggerParameter = e,
          TriggerSource = TargetControl
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

    #region Properties

    /// <summary>
    /// Gets or sets the target UI control.
    /// </summary>
    public static readonly DependencyProperty TargetControlProperty =
      DependencyProperty.Register("TargetControl", typeof(FrameworkElement),
      typeof(TriggerAction), new PropertyMetadata((o, e) =>
        {
          ((TriggerAction)o).HookEvent(
            (FrameworkElement)e.OldValue, (FrameworkElement)e.NewValue);
        }));
    /// <summary>
    /// Gets or sets the target UI control.
    /// </summary>
    [Category("Trigger Action")]
    public FrameworkElement TargetControl
    {
      get { return (FrameworkElement)GetValue(TargetControlProperty); }
      set { SetValue(TargetControlProperty, value); }
    }

    /// <summary>
    /// Gets or sets the name of the event
    /// that will trigger the action.
    /// </summary>
    public static readonly DependencyProperty TriggerEventProperty =
      DependencyProperty.Register("TriggerEvent", typeof(string),
      typeof(TriggerAction), new PropertyMetadata(null));
    /// <summary>
    /// Gets or sets the name of the event
    /// that will trigger the action.
    /// </summary>
    [Category("Trigger Action")]
    public string TriggerEvent
    {
      get { return (string)GetValue(TriggerEventProperty); }
      set { SetValue(TriggerEventProperty, value); }
    }

    /// <summary>
    /// Gets or sets the name of the method
    /// to be invoked.
    /// </summary>
    public static readonly DependencyProperty MethodNameProperty =
      DependencyProperty.Register("MethodName", typeof(string),
      typeof(TriggerAction), new PropertyMetadata(null));
    /// <summary>
    /// Gets or sets the name of the method
    /// to be invoked.
    /// </summary>
    [Category("Trigger Action")]
    public string MethodName
    {
      get { return (string)GetValue(MethodNameProperty); }
      set { SetValue(MethodNameProperty, value); }
    }

    /// <summary>
    /// Gets or sets the value of a parameter to
    /// be passed to the invoked method.
    /// </summary>
    public static readonly DependencyProperty MethodParameterProperty =
      DependencyProperty.Register("MethodParameter", typeof(object),
      typeof(TriggerAction), new PropertyMetadata(null));
    /// <summary>
    /// Gets or sets the value of a parameter to
    /// be passed to the invoked method.
    /// </summary>
    [Category("Trigger Action")]
    public object MethodParameter
    {
      get { return (object)GetValue(MethodParameterProperty); }
      set { SetValue(MethodParameterProperty, value); }
    }

    #endregion

    #region GetMethodParameter

    /// <summary>
    /// Gets the parameter value to be passed to invoked method.
    /// </summary>
    /// <param name="ctrl">Attached control</param>
    public static object GetMethodParameter(UIElement ctrl)
    {
#if SILVERLIGHT
      var fe = ctrl as FrameworkElement;
      if (fe != null)
      {
        var be = fe.GetBindingExpression(MethodParameterProperty);
        if (be != null && be.ParentBinding != null)
        {
          var newBinding = CopyBinding(be.ParentBinding);
          fe.SetBinding(MethodParameterProperty, newBinding);
        }
      }
#endif
      return ctrl.GetValue(MethodParameterProperty);
    }

    private static System.Windows.Data.Binding CopyBinding(System.Windows.Data.Binding oldBinding)
    {
      var result = new System.Windows.Data.Binding();
      result.BindsDirectlyToSource = oldBinding.BindsDirectlyToSource;
      result.Converter = oldBinding.Converter;
      result.ConverterCulture = oldBinding.ConverterCulture;
      result.ConverterParameter = oldBinding.ConverterParameter;
      result.Mode = oldBinding.Mode;
      result.NotifyOnValidationError = oldBinding.NotifyOnValidationError;
      result.Path = oldBinding.Path;
      if (oldBinding.ElementName != null)
        result.ElementName = oldBinding.ElementName;
      else if (oldBinding.RelativeSource != null)
        result.RelativeSource = oldBinding.RelativeSource;
      else
        result.Source = oldBinding.Source;
      result.UpdateSourceTrigger = oldBinding.UpdateSourceTrigger;
      result.ValidatesOnExceptions = oldBinding.ValidatesOnExceptions;
      return result;
    }

    #endregion

    private void Invoke(object parameter)
    {
      var methodName = this.MethodName;
      if (!string.IsNullOrEmpty(methodName))
      {
        var target = this.DataContext;
        if (target != null)
        {
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
                  TriggerSource = TargetControl,
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
