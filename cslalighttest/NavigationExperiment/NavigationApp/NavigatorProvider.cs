using System;
using System.Windows;

namespace NavigationApp
{
  public class NavigatorProvider
  {
    public static readonly DependencyProperty NavigatorProviderProperty =
      DependencyProperty.RegisterAttached("NavigatorProvider",
      typeof(NavigatorProvider),
      typeof(NavigatorProvider),
      null);

    public static void SetNavigatorProvider(UIElement element, object value)
    {
      element.SetValue(NavigatorProviderProperty, value);
    }

    public static NavigatorProvider GetNavigatorProvider(UIElement element)
    {
      return (NavigatorProvider)element.GetValue(NavigatorProviderProperty);
    }

    public static readonly DependencyProperty ControlTypeNameProperty =
      DependencyProperty.RegisterAttached("ControlTypeName",
      typeof(string),
      typeof(NavigatorProvider),
      null);

    public static void SetControlTypeName(UIElement element, string value)
    {
      element.SetValue(ControlTypeNameProperty, value);
    }

    public static string GetControlTypeName(UIElement element)
    {
      return (string)element.GetValue(ControlTypeNameProperty);
    }

    public static readonly DependencyProperty TriggerEventProperty =
      DependencyProperty.RegisterAttached("TriggerEvent",
      typeof(string),
      typeof(NavigatorProvider),
      null);

    public static void SetTriggerEvent(UIElement element, string value)
    {
      element.SetValue(TriggerEventProperty, value);
      NavigatorProvider target = (NavigatorProvider)element.GetValue(NavigatorProviderProperty);
      if (target == null)
      {
        //TODO: move to resource.
        throw new ArgumentException("Please set NavigatorProvider prior to TriggerEvent property.");
      }
      else
      {
        target.RegisterEvent(element, value);
      }
    }
    public static string GetTriggerEvent(UIElement element)
    {
      return (string)element.GetValue(TriggerEventProperty);
    }

    private void RegisterEvent(UIElement element, string triggerEvent)
    {
      var eventRef = element.GetType().GetEvent(triggerEvent);
      if (eventRef != null)
      {
        var invokeMethod = eventRef.EventHandlerType.GetMethod("Invoke");
        var parameters = invokeMethod.GetParameters();
        if (parameters.Length == 2)
        {
          if (typeof(RoutedEventArgs).IsAssignableFrom(parameters[1].ParameterType))
          {
            eventRef.AddEventHandler(element, new RoutedEventHandler(InvokeProvider));
          }
          else if (typeof(EventArgs).IsAssignableFrom(parameters[1].ParameterType))
          {
            eventRef.AddEventHandler(element, new EventHandler(InvokeProvider));
          }
        }
      }
    }

    private void InvokeProvider(object sender, EventArgs e)
    {
      UIElement source = sender as UIElement;
      if (source != null)
      {
        string controlTypeName = (string)source.GetValue(ControlTypeNameProperty);
        if (!string.IsNullOrEmpty(controlTypeName))
        {
          Navigator.Current.Navigate(controlTypeName);
        }
      }
    }
  }
}
