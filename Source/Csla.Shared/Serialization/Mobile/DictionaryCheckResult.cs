
namespace Csla.Serialization.Mobile
{
  /// <summary>
  /// This struct contains the result of check for existing dictionary
  /// key/value pair when creating internal dictionary of strings
  /// in order to minimize the amount of data that <see cref="CslaBinaryWriter"/>
  /// sends over the wire
  /// </summary>
  internal struct DictionaryCheckResult
  {
    internal DictionaryCheckResult(bool isNew, int key)
    {
      this.IsNew = isNew;
      this.Key = key;
    }

    /// <summary>
    /// Indicates that the key did not exist and was just added
    /// </summary>
    internal bool IsNew { get; }
    /// <summary>
    /// Newly added or existing integer key corresponding to 
    /// string value
    /// </summary>
    internal int Key { get; }
  }
}
