using System.Collections;
using Csla.Serialization.Mobile;

namespace Csla.Core
{
  /// <summary>
  /// Represents a local context that provides a dictionary-like storage mechanism.
  /// This context is used to store data that is local to the current logical execution context.
  /// </summary>
  /// <remarks>
  /// The local context is serializable and implements the IDictionary, ICollection, and IEnumerable interfaces.
  /// </remarks>
  public interface ILocalContext : IMobileObject, IDictionary, ICollection, IEnumerable
  {
    /// <summary>
    /// Retrieves the value associated with the specified key from the local context.
    /// </summary>
    /// <param name="key">The key of the value to get.</param>
    /// <returns>The value associated with the specified key, or null if the key does not exist.</returns>
    object GetValueOrNull(string key);
  }
}
