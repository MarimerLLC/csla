//-----------------------------------------------------------------------
// <copyright file="MobileBindingList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Inherit from this base class to easily</summary>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Collections.Generic;
using Csla.Serialization.Mobile;
using Csla.Properties;
using System.Reflection;
using Csla.Reflection;
using System.Diagnostics;

namespace Csla.Core
{
  /// <summary>
  /// Inherit from this base class to easily
  /// create a serializable list class.
  /// </summary>
  /// <typeparam name="T">
  /// Type of the items contained in the list.
  /// </typeparam>
#if TESTING
  [System.Diagnostics.DebuggerStepThrough]
#endif
  [Serializable]
  public class MobileBindingList<T> : BindingList<T>, IMobileList
  {
    #region LoadListMode

    [NonSerialized]
    [NotUndoable]
    private LoadListModeObject _loadListModeObject = null;

    /// <summary>
    /// By wrapping this property inside Using block
    /// you can set property values on current business object
    /// without raising PropertyChanged events
    /// and checking user rights.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    protected LoadListModeObject LoadListMode
    {
      get
      {
        if (_loadListModeObject == null)
        {
          _loadListModeObject = new LoadListModeObject(this);
          SetLoadListMode(true);
        }
        return _loadListModeObject;
      }
    }
    void IMobileList.SetLoadListMode(bool enabled)
    {
      _loadListModeObject = null;
      SetLoadListMode(enabled);
    }

    /// <summary>
    /// Sets the load list mode for the list
    /// </summary>
    /// <param name="enabled">Enabled value</param>
    protected virtual void SetLoadListMode(bool enabled)
    {
    }

    /// <summary>
    /// Class that allows setting of property values on 
    /// current business object
    /// without raising PropertyChanged events
    /// and checking user rights.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
#pragma warning disable CA1063 // Implement IDisposable Correctly
    protected class LoadListModeObject : IDisposable
#pragma warning restore CA1063 // Implement IDisposable Correctly
    {
      private readonly IMobileList _target;
      /// <summary>
      /// Create instance of type
      /// </summary>
      /// <param name="target">List object</param>
      public LoadListModeObject(IMobileList target)
      {
        _target = target;
        _target.SetLoadListMode(true);
      }

      /// <summary>
      /// Disposes the object.
      /// </summary>
#pragma warning disable CA1063 // Implement IDisposable Correctly
      public void Dispose()
#pragma warning restore CA1063 // Implement IDisposable Correctly
      {
        _target.SetLoadListMode(false);
        GC.SuppressFinalize(this);
      }
    }

    #endregion

    #region IMobileObject Members

    void IMobileObject.GetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      OnGetChildren(info, formatter);
    }

    void IMobileObject.GetState(SerializationInfo info)
    {
      OnGetState(info);
    }

    /// <summary>
    /// Override this method to get custom field values
    /// from the serialization stream.
    /// </summary>
    /// <param name="info">Serialization info.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnGetState(SerializationInfo info)
    {
      info.AddValue("Csla.Core.MobileList.AllowEdit", AllowEdit);
      info.AddValue("Csla.Core.MobileList.AllowNew", AllowNew);
      info.AddValue("Csla.Core.MobileList.AllowRemove", AllowRemove);
      info.AddValue("Csla.Core.MobileList.RaiseListChangedEvents", RaiseListChangedEvents);
#if (ANDROID || IOS) || NETFX_CORE
      info.AddValue("Csla.Core.MobileList._supportsChangeNotificationCore", SupportsChangeNotificationCore);
#endif
    }

    /// <summary>
    /// Override this method to get custom child object
    /// values from the serialization stream.
    /// </summary>
    /// <param name="info">Serialization info.</param>
    /// <param name="formatter">Reference to the SerializationFormatterFactory.GetFormatter().</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnGetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      if (!typeof(IMobileObject).IsAssignableFrom(typeof(T)))
        throw new InvalidOperationException(Resources.CannotSerializeCollectionsNotOfIMobileObject);

      List<int> references = new List<int>();
      for (int x = 0; x < this.Count; x++)
      {
        T child = this[x];
        if (child != null)
        {
          SerializationInfo childInfo = formatter.SerializeObject(child);
          references.Add(childInfo.ReferenceId);
        }
      }
      if (references.Count > 0)
        info.AddValue("$list", references);
    }

    void IMobileObject.SetState(SerializationInfo info)
    {
      OnSetState(info);
    }

    void IMobileObject.SetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      OnSetChildren(info, formatter);
    }

    /// <summary>
    /// Override this method to set custom field values
    /// into the serialization stream.
    /// </summary>
    /// <param name="info">Serialization info.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnSetState(SerializationInfo info)
    {
      AllowEdit = info.GetValue<bool>("Csla.Core.MobileList.AllowEdit");
      AllowNew = info.GetValue<bool>("Csla.Core.MobileList.AllowNew");
      AllowRemove = info.GetValue<bool>("Csla.Core.MobileList.AllowRemove");
      RaiseListChangedEvents = info.GetValue<bool>("Csla.Core.MobileList.RaiseListChangedEvents");
    }

    /// <summary>
    /// Override this method to set custom child object
    /// values into the serialization stream.
    /// </summary>
    /// <param name="info">Serialization info.</param>
    /// <param name="formatter">Reference to the SerializationFormatterFactory.GetFormatter().</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnSetChildren(SerializationInfo info, MobileFormatter formatter)
    {
      if (!typeof(IMobileObject).IsAssignableFrom(typeof(T)))
        throw new InvalidOperationException(Resources.CannotSerializeCollectionsNotOfIMobileObject);

      bool originalRaiseListChangedEvents = this.RaiseListChangedEvents;

      try
      {
        this.RaiseListChangedEvents = false;

        if (info.Values.ContainsKey("$list"))
        {
          List<int> references = (List<int>)info.Values["$list"].Value;
          foreach (int reference in references)
          {
            T child = (T)formatter.GetObject(reference);
            this.Add(child);
          }
        }
      }
      finally
      {
          this.RaiseListChangedEvents = originalRaiseListChangedEvents;
      }
    }

    #endregion
  }
}