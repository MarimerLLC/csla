//-----------------------------------------------------------------------
// <copyright file="ReadOnlyBindingList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>A readonly version of BindingList(Of T)</summary>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Csla.Serialization;
using Csla.Serialization.Mobile;
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
  [Serializable]
  public abstract class ReadOnlyBindingList<C> :
    Core.ExtendedBindingList<C>, Core.IBusinessObject, Core.IReadOnlyBindingList
  {
    private bool _isReadOnly;

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
#if !IOS
      protected set { IsReadOnlyCore = value; }
#else
      set { IsReadOnlyCore = value; }
#endif
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
    /// Creates an instance of the object.
    /// </summary>
    protected ReadOnlyBindingList()
    {
      IsReadOnlyCore = true;
      AllowEdit = false;
      AllowRemove = false;
      AllowNew = false;
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
        //DeferredLoadIndexIfNotLoaded();
        AllowRemove = oldValue;
      }
      else
        throw new NotSupportedException(Resources.ClearInvalidException);
    }

    /// <summary>
    /// Prevents insertion of items into the collection.
    /// </summary>
    protected override void AddNewCore()
    {
      if (!IsReadOnly)
        base.AddNewCore();
      else
        throw new NotSupportedException(Resources.InsertInvalidException);
    }

    /// <summary>
    /// Prevents insertion of items into the collection.
    /// </summary>
    /// <param name="index">Index at which to insert the item.</param>
    /// <param name="item">Item to insert.</param>
    protected override void InsertItem(int index, C item)
    {
      if (!IsReadOnly)
      {
        //InsertIndexItem(item);
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
        //RemoveIndexItem(this[index]);
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
        base.SetItem(index, item);
      else
        throw new NotSupportedException(Resources.ChangeInvalidException);
    }

    #region ITrackStatus

    /// <summary>
    /// Gets a value indicating whether this
    /// object or any child object is currently
    /// executing an async operation.
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

    #region MobileObject overrides

    /// <summary>
    /// Gets the state of the object for serialization.
    /// </summary>
    /// <param name="info">Serialization state</param>
    protected override void OnGetState(SerializationInfo info)
    {
      info.AddValue("Csla.Core.ReadOnlyBindingList._isReadOnly", _isReadOnly);
      base.OnGetState(info);
    }

    /// <summary>
    /// Sets the state of the object from serialization.
    /// </summary>
    /// <param name="info">Serialization state</param>
    protected override void OnSetState(SerializationInfo info)
    {
      _isReadOnly = info.GetValue<bool>("Csla.Core.ReadOnlyBindingList._isReadOnly");
      base.OnSetState(info);
    }

    /// <summary>
    /// Serializes any child objects.
    /// </summary>
    /// <param name="info">Serialization state</param>
    /// <param name="formatter">Serializer instance</param>
    protected override void OnSetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      IsReadOnlyCore = false;
      base.OnSetChildren(info, formatter);
      IsReadOnlyCore = true;
    }

    #endregion
  }
}