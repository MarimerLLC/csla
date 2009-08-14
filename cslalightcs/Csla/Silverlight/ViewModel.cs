using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Csla.Silverlight
{
  /// <summary>
  /// Base class used to simplify the creation of view model
  /// objects for use with CSLA .NET business object models.
  /// </summary>
  /// <typeparam name="T">Type of the Model object.</typeparam>
  public abstract class CslaViewModel<T> : System.Windows.FrameworkElement,
    System.ComponentModel.INotifyPropertyChanged
  {
    /// <summary>
    /// Gets or sets the Model object.
    /// </summary>
    public static readonly DependencyProperty ModelProperty =
        DependencyProperty.Register("Model", typeof(T), typeof(CslaViewModel<T>), new PropertyMetadata(null));
    /// <summary>
    /// Gets or sets the Model object.
    /// </summary>
    public T Model
    {
      get { return (T)GetValue(ModelProperty); }
      set { SetValue(ModelProperty, value); }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the
    /// ViewModel should automatically managed the
    /// lifetime of the Model.
    /// </summary>
    public static readonly DependencyProperty ManageObjectLifetimeProperty =
        DependencyProperty.Register("ManageObjectLifetime", typeof(bool), typeof(CslaViewModel<T>), new PropertyMetadata(true));
    /// <summary>
    /// Gets or sets a value indicating whether the
    /// ViewManageObjectLifetime should automatically managed the
    /// lifetime of the ManageObjectLifetime.
    /// </summary>
    public bool ManageObjectLifetime
    {
      get { return (bool)GetValue(ManageObjectLifetimeProperty); }
      set { SetValue(ManageObjectLifetimeProperty, value); }
    }

    #region Can___ properties

    private bool _canSave;
    private bool _canCancel;

    /// <summary>
    /// Gets a value indicating whether the Model can be saved.
    /// </summary>
    public bool CanSave { get { return _canSave; } }
    /// <summary>
    /// Gets a value indicating whether the Model can be canceled.
    /// </summary>
    public bool CanCancel { get { return _canCancel; } }
    /// <summary>
    /// Gets a value indicating whether a new item can be
    /// added to the Model (if it is a collection).
    /// </summary>
    public bool CanAddNew { get { return Model != null && Model is Csla.Core.IBindingList; } }
    /// <summary>
    /// Gets a value indicating whether an item can be
    /// removed from the Model (if it is a collection).
    /// </summary>
    public bool CanRemove { get { return Model != null && Model is System.Collections.IList; } }
    /// <summary>
    /// Gets a value indicating whether the Model can be
    /// marked for deletion (if it is an editable root object).
    /// </summary>
    public bool CanDelete { get { return Model != null && Model is Csla.Core.IEditableBusinessObject; } }

    private void SetProperties()
    {
      bool value;
      value = GetCanSave();
      if (_canSave != value)
      {
        _canSave = value;
        OnPropertyChanged("CanSave");
      }
      value = GetCanCancel();
      if (_canCancel != value)
      {
        _canCancel = value;
        OnPropertyChanged("CanCancel");
      }
    }

    private bool GetCanSave()
    {
      if (Model == null) return false;
      var track = Model as Csla.Core.ITrackStatus;
      if (track != null)
        return track.IsSavable;
      return false;
    }

    private bool GetCanCancel()
    {
      if (!this.ManageObjectLifetime) return false;
      if (Model == null) return false;
      var undo = Model as Csla.Core.ISupportUndo;
      if (undo == null)
        return false;
      var track = Model as Csla.Core.ITrackStatus;
      if (track != null)
        return track.IsDirty;
      return false;
    }

    #endregion

    #region Verbs

    /// <summary>
    /// Saves the Model, first committing changes
    /// if ManagedObjectLifetime is true.
    /// </summary>
    public void Save()
    {
      Csla.Core.ISupportUndo undo;
      if (this.ManageObjectLifetime)
      {
        undo = Model as Csla.Core.ISupportUndo;
        if (undo != null)
          undo.ApplyEdit();
      }

      var savable = (Csla.Core.ISavable)Model;
      savable.Saved += (o, e) =>
      {
        var result = e.NewObject;
        if (this.ManageObjectLifetime)
        {
          undo = result as Csla.Core.ISupportUndo;
          if (undo != null)
            undo.BeginEdit();
        }
        Model = (T)result;
      };
      savable.BeginSave();
    }

    /// <summary>
    /// Cancels changes made to the model 
    /// if ManagedObjectLifetime is true.
    /// </summary>
    public void Cancel()
    {
      if (this.ManageObjectLifetime)
      {
        var undo = Model as Csla.Core.ISupportUndo;
        if (undo != null)
          undo.CancelEdit();
      }
    }

    /// <summary>
    /// Adds a new item to the Model (if it
    /// is a collection).
    /// </summary>
    public void AddNew()
    {
      ((Csla.Core.IBindingList)Model).AddNew();
    }

    /// <summary>
    /// Removes an item from the Model (if it
    /// is a collection).
    /// </summary>
    public void Remove(T item)
    {
      ((System.Collections.IList)Model).Remove(item);
    }

    /// <summary>
    /// Marks the Model for deletion (if it is an
    /// editable root object).
    /// </summary>
    public void Delete()
    {
      ((Csla.Core.IEditableBusinessObject)Model).Delete();
    }

    #endregion

    #region INotifyPropertyChanged Members

    /// <summary>
    /// Event raised when a property changes.
    /// </summary>
    public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raise the PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">Name of the changed property.</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
    }

    #endregion
  }
}
