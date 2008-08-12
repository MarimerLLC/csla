using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace Csla.Wpf
{
  public static class VisualTree
  {
    public static DependencyObject FindParent(string path, DependencyObject current)
    {
      return FindParent(new Queue<string>(path.Split('.')), current);
    }

    private static DependencyObject FindParent(Queue<string> queue, DependencyObject current)
    {
      string propertyName = queue.Dequeue();

      object[] indexParameters = new object[0];
      if (propertyName.Contains('['))
      {
        int start = propertyName.IndexOf('[');
        int end = propertyName.IndexOf(']');
        if (end != propertyName.Length - 1)
          throw new InvalidOperationException("Indexed expressions must be closed");

        int length = (end - start) - 1;
        indexParameters = propertyName.Substring(start + 1, length).Split(',').Cast<object>().ToArray();

        propertyName = propertyName.Substring(0, start);
      }

      PropertyInfo property = current.GetType().GetProperty(propertyName);
      if (property == null)
        throw new InvalidOperationException(string.Format(
          "The specified property name '{0}' does not exist",
          propertyName));

      ParameterInfo[] parameters = property.GetIndexParameters();
      if (parameters.Length != indexParameters.Length)
        throw new InvalidOperationException(string.Format(
          "This property requires {0} index arguments, {1} were provided",
          parameters.Length,
          indexParameters.Length));

      for (int x = 0; x < parameters.Length; x++)
      {
        indexParameters[x] = Convert.ChangeType(
          indexParameters[x],
          parameters[x].ParameterType,
          CultureInfo.InvariantCulture);
      }

      DependencyObject parent = (DependencyObject)property.GetValue(current, (indexParameters.Length > 0 ? indexParameters : null));
      if (queue.Count > 0)
        parent = FindParent(queue, parent);

      return parent;
    }

    private static DependencyObject FindByName(DependencyObject parent, string name)
    {
      DependencyObject child = null;
      int numChildren = VisualTreeHelper.GetChildrenCount(parent);
      for (int i = 0; i < numChildren; i++)
      {
        child = VisualTreeHelper.GetChild(parent, i) as DependencyObject;
        if (child != null && (child.GetValue(FrameworkElement.NameProperty) as string) == name)
          break;
      }

      for (int i = 0; i < numChildren; i++)
      {
        child = VisualTreeHelper.GetChild(parent, i) as DependencyObject;
        child = FindByName(child, name);
        if (child != null && (child.GetValue(FrameworkElement.NameProperty) as string) == name)
          break;
      }

      return child;
    }

    public static DependencyObject FindByName(string name, DependencyObject parent)
    {
      return FindByName(parent, name);
    }
  }
}
