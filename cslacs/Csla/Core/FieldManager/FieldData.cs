using System;

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
    public T Value
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
    public bool IsSelfDirty
    {
      get { return IsDirty; }
    }

    /// <summary>
    /// Gets a value indicating whether the field
    /// has been changed.
    /// </summary>
    public bool IsDirty
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
    public void MarkClean()
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
  }
}
