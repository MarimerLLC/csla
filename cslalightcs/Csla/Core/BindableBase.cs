using System;
using System.ComponentModel;
using Csla.Serialization;

namespace Csla.Core
{
  /// <summary>
  /// This class implements INotifyPropertyChanged.
  /// </summary>
  [Serializable]
  public abstract class BindableBase : MobileObject, System.ComponentModel.INotifyPropertyChanged
  {
    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    protected BindableBase()
    { }

    public event PropertyChangedEventHandler PropertyChanged;

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
    /// has changed.</param>
    /// <remarks>
    /// This method may be called by properties in the business
    /// class to indicate the change in a specific property.
    /// </remarks>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}