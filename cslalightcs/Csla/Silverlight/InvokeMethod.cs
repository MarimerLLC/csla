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
      new InvokeMethod(element);
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

    #endregion

    private System.Reflection.MethodInfo _targetMethod;
    private object _target;

    public InvokeMethod(UIElement element)
    {
      _target = element.GetValue(ResourceProperty);
      if (_target != null)
      {
        var methodName = (string)element.GetValue(MethodNameProperty);
        if (!string.IsNullOrEmpty(methodName))
        {
          var triggerEvent = (string)element.GetValue(TriggerEventProperty);
          if (!string.IsNullOrEmpty(triggerEvent))
          {
            _targetMethod = _target.GetType().GetMethod(methodName);

            var eventRef = element.GetType().GetEvent(triggerEvent);
            if (eventRef != null)
            {
              var invoke = eventRef.EventHandlerType.GetMethod("Invoke");
              var p = invoke.GetParameters();
              if (p.Length==2)
              {
                if (typeof(RoutedEventArgs).IsAssignableFrom(p[1].ParameterType))
                  eventRef.AddEventHandler(element, new RoutedEventHandler(EventHandler));
                else if (typeof(EventArgs).IsAssignableFrom(p[1].ParameterType))
                  eventRef.AddEventHandler(element, new EventHandler(EventHandler));
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

    private void EventHandler(object sender, EventArgs e)
    {
      _targetMethod.Invoke(_target, null);
    }
  }
}
