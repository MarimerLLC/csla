//-----------------------------------------------------------------------
// <copyright file="ReadOnlyObservableBindingList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>A readonly version of ObservableBindingList.</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Csla.Properties;

namespace Csla.Core
{
  /// <summary>
  /// A readonly version of ObservableBindingList.
  /// </summary>
  /// <typeparam name="C">Type of item contained in the list.</typeparam>
  /// <remarks>
  /// This is a subclass of ObservableBindingList that implements
  /// a readonly list, preventing adding and removing of items
  /// from the list. Use the IsReadOnly property
  /// to unlock the list for loading/unloading data.
  /// </remarks>
  [Serializable]
  public class ReadOnlyObservableBindingList<C> : ObservableBindingList<C>,
    Core.IReadOnlyBindingList
  {
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// 
    public ReadOnlyObservableBindingList()
    {
      AllowEdit = false;
      AllowNew = false;
      AllowRemove = false;

      this.CollectionChanged += (o, e) =>
        {
          if (IsReadOnly)
            throw new NotSupportedException(Resources.ChangeReadOnlyListInvalid);
        };
    }

    /// <summary>
    /// Method invoked when collection is changed.
    /// </summary>
    /// <param name="e">Arguments</param>
    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
      if (IsReadOnly)
        throw new NotSupportedException(Resources.ChangeReadOnlyListInvalid);
      base.OnCollectionChanged(e);
    }

    #region IsReadOnly

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
      get { return _isReadOnly; }
      protected set { _isReadOnly = value; }
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