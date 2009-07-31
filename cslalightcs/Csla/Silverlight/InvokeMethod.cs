using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Csla.Core;
using System.Collections.Generic;

namespace Csla.Silverlight
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
      new PropertyMetadata((o, e) =>
      {
        var ctrl = o as UIElement;
        if (ctrl != null)
          new InvokeMethod(ctrl);
      }));

    /// <summary>
    /// Sets the object containing the method to be invoked.
    /// </summary>
    /// <param name="ctrl">Attached control</param>
    /// <param name="value">New value</param>
    public static void SetTarget(UIElement ctrl, object value)
    {
      ctrl.SetValue(TargetProperty, value);
    }

    private void InvokeMethod_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      Refresh();
    }

    /// <summary>
    /// Gets the object containing the method to be invoked.
    /// </summary>
    /// <param name="ctrl">Attached control</param>
    public static object GetTarget(UIElement ctrl)
    {
      return ctrl.GetValue(TargetProperty);
    }

    /// <summary>
    /// Name of method to be invoked.
    /// </summary>
    public static readonly DependencyProperty MethodNameProperty =
      DependencyProperty.RegisterAttached("MethodName",
      typeof(string),
      typeof(InvokeMethod),
      new PropertyMetadata((o, e) =>
      {
        var ctrl = o as UIElement;
        if (ctrl != null)
          new InvokeMethod(ctrl);
      }));

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
      new PropertyMetadata((o, e) =>
      {
        var ctrl = o as UIElement;
        if (ctrl != null)
          new InvokeMethod(ctrl);
      }));

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
      return (string)ctrl.GetValue(MethodParameterProperty);
    }

    /// <summary>
    /// Value indicating whether the UI control should be
    /// manually enabled/disabled.
    /// </summary>
    public static readonly DependencyProperty ManualEnableControlProperty =
      DependencyProperty.RegisterAttached("ManualEnableControl",
      typeof(bool),
      typeof(InvokeMethod),
      new PropertyMetadata((o, e) =>
      {
        var ctrl = o as UIElement;
        if (ctrl != null)
          new InvokeMethod(ctrl);
      }));

    /// <summary>
    /// Sets the value indicating whether the UI control should be
    /// manually enabled/disabled.
    /// </summary>
    /// <param name="ctrl">Attached control</param>
    /// <param name="value">New value</param>
    public static void SetManualEnableControl(UIElement ctrl, bool value)
    {
      ctrl.SetValue(ManualEnableControlProperty, value);
    }

    /// <summary>
    /// Gets the value indicating whether the UI control should be
    /// manually enabled/disabled.
    /// </summary>
    /// <param name="ctrl">Attached control</param>
    public static bool GetManualEnableControl(UIElement ctrl)
    {
      return (bool)ctrl.GetValue(ManualEnableControlProperty);
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

    private UIElement _element;
    private System.Reflection.MethodInfo _targetMethod;
    private object _target;

    /// <summary>
    /// Invokes the target method if all required attached
    /// property values have been set.
    /// </summary>
    /// <param name="ctrl">Attached UI control</param>
    public InvokeMethod(UIElement ctrl)
    {
      _element = ctrl;
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

            var eventRef = ctrl.GetType().GetEvent(triggerEvent);
            if (eventRef != null && AddControl(ctrl.GetHashCode()))
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
                  eventRef.AddEventHandler(ctrl, del);
                }
                else
                {
                  throw new NotSupportedException();
                }
              }
              else
                throw new NotSupportedException();
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
          CslaDataProvider targetProvider = _target as CslaDataProvider;
          if (targetProvider != null)
          {
            if (targetMethodName == "Save")
              _contentControl.IsEnabled = targetProvider.CanSave;
            else if (targetMethodName == "Cancel")
              _contentControl.IsEnabled = targetProvider.CanCancel;
            else if (targetMethodName == "Create")
              _contentControl.IsEnabled = targetProvider.CanCreate;
            else if (targetMethodName == "Fetch")
              _contentControl.IsEnabled = targetProvider.CanFetch;
            else if (targetMethodName == "Delete")
              _contentControl.IsEnabled = targetProvider.CanDelete;
            else if (targetMethodName == "RemoveItem")
              _contentControl.IsEnabled = targetProvider.CanRemoveItem;
            else if (targetMethodName == "AddNewItem")
              _contentControl.IsEnabled = targetProvider.CanAddNewItem;
          }
          else
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
