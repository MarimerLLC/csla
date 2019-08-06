//-----------------------------------------------------------------------
// <copyright file="FieldData.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Contains a field value and related metadata.</summary>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Csla.Core.FieldManager
{
  /// <summary>
  /// Contains a field value and related metadata.
  /// </summary>
  /// <typeparam name="T">Type of field value contained.</typeparam>
  [Serializable()]
  public class FieldData<T> : IFieldData<T>
  {
    private string _name;
    private T _data;
    private bool _isDirty;

    /// <summary>
    /// Creates a new instance of the object.
    /// </summary>
    /// <param name="name">
    /// Name of the field.
    /// </param>
    public FieldData(string name)
    {
      _name = name;
    }

    /// <summary>
    /// Gets the name of the field.
    /// </summary>
    public string Name
    {
      get
      {
        return _name;
      }
    }

    /// <summary>
    /// Gets or sets the value of the field.
    /// </summary>
    public virtual T Value
    {
      get
      {
        return _data;
      }
      set
      {
        _data = value;
        _isDirty = true;
      }
    }

    object IFieldData.Value
    {
      get
      {
        return this.Value;
      }
      set
      {
        if (value == null)
          this.Value = default(T);
        else
          this.Value = (T)value;
      }
    }

    bool ITrackStatus.IsDeleted
    {
      get
      {
        ITrackStatus child = _data as ITrackStatus;
        if (child != null)
        {
          return child.IsDeleted;

        }
        else
        {
          return false;
        }
      }
    }

    bool ITrackStatus.IsSavable
    {
      get { return true; }
    }

    bool ITrackStatus.IsChild
    {
      get
      {
        ITrackStatus child = _data as ITrackStatus;
        if (child != null)
        {
          return child.IsChild;
        }
        else
        {
          return false;
        }
      }
    }

    /// <summary>
    /// Gets a value indicating whether the field
    /// has been changed.
    /// </summary>
    public virtual bool IsSelfDirty
    {
      get { return IsDirty; }
    }

    /// <summary>
    /// Gets a value indicating whether the field
    /// has been changed.
    /// </summary>
    public virtual bool IsDirty
    {
      get
      {
        ITrackStatus child = _data as ITrackStatus;
        if (child != null)
        {
          return child.IsDirty;
        }
        else
        {
          return _isDirty;
        }
      }
    }

    /// <summary>
    /// Marks the field as unchanged.
    /// </summary>
    public virtual void MarkClean()
    {
      _isDirty = false;
    }

    bool ITrackStatus.IsNew
    {
      get
      {
        ITrackStatus child = _data as ITrackStatus;
        if (child != null)
        {
          return child.IsNew;
        }
        else
        {
          return false;
        }
      }
    }

    bool ITrackStatus.IsSelfValid
    {
      get { return IsValid; }
    }

    bool ITrackStatus.IsValid
    {
      get { return IsValid; }
    }

    /// <summary>
    /// Gets a value indicating whether this field
    /// is considered valid.
    /// </summary>
    protected virtual bool IsValid
    {
      get
      {
        ITrackStatus child = _data as ITrackStatus;
        if (child != null)
        {
          return child.IsValid;
        }
        else
        {
          return true;
        }
      }
    }

    #region INotifyBusy Members

    event BusyChangedEventHandler INotifyBusy.BusyChanged
    {
      add { throw new NotImplementedException(); }
      remove { throw new NotImplementedException(); }
    }

    /// <summary>
    /// Gets a value indicating whether this object or
    /// any of its child objects are busy.
    /// </summary>
    [Browsable(false)]
    [Display(AutoGenerateField = false)]
#if !PCL46 && !PCL259 
    [System.ComponentModel.DataAnnotations.ScaffoldColumn(false)]
#endif
    public bool IsBusy
    {
      get
      {
        bool isBusy = false;
        ITrackStatus child = _data as ITrackStatus;
        if (child != null)
          isBusy = child.IsBusy;

        return isBusy;
      }
    }

    bool INotifyBusy.IsSelfBusy
    {
      get { return IsBusy; }
    }

    #endregion

    #region INotifyUnhandledAsyncException Members

    [NotUndoable]
    [NonSerialized]
    private EventHandler<ErrorEventArgs> _unhandledAsyncException;

    /// <summary>
    /// Event indicating that an exception occurred on
    /// a background thread.
    /// </summary>
    public event EventHandler<ErrorEventArgs> UnhandledAsyncException
    {
      add { _unhandledAsyncException = (EventHandler<ErrorEventArgs>)Delegate.Combine(_unhandledAsyncException, value); }
      remove { _unhandledAsyncException = (EventHandler<ErrorEventArgs>)Delegate.Remove(_unhandledAsyncException, value); }
    }

    /// <summary>
    /// Raises the UnhandledAsyncException event.
    /// </summary>
    /// <param name="error">Exception that occurred on the background thread.</param>
    protected virtual void OnUnhandledAsyncException(ErrorEventArgs error)
    {
      if (_unhandledAsyncException != null)
        _unhandledAsyncException(this, error);
    }

    /// <summary>
    /// Raises the UnhandledAsyncException event.
    /// </summary>
    /// <param name="originalSender">Original source of the event.</param>
    /// <param name="error">Exception that occurred on the background thread.</param>
    protected void OnUnhandledAsyncException(object originalSender, Exception error)
    {
      OnUnhandledAsyncException(new ErrorEventArgs(originalSender, error));
    }

    #endregion
  }
}