#if !NETFX_CORE && !XAMARIN
//-----------------------------------------------------------------------
// <copyright file="InvokeMethod.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Invokes a method on a target object when a </summary>
//-----------------------------------------------------------------------
using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.Generic;
using Csla.Properties;

namespace Csla.Xaml
{
  /// <summary>
  /// Invokes a method on a target object when a 
  /// trigger event is raised from the attached
  /// UI control.
  /// </summary>
  public class InvokeMethod : FrameworkElement
  {
#region Attached properties

    /// <summary>
    /// Object containing the method to be invoked.
    /// </summary>
    public static readonly DependencyProperty TargetProperty =
      DependencyProperty.RegisterAttached("Target",
      typeof(object),
      typeof(InvokeMethod),
      new PropertyMetadata(null));

    /// <summary>
    /// Sets the object containing the method to be invoked.
    /// </summary>
    /// <param name="ctrl">Attached control</param>
    /// <param name="value">New value</param>
    public static void SetTarget(UIElement ctrl, object value)
    {
      ctrl.SetValue(TargetProperty, value);
    }

    /// <summary>
    /// Gets the object containing the method to be invoked.
    /// </summary>
    /// <param name="ctrl">Attached control</param>
    public static object GetTarget(UIElement ctrl)
    {
      object result = null;
      result = ctrl.GetValue(TargetProperty);
      if (result == null)
      {
        var fe = ctrl as FrameworkElement;
        if (fe != null)
          result = fe.DataContext;
      }
      var icv = result as ICollectionView;
      if (icv != null)
        result = icv.CurrentItem;
      return result;
    }

    /// <summary>
    /// Name of method to be invoked.
    /// </summary>
    public static readonly DependencyProperty MethodNameProperty =
      DependencyProperty.RegisterAttached("MethodName",
      typeof(string),
      typeof(InvokeMethod),
      new PropertyMetadata(null));

    /// <summary>
    /// Sets the name of method to be invoked.
    /// </summary>
    /// <param name="ctrl">Attached control</param>
    /// <param name="value">New value</param>
    public static void SetMethodName(UIElement ctrl, string value)
    {
      ctrl.SetValue(MethodNameProperty, value);
    }

    /// <summary>
    /// Gets the name of method to be invoked.
    /// </summary>
    /// <param name="ctrl">Attached control</param>
    public static string GetMethodName(UIElement ctrl)
    {
      return (string)ctrl.GetValue(MethodNameProperty);
    }

    /// <summary>
    /// Name of event raised by UI control that triggers
    /// invoking the target method.
    /// </summary>
    public static readonly DependencyProperty TriggerEventProperty =
      DependencyProperty.RegisterAttached("TriggerEvent",
      typeof(string),
      typeof(InvokeMethod),
      new PropertyMetadata((o, e) =>
      {
        var ctrl = o as UIElement;
        if (ctrl != null)
          new InvokeMethod(ctrl);
      }));

    /// <summary>
    /// Sets the name of event raised by UI control that triggers
    /// invoking the target method.
    /// </summary>
    /// <param name="ctrl">Attached control</param>
    /// <param name="value">New value</param>
    public static void SetTriggerEvent(UIElement ctrl, string value)
    {
      ctrl.SetValue(TriggerEventProperty, value);
    }

    /// <summary>
    /// Gets the name of event raised by UI control that triggers
    /// invoking the target method.
    /// </summary>
    /// <param name="ctrl">Attached control</param>
    public static string GetTriggerEvent(UIElement ctrl)
    {
      return (string)ctrl.GetValue(TriggerEventProperty);
    }

    /// <summary>
    /// Parameter value to be passed to invoked method.
    /// </summary>
    public static readonly DependencyProperty MethodParameterProperty =
      DependencyProperty.RegisterAttached("MethodParameter",
      typeof(object),
      typeof(InvokeMethod),
      new PropertyMetadata(null));

    /// <summary>
    /// Sets the parameter value to be passed to invoked method.
    /// </summary>
    /// <param name="ctrl">Attached control</param>
    /// <param name="value">New value</param>
    public static void SetMethodParameter(UIElement ctrl, object value)
    {
      ctrl.SetValue(MethodParameterProperty, value);
    }

    /// <summary>
    /// Gets the parameter value to be passed to invoked method.
    /// </summary>
    /// <param name="ctrl">Attached control</param>
    public static object GetMethodParameter(UIElement ctrl)
    {
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

    private UIElement _element;

    /// <summary>
    /// Invokes the target method if all required attached
    /// property values have been set.
    /// </summary>
    /// <param name="ctrl">Attached UI control</param>
    public InvokeMethod(UIElement ctrl)
    {
      _element = ctrl;
      var triggerEvent = GetTriggerEvent(_element);
      if (!string.IsNullOrEmpty(triggerEvent))
      {
        // hook up the trigger event
        var eventRef = ctrl.GetType().GetEvent(triggerEvent);
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
              eventRef.AddEventHandler(ctrl, del);
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

    private void CallMethod(object sender, EventArgs e)
    {
      object target = GetTarget(_element);
      var methodName = GetMethodName(_element);
      var targetMethod = target.GetType().GetMethod(methodName);
      if (targetMethod == null)
        throw new MissingMethodException(methodName);

      object p = GetMethodParameter(_element);
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
          TriggerSource = (FrameworkElement)_element
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
#endif