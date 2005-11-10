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
  /// <remarks>
  /// To implement a name/value collection:
  /// <list>
  /// <item>Inherit from this base class</item>
  /// <item>Implement a Private constructor</item>
  /// <item>Implement a factory method using the supplied Criteria class</item>
  /// <item>Override <see cref="DataPortal_Fetch" /></item>
  /// </list>
  /// </remarks>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  [Serializable()]
  public abstract class NameValueListBase<K, V> : Core.ReadOnlyBindingList<NameValueListBase<K, V>.NameValuePair>, ICloneable, Core.IBusinessObject
  {

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

    public V Value(K key)
    {
      foreach (NameValuePair item in this)
        if (item.Key.Equals(key))
          return item.Value;
      return default(V);
    }

    public K Key(V value)
    {
      foreach (NameValuePair item in this)
        if (item.Value.Equals(value))
          return item.Key;
      return default(K);
    }

    #region Clone

    /// <summary>
    /// Creates a clone of the object.
    /// </summary>
    /// <returns>A new object containing the exact data of the original object.</returns>
    public virtual object Clone()
    {
      return Core.ObjectCloner.Clone(this);
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
      {

      }
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
    /// requested DataPortal_xyz method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OndataPortalInvoke(DataPortalEventArgs e)
    {

    }

    /// <summary>
    /// Called by the server-side DataPortal after calling the 
    /// requested DataPortal_xyz method.
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