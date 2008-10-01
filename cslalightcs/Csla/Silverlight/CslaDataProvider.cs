using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Csla.Properties;
using System.Reflection;
using Csla.Reflection;
using System.Collections;
using System.Collections.Generic;
using Csla.Core;
using System.Windows;

namespace Csla.Silverlight
{
  /// <summary>
  /// Creates, retrieves and manages business objects
  /// from XAML in a form.
  /// </summary>
  public class CslaDataProvider : INotifyPropertyChanged
  {

    #region Events
    public event EventHandler DataChanged;
    protected void OnDataChanged()
    {
      RefreshCanOperaionsValues();
      RefreshCanOpertaionsOnObjectLevel();
      if (DataChanged != null)
        DataChanged(this, EventArgs.Empty);
    }

    private void dataObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      RefreshCanOperaionsValues();
    }

    private void dataObject_ChildChanged(object sender, ChildChangedEventArgs e)
    {
      RefreshCanOperaionsValues();
    }

    #endregion

    #region Properties

    private bool _isBusy = false;
    public bool IsBusy
    {
      get { return _isBusy; }
      protected set
      {
        _isBusy = value;
        OnPropertyChanged(new PropertyChangedEventArgs("IsBusy"));
        OnPropertyChanged(new PropertyChangedEventArgs("IsNotBusy"));
        RefreshCanOperaionsValues();
      }
    }

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
        RefreshCanOperaionsValues();
    }


    private object _dataObject;

    /// <summary>
    /// Gets or sets a reference to the
    /// object containing the data for binding.
    /// </summary>
    public object Data
    {
      get
      {
        return _dataObject;
      }
      set
      {
        //hook up event handlers for notificaiton propagation
        if (_dataObject != null && _dataObject is INotifyPropertyChanged)
          ((INotifyPropertyChanged)_dataObject).PropertyChanged -= new PropertyChangedEventHandler(dataObject_PropertyChanged);
        if (_dataObject != null && _dataObject is INotifyChildChanged)
          ((INotifyChildChanged)_dataObject).ChildChanged -= new EventHandler<ChildChangedEventArgs>(dataObject_ChildChanged);
        if (_dataObject != null && _dataObject is INotifyBusy)
          ((INotifyBusy)_dataObject).BusyChanged -= new BusyChangedEventHandler(CslaDataProvider_BusyChanged);

        _dataObject = value;
        if (_manageObjectLifetime)
        {
          var undoable = _dataObject as Csla.Core.ISupportUndo;
          if (undoable != null)
            undoable.BeginEdit();
        }

        if (_dataObject != null && _dataObject is INotifyPropertyChanged)
          ((INotifyPropertyChanged)_dataObject).PropertyChanged += new PropertyChangedEventHandler(dataObject_PropertyChanged);
        if (_dataObject != null && _dataObject is INotifyChildChanged)
          ((INotifyChildChanged)_dataObject).ChildChanged += new EventHandler<ChildChangedEventArgs>(dataObject_ChildChanged);

        if (_dataObject != null && _dataObject is INotifyBusy)
          ((INotifyBusy)_dataObject).BusyChanged += new BusyChangedEventHandler(CslaDataProvider_BusyChanged);

        try
        {
          OnPropertyChanged(new PropertyChangedEventArgs("Data"));
          OnDataChanged();
        }
        catch (NullReferenceException ex)
        {
          // Silverlight seems to throw a meaningless null ref exception
          // and this is a workaround to ignore it
          var o = ex;
        }
      }
    }

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
        if (_dataObject != null)
          throw new NotSupportedException(Resources.ObjectNotNull);
        _manageObjectLifetime = value;
      }
    }

    private bool _isInitialLoadEnabled = false;
    private bool _isInitialLoadCompleted = false;

    public bool IsInitialLoadEnabled
    {
      get { return _isInitialLoadEnabled; }
      set
      {
        if (_dataObject != null)
          throw new NotSupportedException(Resources.ObjectNotNull);
        _isInitialLoadEnabled = value;
        InitialFetch();
      }
    }

    private string _objectType;
    public string ObjectType
    {
      get
      {
        return _objectType;
      }
      set
      {
        _objectType = value;
        OnPropertyChanged(new PropertyChangedEventArgs("ObjectType"));
        InitialFetch();
      }
    }

    private string _factoryMethod;
    public string FactoryMethod
    {
      get
      {
        return _factoryMethod;
      }
      set
      {
        _factoryMethod = value;
        OnPropertyChanged(new PropertyChangedEventArgs("FactoryMethod"));
        InitialFetch();
      }
    }

    private ObservableCollection<object> _factoryParameters;
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
        OnPropertyChanged(new PropertyChangedEventArgs("FactoryParameters"));
      }
    }

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
        _error = value;
        IsBusy = false;
        OnPropertyChanged(new PropertyChangedEventArgs("Error"));
        OnDataChanged();
      }
    }
    #endregion

    #region Operations
    /// <summary>
    /// Cancels any changes to the object.
    /// </summary>
    public void Cancel()
    {
      _error = null;
      if (_manageObjectLifetime)
      {
        try
        {
          var undoable = _dataObject as Csla.Core.ISupportUndo;
          if (undoable != null)
          {
            IsBusy = true;
            Data = null;
            undoable.CancelEdit();
            Data = undoable;
            var trackable = _dataObject as ITrackStatus;
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
      _error = null;
      try
      {
        if (_manageObjectLifetime)
        {
          var undoable = _dataObject as Csla.Core.ISupportUndo;
          if (undoable != null)
            undoable.ApplyEdit();
        }
        var obj = _dataObject as Csla.Core.ISavable;
        if (obj != null)
        {
          obj.Saved -= new EventHandler<Csla.Core.SavedEventArgs>(obj_Saved);
          obj.Saved += new EventHandler<Csla.Core.SavedEventArgs>(obj_Saved);
          obj.BeginSave();
        }
      }
      catch (Exception ex)
      {
        this.Error = ex;
      }
    }

    void obj_Saved(object sender, Csla.Core.SavedEventArgs e)
    {
      if (e.Error != null)
        Error = e.Error;
      else
        Data = e.NewObject;
    }

    /// <summary>
    /// Marks an editable root object for deletion.
    /// </summary>
    public void Delete()
    {
      _error = null;
      try
      {
        var obj = _dataObject as Csla.Core.BusinessBase;
        if (obj != null && !obj.IsChild)
        {
          IsBusy = true;
          obj.Delete();
          var trackable = _dataObject as ITrackStatus;
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

    private void QueryCompleted(object sender, EventArgs e)
    {
      IDataPortalResult eventArgs = e as IDataPortalResult;
      if (_manageObjectLifetime && eventArgs.Object != null && eventArgs.Error == null)
      {
        this.Data = eventArgs.Object;
        this.Error = eventArgs.Error;
        _isInitialLoadCompleted = true;
      }
      else if (eventArgs.Error != null)
      {
        this.Error = eventArgs.Error;
      }
      RefreshCanOperaionsValues();
      this.IsBusy = false;
    }

    private void InitialFetch()
    {
      if (_isInitialLoadEnabled && !_isInitialLoadCompleted)
      {
        Fetch();
      }
    }

    public void Fetch()
    {
      Refresh();
    }

    public void Create()
    {
      Refresh();
    }

    /// <summary>
    /// Begins an async add new operation to add a 
    /// new item to an editable list business object.
    /// </summary>
    public void AddNewItem()
    {
      _error = null;
      try
      {
        var obj = _dataObject as Csla.Core.IBindingList;
        if (obj != null)
        {
          IsBusy = true;
          obj.AddNew();
          var trackable = _dataObject as ITrackStatus;
          if (trackable != null)
            IsBusy = trackable.IsBusy;
          else
            IsBusy = false;
        }
        RefreshCanOperaionsValues();
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
    /// <param name="item">
    /// Reference to the child item to remove.
    /// </param>
    public void RemoveItem(object item)
    {
      try
      {
        _error = null;
        var obj = _dataObject as System.Collections.IList;
        if (obj != null)
        {
          IsBusy = true;
          obj.Remove(item);
          var trackable = _dataObject as ITrackStatus;
          if (trackable != null)
            IsBusy = trackable.IsBusy;
          else
            IsBusy = false;
        }
        RefreshCanOperaionsValues();
      }
      catch (Exception ex)
      {
        this.Error = ex;
      }
    }

    private Delegate CreateHandler(Type objectType)
    {
      var args = typeof(DataPortalResult<>).MakeGenericType(objectType);
      MethodInfo method = this.GetType().GetMethod("QueryCompleted", BindingFlags.Instance | BindingFlags.NonPublic);
      Delegate handler = Delegate.CreateDelegate(typeof(EventHandler<>).MakeGenericType(args), this, method);
      return handler;
    }

    public void Refresh()
    {
      if (_objectType != null && _factoryMethod != null)
        try
        {
          _error = null;
          this.IsBusy = true;
          List<object> parameters = new List<object>(FactoryParameters);
          Type objectType = Type.GetType(_objectType);
          parameters.Add(CreateHandler(objectType));

          MethodCaller.CallFactoryMethod(objectType, _factoryMethod, parameters.ToArray());
        }
        catch (Exception ex)
        {
          this.Error = ex;
        }
    }

    public void Rebind()
    {
      object tmp = Data;
      Data = null;
      Data = tmp;
    }

    #endregion

    #region  Can methods that account for user rights and object state

    private bool _canSave = false;
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
          OnPropertyChanged(new PropertyChangedEventArgs("CanSave"));
        }
      }
    }

    private bool _canCancel = false;
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
          OnPropertyChanged(new PropertyChangedEventArgs("CanCancel"));
        }
      }
    }

    private bool _canCreate = false;
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
          OnPropertyChanged(new PropertyChangedEventArgs("CanCreate"));
        }
      }
    }

    private bool _canDelete = false;
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
          OnPropertyChanged(new PropertyChangedEventArgs("CanDelete"));
        }
      }
    }

    private bool _canFetch = false;
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
          OnPropertyChanged(new PropertyChangedEventArgs("CanFetch"));
        }
      }
    }

    private bool _canRemoveItem = false;
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
          OnPropertyChanged(new PropertyChangedEventArgs("CanRemoveItem"));
        }
      }
    }

    private bool _canAddNewItem = false;
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
          OnPropertyChanged(new PropertyChangedEventArgs("CanAddNewItem"));
        }
      }
    }

    private void RefreshCanOperaionsValues()
    {
      ITrackStatus targetObject = this.Data as ITrackStatus;
      ICollection list = this.Data as ICollection;
      INotifyBusy busyObject = this.Data as INotifyBusy;
      bool isObjectBusy = false;
      if (busyObject != null && busyObject.IsBusy)
        isObjectBusy = true;
      if (this.Data != null && targetObject != null)
      {

        if (Csla.Security.AuthorizationRules.CanEditObject(this.Data.GetType()) && targetObject.IsSavable)
          this.CanSave = true;
        else
          this.CanSave = false;

        if (Csla.Security.AuthorizationRules.CanEditObject(this.Data.GetType()) && targetObject.IsDirty && !isObjectBusy)
          this.CanCancel = true;
        else
          this.CanCancel = false;

        if (Csla.Security.AuthorizationRules.CanCreateObject(this.Data.GetType()) && !targetObject.IsDirty && !isObjectBusy)
          this.CanCreate = true;
        else
          this.CanCreate = false;

        if (Csla.Security.AuthorizationRules.CanDeleteObject(this.Data.GetType()) && !isObjectBusy)
          this.CanDelete = true;
        else
          this.CanDelete = false;

        if (Csla.Security.AuthorizationRules.CanGetObject(this.Data.GetType()) && !targetObject.IsDirty && !isObjectBusy)
          this.CanFetch = true;
        else
          this.CanFetch = false;

        if (list != null)
        {
          Type itemType = Csla.Utilities.GetChildItemType(this.Data.GetType());
          if (itemType != null)
          {

            if (Csla.Security.AuthorizationRules.CanDeleteObject(itemType) && ((ICollection)this.Data).Count > 0 && !isObjectBusy)
              this.CanRemoveItem = true;
            else
              this.CanRemoveItem = false;

            if (Csla.Security.AuthorizationRules.CanCreateObject(itemType) && !isObjectBusy)
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

          if (Csla.Security.AuthorizationRules.CanDeleteObject(itemType) && ((ICollection)this.Data).Count > 0 && !isObjectBusy)
            this.CanRemoveItem = true;
          else
            this.CanRemoveItem = false;

          if (Csla.Security.AuthorizationRules.CanCreateObject(itemType) && !isObjectBusy)
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
    public bool CanCreateObject
    {
      get { return _canCreateObject; }
      protected set
      {
        _canCreateObject = value;
        OnPropertyChanged(new PropertyChangedEventArgs("CanCreateObject"));
      }
    }

    private bool _canGetObject;
    public bool CanGetObject
    {
      get { return _canGetObject; }
      protected set
      {
        _canGetObject = value;
        OnPropertyChanged(new PropertyChangedEventArgs("CanGetObject"));
      }
    }

    private bool _canEditObject;
    public bool CanEditObject
    {
      get { return _canEditObject; }
      protected set
      {
        _canEditObject = value;
        OnPropertyChanged(new PropertyChangedEventArgs("CanEditObject"));
      }
    }

    private bool _canDeleteObject;
    public bool CanDeleteObject
    {
      get { return _canDeleteObject; }
      protected set
      {
        _canDeleteObject = value;
        OnPropertyChanged(new PropertyChangedEventArgs("CanDeleteObject"));
      }
    }

    private void RefreshCanOpertaionsOnObjectLevel()
    {
      if (Data != null)
      {
        Type sourceType = Data.GetType();
        if (CanCreateObject != Csla.Security.AuthorizationRules.CanCreateObject(sourceType))
          CanCreateObject = Csla.Security.AuthorizationRules.CanCreateObject(sourceType);

        if (CanGetObject != Csla.Security.AuthorizationRules.CanGetObject(sourceType))
          CanGetObject = Csla.Security.AuthorizationRules.CanGetObject(sourceType);

        if (CanEditObject != Csla.Security.AuthorizationRules.CanEditObject(sourceType))
          CanEditObject = Csla.Security.AuthorizationRules.CanEditObject(sourceType);

        if (CanDeleteObject != Csla.Security.AuthorizationRules.CanDeleteObject(sourceType))
          CanDeleteObject = Csla.Security.AuthorizationRules.CanDeleteObject(sourceType);

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
    /// <param name="e">
    /// Arguments for event.
    /// </param>
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
      if (PropertyChanged != null)
      {
        Delegate[] targets = PropertyChanged.GetInvocationList();
        foreach (var oneTarget in targets)
        {
          try
          {
            oneTarget.DynamicInvoke(this, e);
          }
          catch (TargetInvocationException ex)
          {
            if (ex.InnerException != null && ex.InnerException is NullReferenceException)
            {
              //TODO: should revisit after RTM - should uncomment code below
              // can be thrown due to bug in SL
            }
            else
              throw;
          }
        }
      }
      //if (PropertyChanged != null)
      //  PropertyChanged(this, e);
    }

    #endregion
  }
}
