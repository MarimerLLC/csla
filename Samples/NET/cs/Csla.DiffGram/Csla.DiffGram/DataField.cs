using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.DiffGram
{
  /// <summary>
  /// Contains a property or field
  /// value from a business object.
  /// </summary>
  [Serializable]
  public class DataField
  {
    /// <summary>
    /// Gets or sets the property or field name.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Gets or sets the property or field value.
    /// </summary>
    public object Value { get; set; }
  }
}
