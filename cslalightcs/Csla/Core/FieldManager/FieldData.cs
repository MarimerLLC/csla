using System;
using Csla.Serialization;

namespace Csla.Core.FieldManager
{
  /// <summary>
  /// Contains a field value and related metadata.
  /// </summary>
  /// <typeparam name="T">Type of field value contained.</typeparam>
  [Serializable]
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

    private bool IsValid
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

    bool ITrackStatus.IsSavable
    {
      get { return true; }
    }

    #region INotifyBusy Members

    event BusyChangedEventHandler INotifyBusy.BusyChanged
    {
      add { throw new NotImplementedException(); }
      remove { throw new NotImplementedException(); }
    }

    /// <summary>
    /// Gets a value indicating whether this object
    /// or any child object is executing an async
    /// operation.
    /// </summary>
    public bool IsBusy
    {
      get
      {
        bool isbusy = false;
        ITrackStatus child = _data as ITrackStatus;
        if (child != null)
          isbusy = child.IsBusy;
        return isbusy;
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
    /// Event raised when an unhandled exception occurs
    /// during async processing.
    /// </summary>
    public event EventHandler<ErrorEventArgs> UnhandledAsyncException
    {
      add { _unhandledAsyncException = (EventHandler<ErrorEventArgs>)Delegate.Combine(_unhandledAsyncException, value); }
      remove { _unhandledAsyncException = (EventHandler<ErrorEventArgs>)Delegate.Remove(_unhandledAsyncException, value); }
    }

    /// <summary>
    /// Raise the UnhandledAsyncException event.
    /// </summary>
    /// <param name="error">Event args</param>
    protected virtual void OnUnhandledAsyncException(ErrorEventArgs error)
    {
      if (_unhandledAsyncException != null)
        _unhandledAsyncException(this, error);
    }

    /// <summary>
    /// Raise the UnhandledAsyncException event.
    /// </summary>
    /// <param name="originalSender">Object that threw the exception</param>
    /// <param name="error">Exception object</param>
    protected void OnUnhandledAsyncException(object originalSender, Exception error)
    {
      OnUnhandledAsyncException(new ErrorEventArgs(originalSender, error));
    }

    #endregion
  }
}
