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
    public MetaPropertyChangedEventArgs(string propertyName) : base(propertyName)
    {
    }
  }
}
