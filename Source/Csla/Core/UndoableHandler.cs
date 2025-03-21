﻿#if !IOS

//-----------------------------------------------------------------------
// <copyright file="UndoableHandler.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System.Reflection;
#if NET8_0_OR_GREATER
using System.Runtime.Loader;

using Csla.Runtime;
#endif
using Csla.Reflection;

namespace Csla.Core
{
  internal static class UndoableHandler
  {
#if NET8_0_OR_GREATER
    private static readonly Dictionary<Type, Tuple<string?, List<DynamicMemberHandle>>> _undoableFieldCache = [];
#else
    private static readonly Dictionary<Type, List<DynamicMemberHandle>> _undoableFieldCache = [];
#endif

    public static List<DynamicMemberHandle> GetCachedFieldHandlers(Type type)
    {
#if NET8_0_OR_GREATER

      if (!_undoableFieldCache.TryGetValue(type, out var handlersInfo))
      {
        lock (_undoableFieldCache)
        {
          if (!_undoableFieldCache.TryGetValue(type, out handlersInfo))
          {
            var newHandlers = BuildHandlers(type);

            handlersInfo = AssemblyLoadContextManager.CreateCacheInstance(type, newHandlers, OnAssemblyLoadContextUnload);

            _undoableFieldCache.Add(type, handlersInfo);
          }
        }
      }

      return handlersInfo.Item2;
#else
      if (!_undoableFieldCache.TryGetValue(type, out var handlers))
      {
        var newHandlers = BuildHandlers(type);

        lock (_undoableFieldCache)  //ready to add, lock
        {
          if (!_undoableFieldCache.TryGetValue(type, out handlers))
          {
            _undoableFieldCache.Add(type, newHandlers);

            handlers = newHandlers;
          }
        }
      }

      return handlers;
#endif
    }

    private static List<DynamicMemberHandle> BuildHandlers(Type type)
    {
      var handlers = new List<DynamicMemberHandle>();
      // get the list of fields in this type
      var fields = type.GetFields(
          BindingFlags.NonPublic |
          BindingFlags.Instance |
          BindingFlags.Public);

      foreach (FieldInfo field in fields)
      {
        // make sure we process only our variables
        if (field.DeclaringType == type)
        {
          // see if this field is marked as not undoable 
          if (!NotUndoableField(field))
          {
            // the field is undoable, so it needs to be processed.
            handlers.Add(new DynamicMemberHandle(field));
          }
        }
      }
      return handlers;
    }

    private static bool NotUndoableField(FieldInfo field)
    {
      // see if this field is marked as not undoable or IsInitOnly (ie: readonly property)
      return field.IsInitOnly || Attribute.IsDefined(field, typeof(NotUndoableAttribute));
    }
#if NET8_0_OR_GREATER

    private static void OnAssemblyLoadContextUnload(AssemblyLoadContext context)
    {
      lock (_undoableFieldCache)
        AssemblyLoadContextManager.RemoveFromCache((IDictionary<string, Tuple<string?, DynamicMemberHandle>?>)_undoableFieldCache, context);
    }
#endif
  }
}

#endif
