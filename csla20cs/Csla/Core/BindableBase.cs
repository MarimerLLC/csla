using System;
using System.ComponentModel;
using System.Reflection;

namespace Csla.Core
{
  /// <summary>
  /// This base class declares the IsDirtyChanged event
  /// to be NonSerialized so serialization will work.
  /// </summary>
  [Serializable()]
  public abstract class BindableBase : System.ComponentModel.INotifyPropertyChanged
  {
    [NonSerialized()]
    private PropertyChangedEventHandler _nonSerializableHandlers;
    private PropertyChangedEventHandler _serializableHandlers;

    protected BindableBase()
    {

    }

    /// <summary>
    /// Implements a serialization-safe IsDirtyChanged event.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", 
      "CA1062:ValidateArgumentsOfPublicMethods")]
    public event PropertyChangedEventHandler PropertyChanged
    {
      add
      {
        if (value.Method.IsPublic && (value.Method.DeclaringType.IsSerializable || value.Method.IsStatic))
          _serializableHandlers = (PropertyChangedEventHandler)
            System.Delegate.Combine(_serializableHandlers, value);
        else
          _nonSerializableHandlers = (PropertyChangedEventHandler)
            System.Delegate.Combine(_nonSerializableHandlers, value);
      }
      remove
      {
        if (value.Method.IsPublic && (value.Method.DeclaringType.IsSerializable || value.Method.IsStatic))
          _serializableHandlers = (PropertyChangedEventHandler)
            System.Delegate.Remove(_serializableHandlers, value);
        else
          _nonSerializableHandlers = (PropertyChangedEventHandler)
            System.Delegate.Remove(_nonSerializableHandlers, value);
      }
    }

    /// <summary>
    /// Call this method to raise the PropertyChanged event
    /// for the IsDirty property.
    /// </summary>
    /// <remarks>
    /// This method is automatically called by MarkDirty
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnIsDirtyChanged()
    {
      PropertyInfo [] properties = 
        this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
      foreach (PropertyInfo item in properties)
        OnPropertyChanged(item.Name);
    }

    /// <summary>
    /// Call this method to raise the PropertyChanged event
    /// for a specific property.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    /// <remarks>
    /// This method may be called by properties in the business
    /// class to indicate the change in a specific property.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (_nonSerializableHandlers != null)
        _nonSerializableHandlers.Invoke(this, new PropertyChangedEventArgs(propertyName));
      if (_serializableHandlers != null)
        _serializableHandlers.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}