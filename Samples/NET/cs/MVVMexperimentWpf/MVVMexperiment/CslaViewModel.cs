using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace MVVMexperiment
{
  public abstract class CslaViewModel<T> : System.Windows.DependencyObject,
    System.ComponentModel.INotifyPropertyChanged
  {


    public T Model
    {
      get { return (T)GetValue(ModelProperty); }
      set { SetValue(ModelProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Model.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ModelProperty =
        DependencyProperty.Register("Model", typeof(T), typeof(CslaViewModel<T>), new UIPropertyMetadata(null));

    private bool _manageObjectLifetime;

    public bool ManageObjectLifetime 
    {
      get
      {
        return _manageObjectLifetime;
      }
      set
      {
        _manageObjectLifetime = value;
        OnPropertyChanged("ManageObjectLifetime");
      }
    }

    #region Can___ properties

    private bool _canSave;
    private bool _canCancel;

    public bool CanSave { get { return _canSave; } }
    public bool CanCancel { get { return _canCancel; } }
    public bool CanAddNew { get { return Model != null && Model is System.ComponentModel.IBindingList; } }
    public bool CanRemove { get { return Model != null && Model is System.Collections.IList; } }
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

    public void Save()
    {
      Csla.Core.ISupportUndo undo;
      if (this.ManageObjectLifetime)
      {
        undo = Model as Csla.Core.ISupportUndo;
        if (undo != null)
          undo.ApplyEdit();
      }

      var result = (T)((Csla.Core.ISavable)Model).Save();

      if (this.ManageObjectLifetime)
      {
        undo = result as Csla.Core.ISupportUndo;
        if (undo != null)
          undo.BeginEdit();
      }
      Model = result;
    }

    public void Cancel()
    {
      if (this.ManageObjectLifetime)
      {
        var undo = Model as Csla.Core.ISupportUndo;
        if (undo != null)
          undo.CancelEdit();
      }
    }

    public void AddNew()
    {
      ((System.ComponentModel.IBindingList)Model).AddNew();
    }

    public void Remove(Data item)
    {
      ((System.Collections.IList)Model).Remove(item);
    }

    public void Delete()
    {
      ((Csla.Core.IEditableBusinessObject)Model).Delete();
    }

    #endregion

    #region INotifyPropertyChanged Members

    public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
    }

    #endregion
  }
}
