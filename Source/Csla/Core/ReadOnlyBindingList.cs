//-----------------------------------------------------------------------
// <copyright file="ReadOnlyBindingList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>A readonly version of BindingList(Of T)</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Properties;

namespace Csla.Core
{
  /// <summary>
  /// A readonly version of BindingList(Of T)
  /// </summary>
  /// <typeparam name="C">Type of item contained in the list.</typeparam>
  /// <remarks>
  /// This is a subclass of BindingList(Of T) that implements
  /// a readonly list, preventing adding and removing of items
  /// from the list. Use the IsReadOnly property
  /// to unlock the list for loading/unloading data.
  /// </remarks>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", 
    "CA1710:IdentifiersShouldHaveCorrectSuffix")]
  [Serializable()]
  public abstract class ReadOnlyBindingList<C> :
    Core.ExtendedBindingList<C>, Core.IBusinessObject, Core.IReadOnlyBindingList
  {
    #region Identity

    int IBusinessObject.Identity
    {
      get { return 0; }
    }

    #endregion

    private bool _isReadOnly = true;

    /// <summary>
    /// Gets or sets a value indicating whether the list is readonly.
    /// </summary>
    /// <remarks>
    /// Subclasses can set this value to unlock the collection
    /// in order to alter the collection's data.
    /// </remarks>
    /// <value>True indicates that the list is readonly.</value>
    public bool IsReadOnly
    {
      get { return IsReadOnlyCore; }
      protected set { IsReadOnlyCore = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether
    /// the list is readonly.
    /// </summary>
    protected virtual bool IsReadOnlyCore
    {
      get { return _isReadOnly; }
      set { _isReadOnly = value; }
    }

    bool Core.IReadOnlyBindingList.IsReadOnly
    {
      get { return IsReadOnly; }
      set { IsReadOnly = value; }
    }

    /// <summary>
    /// Sets the LoadListMode for the collection
    /// </summary>
    /// <param name="enabled">Enable or disable mode</param>
    protected override void SetLoadListMode(bool enabled)
    {
      IsReadOnly = !enabled;
      base.SetLoadListMode(enabled);
    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    protected ReadOnlyBindingList()
    {
      this.RaiseListChangedEvents = false;
      AllowEdit = false;
      AllowRemove = false;
      AllowNew = false;
      this.RaiseListChangedEvents = true;
    }

    /// <summary>
    /// Prevents clearing the collection.
    /// </summary>
    protected override void ClearItems()
    {
      if (!IsReadOnly)
      {
        bool oldValue = AllowRemove;
        AllowRemove = true;
        base.ClearItems();
        AllowRemove = oldValue;
      }
      else
        throw new NotSupportedException(Resources.ClearInvalidException);
    }

    /// <summary>
    /// Prevents insertion of items into the collection.
    /// </summary>
#if NETFX_CORE || (ANDROID || IOS)
    protected override void AddNewCore()
    {
      if (!IsReadOnly)
        base.AddNewCore();
      else
        throw new NotSupportedException(Resources.InsertInvalidException);
    }
#else
    protected override object AddNewCore()
    {
      if (!IsReadOnly)
        return base.AddNewCore();
      else
        throw new NotSupportedException(Resources.InsertInvalidException);
    }
#endif

    /// <summary>
    /// Prevents insertion of items into the collection.
    /// </summary>
    /// <param name="index">Index at which to insert the item.</param>
    /// <param name="item">Item to insert.</param>
    protected override void InsertItem(int index, C item)
    {
      if (!IsReadOnly)
      {
        base.InsertItem(index, item);
      }
      else
        throw new NotSupportedException(Resources.InsertInvalidException);
    }

    /// <summary>
    /// Removes the item at the specified index if the collection is
    /// not in readonly mode.
    /// </summary>
    /// <param name="index">Index of the item to remove.</param>
    protected override void RemoveItem(int index)
    {
      if (!IsReadOnly)
      {
        bool oldValue = AllowRemove;
        AllowRemove = true;
        base.RemoveItem(index);
        AllowRemove = oldValue;
      }
      else
        throw new NotSupportedException(Resources.RemoveInvalidException);
    }

    /// <summary>
    /// Replaces the item at the specified index with the 
    /// specified item if the collection is not in
    /// readonly mode.
    /// </summary>
    /// <param name="index">Index of the item to replace.</param>
    /// <param name="item">New item for the list.</param>
    protected override void SetItem(int index, C item)
    {
      if (!IsReadOnly)
      {
        base.SetItem(index, item);
      }
      else
        throw new NotSupportedException(Resources.ChangeInvalidException);
    }

#region ITrackStatus

    /// <summary>
    /// Gets a value indicating whether this object or its
    /// child objects are busy.
    /// </summary>
    public override bool IsBusy
    {
      get
      {
        // run through all the child objects
        // and if any are dirty then then
        // collection is dirty
        foreach (C child in this)
        {
          INotifyBusy busy = child as INotifyBusy;
          if (busy != null && busy.IsBusy)
            return true;
        }

        return false;
      }
    }

#endregion

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
      info.AddValue("Csla.Core.ReadOnlyBindingList._isReadOnly", _isReadOnly);
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
      _isReadOnly = info.GetValue<bool>("Csla.Core.ReadOnlyBindingList._isReadOnly");
    }

    /// <summary>
    /// Override this method to retrieve your child object
    /// references from the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    /// <param name="formatter">
    /// Reference to MobileFormatter instance. Use this to
    /// convert child references to/from reference id values.
    /// </param>
    protected override void OnSetChildren(Csla.Serialization.Mobile.SerializationInfo info, Csla.Serialization.Mobile.MobileFormatter formatter)
    {
      var old = IsReadOnly;
      IsReadOnly = false;
      base.OnSetChildren(info, formatter);
      IsReadOnly = old;
    }

#endregion
  }
}