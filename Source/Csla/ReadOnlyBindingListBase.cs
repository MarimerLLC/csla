//-----------------------------------------------------------------------
// <copyright file="ReadOnlyBindingListBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This is the base class from which readonly collections</summary>
//-----------------------------------------------------------------------
#if NETFX_CORE
using System;

namespace Csla
{
  /// <summary>
  /// This is the base class from which readonly collections
  /// of readonly objects should be derived.
  /// </summary>
  /// <typeparam name="T">Type of the list class.</typeparam>
  /// <typeparam name="C">Type of child objects contained in the list.</typeparam>
  [Serializable]
  public abstract class ReadOnlyBindingListBase<T, C> : ReadOnlyListBase<T, C>
    where T : ReadOnlyBindingListBase<T, C>
  { }
}
#else
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using Csla.Properties;

namespace Csla
{
  /// <summary>
  /// This is the base class from which readonly collections
  /// of readonly objects should be derived.
  /// </summary>
  /// <typeparam name="T">Type of the list class.</typeparam>
  /// <typeparam name="C">Type of child objects contained in the list.</typeparam>
  [System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  [Serializable()]
  public abstract class ReadOnlyBindingListBase<T, C> : 
    Core.ReadOnlyBindingList<C>, Csla.Core.IReadOnlyCollection, 
    ICloneable, Server.IDataPortalTarget
    where T : ReadOnlyBindingListBase<T, C>
  {

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    protected ReadOnlyBindingListBase()
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
    /// <returns>
    /// A new object containing the exact data of the original object.
    /// </returns>
    public T Clone()
    {
      return (T)GetClone();
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
    /// requested DataPortal_xyz method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
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

    /// <summary>
    /// Called by the server-side DataPortal prior to calling the 
    /// requested DataPortal_XYZ method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void Child_OnDataPortalInvoke(DataPortalEventArgs e)
    {
    }

    /// <summary>
    /// Called by the server-side DataPortal after calling the 
    /// requested DataPortal_XYZ method.
    /// </summary>
    /// <param name="e">The DataPortalContext object passed to the DataPortal.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1707:IdentifiersShouldNotContainUnderscores", MessageId = "Member")]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void Child_OnDataPortalInvokeComplete(DataPortalEventArgs e)
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
    protected virtual void Child_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {
    }

#endregion

#region ToArray

    /// <summary>
    /// Get an array containing all items in the list.
    /// </summary>
    public C[] ToArray()
    {
      List<C> result = new List<C>();
      foreach (C item in this)
        result.Add(item);
      return result.ToArray();
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
    {
      this.Child_OnDataPortalInvoke(e);
    }

    void Csla.Server.IDataPortalTarget.Child_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      this.Child_OnDataPortalInvokeComplete(e);
    }

    void Csla.Server.IDataPortalTarget.Child_OnDataPortalException(DataPortalEventArgs e, Exception ex)
    {
      this.Child_OnDataPortalException(e, ex);
    }

#endregion
  }
}
#endif