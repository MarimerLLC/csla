//-----------------------------------------------------------------------
// <copyright file="VisualTree.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Contains methods to help work with the visual tree.</summary>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using Csla.Properties;

namespace Csla.Xaml
{
  /// <summary>
  /// Contains methods to help work with the visual tree.
  /// </summary>
  public static class VisualTree
  {
    /// <summary>
    /// Finds the parent of an object.
    /// </summary>
    /// <param name="path">Path</param>
    /// <param name="current">Current object</param>
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
          throw new InvalidOperationException(Resources.IndexedExpressionsMustBeClosed);

        int length = (end - start) - 1;
        indexParameters = propertyName.Substring(start + 1, length).Split(',').Cast<object>().ToArray();

        propertyName = propertyName.Substring(0, start);
      }

      PropertyInfo property = current.GetType().GetProperty(propertyName);
      if (property == null)
        throw new InvalidOperationException(string.Format(
          Resources.PropertyNameDoesNotExist,
          propertyName));

      ParameterInfo[] parameters = property.GetIndexParameters();
      if (parameters.Length != indexParameters.Length)
        throw new InvalidOperationException(string.Format(
          Resources.PropertyRequiresIndexArguments,
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

      if (child == null)
      {
        for (int i = 0; i < numChildren; i++)
        {
          child = VisualTreeHelper.GetChild(parent, i) as DependencyObject;
          child = FindByName(child, name);
          if (child != null && (child.GetValue(FrameworkElement.NameProperty) as string) == name)
            break;
        }
      }

      return child;
    }

    /// <summary>
    /// Finds an element by name.
    /// </summary>
    /// <param name="name">Name of the element</param>
    /// <param name="parent">Parent object</param>
    public static DependencyObject FindByName(string name, DependencyObject parent)
    {
      return FindByName(parent, name);
    }
  }
}