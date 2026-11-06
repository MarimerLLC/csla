//-----------------------------------------------------------------------
// <copyright file="ApplicationContextGraph.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Attaches an object graph to an ApplicationContext</summary>
//-----------------------------------------------------------------------

using System.Collections;
using System.Runtime.CompilerServices;
using Csla.Runtime;
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
      if (applicationContext is null)
        throw new ArgumentNullException(nameof(applicationContext));

      new Walker(applicationContext, skipActiveLocalScopes: false).Attach(obj);
    }

    /// <summary>
    /// Sets the <see cref="ApplicationContext"/> of every object
    /// graph held in a session, including objects held in
    /// nested dictionaries (such as LocalContext and ClientContext)
    /// and lists.
    /// </summary>
    /// <param name="session">Session data.</param>
    /// <param name="applicationContext">Target ApplicationContext.</param>
    /// <remarks>
    /// Objects that are in a local data portal operation (their
    /// ApplicationContext belongs to the operation's isolated scope)
    /// are left alone until the operation completes.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="session"/> or <paramref name="applicationContext"/> is <see langword="null"/>.</exception>
    public static void SetApplicationContext(Session session, ApplicationContext applicationContext)
    {
      if (session is null)
        throw new ArgumentNullException(nameof(session));
      if (applicationContext is null)
        throw new ArgumentNullException(nameof(applicationContext));

      new Walker(applicationContext, skipActiveLocalScopes: true).Attach(session);
    }

    private sealed class Walker(ApplicationContext applicationContext, bool skipActiveLocalScopes)
    {
      private HashSet<object>? _visitedContainers;
      private Dictionary<ApplicationContext, bool>? _activeLocalScopes;

      public void Attach(object? value)
      {
        switch (value)
        {
          case null:
          case string:
            return;
          case IUseApplicationContext obj:
            AttachObject(obj);
            return;
          case IEnumerable container:
            AttachContainer(container);
            return;
        }
      }

      private void AttachObject(IUseApplicationContext obj)
      {
        var current = obj.ApplicationContext;
        if (ReferenceEquals(current, applicationContext) || IsInActiveLocalScope(current))
          return;

        obj.ApplicationContext = applicationContext;

        if (obj is IUseFieldManager useFieldManager)
          Attach(useFieldManager.FieldManager);

        if (obj is IUseBusinessRules useBusinessRules)
          Attach(useBusinessRules.BusinessRules);

        if (obj is IManageProperties target)
        {
          foreach (var managedProperty in target.GetManagedProperties())
          {
            var isLazyLoadedProperty = (managedProperty.RelationshipType & RelationshipTypes.LazyLoad) == RelationshipTypes.LazyLoad;

            //only dig into property if its not a lazy loaded property
            //  or if it is, then only if its loaded
            if ((!isLazyLoadedProperty || target.FieldExists(managedProperty))
              && (typeof(IUseApplicationContext).IsAssignableFrom(managedProperty.Type)
                || typeof(IEnumerable).IsAssignableFrom(managedProperty.Type)))
            {
              // child object, or a container (such as a Csla.Core.MobileList
              // or MobileDictionary) that may hold child objects
              Attach(target.ReadProperty(managedProperty));
            }
          }
        }

        if (obj is IContainsDeletedList containsDeletedList)
          foreach (var item in containsDeletedList.DeletedList)
            Attach(item);

        if (obj is IEnumerable list)
          foreach (var item in list)
            Attach(item);
      }

      private void AttachContainer(IEnumerable container)
      {
        if (IsValueTypeArray(container))
          return;

        _visitedContainers ??= new HashSet<object>(ReferenceComparer<object>.Instance);
        if (!_visitedContainers.Add(container))
          return;

        if (container is IDictionary dictionary)
        {
          // keys can be serializable business objects too
          var entries = new List<DictionaryEntry>(dictionary.Count);
          var enumerator = dictionary.GetEnumerator();
          while (enumerator.MoveNext())
            entries.Add(enumerator.Entry);
          foreach (var entry in entries)
          {
            Attach(entry.Key);
            Attach(entry.Value);
          }
        }
        else
        {
          foreach (var item in container.Cast<object?>().ToArray())
            Attach(item);
        }
      }

      /// <summary>
      /// Gets whether the ApplicationContext belongs to the isolated
      /// scope of a local data portal operation that is still running.
      /// After the operation, LocalProxy points that ApplicationContext
      /// at the caller's scope, so this is false again.
      /// </summary>
      private bool IsInActiveLocalScope(ApplicationContext? current)
      {
        if (!skipActiveLocalScopes || current is null)
          return false;

        _activeLocalScopes ??= new Dictionary<ApplicationContext, bool>(ReferenceComparer<ApplicationContext>.Instance);
        if (!_activeLocalScopes.TryGetValue(current, out var result))
        {
          try
          {
            result = current.GetRequiredService<IRuntimeInfo>().LocalProxyNewScopeExists;
          }
          catch (ObjectDisposedException)
          {
            // the scope is gone, so the object must be re-attached
            result = false;
          }
          _activeLocalScopes[current] = result;
        }
        return result;
      }

      private static bool IsValueTypeArray(object value)
        => value is Array && value.GetType().GetElementType() is { IsValueType: true };
    }

    private sealed class ReferenceComparer<T> : IEqualityComparer<T> where T : class
    {
      public static readonly ReferenceComparer<T> Instance = new();

      public bool Equals(T? x, T? y) => ReferenceEquals(x, y);

      public int GetHashCode(T obj) => RuntimeHelpers.GetHashCode(obj);
    }
  }
}
