//-----------------------------------------------------------------------
// <copyright file="CslaDataProvider.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Creates, retrieves and manages business objects</summary>
//-----------------------------------------------------------------------
using System;
using System.Windows;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Csla.Properties;
using System.Reflection;
using Csla.Reflection;
using System.Collections;
using System.Collections.Generic;
using Csla.Core;

namespace Csla.Xaml
{
  /// <summary>
  /// Creates, retrieves and manages business objects
  /// from XAML in a form.
  /// </summary>
  public class CslaDataProvider : FrameworkElement, 
    INotifyPropertyChanged
  {
    #region Events

    /// <summary>
    /// Event raised when the data object has
    /// changed to another data object, or when
    /// an exception has occurred during processing.
    /// </summary>
    public event EventHandler DataChanged;

    /// <summary>
    /// Raises the DataChanged event.
    /// </summary>
    protected void OnDataChanged()
    {
      RefreshCanOperationsValues();
      RefreshCanOperationsOnObjectLevel();
      if (DataChanged != null)
        DataChanged(this, EventArgs.Empty);
    }

    /// <summary>
    /// Event raised when a Save operation
    /// is complete.
    /// </summary>
    public event EventHandler<Csla.Core.SavedEventArgs> Saved;

    /// <summary>
    /// Raises the Saved event.
    /// </summary>
    /// <param name="newObject">Reference to new
    /// object resulting from the save.</param>
    /// <param name="error">Reference to any exception
    /// object that may have resulted from the operation.</param>
    /// <param name="userState">Reference to any user state
    /// object provided by the caller.</param>
    protected virtual void OnSaved(object newObject, Exception error, object userState)
    {
      if (Saved != null)
        Saved(this, new Csla.Core.SavedEventArgs(newObject, error, userState));
    }

    private void dataObject_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      RefreshCanOperationsValues();
    }

    private void dataObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      RefreshCanOperationsValues();
    }

    private void dataObject_ChildChanged(object sender, ChildChangedEventArgs e)
    {
      RefreshCanOperationsValues();
    }

    #endregion

    #region IsBusy

    private bool _isBusy = false;

    /// <summary>
    /// Gets a value indicating whether this object is
    /// executing an asynchronous process.
    /// </summary>
    public bool IsBusy
    {
      get { return _isBusy; }
      protected set
      {
        _isBusy = value;
        OnPropertyChanged("IsBusy");
        OnPropertyChanged("IsNotBusy");
        RefreshCanOperationsValues();
      }
    }

    /// <summary>
    /// Gets a value indicating whether this object is
    /// not executing an asynchronous process.
    /// </summary>
    public bool IsNotBusy
    {
      get
      { return !IsBusy; }
    }

    void CslaDataProvider_BusyChanged(object sender, BusyChangedEventArgs e)
    {
      // only set busy state for entire object.  Ignore busy state based
      // on asynch rules being active
      if (e.PropertyName == string.Empty)
        IsBusy = e.Busy;
      else
        RefreshCanOperationsValues();
    }

    #endregion

    #region ObjectInstance/Data Properties

    private bool _runningQuery = false;

    /// <summary>
    /// Gets or sets a reference to the data
    /// object.
    /// </summary>
    public static readonly DependencyProperty ObjectInstanceProperty =
     DependencyProperty.Register("ObjectInstance", typeof(object),
     typeof(CslaDataProvider), new PropertyMetadata((o, e) => 
     {
       var caller = o as CslaDataProvider;
       if (caller != null)
       {
         caller.HookObjectEvents(e.OldValue, e.NewValue);
         caller.RaiseDataChangedEvents();
       }
     }));

    /// <summary>
    /// Gets or sets a reference to the data
    /// object.
    /// </summary>
    public object ObjectInstance
    {
      get { return this.GetValue(ObjectInstanceProperty); }
      set 
      {
        var oldValue = ObjectInstance;
        this.SetValue(ObjectInstanceProperty, value);
        if (ReferenceEquals(oldValue, value))
          RaiseDataChangedEvents();
      }
    }

    private void RaiseDataChangedEvents()
    {
      if (!_runningQuery)
        SetError(null);
      OnPropertyChanged("Data");
      OnDataChanged();
    }

    private void HookObjectEvents(object oldValue, object newValue)
    {
      // unhook events from old value
      if (oldValue != null)
      {
        var npc = oldValue as INotifyPropertyChanged;
        if (npc != null)
          npc.PropertyChanged -= dataObject_PropertyChanged;
        var nclc = oldValue as System.Collections.Specialized.INotifyCollectionChanged;
        if (nclc != null)
          nclc.CollectionChanged -= dataObject_CollectionChanged;
        var ncc = oldValue as INotifyChildChanged;
        if (ncc != null)
          ncc.ChildChanged -= dataObject_ChildChanged;
        var nb = oldValue as INotifyBusy;
        if (nb != null)
          nb.BusyChanged -= CslaDataProvider_BusyChanged;
      }

      // hook events on new value
      if (newValue != null)
      {
        if (_manageObjectLifetime)
        {
          var undoable = newValue as Csla.Core.ISupportUndo;
          if (undoable != null)
            undoable.BeginEdit();
        }

        var npc = newValue as INotifyPropertyChanged;
        if (npc != null)
          npc.PropertyChanged += dataObject_PropertyChanged;
        var nclc = newValue as System.Collections.Specialized.INotifyCollectionChanged;
        if (nclc != null)
          nclc.CollectionChanged += dataObject_CollectionChanged;
        var ncc = newValue as INotifyChildChanged;
        if (ncc != null)
          ncc.ChildChanged += dataObject_ChildChanged;
        var nb = newValue as INotifyBusy;
        if (nb != null)
          nb.BusyChanged += CslaDataProvider_BusyChanged;
      }
    }

    /// <summary>
    /// Gets a reference to the data object.
    /// </summary>
    public object Data
    {
      get
      {
        if (_isInitialLoadEnabled && !_isInitialLoadCompleted)
        {
          _isInitialLoadCompleted = true;
          Refresh();
        }
        return ObjectInstance;
      }
    }

    #endregion

    #region ManageObjectLifetime

    private bool _manageObjectLifetime = true;

    /// <summary>
    /// Gets or sets a value indicating whether
    /// the business object's lifetime should
    /// be managed automatically.
    /// </summary>
    public bool ManageObjectLifetime
    {
      get { return _manageObjectLifetime; }
      set
      {
        if (ObjectInstance != null)
          throw new NotSupportedException(Csla.Properties.Resources.ObjectNotNull);
        _manageObjectLifetime = value;
      }
    }

    #endregion

    #region IsInitialLoadEnabled

    private bool _isInitialLoadEnabled = false;
    private bool _isInitialLoadCompleted = false;

    /// <summary>
    /// Gets or sets a value indicating whether the
    /// data provider should load data as the page
    /// is loaded.
    /// </summary>
    public bool IsInitialLoadEnabled
    {
      get { return _isInitialLoadEnabled; }
      set
      {
        if (ObjectInstance != null)
          throw new NotSupportedException(Csla.Properties.Resources.ObjectNotNull);
        _isInitialLoadEnabled = value;
      }
    }

    #endregion

    #region ObjectType, FactoryMethod, Factory params

    private string _objectType;

    /// <summary>
    /// Gets or sets the assembly qualified type
    /// name for the business object to be
    /// created or retrieved.
    /// </summary>
    public string ObjectType
    {
      get
      {
        return _objectType;
      }
      set
      {
        _objectType = value;
        OnPropertyChanged("ObjectType");
      }
    }

    private string _factoryMethod;

    /// <summary>
    /// Gets or sets the name of the static factory
    /// method to be invoked to create or retrieve
    /// the business object.
    /// </summary>
    public string FactoryMethod
    {
      get
      {
        return _factoryMethod;
      }
      set
      {
        _factoryMethod = value;
        OnPropertyChanged("FactoryMethod");
      }
    }

    private ObservableCollection<object> _factoryParameters;

    /// <summary>
    /// Gets or sets a collection of parameter values
    /// that are passed to the factory method.
    /// </summary>
    public ObservableCollection<object> FactoryParameters
    {
      get
      {
        if (_factoryParameters == null)
          _factoryParameters = new ObservableCollection<object>();
        return _factoryParameters;
      }
      set
      {
        _factoryParameters =
          new ObservableCollection<object>();
        List<object> temp = new List<object>(value);
        foreach (object oneParameters in temp)
        {
          _factoryParameters.Add(oneParameters);
        }
        OnPropertyChanged("FactoryParameters");
      }
    }

    #endregion

    #region Error Property and Error handler

    private Exception _error;

    /// <summary>
    /// Gets a reference to the Exception object
    /// (if any) from the last data portal operation.
    /// </summary>
    public Exception Error
    {
      get { return _error; }
      private set
      {
        SetError(value);
        OnDataChanged();
      }
    }

    private void SetError(Exception value)
    {
      var changed = !ReferenceEquals(_error, value);
      if (changed)
        _error = value;
      IsBusy = false;
      if (changed)
        OnPropertyChanged("Error");
    }

    private object _dataChangedHandler;

    /// <summary>
    /// Gets or sets a reference to an object that
    /// will handle the DataChanged event raised
    /// by this data provider.
    /// </summary>
    /// <remarks>
    /// This property is designed to 
    /// reference an IErrorDialog control.
    /// </remarks>
    public object DataChangedHandler
    {
      get
      {
        return _dataChangedHandler;
      }
      set
      {
        _dataChangedHandler = value;
        var dialog = value as IErrorDialog;
        if (dialog != null)
          dialog.Register(this);
        OnPropertyChanged("DataChangedHandler");
      }
    }

    #endregion

    #region Refresh/Query

    /// <summary>
    /// Causes the data provider to execute the
    /// factory method, refreshing the business
    /// object by creating or retrieving a new
    /// instance.
    /// </summary>
    public void Refresh()
    {
      if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
      {
        if (_objectType != null && _factoryMethod != null)
          try
          {
            SetError(null);
            this.IsBusy = true;
            List<object> parameters = new List<object>(FactoryParameters);
            Type objectType = Csla.Reflection.MethodCaller.GetType(_objectType);
            parameters.Add(CreateHandler(objectType));

            MethodCaller.CallFactoryMethod(objectType, _factoryMethod, parameters.ToArray());
          }
          catch (Exception ex)
          {
            this.Error = ex;
          }
      }
    }

    private Delegate CreateHandler(Type objectType)
    {
      var args = typeof(DataPortalResult<>).MakeGenericType(objectType);
      System.Reflection.MethodInfo method = MethodCaller.GetNonPublicMethod(this.GetType(), "QueryCompleted");
      Delegate handler = Delegate.CreateDelegate(typeof(EventHandler<>).MakeGenericType(args), this, method);
      return handler;
    }


    private void QueryCompleted(object sender, EventArgs e)
    {
      IDataPortalResult eventArgs = e as IDataPortalResult;
      _runningQuery = true;
      SetError(eventArgs.Error);
      ObjectInstance = eventArgs.Object;
      RefreshCanOperationsValues();
      _runningQuery = false;
      this.IsBusy = false;
    }

    #endregion

    #region Operations

    /// <summary>
    /// Cancels any changes to the object.
    /// </summary>
    public void Cancel()
    {
      SetError(null);
      if (_manageObjectLifetime)
      {
        try
        {
          var undoable = ObjectInstance as Csla.Core.ISupportUndo;
          if (undoable != null)
          {
            IsBusy = true;
            ObjectInstance = null;
            undoable.CancelEdit();
            ObjectInstance = undoable;
            var trackable = ObjectInstance as ITrackStatus;
            if (trackable != null)
              IsBusy = trackable.IsBusy;
            else
              IsBusy = false;
          }
        }
        catch (Exception ex)
        {
          this.Error = ex;
        }
      }
    }

    /// <summary>
    /// Accepts any changes to the object and
    /// invokes the object's BeginSave() method.
    /// </summary>
    public void Save()
    {
      SetError(null);
      try
      {

        var obj = ObjectInstance as Csla.Core.ISavable;
        if (obj != null)
        {
          if (_manageObjectLifetime)
          {
            // clone the object if possible

            ICloneable clonable = ObjectInstance as ICloneable;
            if (clonable != null)
              obj = (Csla.Core.ISavable)clonable.Clone();

            var undoable = obj as Csla.Core.ISupportUndo;
            if (undoable != null)
              undoable.ApplyEdit();
          }


          obj.Saved -= new EventHandler<Csla.Core.SavedEventArgs>(obj_Saved);
          obj.Saved += new EventHandler<Csla.Core.SavedEventArgs>(obj_Saved);
          IsBusy = true;
          obj.BeginSave();
        }
      }
      catch (Exception ex)
      {
        IsBusy = false;
        this.Error = ex;
        OnSaved(ObjectInstance, ex, null);
      }
    }

    void obj_Saved(object sender, Csla.Core.SavedEventArgs e)
    {
      IsBusy = false;
      if (e.Error != null)
        Error = e.Error;
      else
        ObjectInstance = e.NewObject;

      OnSaved(e.NewObject, e.Error, e.UserState);
    }

    /// <summary>
    /// Marks an editable root object for deletion.
    /// </summary>
    public void Delete()
    {
      SetError(null);
      try
      {
        var obj = ObjectInstance as Csla.Core.BusinessBase;
        if (obj != null)
        {
          IsBusy = true;
          obj.Delete();
          var trackable = ObjectInstance as ITrackStatus;
          if (trackable != null)
            IsBusy = trackable.IsBusy;
          else
            IsBusy = false;
        }
      }
      catch (Exception ex)
      {
        this.Error = ex;
      }
    }

    /// <summary>
    /// Begins an async add new operation to add a 
    /// new item to an editable list business object.
    /// </summary>
    public void AddNewItem()
    {
      SetError(null);
      try
      {
        var obj = ObjectInstance as Csla.Core.IBindingList;
        if (obj != null)
        {
          IsBusy = true;
          obj.AddNew();
          var trackable = ObjectInstance as ITrackStatus;
          if (trackable != null)
            IsBusy = trackable.IsBusy;
          else
            IsBusy = false;
        }
        RefreshCanOperationsValues();
      }
      catch (Exception ex)
      {
        this.Error = ex;
      }
    }

    /// <summary>
    /// Removes an item from an editable list
    /// business object.
    /// </summary>
    /// <param name="sender">Object invoking this method.</param>
    /// <param name="e">
    /// ExecuteEventArgs, where MethodParameter contains 
    /// the item to be removed from the list.
    /// </param>
    public void RemoveItem(object sender, ExecuteEventArgs e)
    {
      try
      {
        var item = e.MethodParameter;
        SetError(null);
        var obj = ObjectInstance as System.Collections.IList;
        if (obj != null)
        {
          IsBusy = true;
          obj.Remove(item);
          var trackable = ObjectInstance as ITrackStatus;
          if (trackable != null)
            IsBusy = trackable.IsBusy;
          else
            IsBusy = false;
        }
        RefreshCanOperationsValues();
      }
      catch (Exception ex)
      {
        this.Error = ex;
      }
    }

    /// <summary>
    /// Causes the data provider to trigger data binding
    /// to rebind to the current business object.
    /// </summary>
    public void Rebind()
    {
      object tmp = ObjectInstance;
      ObjectInstance = null;
      ObjectInstance = tmp;
    }

    #endregion

    #region  Can methods that account for user rights and object state

    private bool _canSave = false;

    /// <summary>
    /// Gets a value indicating whether the business object
    /// can currently be saved.
    /// </summary>
    public bool CanSave
    {
      get
      {
        return _canSave;
      }
      private set
      {
        if (_canSave != value)
        {
          _canSave = value;
          OnPropertyChanged("CanSave");
        }
      }
    }

    private bool _canCancel = false;

    /// <summary>
    /// Gets a value indicating whether the business object
    /// can currently be canceled.
    /// </summary>
    public bool CanCancel
    {
      get
      {
        return _canCancel;
      }
      private set
      {
        if (_canCancel != value)
        {
          _canCancel = value;
          OnPropertyChanged("CanCancel");
        }
      }
    }

    private bool _canCreate = false;

    /// <summary>
    /// Gets a value indicating whether an instance
    /// of the business object
    /// can currently be created.
    /// </summary>
    public bool CanCreate
    {
      get
      {
        return _canCreate;
      }
      private set
      {
        if (_canCreate != value)
        {
          _canCreate = value;
          OnPropertyChanged("CanCreate");
        }
      }
    }

    private bool _canDelete = false;

    /// <summary>
    /// Gets a value indicating whether the business object
    /// can currently be deleted.
    /// </summary>
    public bool CanDelete
    {
      get
      {
        return _canDelete;
      }
      private set
      {
        if (_canDelete != value)
        {
          _canDelete = value;
          OnPropertyChanged("CanDelete");
        }
      }
    }

    private bool _canFetch = false;

    /// <summary>
    /// Gets a value indicating whether an instance
    /// of the business object
    /// can currently be retrieved.
    /// </summary>
    public bool CanFetch
    {
      get
      {
        return _canFetch;
      }
      private set
      {
        if (_canFetch != value)
        {
          _canFetch = value;
          OnPropertyChanged("CanFetch");
        }
      }
    }

    private bool _canRemoveItem = false;

    /// <summary>
    /// Gets a value indicating whether the business object
    /// can currently be removed.
    /// </summary>
    public bool CanRemoveItem
    {
      get
      {
        return _canRemoveItem;
      }
      private set
      {
        if (_canRemoveItem != value)
        {
          _canRemoveItem = value;
          OnPropertyChanged("CanRemoveItem");
        }
      }
    }

    private bool _canAddNewItem = false;

    /// <summary>
    /// Gets a value indicating whether the business object
    /// can currently be added.
    /// </summary>
    public bool CanAddNewItem
    {
      get
      {
        return _canAddNewItem;
      }
      private set
      {
        if (_canAddNewItem != value)
        {
          _canAddNewItem = value;
          OnPropertyChanged("CanAddNewItem");
        }
      }
    }

    private void RefreshCanOperationsValues()
    {
      ITrackStatus targetObject = this.Data as ITrackStatus;
      ICollection list = this.Data as ICollection;
      INotifyBusy busyObject = this.Data as INotifyBusy;
      bool isObjectBusy = false;
      if (busyObject != null && busyObject.IsBusy)
        isObjectBusy = true;
      if (this.Data != null && targetObject != null)
      {

        if (Csla.Rules.BusinessRules.HasPermission(Rules.AuthorizationActions.EditObject, this.Data) && targetObject.IsSavable)
          this.CanSave = true;
        else
          this.CanSave = false;

        if (Csla.Rules.BusinessRules.HasPermission(Rules.AuthorizationActions.EditObject, this.Data) && targetObject.IsDirty && !isObjectBusy)
          this.CanCancel = true;
        else
          this.CanCancel = false;

        if (Csla.Rules.BusinessRules.HasPermission(Rules.AuthorizationActions.CreateObject, this.Data) && !targetObject.IsDirty && !isObjectBusy)
          this.CanCreate = true;
        else
          this.CanCreate = false;

        if (Csla.Rules.BusinessRules.HasPermission(Rules.AuthorizationActions.DeleteObject, this.Data) && !isObjectBusy)
          this.CanDelete = true;
        else
          this.CanDelete = false;

        if (Csla.Rules.BusinessRules.HasPermission(Rules.AuthorizationActions.GetObject, this.Data) && !targetObject.IsDirty && !isObjectBusy)
          this.CanFetch = true;
        else
          this.CanFetch = false;

        if (list != null)
        {
          Type itemType = Csla.Utilities.GetChildItemType(this.Data.GetType());
          if (itemType != null)
          {

            if (Csla.Rules.BusinessRules.HasPermission(Rules.AuthorizationActions.DeleteObject, itemType) && ((ICollection)this.Data).Count > 0 && !isObjectBusy)
              this.CanRemoveItem = true;
            else
              this.CanRemoveItem = false;

            if (Csla.Rules.BusinessRules.HasPermission(Rules.AuthorizationActions.CreateObject, itemType) && !isObjectBusy)
              this.CanAddNewItem = true;
            else
              this.CanAddNewItem = false;
          }
          else
          {
            this.CanAddNewItem = false;
            this.CanRemoveItem = false;
          }
        }
        else
        {
          this.CanRemoveItem = false;
          this.CanAddNewItem = false;
        }
      }
      else if (list != null)
      {
        Type itemType = Csla.Utilities.GetChildItemType(this.Data.GetType());
        if (itemType != null)
        {

          if (Csla.Rules.BusinessRules.HasPermission(Rules.AuthorizationActions.DeleteObject, itemType) && ((ICollection)this.Data).Count > 0 && !isObjectBusy)
            this.CanRemoveItem = true;
          else
            this.CanRemoveItem = false;

          if (Csla.Rules.BusinessRules.HasPermission(Rules.AuthorizationActions.CreateObject, itemType) && !isObjectBusy)
            this.CanAddNewItem = true;
          else
            this.CanAddNewItem = false;
        }
        else
        {
          this.CanAddNewItem = false;
          this.CanRemoveItem = false;
        }
      }
      else
      {
        this.CanCancel = false;
        this.CanCreate = false;
        this.CanDelete = false;
        this.CanFetch = !this.IsBusy;
        this.CanSave = false;
        this.CanRemoveItem = false;
        this.CanAddNewItem = false;
      }
    }

    #endregion

    #region Can methods that only account for user rights

    private bool _canCreateObject;

    /// <summary>
    /// Gets a value indicating whether the current
    /// user is authorized to create an object.
    /// </summary>
    public bool CanCreateObject
    {
      get { return _canCreateObject; }
      protected set
      {
        _canCreateObject = value;
        OnPropertyChanged("CanCreateObject");
      }
    }

    private bool _canGetObject;

    /// <summary>
    /// Gets a value indicating whether the current
    /// user is authorized to retrieve an object.
    /// </summary>
    public bool CanGetObject
    {
      get { return _canGetObject; }
      protected set
      {
        _canGetObject = value;
        OnPropertyChanged("CanGetObject");
      }
    }

    private bool _canEditObject;

    /// <summary>
    /// Gets a value indicating whether the current
    /// user is authorized to save (insert or update
    /// an object.
    /// </summary>
    public bool CanEditObject
    {
      get { return _canEditObject; }
      protected set
      {
        _canEditObject = value;
        OnPropertyChanged("CanEditObject");
      }
    }

    private bool _canDeleteObject;

    /// <summary>
    /// Gets a value indicating whether the current
    /// user is authorized to delete
    /// an object.
    /// </summary>
    public bool CanDeleteObject
    {
      get { return _canDeleteObject; }
      protected set
      {
        _canDeleteObject = value;
        OnPropertyChanged("CanDeleteObject");
      }
    }

    private void RefreshCanOperationsOnObjectLevel()
    {
      if (Data != null)
      {
        Type sourceType = Data.GetType();
        var newValue = Csla.Rules.BusinessRules.HasPermission(Rules.AuthorizationActions.CreateObject, Data);
        if (CanCreateObject != newValue)
          CanCreateObject = newValue;

        newValue = Csla.Rules.BusinessRules.HasPermission(Rules.AuthorizationActions.GetObject, Data);
        if (CanGetObject != newValue)
          CanGetObject = newValue;

        newValue = Csla.Rules.BusinessRules.HasPermission(Rules.AuthorizationActions.EditObject, Data);
        if (CanEditObject != newValue)
          CanEditObject = newValue;

        newValue = Csla.Rules.BusinessRules.HasPermission(Rules.AuthorizationActions.DeleteObject, Data);
        if (CanDeleteObject != newValue)
          CanDeleteObject = newValue;

      }
      else
      {
        this.CanCreateObject = false;
        this.CanDeleteObject = false;
        this.CanGetObject = false;
        this.CanEditObject = false;
      }

    }

    #endregion

    #region INotifyPropertyChanged Members

    /// <summary>
    /// Event raised when a property of the
    /// object has changed.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raises the PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">
    /// Name of the changed property
    /// </param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion
  }
}