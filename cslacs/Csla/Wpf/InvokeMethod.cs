using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;

namespace Csla.Wpf
{
  /// <summary>
  /// Invokes a method on a target object when a 
  /// trigger event is raised from the attached
  /// UI control.
  /// </summary>
  public class InvokeMethod : DependencyObject
  {
    #region Attached properties

    /// <summary>
    /// Object containing the method to be invoked.
    /// </summary>
    public static readonly DependencyProperty TargetProperty =
      DependencyProperty.RegisterAttached("Target",
      typeof(object),
      typeof(InvokeMethod),
      new FrameworkPropertyMetadata((o, e) =>
      {
        new InvokeMethod(o);
      }));

    /// <summary>
    /// Sets the object containing the method to be invoked.
    /// </summary>
    /// <param name="element">Attached control</param>
    /// <param name="value">New value</param>
    public static void SetTarget(DependencyObject element, object value)
    {
      element.SetValue(TargetProperty, value);
    }

    private void InvokeMethod_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      Refresh();
    }

    /// <summary>
    /// Gets the object containing the method to be invoked.
    /// </summary>
    /// <param name="element">Attached control</param>
    public static object GetTarget(DependencyObject element)
    {
      return element.GetValue(TargetProperty);
    }

    /// <summary>
    /// Name of method to be invoked.
    /// </summary>
    public static readonly DependencyProperty MethodNameProperty =
      DependencyProperty.RegisterAttached("MethodName",
      typeof(string),
      typeof(InvokeMethod),
      new FrameworkPropertyMetadata((o, e) =>
      {
        new InvokeMethod(o);
      }));

    /// <summary>
    /// Sets the name of method to be invoked.
    /// </summary>
    /// <param name="element">Attached control</param>
    /// <param name="value">New value</param>
    public static void SetMethodName(DependencyObject element, string value)
    {
      element.SetValue(MethodNameProperty, value);
    }

    /// <summary>
    /// Gets the name of method to be invoked.
    /// </summary>
    /// <param name="element">Attached control</param>
    public static string GetMethodName(DependencyObject element)
    {
      return (string)element.GetValue(MethodNameProperty);
    }

    /// <summary>
    /// Name of event raised by UI control that triggers
    /// invoking the target method.
    /// </summary>
    public static readonly DependencyProperty TriggerEventProperty =
      DependencyProperty.RegisterAttached("TriggerEvent",
      typeof(string),
      typeof(InvokeMethod),
      new FrameworkPropertyMetadata((o, e) =>
      {
        new InvokeMethod(o);
      }));

    /// <summary>
    /// Sets the name of event raised by UI control that triggers
    /// invoking the target method.
    /// </summary>
    /// <param name="element">Attached control</param>
    /// <param name="value">New value</param>
    public static void SetTriggerEvent(DependencyObject element, string value)
    {
      element.SetValue(TriggerEventProperty, value);
    }

    /// <summary>
    /// Gets the name of event raised by UI control that triggers
    /// invoking the target method.
    /// </summary>
    /// <param name="element">Attached control</param>
    public static string GetTriggerEvent(DependencyObject element)
    {
      return (string)element.GetValue(TriggerEventProperty);
    }

    /// <summary>
    /// Parameter value to be passed to invoked method.
    /// </summary>
    public static readonly DependencyProperty MethodParameterProperty =
      DependencyProperty.RegisterAttached("MethodParameter",
      typeof(object),
      typeof(InvokeMethod),
      new FrameworkPropertyMetadata((o, e) =>
      {
        new InvokeMethod(o);
      }));

    /// <summary>
    /// Sets the parameter value to be passed to invoked method.
    /// </summary>
    /// <param name="element">Attached control</param>
    /// <param name="value">New value</param>
    public static void SetMethodParameter(DependencyObject element, object value)
    {
      element.SetValue(MethodParameterProperty, value);
    }

    /// <summary>
    /// Gets the parameter value to be passed to invoked method.
    /// </summary>
    /// <param name="element">Attached control</param>
    public static object GetMethodParameter(DependencyObject element)
    {
      return (string)element.GetValue(MethodParameterProperty);
    }

    /// <summary>
    /// Value indicating whether the UI control should be
    /// manually enabled/disabled.
    /// </summary>
    public static readonly DependencyProperty ManualEnableControlProperty =
      DependencyProperty.RegisterAttached("ManualEnableControl",
      typeof(bool),
      typeof(InvokeMethod),
      new FrameworkPropertyMetadata((o, e) =>
      {
        new InvokeMethod(o);
      }));

    /// <summary>
    /// Sets the value indicating whether the UI control should be
    /// manually enabled/disabled.
    /// </summary>
    /// <param name="element">Attached control</param>
    /// <param name="value">New value</param>
    public static void SetManualEnableControl(DependencyObject element, bool value)
    {
      element.SetValue(ManualEnableControlProperty, value);
    }

    /// <summary>
    /// Gets the value indicating whether the UI control should be
    /// manually enabled/disabled.
    /// </summary>
    /// <param name="element">Attached control</param>
    public static bool GetManualEnableControl(DependencyObject element)
    {
      return (bool)element.GetValue(ManualEnableControlProperty);
    }

    private static List<int> processedControls = new List<int>();
    private static object locker = new object();
    private static bool AddControl(int controlId)
    {
      lock (locker)
      {
        if (processedControls.Contains(controlId))
          return false;
        else
        {
          processedControls.Add(controlId);
          return true;
        }
      }
    }
    private ContentControl _contentControl;

    #endregion

    private DependencyObject _element;
    private System.Reflection.MethodInfo _targetMethod;
    private object _target;

    /// <summary>
    /// Invokes the target method if all required attached
    /// property values have been set.
    /// </summary>
    /// <param name="element">Attached UI control</param>
    public InvokeMethod(DependencyObject element)
    {
      _element = element;
      _contentControl = _element as ContentControl;
      _target = GetTarget(_element);

      if (_target != null)
      {
        var methodName = GetMethodName(_element);
        if (!string.IsNullOrEmpty(methodName))
        {
          var triggerEvent = GetTriggerEvent(_element);
          if (!string.IsNullOrEmpty(triggerEvent))
          {
            // at this point all required fields have been set,
            // so hook up the event

            _targetMethod = _target.GetType().GetMethod(methodName);

            var eventRef = element.GetType().GetEvent(triggerEvent);
            if (eventRef != null && AddControl(element.GetHashCode()))
            {
              Refresh();

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
                  eventRef.AddEventHandler(element, del);
                }
                else
                {
                  throw new NotSupportedException();
                }
              }
              else
              {
                throw new NotSupportedException();
              }
            }
          }
        }
      }
    }

    private void Refresh()
    {
      if (_target != null && _contentControl != null)
      {
        var targetMethodName = GetMethodName(_element);
        if (!string.IsNullOrEmpty(targetMethodName) && !GetManualEnableControl(_element))
        {
          string canPropertyName = "Can" + targetMethodName;
          var propertyInfo = Csla.Reflection.MethodCaller.GetProperty(_target.GetType(), canPropertyName);
          if (propertyInfo != null)
          {
            object returnValue = Csla.Reflection.MethodCaller.GetPropertyValue(_target, propertyInfo);
            if (returnValue != null && returnValue is bool)
              _contentControl.IsEnabled = (bool)returnValue;
          }
        }
      }
    }


    private void CallMethod(object sender, EventArgs e)
    {
      object p = GetMethodParameter(_element);
      var pCount = _targetMethod.GetParameters().Length;
      if (pCount == 0)
        _targetMethod.Invoke(_target, null);
      else if (pCount == 1)
        _targetMethod.Invoke(_target, new object[] { p });
      else if (pCount == 2)
        _targetMethod.Invoke(_target, new object[] { _element, p });
      else if (pCount == 3)
        _targetMethod.Invoke(_target, new object[] { _element, e, p });
    }
  }
}
