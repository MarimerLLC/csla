#if !NETFX_CORE && !IOS
//-----------------------------------------------------------------------
// <copyright file="UndoableHandler.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Reflection;

#if NET5_0_OR_GREATER
using System.Runtime.Loader;

using Csla.ALC;
#endif

using Csla.Reflection;

namespace Csla.Core
{
    static class UndoableHandler
    {
#if NET5_0_OR_GREATER
        private static readonly Dictionary<Type, Tuple<string, List<DynamicMemberHandle>>> _undoableFieldCache = 
          new Dictionary<Type, Tuple<string,  List<DynamicMemberHandle>>>();
#else
    private static readonly Dictionary<Type, List<DynamicMemberHandle>> _undoableFieldCache = new Dictionary<Type, List<DynamicMemberHandle>>();
#endif

        public static List<DynamicMemberHandle> GetCachedFieldHandlers(Type type)
        {
            List<DynamicMemberHandle> handlers;

#if NET5_0_OR_GREATER
            var found = _undoableFieldCache.TryGetValue(type, out var handlersInfo);

            handlers = handlersInfo?.Item2;

            if (!found)
            {

                lock (_undoableFieldCache)
                {
                    found = _undoableFieldCache.TryGetValue(type, out handlersInfo);

                    handlers = handlersInfo?.Item2;

                    if (!found)
                    {
                        var newHandlers = BuildHandlers(type);

                        var cacheInstance = ALCManager.CreateCacheInstance(type, newHandlers, OnAssemblyLoadContextUnload);

                        _undoableFieldCache.Add(type, cacheInstance);

                        handlers = newHandlers;
                    }
                }
            }
#else
            if (!_undoableFieldCache.TryGetValue(type, out handlers))
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
#endif

            return handlers;
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

#if NET5_0_OR_GREATER
        private static void OnAssemblyLoadContextUnload(AssemblyLoadContext context)
        {
            lock (_undoableFieldCache)
                ALCManager.RemoveFromCache(_undoableFieldCache, context);
        }
#endif
    }
}
#endif
