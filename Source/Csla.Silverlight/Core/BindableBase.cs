//-----------------------------------------------------------------------
// <copyright file="BindableBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>This class implements INotifyPropertyChanged.</summary>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel;
using Csla.Serialization;

namespace Csla.Core
{
  /// <summary>
  /// This class implements INotifyPropertyChanged.
  /// </summary>
  [Serializable]
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

    /// <summary>
    /// Event raised when a property is changed.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Event raised when a property is changing.
    /// </summary>
    public event PropertyChangingEventHandler PropertyChanging;

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

    /// <summary>
    /// Call this method to raise the PropertyChanged event
    /// for a specific property.
    /// </summary>
    /// <param name="propertyName">Name of the property that
    /// is being changed.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }


    /// <summary>
    /// Call this method to raise the PropertyChanged event
    /// for a specific property.
    /// </summary>
    /// <param name="propertyInfo">The property info for the property that has changed.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnPropertyChanged(IPropertyInfo propertyInfo)
    {
      OnPropertyChanged(propertyInfo.Name);
    }

    /// <summary>
    /// Raises the PropertyChanging event.
    /// </summary>
    /// <param name="propertyName">Name of the property that
    /// is being changed.</param>
    /// <remarks>
    /// This method may be called by properties in the business
    /// class to indicate the change that is about to occur 
    /// in a specific property.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnPropertyChanging(string propertyName)
    {
      if (PropertyChanging != null)
        PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
    }
    /// <summary>
    /// Raises the PropertyChanging event.
    /// </summary>
    /// <param name="propertyInfo">The property info for the property that has changed.</param>
    /// <remarks>
    /// This method may be called by properties in the business
    /// class to indicate the change that is about to occur 
    /// in a specific property.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnPropertyChanging(IPropertyInfo propertyInfo)
    {
      OnPropertyChanging(propertyInfo.Name);
    }
  }
}