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

      new Walker(applicationContext, keepUsableContexts: false).Attach(obj);
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
    /// Only objects whose ApplicationContext can't resolve services
    /// (it has no accessor, or its scope is disposed) are attached.
    /// Every circuit with the same session cookie shares the session,
    /// so an object whose scope is alive may belong to another circuit,
    /// or to a local data portal operation that is still running.
    /// </remarks>
    /// <exception cref="ArgumentNullException"><paramref name="session"/> or <paramref name="applicationContext"/> is <see langword="null"/>.</exception>
    public static void SetApplicationContext(Session session, ApplicationContext applicationContext)
    {
      if (session is null)
        throw new ArgumentNullException(nameof(session));
      if (applicationContext is null)
        throw new ArgumentNullException(nameof(applicationContext));

      new Walker(applicationContext, keepUsableContexts: true).Attach(session);
    }

    private sealed class Walker(ApplicationContext applicationContext, bool keepUsableContexts)
    {
      private HashSet<object>? _visitedContainers;
      private Dictionary<ApplicationContext, bool>? _usableContexts;

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
        if (ReferenceEquals(current, applicationContext) || (keepUsableContexts && IsUsable(current)))
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
              && !managedProperty.Type.IsValueType
              && managedProperty.Type != typeof(string))
            {
              // a child object, or a container (such as a Csla.Core.MobileList
              // or MobileDictionary) that may hold child objects; the value's
              // runtime type decides, because the property can be declared
              // as object or an interface
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
        if (HoldsOnlyPlainValues(container))
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
      /// Gets whether the ApplicationContext can still resolve services:
      /// it has an accessor, and the scope behind it isn't disposed.
      /// </summary>
      private bool IsUsable(ApplicationContext? current)
      {
        // a context without an accessor (such as the placeholder that
        // CriteriaBase uses) has no scope to resolve services from
        if (current?.ApplicationContextAccessor is null)
          return false;

        _usableContexts ??= new Dictionary<ApplicationContext, bool>(ReferenceComparer<ApplicationContext>.Instance);
        if (!_usableContexts.TryGetValue(current, out var result))
        {
          try
          {
            _ = current.GetRequiredService<IRuntimeInfo>();
            result = true;
          }
          catch (ObjectDisposedException)
          {
            result = false;
          }
          _usableContexts[current] = result;
        }
        return result;
      }

      /// <summary>
      /// Gets whether a container can only hold values that can't be
      /// business objects: an array of value types or strings, or a
      /// generic collection (such as MobileList&lt;int&gt; or
      /// MobileDictionary&lt;string, int&gt;) whose type arguments are
      /// all value types or strings. Skipping these avoids enumerating
      /// and boxing every element on each session read.
      /// </summary>
      private static bool HoldsOnlyPlainValues(object container)
      {
        var type = container.GetType();
        if (type.IsArray)
          return IsPlainValueType(type.GetElementType());
        if (!type.IsGenericType)
          return false;
        foreach (var argument in type.GetGenericArguments())
          if (!IsPlainValueType(argument))
            return false;
        return true;
      }

      private static bool IsPlainValueType(Type? type)
        => type is not null && (type.IsValueType || type == typeof(string));
    }

    private sealed class ReferenceComparer<T> : IEqualityComparer<T> where T : class
    {
      public static readonly ReferenceComparer<T> Instance = new();

      public bool Equals(T? x, T? y) => ReferenceEquals(x, y);

      public int GetHashCode(T obj) => RuntimeHelpers.GetHashCode(obj);
    }
  }
}
