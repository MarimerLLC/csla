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
        ((CslaDataProvider)attachedObject._target).DataChanged += new EventHandler(attachedObject.target_DataChanged);
      }
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
            if (eventRef != null)
            {
              Refresh();

              var invoke = eventRef.EventHandlerType.GetMethod("Invoke");
              var p = invoke.GetParameters();
              if (p.Length == 2)
              {
                if (typeof(RoutedEventArgs).IsAssignableFrom(p[1].ParameterType))
                  eventRef.AddEventHandler(element, new RoutedEventHandler(CallMethod));
                else if (typeof(EventArgs).IsAssignableFrom(p[1].ParameterType))
                  eventRef.AddEventHandler(element, new EventHandler(CallMethod));
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
      if (_target != null && ((CslaDataProvider)_target).Data != null)
      {
        ITrackStatus targetObject = ((CslaDataProvider)_target).Data as ITrackStatus;
        IEditableCollection list = ((CslaDataProvider)_target).Data as IEditableCollection;
        if (_contentControl != null && targetObject != null && _element.GetValue(MethodNameProperty) != null)
        {

          if (_element.GetValue(MethodNameProperty).ToString() == "Save")
          {

            if (Csla.Security.AuthorizationRules.CanEditObject(((CslaDataProvider)_target).Data.GetType()) && targetObject.IsSavable)
              _contentControl.IsEnabled = true;
            else
              _contentControl.IsEnabled = false;
          }
          else if (_element.GetValue(MethodNameProperty).ToString() == "Cancel")
          {
            if (Csla.Security.AuthorizationRules.CanEditObject(((CslaDataProvider)_target).Data.GetType()) && targetObject.IsDirty)
              _contentControl.IsEnabled = true;
            else
              _contentControl.IsEnabled = false;
          }
          else if (_element.GetValue(MethodNameProperty).ToString() == "Fetch")
          {
            if (Csla.Security.AuthorizationRules.CanGetObject(((CslaDataProvider)_target).Data.GetType()) && !targetObject.IsDirty)
              _contentControl.IsEnabled = true;
            else
              _contentControl.IsEnabled = false;
          }
          else if (list != null && _element.GetValue(MethodNameProperty).ToString() == "AddNewItem")
          {
            Type innerType = Csla.Utilities.GetChildItemType(((CslaDataProvider)_target).Data.GetType());
            if (innerType == null)
              innerType = ((CslaDataProvider)_target).Data.GetType();
            if (Csla.Security.AuthorizationRules.CanCreateObject(innerType))
              _contentControl.IsEnabled = true;
            else
              _contentControl.IsEnabled = false;
          }
          else if (_element.GetValue(MethodNameProperty).ToString() == "RemoveItem")
          {
            Type innerType = Csla.Utilities.GetChildItemType(((CslaDataProvider)_target).Data.GetType());
            if (innerType == null)
              innerType = ((CslaDataProvider)_target).Data.GetType();
            if (Csla.Security.AuthorizationRules.CanDeleteObject(innerType))
              _contentControl.IsEnabled = true;
            else
              _contentControl.IsEnabled = false;
          }
          else if (_element.GetValue(MethodNameProperty).ToString() == "Delete")
          {
            if (Csla.Security.AuthorizationRules.CanDeleteObject(((CslaDataProvider)_target).Data.GetType()))
              _contentControl.IsEnabled = true;
            else
              _contentControl.IsEnabled = false;
          }
        }
      }
    }

    private void target_DataChanged(object sender, EventArgs e)
    {
      Refresh();
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
