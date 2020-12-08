//-----------------------------------------------------------------------
// <copyright file="BindableBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>This class implements INotifyPropertyChanged</summary>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel;

namespace Csla.Core
{
  /// <summary>
  /// This class implements INotifyPropertyChanged
  /// and INotifyPropertyChanging in a 
  /// serialization-safe manner.
  /// </summary>
  [Serializable()]
  public abstract class BindableBase : 
    MobileObject, 
    INotifyPropertyChanged, 
    INotifyPropertyChanging
  {
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    protected BindableBase()
    { }

    [NonSerialized()]
    private PropertyChangedEventHandler _nonSerializableChangedHandlers;
    private PropertyChangedEventHandler _serializableChangedHandlers;

    /// <summary>
    /// Implements a serialization-safe PropertyChanged event.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", 
      "CA1062:ValidateArgumentsOfPublicMethods")]
    public event PropertyChangedEventHandler PropertyChanged
    {
      add
      {
        if (ShouldHandlerSerialize(value))
          _serializableChangedHandlers = (PropertyChangedEventHandler)
            System.Delegate.Combine(_serializableChangedHandlers, value);
        else
          _nonSerializableChangedHandlers = (PropertyChangedEventHandler)
            System.Delegate.Combine(_nonSerializableChangedHandlers, value);
      }
      remove
      {
          if (ShouldHandlerSerialize(value))
          _serializableChangedHandlers = (PropertyChangedEventHandler)
            System.Delegate.Remove(_serializableChangedHandlers, value);
        else
          _nonSerializableChangedHandlers = (PropertyChangedEventHandler)
            System.Delegate.Remove(_nonSerializableChangedHandlers, value);
      }
    }

    /// <summary>
    /// Override this method to change the default logic for determining 
    /// if the event handler should be serialized
    /// </summary>
    /// <param name="value">the event handler to review</param>
    /// <returns></returns>
    protected virtual bool ShouldHandlerSerialize(PropertyChangedEventHandler value)
    {
      return value.Method.IsPublic &&
             value.Method.DeclaringType != null &&
             (value.Method.DeclaringType.IsSerializable || value.Method.IsStatic);
    }

    /// <summary>
    /// Call this method to raise the PropertyChanged event
    /// for a specific property.
    /// </summary>
    /// <param name="propertyName">Name of the property that
    /// has changed.</param>
    /// <remarks>
    /// This method may be called by properties in the business
    /// class to indicate the change in a specific property.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (_nonSerializableChangedHandlers != null)
        _nonSerializableChangedHandlers.Invoke(this,
          new PropertyChangedEventArgs(propertyName));
      if (_serializableChangedHandlers != null)
        _serializableChangedHandlers.Invoke(this,
          new PropertyChangedEventArgs(propertyName));
    }

        /// <summary>
    /// Call this method to raise the PropertyChanged event
    /// for a MetaData (IsXYZ) property
    /// </summary>
    /// <param name="propertyName">Name of the property that
    /// has changed.</param>
    /// <remarks>
    /// This method may be called by properties in the business
    /// class to indicate the change in a specific property.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMetaPropertyChanged(string propertyName)
    {
      if (_nonSerializableChangedHandlers != null)
        _nonSerializableChangedHandlers.Invoke(this,
          new MetaPropertyChangedEventArgs(propertyName));
      if (_serializableChangedHandlers != null)
        _serializableChangedHandlers.Invoke(this,
          new MetaPropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Call this method to raise the PropertyChanged event
    /// for a specific property.
    /// </summary>
    /// <param name="propertyInfo">PropertyInfo of the property that
    /// has changed.</param>
    /// <remarks>
    /// This method may be called by properties in the business
    /// class to indicate the change in a specific property.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnPropertyChanged(IPropertyInfo propertyInfo)
    {
      OnPropertyChanged(propertyInfo.Name);
    }

    /// <summary>
    /// Call this method to raise the PropertyChanged event
    /// for all object properties.
    /// </summary>
    /// <remarks>
    /// This method is for backward compatibility with
    /// CSLA .NET 1.x.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnIsDirtyChanged()
    {
      OnUnknownPropertyChanged();
    }

    /// <summary>
    /// Call this method to raise the PropertyChanged event
    /// for all object properties.
    /// </summary>
   /// <remarks>
    /// This method is automatically called by MarkDirty. It
    /// actually raises PropertyChanged for an empty string,
    /// which tells data binding to refresh all properties.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnUnknownPropertyChanged()
    {
      OnPropertyChanged(string.Empty);
    }

    [NonSerialized()]
    private PropertyChangingEventHandler _nonSerializableChangingHandlers;
    private PropertyChangingEventHandler _serializableChangingHandlers;

    /// <summary>
    /// Implements a serialization-safe PropertyChanging event.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design",
      "CA1062:ValidateArgumentsOfPublicMethods")]
    public event PropertyChangingEventHandler PropertyChanging
    {
      add
      {
          if (ShouldHandlerSerialize(value))
          _serializableChangingHandlers = (PropertyChangingEventHandler)
            System.Delegate.Combine(_serializableChangingHandlers, value);
        else
          _nonSerializableChangingHandlers = (PropertyChangingEventHandler)
            System.Delegate.Combine(_nonSerializableChangingHandlers, value);
      }
      remove
      {
          if (ShouldHandlerSerialize(value))
          _serializableChangingHandlers = (PropertyChangingEventHandler)
            System.Delegate.Remove(_serializableChangingHandlers, value);
        else
          _nonSerializableChangingHandlers = (PropertyChangingEventHandler)
            System.Delegate.Remove(_nonSerializableChangingHandlers, value);
      }
    }

    /// <summary>
    /// Call this method to raise the PropertyChanging event
    /// for all object properties.
    /// </summary>
    /// <remarks>
    /// This method is for backward compatibility with
    /// CSLA .NET 1.x.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnIsDirtyChanging()
    {
      OnUnknownPropertyChanging();
    }

    /// <summary>
    /// Call this method to raise the PropertyChanging event
    /// for all object properties.
    /// </summary>
    /// <remarks>
    /// This method is automatically called by MarkDirty. It
    /// actually raises PropertyChanging for an empty string,
    /// which tells data binding to refresh all properties.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnUnknownPropertyChanging()
    {
      OnPropertyChanging(string.Empty);
    }

    /// <summary>
    /// Call this method to raise the PropertyChanging event
    /// for a specific property.
    /// </summary>
    /// <param name="propertyName">Name of the property that
    /// has Changing.</param>
    /// <remarks>
    /// This method may be called by properties in the business
    /// class to indicate the change in a specific property.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnPropertyChanging(string propertyName)
    {
      if (_nonSerializableChangingHandlers != null)
        _nonSerializableChangingHandlers.Invoke(this,
          new PropertyChangingEventArgs(propertyName));
      if (_serializableChangingHandlers != null)
        _serializableChangingHandlers.Invoke(this,
          new PropertyChangingEventArgs(propertyName));
    }

    /// <summary>
    /// Call this method to raise the PropertyChanging event
    /// for a specific property.
    /// </summary>
    /// <param name="propertyInfo">PropertyInfo of the property that
    /// has Changing.</param>
    /// <remarks>
    /// This method may be called by properties in the business
    /// class to indicate the change in a specific property.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnPropertyChanging(IPropertyInfo propertyInfo)
    {
      OnPropertyChanging(propertyInfo.Name);
    }

    /// <summary>
    /// Override this method to change the default logic for determining 
    /// if the event handler should be serialized
    /// </summary>
    /// <param name="value">the event handler to review</param>
    /// <returns></returns>
    protected virtual bool ShouldHandlerSerialize(PropertyChangingEventHandler value)
    {
      return value.Method.IsPublic &&
             value.Method.DeclaringType != null &&
             (value.Method.DeclaringType.IsSerializable || value.Method.IsStatic);
    }
  }
}