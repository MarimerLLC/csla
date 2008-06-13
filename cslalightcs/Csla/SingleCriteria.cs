using System;
using Csla.Serialization;
using Csla.Core.FieldManager;

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

#if SILVERLIGHT
    /// <summary>
    /// Creates an instance of the type.
    /// This is for use by the MobileFormatter,
    /// you must provide a criteria value
    /// parameter.
    /// </summary>
    public SingleCriteria()
    { }
#else
    private SingleCriteria()
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
    protected override void OnGetState(Csla.Serialization.Mobile.SerializationInfo info)
    {
      base.OnGetState(info);
      info.AddValue("Csla.Silverlight.SingleCriteria._value", _value);
    }

    /// <summary>
    /// Override this method to retrieve your field values
    /// from the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    protected override void OnSetState(Csla.Serialization.Mobile.SerializationInfo info)
    {
      base.OnSetState(info);
      _value = info.GetValue<C>("Csla.Silverlight.SingleCriteria._value");
    }

    #endregion
  }
}
