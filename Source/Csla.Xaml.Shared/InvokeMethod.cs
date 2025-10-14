#if !NETFX_CORE && !XAMARIN && !MAUI
//-----------------------------------------------------------------------
// <copyright file="InvokeMethod.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Invokes a method on a target object when a </summary>
//-----------------------------------------------------------------------
using System.ComponentModel;
using System.Windows;

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
    /// <exception cref="ArgumentNullException"><paramref name="ctrl"/> is <see langword="null"/>.</exception>
    public static void SetTarget(UIElement ctrl, object? value)
    {
      if (ctrl is null)
        throw new ArgumentNullException(nameof(ctrl));

      ctrl.SetValue(TargetProperty, value);
    }

    /// <summary>
    /// Gets the object containing the method to be invoked.
    /// </summary>
    /// <param name="ctrl">Attached control</param>
    /// <exception cref="ArgumentNullException"><paramref name="ctrl"/> is <see langword="null"/>.</exception>
    public static object? GetTarget(UIElement ctrl)
    {
      if (ctrl is null)
        throw new ArgumentNullException(nameof(ctrl));

      var result = ctrl.GetValue(TargetProperty);
      if (result == null && ctrl is FrameworkElement fe)
      {
        result = fe.DataContext;
      }

      if (result is ICollectionView icv)
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
    /// <exception cref="ArgumentNullException"><paramref name="ctrl"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="value"/> is <see langword="null"/>, <see cref="string.Empty"/> or only consists of white spaces.</exception>
    public static void SetMethodName(UIElement ctrl, string value)
    {
      if (ctrl is null)
        throw new ArgumentNullException(nameof(ctrl));
      if (string.IsNullOrWhiteSpace(value))
        throw new ArgumentException(string.Format(Properties.Resources.StringNotNullOrWhiteSpaceException, nameof(value)), nameof(value));

      ctrl.SetValue(MethodNameProperty, value);
    }

    /// <summary>
    /// Gets the name of method to be invoked.
    /// </summary>
    /// <param name="ctrl">Attached control</param>
    /// <exception cref="ArgumentNullException"><paramref name="ctrl"/> is <see langword="null"/>.</exception>
    public static string GetMethodName(UIElement ctrl)
    {
      if (ctrl is null)
        throw new ArgumentNullException(nameof(ctrl));

      return (string)ctrl.GetValue(MethodNameProperty) ?? "";
    }

    /// <summary>
    /// Name of event raised by UI control that triggers
    /// invoking the target method.
    /// </summary>
    public static readonly DependencyProperty TriggerEventProperty =
      DependencyProperty.RegisterAttached("TriggerEvent",
      typeof(string),
      typeof(InvokeMethod),
      new PropertyMetadata((o, _) =>
      {
        if (o is UIElement ctrl)
          new InvokeMethod(ctrl);
      }));

    /// <summary>
    /// Sets the name of event raised by UI control that triggers
    /// invoking the target method.
    /// </summary>
    /// <param name="ctrl">Attached control</param>
    /// <param name="value">New value</param>
    /// <exception cref="ArgumentNullException"><paramref name="ctrl"/> is <see langword="null"/>.</exception>
    public static void SetTriggerEvent(UIElement ctrl, string? value)
    {
      if (ctrl is null)
        throw new ArgumentNullException(nameof(ctrl));

      ctrl.SetValue(TriggerEventProperty, value);
    }

    /// <summary>
    /// Gets the name of event raised by UI control that triggers
    /// invoking the target method.
    /// </summary>
    /// <param name="ctrl">Attached control</param>
    /// <exception cref="ArgumentNullException"><paramref name="ctrl"/> is <see langword="null"/>.</exception>
    public static string? GetTriggerEvent(UIElement ctrl)
    {
      if (ctrl is null)
        throw new ArgumentNullException(nameof(ctrl));

      return (string?)ctrl.GetValue(TriggerEventProperty);
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
    /// <exception cref="ArgumentNullException"><paramref name="ctrl"/> is <see langword="null"/>.</exception>
    public static void SetMethodParameter(UIElement ctrl, object? value)
    {
      if (ctrl is null)
        throw new ArgumentNullException(nameof(ctrl));

      ctrl.SetValue(MethodParameterProperty, value);
    }

    /// <summary>
    /// Gets the parameter value to be passed to invoked method.
    /// </summary>
    /// <param name="ctrl">Attached control</param>
    /// <exception cref="ArgumentNullException"><paramref name="ctrl"/> is <see langword="null"/>.</exception>
    public static object? GetMethodParameter(UIElement ctrl)
    {
      if (ctrl is null)
        throw new ArgumentNullException(nameof(ctrl));

      return ctrl.GetValue(MethodParameterProperty);
    }

    #endregion

    private readonly UIElement _element;

    /// <summary>
    /// Invokes the target method if all required attached
    /// property values have been set.
    /// </summary>
    /// <param name="ctrl">Attached UI control</param>
    /// <exception cref="ArgumentNullException"><paramref name="ctrl"/> is <see langword="null"/>.</exception>
    public InvokeMethod(UIElement ctrl)
    {
      _element = ctrl ?? throw new ArgumentNullException(nameof(ctrl));
      var triggerEvent = GetTriggerEvent(_element);
      if (!string.IsNullOrEmpty(triggerEvent))
      {
        // hook up the trigger event
        var eventRef = ctrl.GetType().GetEvent(triggerEvent);
        if (eventRef != null)
        {
          var invoke = eventRef.EventHandlerType?.GetMethod("Invoke");
          var p = invoke?.GetParameters() ?? [];
          if (p.Length == 2)
          {
            var p1Type = p[1].ParameterType;
            if (typeof(EventArgs).IsAssignableFrom(p1Type))
            {
              var del = Delegate.CreateDelegate(eventRef.EventHandlerType!,
                this,
                GetType().GetMethod("CallMethod", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic)!);
              eventRef.AddEventHandler(ctrl, del);
            }
            else
            {
              throw new NotSupportedException(Properties.Resources.ExecuteBadTriggerEvent);
            }
          }
          else
            throw new NotSupportedException(Properties.Resources.ExecuteBadTriggerEvent);
        }
      }
    }

    private void CallMethod(object? sender, EventArgs e)
    {
      object target = GetTarget(_element) ?? throw new InvalidOperationException("Target must not be null.");
      var methodName = GetMethodName(_element);
      var targetMethod = target.GetType().GetMethod(methodName);
      if (targetMethod == null)
        throw new MissingMethodException(methodName);

      object? p = GetMethodParameter(_element);
      var pCount = targetMethod.GetParameters().Length;
      try
      {
        if (pCount == 0)
          targetMethod.Invoke(target, null);
        else if (pCount == 2)
          targetMethod.Invoke(
            target,
            [
              this, new ExecuteEventArgs(p, e, (FrameworkElement)_element)
            ]);
        else
          throw new NotSupportedException(Properties.Resources.ExecuteBadParams);
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