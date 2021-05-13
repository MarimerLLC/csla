#if NET5_0_OR_GREATER

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Loader;

namespace Csla.Runtime
{
    /// <summary>
    /// Provides common access to "AssemblyLoadContext" logic, which is used for "Plugins" concept of .NET 5+
    /// </summary>
    public static class AssemblyLoadContextManager
  {
        /// <summary>
        /// Simplified helper for "unloadable" cache item creation, linked to currently active "ContextualReflectionScope".
        /// </summary>
        /// 
        /// <param name="objectType">Caching item source.</param>
        /// <param name="cachingItem">Caching item.</param>
        /// <param name="unloadAction">Action which will be performed after "AssemblyLoadContext" unload.</param>
        /// <param name="excludeNonCollectible">
        /// Provides possibility to not cache items which are referenced to main application - "AssemblyLoadContext.Default".
        /// </param>
        /// 
        /// <typeparam name="TValue">Type of caching item.</typeparam>
        /// 
        /// <returns>Tuple structure where "Item1" is name of active "AssemblyLoadContext" and "Item2" - caching item.</returns>
        /// 
        /// <exception cref="ArgumentNullException">Throws if nor object type, nor caching item, not unload action was provided.</exception>
        public static Tuple<string, TValue> CreateCacheInstance<TValue>(
            Type objectType,
            TValue cachingItem,
            Action<AssemblyLoadContext> unloadAction,
            bool excludeNonCollectible = false)
        {
            if (objectType == null)
            {
                throw new ArgumentNullException("objectType");
            }

            if (cachingItem == null)
            {
              throw new ArgumentNullException("cachingItem");
            }

            if (unloadAction == null)
            {
              throw new ArgumentNullException(nameof(Action<AssemblyLoadContext>));
            }

            string assemblyLoadContextName = null;

            if ((!excludeNonCollectible || objectType.Assembly.IsCollectible)
                && AssemblyLoadContext.CurrentContextualReflectionContext != null
                && AssemblyLoadContext.CurrentContextualReflectionContext.Name != AssemblyLoadContext.Default.Name)
            {
                assemblyLoadContextName = AssemblyLoadContext.CurrentContextualReflectionContext.Name;

                AssemblyLoadContext.CurrentContextualReflectionContext.Unloading += unloadAction;
            }

            return new Tuple<string, TValue>(assemblyLoadContextName, cachingItem);
        }

        /// <summary>
        /// Provides possibility of partial cache flushing after unloading of certain "AssemblyLoadContext".
        /// </summary>
        /// 
        /// <param name="dictionary">Cached items dictionary.</param>
        /// <param name="context">Unloading "AssemblyLoadContext".</param>
        /// <param name="usingConcurrentDictionary">Guide method how cached item should be removed from dictionary.</param>
        /// 
        /// <typeparam name="TKey">Key of cached item from dictionary.</typeparam>
        /// <typeparam name="TValue">Value of cached item from dictionary.</typeparam>
        /// 
        /// <exception cref="ArgumentNullException">Throws if cached items dictionary was not provided.</exception>
        public static void RemoveFromCache<TKey, TValue>(
          IDictionary<TKey, Tuple<string, TValue>> dictionary,
          AssemblyLoadContext context,
          bool usingConcurrentDictionary = false)
        {
            if (dictionary == null)
            {
                throw new ArgumentNullException("IDictionary<T, Tuple<string, TC>>");
            }

            if (context == null)
            {
                return;
            }

            var obsoleteCacheKeys = new List<TKey>();

            foreach (var (cacheKey, cacheInstance) in dictionary)
            {
                if (cacheInstance.Item1 == context.Name)
                {
                    obsoleteCacheKeys.Add(cacheKey);
                }
            }

            foreach (var cacheKey in obsoleteCacheKeys)
            {
                _ = usingConcurrentDictionary
                  ? ((ConcurrentDictionary<TKey, Tuple<string, TValue>>)dictionary).TryRemove(cacheKey, out _)
                  : dictionary.Remove(cacheKey);
            }
        }
    }
}
#endif
