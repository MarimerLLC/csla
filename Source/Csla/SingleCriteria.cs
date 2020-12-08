//-----------------------------------------------------------------------
// <copyright file="SingleCriteria.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>A single-value criteria used to retrieve business</summary>
//-----------------------------------------------------------------------
#if !MONO
using System;
using Csla.Serialization.Mobile;
using Csla.Core;

namespace Csla
{
  /// <summary>
  /// A single-value criteria used to retrieve business
  /// objects that only require one criteria value.
  /// </summary>
  /// <typeparam name="C">
  /// Type of the criteria value.
  /// </typeparam>
  /// <remarks></remarks>
  [Serializable()]
  [Obsolete("Use custom class derived from CriteriaBase e.g. ProjectCriteria : CriteriaBase<ProjectCriteria> instead.")]
  public class SingleCriteria<C> : CriteriaBase<SingleCriteria<C>>
  {
    private C _value;

    /// <summary>
    /// Gets the criteria value provided by the caller.
    /// </summary>
    public C Value
    {
      get { return _value; }
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
    {
      _value = value;
    }

    /// <summary>
    /// Creates an instance of the type.
    /// This is for use by the MobileFormatter,
    /// you must provide a criteria value
    /// parameter.
    /// </summary>
#if (ANDROID || IOS) || NETFX_CORE
    public SingleCriteria()
    { }
#else
    protected SingleCriteria()
    { }
#endif

    /// <summary>
    /// Creates an instance of the type.
    /// This is for use by the MobileFormatter,
    /// you must provide a criteria value
    /// parameter.
    /// </summary>
    /// <param name="type">Business object type.</param>
    /// <param name="value">Criteria value.</param>
    protected SingleCriteria(Type type, C value)
    {
      _value = value;
    }

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
      info.AddValue("Csla.Xaml.SingleCriteria._value", _value);
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
      _value = info.GetValue<C>("Csla.Xaml.SingleCriteria._value");
    }

    #endregion
  }

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
  [Obsolete("Use custom class derived from CriteriaBase e.g. ProjectCriteria : CriteriaBase<ProjectCriteria> instead.")]
  public class SingleCriteria<B, C> : SingleCriteria<C>
  {
    /// <summary>
    /// Creates an instance of the type,
    /// initializing it with the criteria
    /// value.
    /// </summary>
    /// <param name="value">
    /// The criteria value.
    /// </param>
    public SingleCriteria(C value)
      : base(typeof(B), value)
    { }

    /// <summary>
    /// Creates an instance of the type.
    /// This is for use by the MobileFormatter,
    /// you must provide a criteria value
    /// parameter.
    /// </summary>
#if (ANDROID || IOS) || NETFX_CORE
    public SingleCriteria()
    { }
#else
    protected SingleCriteria()
    { }
#endif
  }
}
#endif