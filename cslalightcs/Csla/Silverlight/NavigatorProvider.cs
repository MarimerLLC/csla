using System;
using System.Windows;
using Csla.Properties;

namespace Csla.Silverlight
{
  public class NavigatorProvider
  {
    /// <summary>
    /// Navigator provider instance
    /// </summary>
    public static readonly DependencyProperty NavigatorProviderProperty =
      DependencyProperty.RegisterAttached("NavigatorProvider",
      typeof(NavigatorProvider),
      typeof(NavigatorProvider),
      null);

    /// <summary>
    /// Set navigator provider
    /// </summary>
    /// <param name="element">
    /// UI element to set navigator provider on.
    /// </param>
    /// <param name="value">
    /// Instance of navigator provider.
    /// </param>
    public static void SetNavigatorProvider(UIElement element, object value)
    {
      element.SetValue(NavigatorProviderProperty, value);
    }

    /// <summary>
    /// Retreive NavigatorProvider instance associated with UI element specified.
    /// </summary>
    /// <param name="element">
    /// </param>
    /// UI element to get navigator provider for.
    /// <returns>
    /// NavigatorProvider instance associated with UI element specified.
    /// </returns>
    public static NavigatorProvider GetNavigatorProvider(UIElement element)
    {
      return (NavigatorProvider)element.GetValue(NavigatorProviderProperty);
    }

    /// <summary>
    /// Name of the control to show when a trigger event occurs. 
    /// Must be assembly qualified type name.
    /// </summary>
    public static readonly DependencyProperty ControlTypeNameProperty =
      DependencyProperty.RegisterAttached("ControlTypeName",
      typeof(string),
      typeof(NavigatorProvider),
      null);

    /// <summary>
    /// Set control type for UI element
    /// </summary>
    /// <param name="element">
    /// UI element to set control name on.
    /// </param>
    /// <param name="value">
    /// Control name
    /// </param>
    public static void SetControlTypeName(UIElement element, string value)
    {
      element.SetValue(ControlTypeNameProperty, value);
    }

    /// <summary>
    /// Retreive control name associated with UI element specified.
    /// </summary>
    /// <param name="element">
    /// UI element to get control name for.
    /// </param>
    /// <returns>
    /// Control name
    /// </returns>
    public static string GetControlTypeName(UIElement element)
    {
      return (string)element.GetValue(ControlTypeNameProperty);
    }

    /// <summary>
    /// Name of the event that triggers NavigatorProvider action.
    /// </summary>
    public static readonly DependencyProperty TriggerEventProperty =
      DependencyProperty.RegisterAttached("TriggerEvent",
      typeof(string),
      typeof(NavigatorProvider),
      null);

    /// <summary>
    /// Set name of the event that triggers NavigatorProvider action. for UI element
    /// </summary>
    /// <param name="element">
    /// UI element to set name of the event that triggers NavigatorProvider action on.
    /// </param>
    /// <param name="value">
    /// Event name.
    /// </param>
    public static void SetTriggerEvent(UIElement element, string value)
    {
      element.SetValue(TriggerEventProperty, value);
      NavigatorProvider target = (NavigatorProvider)element.GetValue(NavigatorProviderProperty);
      if (target == null)
      {
        //TODO: review resource text
        throw new ArgumentException(Resources.NavigatorProviderSetPriorToTriggerEvent);
      }
      else
      {
        target.RegisterEvent(element, value);
      }
    }

    /// <summary>
    /// Retreive name of the event that triggers NavigatorProvider action
    /// associated with UI element specified.
    /// </summary>
    /// <param name="element">
    /// UI element to get name of the event that triggers NavigatorProvider action for.
    /// </param>
    /// <returns>
    /// Name of the event that triggers NavigatorProvider action.
    /// </returns>
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
          // Invoke Navigator to show specified control
          Navigator.Current.Navigate(controlTypeName);
        }
      }
    }
  }
}
