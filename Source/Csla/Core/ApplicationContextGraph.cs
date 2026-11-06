//-----------------------------------------------------------------------
// <copyright file="ApplicationContextGraph.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Attaches an object graph to an ApplicationContext</summary>
//-----------------------------------------------------------------------

using System.Collections;
using System.Runtime.CompilerServices;
using Csla.State;

namespace Csla.Core
{
  /// <summary>
  /// Attaches an object graph to an <see cref="ApplicationContext"/>,
  /// so objects created in another (often short-lived) scope use
  /// the services of the target scope.
  /// </summary>
  internal static class ApplicationContextGraph
  {
    /// <summary>
    /// Sets the <see cref="ApplicationContext"/> of an object
    /// and every object in its graph.
    /// </summary>
    /// <param name="obj">Root of the object graph.</param>
    /// <param name="applicationContext">Target ApplicationContext.</param>
    /// <remarks>
    /// The walk stops at any object that already uses
    /// <paramref name="applicationContext"/>.
    /// </remarks>
    public static void SetApplicationContext(IUseApplicationContext? obj, ApplicationContext applicationContext)
    {
      if (obj != null && !ReferenceEquals(obj.ApplicationContext, applicationContext))
      {
        obj.ApplicationContext = applicationContext;

        if (obj is IUseFieldManager useFieldManager)
          SetApplicationContext(useFieldManager.FieldManager, applicationContext);

        if (obj is IUseBusinessRules useBusinessRules)
          SetApplicationContext(useBusinessRules.BusinessRules, applicationContext);

        if (obj is IManageProperties target)
        {
          foreach (var managedProperty in target.GetManagedProperties())
          {
            var isLazyLoadedProperty = (managedProperty.RelationshipType & RelationshipTypes.LazyLoad) == RelationshipTypes.LazyLoad;

            //only dig into property if its not a lazy loaded property
            //  or if it is, then only if its loaded
            if (!isLazyLoadedProperty
              || (isLazyLoadedProperty && target.FieldExists(managedProperty)))
            {
              if (typeof(IUseApplicationContext).IsAssignableFrom(managedProperty.Type))
              {
                //property is directly assignable to the IUseApplicationContext so set it
                SetApplicationContext((IUseApplicationContext?)target.ReadProperty(managedProperty), applicationContext);
              }
              else if (typeof(IEnumerable).IsAssignableFrom(managedProperty.Type))
              {
                //property is a list and needs to be processed (could be a Csla.Core.MobileList or something like that)
                var enumerable = (IEnumerable?)target.ReadProperty(managedProperty);
                if (enumerable != null)
                {
                  foreach (var item in enumerable)
                  {
                    SetApplicationContext(item as IUseApplicationContext, applicationContext);
                  }
                }
              }
            }
          }
        }

        if (obj is IContainsDeletedList containsDeletedList)
          foreach (var item in containsDeletedList.DeletedList)
            SetApplicationContext(item as IUseApplicationContext, applicationContext);

        if (obj is IEnumerable list)
          foreach (var item in list)
            SetApplicationContext(item as IUseApplicationContext, applicationContext);
      }
    }

    /// <summary>
    /// Sets the <see cref="ApplicationContext"/> of every object
    /// graph held in a session, including objects held in
    /// nested dictionaries (such as LocalContext and ClientContext)
    /// and lists.
    /// </summary>
    /// <param name="session">Session data.</param>
    /// <param name="applicationContext">Target ApplicationContext.</param>
    /// <exception cref="ArgumentNullException"><paramref name="session"/> or <paramref name="applicationContext"/> is <see langword="null"/>.</exception>
    public static void SetApplicationContext(Session session, ApplicationContext applicationContext)
    {
      if (session is null)
        throw new ArgumentNullException(nameof(session));
      if (applicationContext is null)
        throw new ArgumentNullException(nameof(applicationContext));

      var visited = new HashSet<object>(ReferenceComparer.Instance) { session };
      foreach (var value in session.Values.ToArray())
        SetValueApplicationContext(value, applicationContext, visited);
    }

    private static void SetValueApplicationContext(object? value, ApplicationContext applicationContext, HashSet<object> visited)
    {
      switch (value)
      {
        case null:
        case string:
          return;
        case IUseApplicationContext useApplicationContext:
          SetApplicationContext(useApplicationContext, applicationContext);
          return;
      }

      if (value is not IEnumerable container || IsValueTypeArray(value) || !visited.Add(value))
        return;

      var items = value is IDictionary dictionary
        ? dictionary.Values.Cast<object?>().ToArray()
        : container.Cast<object?>().ToArray();
      foreach (var item in items)
        SetValueApplicationContext(item, applicationContext, visited);
    }

    private static bool IsValueTypeArray(object value)
      => value is Array && value.GetType().GetElementType() is { IsValueType: true };

    private sealed class ReferenceComparer : IEqualityComparer<object>
    {
      public static readonly ReferenceComparer Instance = new();

      public new bool Equals(object? x, object? y) => ReferenceEquals(x, y);

      public int GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);
    }
  }
}
