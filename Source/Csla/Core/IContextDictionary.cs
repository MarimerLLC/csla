using System.Collections;
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

    /// <summary>
    /// Attempts to add the specified key and value to the ContextDictionary.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
    /// <returns>
    /// true if the key/value pair was added to the ContextDictionary
    /// successfully; false if the key already exists.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">key is null.</exception>
    /// <exception cref="System.OverflowException">The dictionary already contains the maximum number of elements (System.Int32.MaxValue).</exception>
    bool TryAdd(object key, object value);

    /// <summary>
    /// Determines whether the ContextDictionary contains
    /// the specified key.
    /// </summary>
    /// <param name="key">The key to locate in the ContextDictionary.</param>
    /// <returns>
    /// true if the ContextDictionary contains an
    /// element with the specified key; otherwise, false.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">key is null.</exception>
    public bool ContainsKey(object key);

    /// <summary>
    /// Attempts to remove and return the value that has the specified key from the ContextDictionary.
    /// </summary>
    /// <param name="key">The key of the element to remove and return.</param>
    /// <param name="value">
    /// When this method returns, contains the object removed from the ContextDictionary,
    /// or the default value of the TValue type if key does not exist.
    /// </param>
    /// <returns>true if the object was removed successfully; otherwise, false.</returns>
    /// <exception cref="System.ArgumentNullException">key is null.</exception>
    public bool TryRemove(object key, out object value);

    /// <summary>
    /// Attempts to get the value associated with the specified key from the ContextDictionary.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <param name="value">
    /// When this method returns, contains the object from the ContextDictionary
    /// that has the specified key, or the default value of the type if the operation
    /// failed.
    /// </param>
    /// <returns>true if the key was found in the ContextDictionary; otherwise, false.</returns>
    /// <exception cref="System.ArgumentNullException">key is null.</exception>
    public bool TryGetValue(object key, out object value);

    /// <summary>
    /// Updates the value associated with key to newValue if the existing value with
    /// key is equal to comparisonValue.
    /// </summary>
    /// <param name="key">The key of the value that is compared with comparisonValue and possibly replaced.</param>
    /// <param name="newValue">
    /// The value that replaces the value of the element that has the specified key if
    /// the comparison results in equality.
    /// </param>
    /// <param name="comparisonValue">
    /// The value that is compared with the value of the element that has the specified
    /// key.
    /// </param>
    /// <returns>true if the value with key was equal to comparisonValue and was replaced with newValue; otherwise, false.</returns>
    /// <exception cref="System.ArgumentNullException">key is null.</exception>
    bool TryUpdate(object key, object newValue, object comparisonValue);

    /// <summary>
    /// Adds a key/value pair to the ContextDictionary
    /// by using the specified function if the key does not already exist. Returns the
    /// new value, or the existing value if the key exists.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="valueFactory">The function used to generate a value for the key.</param>
    /// <returns>
    /// The value for the key. This will be either the existing value for the key if
    /// the key is already in the dictionary, or the new value if the key was not in
    /// the dictionary.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">key or valueFactory is null.</exception>
    /// <exception cref="System.OverflowException">The dictionary already contains the maximum number of elements (System.Int32.MaxValue).</exception>
    object GetOrAdd(object key, Func<object, object> valueFactory);

    /// <summary>
    /// Adds a key/value pair to the ContextDictionary
    /// if the key does not already exist. Returns the new value, or the existing value
    /// if the key exists.
    /// </summary>
    /// <param name="key">The key of the element to add.</param>
    /// <param name="value">The value to be added, if the key does not already exist.</param>
    /// <returns>
    /// The value for the key. This will be either the existing value for the key if
    /// the key is already in the dictionary, or the new value if the key was not in
    /// the dictionary.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">key or valueFactory is null.</exception>
    /// <exception cref="System.OverflowException">The dictionary already contains the maximum number of elements (System.Int32.MaxValue).</exception>
    public object GetOrAdd(object key, object value);

    /// <summary>
    /// Uses the specified functions to add a key/value pair to the ContextDictionary
    /// if the key does not already exist, or to update a key/value pair in the ContextDictionary
    /// if the key already exists.
    /// </summary>
    /// <param name="key">The key to be added or whose value should be updated.</param>
    /// <param name="addValueFactory">The function used to generate a value for an absent key.</param>
    /// <param name="updateValueFactory">The function used to generate a new value for an existing key based on the key's existing value.</param>
    /// <returns>
    /// The new value for the key. This will be either be the result of addValueFactory
    /// (if the key was absent) or the result of updateValueFactory (if the key was present).
    /// </returns>
    /// <exception cref="System.ArgumentNullException">key, addValueFactory, or updateValueFactory is null.</exception>
    /// <exception cref="System.OverflowException">The dictionary already contains the maximum number of elements (System.Int32.MaxValue).</exception>
    public object AddOrUpdate(object key, Func<object, object> addValueFactory, Func<object, object, object> updateValueFactory);

    /// <summary>
    /// Adds a key/value pair to the ContextDictionary
    /// if the key does not already exist, or updates a key/value pair in the ContextDictionary
    /// by using the specified function if the key already exists.
    /// </summary>
    /// <param name="key">The key to be added or whose value should be updated.</param>
    /// <param name="addValue">The value to be added for an absent key.</param>
    /// <param name="updateValueFactory">The function used to generate a new value for an existing key based on the key's existing value.</param>
    /// <returns>
    /// The new value for the key. This will be either be addValue (if the key was absent)
    /// or the result of updateValueFactory (if the key was present).
    /// </returns>
    /// <exception cref="System.ArgumentNullException">key or updateValueFactory is null.</exception>
    /// <exception cref="System.OverflowException">The dictionary already contains the maximum number of elements (System.Int32.MaxValue).</exception>
    public object AddOrUpdate(object key, object addValue, Func<object, object, object> updateValueFactory);
  }
}
