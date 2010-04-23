using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Core
{
  /// <summary>
  /// Maintains metadata about a method or property.
  /// </summary>
  public interface IMemberInfo
  {
    /// <summary>
    /// Gets the member name value.
    /// </summary>
    string Name { get; }
  }
}
