using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;
using Csla.Properties;

namespace Csla
{

  /// <summary>
  /// This is the base class from which readonly name/value
  /// collections should be derived.
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  [Serializable()]
  public abstract class NameValueListBase<K, V> : 
    Core.ReadOnlyBindingList<NameValueListBase<K, V>.NameValuePair>, 
    ICloneable, Core.IBusinessObject
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
    public bool ContainsValue(V value)
    {
      foreach (NameValuePair item in this)
        if (item.Value.Equals(value))
          return true;
      return false;
    }

    #endregion

    #region Constructors

    protected NameValueListBase()
    {

    }

    #endregion

    #region NameValuePair class

    /// <summary>
    /// Contains a key and value pair.
    /// </summary>
    [Serializable()]
    public class NameValuePair
    {
      private K _key;
      private V _value;

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

      public NameValuePair(K key, V value)
      {
        _key = key;
        _value = value;
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

    public NameValueListBase<K, V> Clone()
    {
      return (NameValueListBase<K, V>)GetClone();
    }

    #endregion

    #region Criteria

    /// <summary>
    /// Default Criteria for retrieving simple
    /// name/value lists.
    /// </summary>
    /// <remarks>
    /// This criteria merely specifies the type of
    /// collection to be retrieved. That type information
    /// is used by the DataPortal to create the correct
    /// type of collection object during data retrieval.
    /// </remarks>
    [Serializable()]
    protected class Criteria : CriteriaBase
    {
      public Criteria(Type collectionType)
        : base(collectionType)
      { }
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
    /// <param name="Criteria">An object containing criteria values to identify the object.</param>
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
    protected virtual void DataPortal_OndataPortalInvoke(DataPortalEventArgs e)
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

  }
}