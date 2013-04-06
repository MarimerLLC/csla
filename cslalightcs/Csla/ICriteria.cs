using System;

namespace Csla
{
  /// <summary>
  /// Defines the base requirements for a criteria
  /// object supported by the data portal.
  /// </summary>
  public interface ICriteria
  {
    /// <summary>
    /// Type of the business object to be instantiated by
    /// the server-side DataPortal. 
    /// </summary>
    Type ObjectType { get; }
  }
}
