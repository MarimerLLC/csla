using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Csla.Properties;
using System.Reflection;
using Csla.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using Csla.Core;

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
      IsBusy = e.Busy;
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

    private string _fetchFactoryMethod;
    public string FetchFactoryMethod
    {
      get
      {
        return _fetchFactoryMethod;
      }
      set
      {
        _fetchFactoryMethod = value;
        OnPropertyChanged(new PropertyChangedEventArgs("FetchFactoryMethod"));
        InitialFetch();
      }
    }

    private string _createFactoryMethod;
    public string CreateFactoryMethod
    {
      get
      {
        return _createFactoryMethod;
      }
      set
      {
        _createFactoryMethod = value;
        OnPropertyChanged(new PropertyChangedEventArgs("CreateFactoryMethod"));
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
        OnPropertyChanged(new PropertyChangedEventArgs("Error"));
      }
    }
    #endregion

    #region Operations
    /// <summary>
    /// Cancels any changes to the object.
    /// </summary>
    public void Cancel()
    {
      if (_manageObjectLifetime)
      {
        var undoable = _dataObject as Csla.Core.ISupportUndo;
        if (undoable != null)
        {
          undoable.CancelEdit();
          undoable.BeginEdit();
        }
      }
    }

    /// <summary>
    /// Accepts any changes to the object and
    /// invokes the object's BeginSave() method.
    /// </summary>
    public void Save()
    {
      Error = null;
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
      var obj = _dataObject as Csla.Core.BusinessBase;
      if (obj != null && !obj.IsChild)
        obj.Delete();
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

    private Delegate CreateHandler(Type objectType)
    {
      var args = typeof(DataPortalResult<>).MakeGenericType(objectType);
      MethodInfo method = this.GetType().GetMethod("QueryCompleted", BindingFlags.Instance | BindingFlags.NonPublic);
      Delegate handler = Delegate.CreateDelegate(typeof(EventHandler<>).MakeGenericType(args), this, method);
      return handler;
    }


    public void Fetch()
    {
      if (_objectType != null && _fetchFactoryMethod != null)
        try
        {
          this.IsBusy = true;
          BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy;
          List<object> parameters = new List<object>(FactoryParameters);
          Type objectType = Type.GetType(_objectType);

          parameters.Add(CreateHandler(objectType));

          MethodInfo factory = objectType.GetMethod(
            _fetchFactoryMethod, flags, null,
            MethodCaller.GetParameterTypes(parameters.ToArray()), null);

          if (factory == null)
          {
            // strongly typed factory couldn't be found
            // so find one with the correct number of
            // parameters 
            int parameterCount = parameters.ToArray().Length;
            MethodInfo[] methods = objectType.GetMethods(flags);
            foreach (MethodInfo method in methods)
              if (method.Name == _fetchFactoryMethod && method.GetParameters().Length == parameterCount)
              {
                factory = method;
                break;
              }
          }
          if (factory == null)
          {
            // no matching factory could be found
            // so throw exception
            throw new InvalidOperationException(
              string.Format(Resources.NoSuchFactoryMethod, _fetchFactoryMethod));
          }

          // invoke factory method
          try
          {
            factory.Invoke(null, parameters.ToArray());
          }
          catch (Exception ex)
          {
            _error = ex;
          }
        }
        catch (Exception ex)
        {
          _error = ex;
        }
    }

    public void Create()
    {
      if (_objectType != null && _createFactoryMethod != null)
        try
        {
          this.IsBusy = true;
          BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy;
          List<object> parameters = new List<object>(FactoryParameters);
          Type objectType = Type.GetType(_objectType);
          parameters.Add(CreateHandler(objectType));
          MethodInfo factory = objectType.GetMethod(
            _createFactoryMethod, flags, null,
            MethodCaller.GetParameterTypes(parameters.ToArray()), null);

          if (factory == null)
          {
            // strongly typed factory couldn't be found
            // so find one with the correct number of
            // parameters 
            int parameterCount = parameters.ToArray().Length;
            MethodInfo[] methods = objectType.GetMethods(flags);
            foreach (MethodInfo method in methods)
            {
              if (method.Name == _createFactoryMethod && method.GetParameters().Length == parameterCount)
              {
                factory = method;
                break;
              }
            }
            if (factory == null)
            {
              foreach (MethodInfo method in methods)
              {
                if (method.Name == _createFactoryMethod && method.GetParameters().Length == 1)
                {
                  factory = method;
                  while (parameters.Count > 1)
                  {
                    parameters.RemoveAt(0);
                  }
                  break;
                }
              }
            }
          }
          if (factory == null)
          {
            // no matching factory could be found
            // so throw exception
            throw new InvalidOperationException(
              string.Format(Resources.NoSuchFactoryMethod, _createFactoryMethod));
          }

          // invoke factory method
          try
          {
            factory.Invoke(null, parameters.ToArray());
          }
          catch (Exception ex)
          {
            _error = ex;
          }
        }
        catch (Exception ex)
        {
          _error = ex;
        }
    }

    /// <summary>
    /// Begins an async add new operation to add a 
    /// new item to an editable list business object.
    /// </summary>
    public void AddNewItem()
    {
      var obj = _dataObject as Csla.Core.IBindingList;
      if (obj != null)
        obj.AddNew();
      RefreshCanOperaionsValues();
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
      var obj = _dataObject as System.Collections.IList;
      if (obj != null)
        obj.Remove(item);
      RefreshCanOperaionsValues();
    }
    #endregion

    #region  Can Methods

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
      IEditableCollection list = this.Data as IEditableCollection;
      if (this.Data != null && targetObject != null)
      {

        if (Csla.Security.AuthorizationRules.CanEditObject(this.Data.GetType()) && targetObject.IsSavable && !this.IsBusy)
          this.CanSave = true;
        else
          this.CanSave = false;

        if (Csla.Security.AuthorizationRules.CanEditObject(this.Data.GetType()) && targetObject.IsDirty && !this.IsBusy)
          this.CanCancel = true;
        else
          this.CanCancel = false;

        if (Csla.Security.AuthorizationRules.CanCreateObject(this.Data.GetType()) && !targetObject.IsDirty && !this.IsBusy)
          this.CanCreate = true;
        else
          this.CanCreate = false;

        if (Csla.Security.AuthorizationRules.CanDeleteObject(this.Data.GetType()) && !this.IsBusy)
          this.CanDelete = true;
        else
          this.CanDelete = false;

        if (Csla.Security.AuthorizationRules.CanGetObject(this.Data.GetType()) && !targetObject.IsDirty && !this.IsBusy)
          this.CanFetch = true;
        else
          this.CanFetch = false;

        if (list != null)
        {
          Type itemType = Csla.Utilities.GetChildItemType(this.Data.GetType());
          if (itemType != null)
          {

            if (Csla.Security.AuthorizationRules.CanDeleteObject(itemType) && ((ICollection)this.Data).Count > 0 && !this.IsBusy)
              this.CanRemoveItem = true;
            else
              this.CanRemoveItem = false;

            if (Csla.Security.AuthorizationRules.CanCreateObject(itemType) && !this.IsBusy)
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
      else
      {
        this.CanCancel = false;
        this.CanCreate = false;
        this.CanDelete = false;
        this.CanFetch =  !this.IsBusy;
        this.CanSave = false;
        this.CanRemoveItem = false;
        this.CanAddNewItem = false;
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
        PropertyChanged(this, e);
    }

    #endregion
  }
}
