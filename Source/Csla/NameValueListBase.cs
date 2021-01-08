//-----------------------------------------------------------------------
// <copyright file="NameValueListBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This is the base class from which readonly name/value</summary>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel;
using Csla.Properties;
using Csla.Core;
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
    Core.ReadOnlyBindingList<NameValueListBase<K, V>.NameValuePair>, 
    ICloneable, Core.IBusinessObject, Server.IDataPortalTarget
  {

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
    /// Creates an instance of the object.
    /// </summary>
    protected NameValueListBase()
    {
      Initialize();
    }

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
    [Serializable()]
    public class NameValuePair : MobileObject
    {
      private K _key;
      private V _value;

#if (ANDROID || IOS) || NETFX_CORE
      /// <summary>
      /// Creates an instance of the object.
      /// </summary>
      public NameValuePair()
      { }
#else
      private NameValuePair() { }
#endif

      /// <summary>
      /// The Key or Name value.
      /// </summary>
      public K Key
      {
        get { return _key; }
      }

      /// <summary>
      /// The Value corresponding to the key/name.
      /// </summary>
      public V Value
      {
        get { return _value; }
      }

      /// <summary>
      /// Creates an instance of the object.
      /// </summary>
      /// <param name="key">The key.</param>
      /// <param name="value">The value.</param>
      public NameValuePair(K key, V value)
      {
        _key = key;
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
        info.AddValue("NameValuePair._key", _key);
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
        _key = info.GetValue<K>("NameValuePair._key");
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
      return Core.ObjectCloner.Clone(this);
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

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "criteria")]
    private void DataPortal_Create(object criteria)
    {
      throw new NotSupportedException(Resources.CreateNotSupportedException);
    }

    /// <summary>
    /// Override this method to allow retrieval of an existing business
    /// object based on data in the database.
    /// </summary>
    /// <param name="criteria">An object containing criteria values to identify the object.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    protected virtual void DataPortal_Fetch(object criteria)
    {
      throw new NotSupportedException(Resources.FetchNotSupportedException);
    }

    private void DataPortal_Update()
    {
      throw new NotSupportedException(Resources.UpdateNotSupportedException);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "criteria")]
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

    void Csla.Server.IDataPortalTarget.CheckRules()
    { }

    void Csla.Server.IDataPortalTarget.MarkAsChild()
    { }

    void Csla.Server.IDataPortalTarget.MarkNew()
    { }

    void Csla.Server.IDataPortalTarget.MarkOld()
    { }

    void Csla.Server.IDataPortalTarget.DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {
      this.DataPortal_OnDataPortalInvoke(e);
    }

    void Csla.Server.IDataPortalTarget.DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      this.DataPortal_OnDataPortalInvokeComplete(e);
    }

    void Csla.Server.IDataPortalTarget.DataPortal_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {
      this.DataPortal_OnDataPortalException(e, ex);
    }

    void Csla.Server.IDataPortalTarget.Child_OnDataPortalInvoke(DataPortalEventArgs e)
    { }

    void Csla.Server.IDataPortalTarget.Child_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    { }

    void Csla.Server.IDataPortalTarget.Child_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    { }

    #endregion
  }
}