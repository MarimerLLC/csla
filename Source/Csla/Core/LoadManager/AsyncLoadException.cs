namespace Csla.Core.LoadManager
{
  /// <summary>
  /// Exception class to add the PropertyInfo and better nmessage to an async exception
  /// </summary>
  [Serializable]
  public class AsyncLoadException : Exception
  {

    /// <summary>
    /// The property that Async LazyLoad failed on.
    /// </summary>
    /// <value>
    /// The property.
    /// </value>
    public IPropertyInfo Property { get; private set; }

    /// <summary>
    /// Constructor for AsyncLoadException
    /// </summary>
    /// <param name="property">the IPropertyInfo that desccribes the property</param>
    /// <param name="message">Clear text message for user</param>
    /// <param name="ex">the actual exception</param>
    /// <exception cref="ArgumentNullException"><paramref name="property"/> is <see langword="null"/>.</exception>
    public AsyncLoadException(IPropertyInfo property, string? message, Exception? ex) : base(message, ex)
    {
      Property = property;
    }
  }
}
