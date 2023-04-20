//-----------------------------------------------------------------------
// <copyright file="ViewModel.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Base type for creating your own view model</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Csla.Reflection;
using Csla.Rules;

namespace Csla.Blazor
{
  /// <summary>
  /// Base type for creating your own view model.
  /// </summary>
  public class ViewModel<T>
  {
    /// <summary>
    /// Gets the current ApplicationContext instance.
    /// </summary>
    protected ApplicationContext ApplicationContext { get; private set; }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="applicationContext"></param>
    public ViewModel(ApplicationContext applicationContext)
    {
      ApplicationContext = applicationContext;
    }

    #region Events

    /// <summary>
    /// Event raised after Model has been saved.
    /// </summary>
    public event Action Saved;
    /// <summary>
    /// Event raised when Model is changing.
    /// </summary>
    public event Action<T, T> ModelChanging;
    /// <summary>
    /// Event raised when Model has changed.
    /// </summary>
    public event Action ModelChanged;
    /// <summary>
    /// Event raised when the Model object
    /// raises its PropertyChanged event.
    /// </summary>
    public event PropertyChangedEventHandler ModelPropertyChanged;
    /// <summary>
    /// Event raised when the Model object
    /// raises its ModelChildChanged event.
    /// </summary>
    public event Action<object, Core.ChildChangedEventArgs> ModelChildChanged;
    /// <summary>
    /// Event raised when the Model object
    /// raises its ModelCollectionChanged event.
    /// </summary>
    public event Action<object, NotifyCollectionChangedEventArgs> ModelCollectionChanged;

    /// <summary>
    /// Raises the ModelChanging event.
    /// </summary>
    /// <param name="oldValue">Old Model value</param>
    /// <param name="newValue">New Model value</param>
    protected virtual void OnModelChanging(T oldValue, T newValue)
    {
      if (ReferenceEquals(oldValue, newValue)) return;

      if (ManageObjectLifetime && newValue is Core.ISupportUndo undo)
        undo.BeginEdit();

      _propertyInfoCache.Clear();

      // unhook events from old value
      if (oldValue != null)
      {
        UnhookChangedEvents(oldValue);

        if (oldValue is Core.INotifyBusy nb)
          nb.BusyChanged -= OnBusyChanged;
      }

      // hook events on new value
      if (newValue != null)
      {
        HookChangedEvents(newValue);

        if (newValue is Core.INotifyBusy nb)
          nb.BusyChanged += OnBusyChanged;
      }

      ModelChanging?.Invoke(oldValue, newValue);
    }

    /// <summary>
    /// Unhooks changed event handlers from the model.
    /// </summary>
    /// <param name="model"></param>
    protected void UnhookChangedEvents(T model)
    {
      if (model is INotifyPropertyChanged npc)
        npc.PropertyChanged -= OnModelPropertyChanged;

      if (model is IBusinessBase ncc)
        ncc.ChildChanged -= OnModelChildChanged;

      if (model is INotifyCollectionChanged cc)
        cc.CollectionChanged -= OnModelCollectionChanged;
    }

    /// <summary>
    /// Hooks changed events on the model.
    /// </summary>
    /// <param name="model"></param>
    private void HookChangedEvents(T model)
    {
      if (model is INotifyPropertyChanged npc)
        npc.PropertyChanged += OnModelPropertyChanged;

      if (model is IBusinessBase ncc)
        ncc.ChildChanged += OnModelChildChanged;

      if (model is INotifyCollectionChanged cc)
        cc.CollectionChanged += OnModelCollectionChanged;
    }


    private void OnBusyChanged(object sender, Core.BusyChangedEventArgs e)
    {
      // only set busy state for entire object. Ignore busy state based
      // on async rules being active
      if (string.IsNullOrEmpty(e.PropertyName))
        IsBusy = e.Busy;
    }

    /// <summary>
    /// Raises the ModelChanged event.
    /// </summary>
    protected virtual void OnModelChanged()
    {
      ModelChanged?.Invoke();
    }

    /// <summary>
    /// Raises the ModelPropertyChanged event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      ModelPropertyChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ModelChildChanged event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnModelChildChanged(object sender, Core.ChildChangedEventArgs e)
    {
      ModelChildChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the ModelCollectionChanged event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected virtual void OnModelCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      ModelCollectionChanged?.Invoke(this, e);
    }

    #endregion

    #region Methods

    /// <summary>
    /// Refresh the Model.
    /// </summary>
    /// <param name="factory">Async data portal or factory method</param>
    public async Task<T> RefreshAsync(Func<Task<T>> factory)
    {
      Exception = null;
      ViewModelErrorText = null;
      try
      {
        IsBusy = true;
        Model = await factory();
      }
      catch (DataPortalException ex)
      {
        Model = default;
        Exception = ex;
        ViewModelErrorText = ex.BusinessExceptionMessage;
      }
      catch (Exception ex)
      {
        Model = default;
        Exception = ex;
        ViewModelErrorText = ex.Message;
      }
      finally
      {
        IsBusy = false;
      }

      return Model;
    }

    /// <summary>
    /// Saves the Model.
    /// </summary>
    /// <returns></returns>
    public async Task SaveAsync()
    {
      Exception = null;
      ViewModelErrorText = null;
      if (Model is Core.ITrackStatus obj && !obj.IsSavable)
      {
        ViewModelErrorText = ModelErrorText;
        return;
      }
      try
      {
        UnhookChangedEvents(Model);
        var savable = Model as Core.ISavable;
        if (ManageObjectLifetime)
        {
          // clone the object if possible
          if (Model is ICloneable cloneable)
            savable = (Core.ISavable)cloneable.Clone();

          //apply changes
          if (savable is Core.ISupportUndo undoable)
            undoable.ApplyEdit();
        }
        IsBusy = true;
        Model = await DoSaveAsync();
        Saved?.Invoke();
      }
      catch (DataPortalException ex)
      {
        Exception = ex;
        ViewModelErrorText = ex.BusinessExceptionMessage;
      }
      catch (Exception ex)
      {
        Exception = ex;
        ViewModelErrorText = ex.Message;
      }
      finally
      {
        HookChangedEvents(Model);
        IsBusy = false;
      }
    }

    /// <summary>
    /// Override to provide custom Model save behavior.
    /// </summary>
    /// <returns></returns>
    protected virtual async Task<T> DoSaveAsync()
    {
      if (Model is Core.ISavable savable)
      {
        var result = (T)await savable.SaveAsync();
        if (Model is Core.IEditableBusinessObject editable)
          new Core.GraphMerger(ApplicationContext).MergeGraph(editable, (Core.IEditableBusinessObject)result);
        else
          Model = result;
      }
      return Model;
    }

    /// <summary>
    /// Cancels changes made to the model 
    /// if ManagedObjectLifetime is true.
    /// </summary>
    protected virtual void DoCancel()
    {
      if (ManageObjectLifetime)
      {
        if (Model is Core.ISupportUndo undo)
        {
          UnhookChangedEvents(Model);
          try
          {
            undo.CancelEdit();
            undo.BeginEdit();
          }
          finally
          {
            HookChangedEvents(Model);
          }
        }
      }
    }

    #endregion

    #region Properties

    private T _model;
    /// <summary>
    /// Gets or sets the Model object.
    /// </summary>
    public T Model 
    {
      get => _model;
      set
      {
        if (!ReferenceEquals(_model, value))
        {
          OnModelChanging(_model, value);
          _model = value;
          OnModelChanged();
        }
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the
    /// view model should manage the lifetime of
    /// the business object, including using n-level
    /// undo.
    /// </summary>
    public bool ManageObjectLifetime { get; set; } = false;

    /// <summary>
    /// Gets a value indicating whether this object is
    /// executing an asynchronous process.
    /// </summary>
    public bool IsBusy { get; protected set; } = false;

    #endregion

    #region GetPropertyInfo

    /// <summary>
    /// Get a PropertyInfo object for a property.
    /// PropertyInfo provides access
    /// to the meta-state of the property.
    /// </summary>
    /// <param name="property">Property expression</param>
    /// <returns></returns>
    public IPropertyInfo GetPropertyInfo<P>(Expression<Func<P>> property)
    {
      if (property == null)
        throw new ArgumentNullException(nameof(property));

      var keyName = property.GetKey();
      var identifier = Microsoft.AspNetCore.Components.Forms.FieldIdentifier.Create(property);
      return GetPropertyInfo(keyName, identifier.Model, identifier.FieldName);
    }

    /// <summary>
    /// Get a PropertyInfo object for a property.
    /// PropertyInfo provides access
    /// to the meta-state of the property.
    /// </summary>
    /// <param name="property">Property expression</param>
    /// <param name="id">Unique identifier for property in list or array</param>
    /// <returns></returns>
    public IPropertyInfo GetPropertyInfo<P>(Expression<Func<P>> property, string id)
    {
      if (property == null)
        throw new ArgumentNullException(nameof(property));

      var keyName = property.GetKey() + $"[{id}]";
      var identifier = Microsoft.AspNetCore.Components.Forms.FieldIdentifier.Create(property);
      return GetPropertyInfo(keyName, identifier.Model, identifier.FieldName);
    }

    /// <summary>
    /// Get a PropertyInfo object for a property
    /// of the Model. PropertyInfo provides access
    /// to the meta-state of the property.
    /// </summary>
    /// <param name="propertyName">Property name</param>
    /// <returns></returns>
    public IPropertyInfo GetPropertyInfo(string propertyName)
    {
      var keyName = Model.GetType().FullName + "." + propertyName;
      return GetPropertyInfo(keyName, Model, propertyName);
    }

    /// <summary>
    /// Get a PropertyInfo object for a property
    /// of the Model. PropertyInfo provides access
    /// to the meta-state of the property.
    /// </summary>
    /// <param name="propertyName">Property name</param>
    /// <param name="id">Unique identifier for property in list or array</param>
    /// <returns></returns>
    public IPropertyInfo GetPropertyInfo(string propertyName, string id)
    {
      var keyName = Model.GetType().FullName + "." + propertyName + $"[{id}]";
      return GetPropertyInfo(keyName, Model, propertyName);
    }

    private readonly Dictionary<string, object> _propertyInfoCache = new Dictionary<string, object>();
    
    private IPropertyInfo GetPropertyInfo(string keyName, object model, string propertyName)
    {
      PropertyInfo result;
      if (_propertyInfoCache.TryGetValue(keyName, out object temp))
      {
        result = (PropertyInfo)temp;
      }
      else
      {
        result = new PropertyInfo(model, propertyName);
        _propertyInfoCache.Add(keyName, result);
      }
      return result;
    }

    #endregion

    #region Errors and Exception

    /// <summary>
    /// Gets any error text generated by refresh or save operations.
    /// </summary>
    public string ViewModelErrorText { get; protected set; }

    /// <summary>
    /// Gets the first validation error 
    /// message from the Model.
    /// </summary>
    protected virtual string ModelErrorText
    {
      get
      {
        if (Model is IDataErrorInfo obj)
        {
          return obj.Error;
        }
        return string.Empty;
      }
    }

    /// <summary>
    /// Gets the last exception caught by
    /// the view model during refresh or save
    /// operations.
    /// </summary>
    public Exception Exception { get; private set; }

    #endregion

    #region ObjectLevelPermissions

    private bool _canCreateObject;
    
    /// <summary>
    /// Gets a value indicating whether the current user
    /// is authorized to create an instance of the
    /// business domain type.
    /// </summary>
    /// <returns></returns>
    public bool CanCreateObject
    {
      get
      {
        SetPropertiesAtObjectLevel(); 
        return _canCreateObject;
      }
      protected set
      {
        if (_canCreateObject != value)
          _canCreateObject = value;
      }
    }

    private bool _canGetObject;

    /// <summary>
    /// Gets a value indicating whether the current user
    /// is authorized to retrieve an instance of the
    /// business domain type.
    /// </summary>
    /// <returns></returns>
    public bool CanGetObject
    {
      get
      {
        SetPropertiesAtObjectLevel();
        return _canGetObject;
      }
      protected set
      {
        if (_canGetObject != value)
          _canGetObject = value;
      }
    }

    private bool _canEditObject;

    /// <summary>
    /// Gets a value indicating whether the current user
    /// is authorized to edit/save an instance of the
    /// business domain type.
    /// </summary>
    /// <returns></returns>
    public bool CanEditObject
    {
      get
      {
        SetPropertiesAtObjectLevel();
        return _canEditObject;
      }
      protected set
      {
        if (_canEditObject != value)
          _canEditObject = value;
      }
    }

    private bool _canDeleteObject;
    
    /// <summary>
    /// Gets a value indicating whether the current user
    /// is authorized to delete an instance of the
    /// business domain type.
    /// </summary>
    /// <returns></returns>
    public bool CanDeleteObject
    {
      get
      {
        SetPropertiesAtObjectLevel();
        return _canDeleteObject;
      }
      protected set
      {
        if (_canDeleteObject != value)
          _canDeleteObject = value;
      }
    }

    private bool ObjectPropertiesSet;

    /// <summary>
    /// Sets the properties at object level.
    /// </summary>
    private void SetPropertiesAtObjectLevel()
    {
      if (ObjectPropertiesSet)
        return;
      ObjectPropertiesSet = true;

      Type sourceType = typeof(T);

      CanCreateObject = BusinessRules.HasPermission(ApplicationContext, Rules.AuthorizationActions.CreateObject, sourceType);
      CanGetObject = BusinessRules.HasPermission(ApplicationContext, Rules.AuthorizationActions.GetObject, sourceType);
      CanEditObject = BusinessRules.HasPermission(ApplicationContext, Rules.AuthorizationActions.EditObject, sourceType);
      CanDeleteObject = BusinessRules.HasPermission(ApplicationContext, Rules.AuthorizationActions.DeleteObject, sourceType);
    }

    #endregion
  }
}
