//-----------------------------------------------------------------------
// <copyright file="MobileBindingList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Inherit from this base class to easily</summary>
//-----------------------------------------------------------------------

using System.ComponentModel;
using Csla.Serialization.Mobile;
using Csla.Properties;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;

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
  [DebuggerStepThrough]
#endif
  [Serializable]
  public class MobileBindingList<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T> : BindingList<T>, IMobileList, IMobileObjectMetastate
  {
    #region LoadListMode

    [NonSerialized]
    [NotUndoable]
    private LoadListModeObject? _loadListModeObject = null;

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
      if (_oldRLCE == null)
        _oldRLCE = new Stack<bool>();
      if (enabled)
      {
        _oldRLCE.Push(RaiseListChangedEvents);
        RaiseListChangedEvents = false;
      }
      else
      {
        if (_oldRLCE.Count > 0)
          RaiseListChangedEvents = _oldRLCE.Pop();
      }
    }

    [NonSerialized]
    [NotUndoable]
    private Stack<bool>? _oldRLCE;

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
      /// <exception cref="ArgumentNullException"><paramref name="target"/> is <see langword="null"/>.</exception>
      public LoadListModeObject(IMobileList target)
      {
        _target = target ?? throw new ArgumentNullException(nameof(target));
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
#if (ANDROID || IOS)
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
      for (int x = 0; x < Count; x++)
      {
        T? child = this[x];
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

      bool originalRaiseListChangedEvents = RaiseListChangedEvents;

      try
      {
        RaiseListChangedEvents = false;

        if (info.Values.TryGetValue("$list", out var value))
        {
          List<int> references = (List<int>)value.Value!;
          foreach (int reference in references)
          {
            T child = (T)formatter.GetObject(reference)!;
            if (child is IBusinessBase bb)
            {
              var editLevelAdded = bb.EditLevelAdded;
              Add(child);
              bb.EditLevelAdded = editLevelAdded;
            }
            else
            {
              Add(child);
            }
          }
        }
      }
      finally
      {
          RaiseListChangedEvents = originalRaiseListChangedEvents;
      }
    }

    #endregion

    #region IMobileObjectMetastate Members

    /// <inheritdoc />
    byte[] IMobileObjectMetastate.GetMetastate()
    {
      using var stream = new MemoryStream();
      using var writer = new BinaryWriter(stream);
      OnGetMetastate(writer);
      return stream.ToArray();
    }

    /// <inheritdoc />
    void IMobileObjectMetastate.SetMetastate(byte[] metastate)
    {
      if (metastate == null)
        throw new ArgumentNullException(nameof(metastate));
      if (metastate.Length == 0)
        throw new ArgumentException("Metastate cannot be empty.", nameof(metastate));

      using var stream = new MemoryStream(metastate);
      using var reader = new BinaryReader(stream);
      OnSetMetastate(reader);
    }

    /// <summary>
    /// Override this method to write field values directly
    /// to a binary stream for metastate serialization.
    /// </summary>
    /// <param name="writer">Binary writer for the output stream.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnGetMetastate(BinaryWriter writer)
    {
      writer.Write(AllowEdit);
      writer.Write(AllowNew);
      writer.Write(AllowRemove);
      writer.Write(RaiseListChangedEvents);
#if (ANDROID || IOS)
      writer.Write(SupportsChangeNotificationCore);
#endif
    }

    /// <summary>
    /// Override this method to read field values directly
    /// from a binary stream for metastate deserialization.
    /// </summary>
    /// <param name="reader">Binary reader for the input stream.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnSetMetastate(BinaryReader reader)
    {
      AllowEdit = reader.ReadBoolean();
      AllowNew = reader.ReadBoolean();
      AllowRemove = reader.ReadBoolean();
      RaiseListChangedEvents = reader.ReadBoolean();
#if (ANDROID || IOS)
      // Read and discard - SupportsChangeNotificationCore is read-only in BindingList<T>
      _ = reader.ReadBoolean();
#endif
    }

    #endregion
  }
}