using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Security
{
  /// <summary>
  /// Defines the authorization interface through which an
  /// object can indicate which properties the current
  /// user can read and write.
  /// </summary>
  public interface IAuthorizeReadWrite
  {
    /// <summary>
    /// Returns <see langword="true" /> if the user is allowed to write the
    /// to the specified property.
    /// </summary>
    /// <returns><see langword="true" /> if write is allowed.</returns>
    /// <param name="propertyName">Name of the property to read.</param>
    bool CanWriteProperty(string propertyName);
    /// <summary>
    /// Returns <see langword="true" /> if the user is allowed to read the
    /// specified property.
    /// </summary>
    /// <returns><see langword="true" /> if read is allowed.</returns>
    /// <param name="propertyName">Name of the property to read.</param>
    bool CanReadProperty(string propertyName);
  }
}
