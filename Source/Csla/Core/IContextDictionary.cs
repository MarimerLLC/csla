using Csla.Serialization.Mobile;

namespace Csla.Core
{
  /// <summary>
  /// Represents a context dictionary that provides a dictionary-like storage mechanism.
  /// This context is used to store data that is local to the current logical execution context.
  /// </summary>
  /// <remarks>
  /// The context dictionary is serializable and implements the IMobileObject, IDictionary, ICollection, and IEnumerable interfaces.
  /// </remarks>
  public interface IContextDictionary : IMobileObject, IDictionary, ICollection, IEnumerable
  {
    /// <summary>
    /// Retrieves the value associated with the specified key from the local context.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <returns>The value associated with the specified key, or null if the key does not exist.</returns>
    object GetValueOrNull(string key);

    /// <inheritdoc cref="System.Collections.Concurrent.ConcurrentDictionary{TKey, TValue}.TryAdd(TKey, TValue)"/>
    bool TryAdd(object key, object value);

    /// <inheritdoc cref="System.Collections.Concurrent.ConcurrentDictionary{TKey, TValue}.ContainsKey(TKey)"/>
    public bool ContainsKey(object key);

    /// <inheritdoc cref="System.Collections.Concurrent.ConcurrentDictionary{TKey, TValue}.TryRemove(TKey, out TValue)"/>
    public bool TryRemove(object key, out object value);

    /// <inheritdoc cref="System.Collections.Concurrent.ConcurrentDictionary{TKey, TValue}.TryGetValue(TKey, out TValue)"/>
    public bool TryGetValue(object key, out object value);

    /// <inheritdoc cref="System.Collections.Concurrent.ConcurrentDictionary{TKey, TValue}.TryUpdate(TKey, TValue, TValue)"/>
    bool TryUpdate(object key, object newValue, object comparisonValue);

    /// <inheritdoc cref="System.Collections.Concurrent.ConcurrentDictionary{TKey, TValue}.GetOrAdd(TKey, Func{TKey, TValue})"/>
    object GetOrAdd(object key, Func<object, object> valueFactory);

    /// <inheritdoc cref="System.Collections.Concurrent.ConcurrentDictionary{TKey, TValue}.GetOrAdd(TKey, TValue)"/>
    public object GetOrAdd(object key, object value);

    /// <inheritdoc cref="System.Collections.Concurrent.ConcurrentDictionary{TKey, TValue}.AddOrUpdate(TKey, Func{TKey, TValue}, Func{TKey, TValue, TValue})"/>
    public object AddOrUpdate(object key, Func<object, object> addValueFactory, Func<object, object, object> updateValueFactory);

    /// <inheritdoc cref="System.Collections.Concurrent.ConcurrentDictionary{TKey, TValue}.AddOrUpdate(TKey, TValue, Func{TKey, TValue, TValue})"/>
    public object AddOrUpdate(object key, object addValue, Func<object, object, object> updateValueFactory);
  }
}
