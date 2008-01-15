using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla
{
  /// <summary>
  /// A single-value criteria used to retrieve business
  /// objects that only require one criteria value.
  /// </summary>
  /// <typeparam name="B">
  /// Type of business object to retrieve.
  /// </typeparam>
  /// <typeparam name="C">
  /// Type of the criteria value.
  /// </typeparam>
  /// <remarks></remarks>
  [Serializable()]
  public class SingleCriteria<B, C> : CriteriaBase
  {
    private C _value;

    /// <summary>
    /// Gets the criteria value provided by the caller.
    /// </summary>
    public C Value
    {
      get
      {
        return _value;
      }
    }

    /// <summary>
    /// Creates an instance of the type,
    /// initializing it with the criteria
    /// value.
    /// </summary>
    /// <param name="value">
    /// The criteria value.
    /// </param>
    public SingleCriteria(C value)
      : base(typeof(B))
    {
      _value = value;
    }
  }
}
