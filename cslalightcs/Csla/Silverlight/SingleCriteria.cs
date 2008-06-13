using System;
using Csla.Serialization;
using Csla.Core.FieldManager;

namespace Csla.Silverlight
{
  /// <summary>
  /// A single-value criteria used to retrieve business
  /// objects that only require one criteria value.
  /// </summary>
  /// <typeparam name="T">
  /// Type of the criteria value.
  /// </typeparam>
  /// <remarks></remarks>
  [Serializable]
  public class SingleCriteria<T> : CriteriaBase
  {
    private T _value;

    /// <summary>
    /// Gets the criteria value provided by the caller.
    /// </summary>
    public T Value
    {
      get
      {
        return _value;
      }
      private set
      {
        _value = value;
      }
    }

    /// <summary>
    /// Creates an instance of the object. For use by
    /// MobileFormatter only - you must provide a 
    /// Type parameter in your code.
    /// </summary>
    public SingleCriteria() 
      : base()
    { }

    /// <summary>
    /// Creates an instance of the type,
    /// initializing it with the criteria
    /// value.
    /// </summary>
    /// <param name="objectType">
    /// Type of business object to retrieve.
    /// </param>
    /// <param name="value">
    /// The criteria value.
    /// </param>
    public SingleCriteria(Type objectType, T value)
      : base(objectType)
    {
      this.Value = value;
    }

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
      _value = info.GetValue<T>("Csla.Silverlight.SingleCriteria._value");
    }

    #endregion
  }
}
