using System.ComponentModel;

namespace Csla.Core
{

  /// <summary>
  /// Used to distinguish Meta Properties 
  /// </summary>
  public class MetaPropertyChangedEventArgs : PropertyChangedEventArgs
  {
    /// <summary>
    /// Property changed event args
    /// </summary>
    /// <param name="propertyName">Name of changed property.</param>
    /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/>.</exception>
    public MetaPropertyChangedEventArgs(string propertyName) : base(propertyName)
    {
      if (propertyName is null)
        throw new ArgumentNullException(nameof(propertyName));
    }
  }
}
