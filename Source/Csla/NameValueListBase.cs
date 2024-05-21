//-----------------------------------------------------------------------
// <copyright file="NameValueListBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This is the base class from which readonly name/value</summary>
//-----------------------------------------------------------------------

using System.ComponentModel;
using Csla.Core;
using Csla.Properties;
using Csla.Serialization.Mobile;

namespace Csla
{

  /// <summary>
  /// This is the base class from which readonly name/value
  /// collections should be derived.
  /// </summary>
  /// <typeparam name="K">Type of the key values.</typeparam>
  /// <typeparam name="V">Type of the values.</typeparam>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  [Serializable]
  public abstract class NameValueListBase<K, V> :
    ReadOnlyBindingList<NameValueListBase<K, V>.NameValuePair>,
    ICloneable,
    Server.IDataPortalTarget,
    IUseApplicationContext
  {
    /// <summary>
    /// Gets the current ApplicationContext
    /// </summary>
    protected ApplicationContext ApplicationContext { get; private set; }
    ApplicationContext IUseApplicationContext.ApplicationContext
    {
      get => ApplicationContext;
      set
      {
        ApplicationContext = value;
        Initialize();
      }
    }

    #region Core Implementation

    /// <summary>
    /// Returns the value corresponding to the
    /// specified key.
    /// </summary>
    /// <param name="key">Key value for which to retrieve a value.</param>
    public V Value(K key)
    {
      foreach (NameValuePair item in this)
        if (item.Key.Equals(key))
          return item.Value;
      return default(V);
    }

    /// <summary>
    /// Returns the key corresponding to the
    /// first occurance of the specified value
    /// in the list.
    /// </summary>
    /// <param name="value">Value for which to retrieve the key.</param>
    public K Key(V value)
    {
      foreach (NameValuePair item in this)
        if (item.Value.Equals(value))
          return item.Key;
      return default(K);
    }

    /// <summary>
    /// Gets a value indicating whether the list contains the
    /// specified key.
    /// </summary>
    /// <param name="key">Key value for which to search.</param>
    public bool ContainsKey(K key)
    {
      foreach (NameValuePair item in this)
        if (item.Key.Equals(key))
          return true;
      return false;
    }

    /// <summary>
    /// Gets a value indicating whether the list contains the
    /// specified value.
    /// </summary>
    /// <param name="value">Value for which to search.</param>
    public bool ContainsValue(V value)
    {
      foreach (NameValuePair item in this)
        if (item.Value.Equals(value))
          return true;
      return false;
    }

    /// <summary>
    /// Get the item for the first matching
    /// value in the collection.
    /// </summary>
    /// <param name="value">
    /// Value to search for in the list.
    /// </param>
    /// <returns>Item from the list.</returns>
    public NameValuePair GetItemByValue(V value)
    {

      foreach (NameValuePair item in this)
      {
        if (item != null && item.Value.Equals(value))
        {
          return item;
        }
      }
      return null;

    }

    /// <summary>
    /// Get the item for the first matching
    /// key in the collection.
    /// </summary>
    /// <param name="key">
    /// Key to search for in the list.
    /// </param>
    /// <returns>Item from the list.</returns>
    public NameValuePair GetItemByKey(K key)
    {

      foreach (NameValuePair item in this)
      {
        if (item != null && item.Key.Equals(key))
        {
          return item;
        }
      }
      return null;

    }

    #endregion

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    protected NameValueListBase()
    { }

    #region Initialize

    /// <summary>
    /// Override this method to set up event handlers so user
    /// code in a partial class can respond to events raised by
    /// generated code.
    /// </summary>
    protected virtual void Initialize()
    { /* allows subclass to initialize events before any other activity occurs */ }

    #endregion

    #region NameValuePair class

    /// <summary>
    /// Contains a key and value pair.
    /// </summary>
    [Serializable]
    public class NameValuePair : MobileObject
    {
      private V _value;

      /// <summary>
      /// Creates an instance of the type (for use by MobileFormatter only).
      /// </summary>
      public NameValuePair()
      { }

      /// <summary>
      /// The Key or Name value.
      /// </summary>
      public K Key { get; private set; }

      /// <summary>
      /// The Value corresponding to the key/name.
      /// </summary>
      public V Value
      {
        get { return _value; }
      }

      /// <summary>
      /// Creates an instance of the type.
      /// </summary>
      /// <param name="key">The key.</param>
      /// <param name="value">The value.</param>
      public NameValuePair(K key, V value)
      {
        Key = key;
        _value = value;
      }

      /// <summary>
      /// Returns a string representation of the
      /// value for this item.
      /// </summary>
      public override string ToString()
      {
        return _value.ToString();
      }

      /// <summary>
      /// Override this method to manually get custom field
      /// values from the serialization stream.
      /// </summary>
      /// <param name="info">Serialization info.</param>
      /// <param name="mode">Serialization mode.</param>
      protected override void OnGetState(SerializationInfo info, StateMode mode)
      {
        base.OnGetState(info, mode);
        info.AddValue("NameValuePair._key", Key);
        info.AddValue("NameValuePair._value", _value);
      }

      /// <summary>
      /// Override this method to manually set custom field
      /// values into the serialization stream.
      /// </summary>
      /// <param name="info">Serialization info.</param>
      /// <param name="mode">Serialization mode.</param>
      protected override void OnSetState(SerializationInfo info, StateMode mode)
      {
        base.OnSetState(info, mode);
        Key = info.GetValue<K>("NameValuePair._key");
        _value = info.GetValue<V>("NameValuePair._value");
      }

    }

    #endregion

    #region ICloneable

    object ICloneable.Clone()
    {
      return GetClone();
    }

    /// <summary>
    /// Creates a clone of the object.
    /// </summary>
    /// <returns>A new object containing the exact data of the original object.</returns>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual object GetClone()
    {
      return ObjectCloner.GetInstance(ApplicationContext).Clone(this);
    }

    /// <summary>
    /// Creates a clone of the object.
    /// </summary>
    public NameValueListBase<K, V> Clone()
    {
      return (NameValueListBase<K, V>)GetClone();
    }

    #endregion

    #region Data Access

    private void DataPortal_Update()
    {
      throw new NotSupportedException(Resources.UpdateNotSupportedException);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "criteria")]
    [Delete]
    private void DataPortal_Delete(object criteria)
    {
      throw new NotSupportedException(Resources.DeleteNotSupportedException);
    }

    /// <summary>
    /// Called by the server-side DataPortal prior to calling the 
    /// requested DataPortal_XYZ method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {

    }

    /// <summary>
    /// Called by the server-side DataPortal after calling the 
    /// requested DataPortal_XYZ method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {

    }

    /// <summary>
    /// Called by the server-side DataPortal if an exception
    /// occurs during data access.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    /// <param name="ex">The Exception thrown during data access.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {

    }

    #endregion

    #region IDataPortalTarget Members

    void Server.IDataPortalTarget.CheckRules()
    { }

    Task Server.IDataPortalTarget.CheckRulesAsync() => Task.CompletedTask;

    async Task Csla.Server.IDataPortalTarget.WaitForIdle(TimeSpan timeout) => await BusyHelper.WaitForIdle(this, timeout).ConfigureAwait(false);
    async Task Csla.Server.IDataPortalTarget.WaitForIdle(CancellationToken ct) => await BusyHelper.WaitForIdle(this, ct).ConfigureAwait(false);

    void Server.IDataPortalTarget.MarkAsChild()
    { }

    void Server.IDataPortalTarget.MarkNew()
    { }

    void Server.IDataPortalTarget.MarkOld()
    { }

    void Server.IDataPortalTarget.DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {
      DataPortal_OnDataPortalInvoke(e);
    }

    void Server.IDataPortalTarget.DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      DataPortal_OnDataPortalInvokeComplete(e);
    }

    void Server.IDataPortalTarget.DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {
      DataPortal_OnDataPortalException(e, ex);
    }

    void Server.IDataPortalTarget.Child_OnDataPortalInvoke(DataPortalEventArgs e)
    { }

    void Server.IDataPortalTarget.Child_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    { }

    void Server.IDataPortalTarget.Child_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    { }

    #endregion
  }
}