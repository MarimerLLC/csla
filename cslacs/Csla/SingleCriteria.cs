using System;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using Csla.Core;

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

    /// <summary>
    /// Creates an instance of the type.
    /// This is for use by the MobileFormatter,
    /// you must provide a criteria value
    /// parameter.
    /// </summary>
#if SILVERLIGHT
    public SingleCriteria()
    { }
#else
    protected SingleCriteria()
    { }
#endif

    #region MobileFormatter

    /// <summary>
    /// Override this method to insert your field values
    /// into the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="mode">
    /// The StateMode indicating why this method was invoked.
    /// </param>
    protected override void OnGetState(SerializationInfo info, StateMode mode)
    {
      base.OnGetState(info, mode);
      info.AddValue("Csla.Silverlight.SingleCriteria._value", _value);
    }

    /// <summary>
    /// Override this method to retrieve your field values
    /// from the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="mode">
    /// The StateMode indicating why this method was invoked.
    /// </param>
    protected override void OnSetState(SerializationInfo info, StateMode mode)
    {
      base.OnSetState(info, mode);
      _value = info.GetValue<C>("Csla.Silverlight.SingleCriteria._value");
    }

    #endregion
  }
}
