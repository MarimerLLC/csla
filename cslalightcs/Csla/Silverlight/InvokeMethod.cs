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
  public class InvokeMethod
  {
    #region Attached properties

    public static readonly DependencyProperty ResourceProperty =
      DependencyProperty.RegisterAttached("Resource",
      typeof(object),
      typeof(InvokeMethod),
      null);

    public static void SetResource(UIElement element, object value)
    {
      element.SetValue(ResourceProperty, value);
      InvokeMethod attachedObject = new InvokeMethod(element);
      if (attachedObject._target is CslaDataProvider)
      {
        ((CslaDataProvider)attachedObject._target).PropertyChanged += new PropertyChangedEventHandler(attachedObject.InvokeMethod_PropertyChanged);
      }
    }

    private void InvokeMethod_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      Refresh();
    }

    public static object GetResource(UIElement element)
    {
      return element.GetValue(ResourceProperty);
    }

    public static readonly DependencyProperty MethodNameProperty =
      DependencyProperty.RegisterAttached("MethodName",
      typeof(string),
      typeof(InvokeMethod),
      null);

    public static void SetMethodName(UIElement element, string value)
    {
      element.SetValue(MethodNameProperty, value);
      new InvokeMethod(element);
    }

    public static string GetMethodName(UIElement element)
    {
      return (string)element.GetValue(MethodNameProperty);
    }

    public static readonly DependencyProperty TriggerEventProperty =
      DependencyProperty.RegisterAttached("TriggerEvent",
      typeof(string),
      typeof(InvokeMethod),
      null);

    public static void SetTriggerEvent(UIElement element, string value)
    {
      element.SetValue(TriggerEventProperty, value);
      new InvokeMethod(element);
    }

    public static string GetTriggerEvent(UIElement element)
    {
      return (string)element.GetValue(TriggerEventProperty);
    }

    public static readonly DependencyProperty MethodParameterProperty =
      DependencyProperty.RegisterAttached("MethodParameter",
      typeof(object),
      typeof(InvokeMethod),
      null);

    public static void SetMethodParameter(UIElement element, object value)
    {
      element.SetValue(MethodParameterProperty, value);
      new InvokeMethod(element);
    }

    public static object GetMethodParameter(UIElement element)
    {
      return (string)element.GetValue(MethodParameterProperty);
    }

    public static readonly DependencyProperty ManageEnabledStateForControlProperty =
      DependencyProperty.RegisterAttached("ManageEnabledStateForControl",
      typeof(bool),
      typeof(InvokeMethod),
      null);

    public static void SetManageEnabledStateForControl(UIElement element, object value)
    {
      element.SetValue(ManageEnabledStateForControlProperty, value);
      new InvokeMethod(element);
    }

    public static object GetManageEnabledStateForControl(UIElement element)
    {
      return (bool)element.GetValue(ManageEnabledStateForControlProperty);
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

    public InvokeMethod(UIElement element)
    {
      if (element is ContentControl)
        _contentControl = (ContentControl)element;
      _element = element;
      _target = element.GetValue(ResourceProperty);
      if (_target != null)
      {
        var manageStateValue = element.GetValue(ManageEnabledStateForControlProperty);
        if (manageStateValue == null)
        {
          element.SetValue(ManageEnabledStateForControlProperty, false);
        }
        var methodName = (string)element.GetValue(MethodNameProperty);
        if (!string.IsNullOrEmpty(methodName))
        {
          var triggerEvent = (string)element.GetValue(TriggerEventProperty);
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
                if (typeof(RoutedEventArgs).IsAssignableFrom(p[1].ParameterType))
                {
                  eventRef.AddEventHandler(element, new RoutedEventHandler(CallMethod));
                }
                else if (typeof(EventArgs).IsAssignableFrom(p[1].ParameterType))
                {
                  eventRef.AddEventHandler(element, new EventHandler(CallMethod));
                }
                else
                  throw new NotSupportedException();
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
      if (_target != null && _element != null && _contentControl != null && _element.GetValue(MethodNameProperty) != null)
      {
        if ((bool)_element.GetValue(ManageEnabledStateForControlProperty) == true)
        {
          if (_target is CslaDataProvider)
          {
            CslaDataProvider targetProvider = _target as CslaDataProvider;
            if (_element.GetValue(MethodNameProperty).ToString() == "Save")
              _contentControl.IsEnabled = targetProvider.CanSave;
            if (_element.GetValue(MethodNameProperty).ToString() == "Cancel")
              _contentControl.IsEnabled = targetProvider.CanCancel;
            if (_element.GetValue(MethodNameProperty).ToString() == "Create")
              _contentControl.IsEnabled = targetProvider.CanCreate;
            if (_element.GetValue(MethodNameProperty).ToString() == "Fetch")
              _contentControl.IsEnabled = targetProvider.CanFetch;
            if (_element.GetValue(MethodNameProperty).ToString() == "Delete")
              _contentControl.IsEnabled = targetProvider.CanDelete;
            if (_element.GetValue(MethodNameProperty).ToString() == "RemoveItem")
              _contentControl.IsEnabled = targetProvider.CanRemoveItem;
            if (_element.GetValue(MethodNameProperty).ToString() == "AddNewItem")
              _contentControl.IsEnabled = targetProvider.CanAddNewItem;
          }
          else
          {
            string targetMethodName = (string)_element.GetValue(MethodNameProperty);
            string canMethodName = "Can" + targetMethodName;
            object returnValue = Csla.Reflection.MethodCaller.CallMethodIfImplemented(_target, canMethodName, null);
            if (returnValue != null && returnValue is bool && (bool)returnValue == true)
              _contentControl.IsEnabled = true;
            else
              _contentControl.IsEnabled = false;
          }
        }
      }
    }


    private void CallMethod(object sender, EventArgs e)
    {
      object p = _element.GetValue(MethodParameterProperty);
      if (p == null)
        _targetMethod.Invoke(_target, null);
      else
        _targetMethod.Invoke(_target, new object[] { p });
    }
  }
}
