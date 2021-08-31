//-----------------------------------------------------------------------
// <copyright file="ViewModelBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Base class used to create ViewModel objects that</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Windows;
using Csla.Core;
using Csla.Reflection;
using Csla.Rules;
#if WINDOWS_UWP
using Windows.UI.Xaml;
#endif

#if ANDROID
namespace Csla.Axml
#elif IOS
namespace Csla.Iosui
#else
namespace Csla.Xaml
#endif
{
  /// <summary>
  /// Base class used to create ViewModel objects that
  /// implement their own commands/verbs/actions.
  /// </summary>
  /// <typeparam name="T">Type of the Model object.</typeparam>
#if ANDROID || IOS || XAMARIN
    public abstract class ViewModelBase<T> : INotifyPropertyChanged, IViewModel
#else
  public abstract class ViewModelBase<T> : DependencyObject,
    INotifyPropertyChanged, IViewModel
#endif
  {
    /// <summary>
    /// Create new instance of base class used to create ViewModel objects that
    /// implement their own commands/verbs/actions.
    /// </summary>
    protected ViewModelBase()
    {
      SetPropertiesAtObjectLevel();
    }

#region Obsolete Code

    /// <summary>
    /// Method used to perform async initialization of the
    /// viewmodel. This method is usually invoked immediately
    /// following construction of the object instance.
    /// </summary>
    /// <returns></returns>
    [Obsolete("Use RefreshAsync", false)]
    public async Task<ViewModelBase<T>> InitAsync()
    {
      try
      {
        IsBusy = true;
        Model = await DoInitAsync();
        IsBusy = false;
      }
#pragma warning disable CA1031 // Do not catch general exception types
      catch (Exception ex)
      {
        IsBusy = false;
        this.Error = ex;
      }
#pragma warning restore CA1031 // Do not catch general exception types
      return this;
    }

#pragma warning disable 1998
    /// <summary>
    /// Override this method to implement async initialization of
    /// the model object. The result of this method is used
    /// to set the Model property of the viewmodel.
    /// </summary>
    /// <returns>A Task that creates the model object.</returns>
    [Obsolete("Use RefreshAsync", false)]
    protected async virtual Task<T> DoInitAsync()
    {
      throw new NotImplementedException("DoInitAsync");
    }
#pragma warning restore 1998

    private Exception _error;

    /// <summary>
    /// Gets the Error object corresponding to the
    /// last asynchronous operation.
    /// </summary>
    [Browsable(false)]
    [Display(AutoGenerateField = false)]
    [ScaffoldColumn(false)]
    [Obsolete("Use RefreshAsync/SaveAsync", false)]
    public Exception Error
    {
      get { return _error; }
      protected set
      {
        if (!ReferenceEquals(_error, value))
        {
          _error = value;
          OnPropertyChanged(nameof(Error));
          if (_error != null)
            OnError(_error);
        }
      }
    }

    /// <summary>
    /// Event raised when an error occurs during processing.
    /// </summary>
    [Obsolete("Use RefreshAsync/SaveAsync", false)]
    public event EventHandler<ErrorEventArgs> ErrorOccurred;

    /// <summary>
    /// Raises ErrorOccurred event when an error occurs
    /// during processing.
    /// </summary>
    /// <param name="error"></param>
    [Obsolete("Use RefreshAsync/SaveAsync", false)]
    protected virtual void OnError(Exception error)
    {
#if ANDROID || IOS
      ErrorOccurred?.Invoke(this, new ErrorEventArgs(this, error));
#else
      ErrorOccurred?.Invoke(this, new ErrorEventArgs { Error = error });
#endif
    }

#if !(ANDROID || IOS)
    /// <summary>
    /// Creates or retrieves a new instance of the 
    /// Model by invoking a static factory method.
    /// </summary>
    /// <param name="factoryMethod">Static factory method function.</param>
    /// <example>DoRefresh(BusinessList.GetList)</example>
    /// <example>DoRefresh(() => BusinessList.GetList())</example>
    /// <example>DoRefresh(() => BusinessList.GetList(id))</example>
    [Obsolete("Use RefreshAsync", false)]
    protected virtual void DoRefresh(Func<T> factoryMethod)
    {
      if (typeof(T) != null)
      {
        OnRefreshing(Model);
        Error = null;
        try
        {
          Model = factoryMethod.Invoke();
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception ex)
        {
          Error = ex;
        }
#pragma warning restore CA1031 // Do not catch general exception types
        OnRefreshed();
      }
    }

    /// <summary>
    /// Creates or retrieves a new instance of the 
    /// Model by invoking a static factory method.
    /// </summary>
    /// <param name="factoryMethod">Name of the static factory method.</param>
    /// <param name="factoryParameters">Factory method parameters.</param>
    [Obsolete("Use RefreshAsync", false)]
    protected virtual void DoRefresh(string factoryMethod, params object[] factoryParameters)
    {
      if (typeof(T) != null)
      {
        OnRefreshing(Model);
        Error = null;
        try
        {
          Model = (T)MethodCaller.CallFactoryMethod(typeof(T), factoryMethod, factoryParameters);
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception ex)
        {
          Error = ex;
        }
#pragma warning restore CA1031 // Do not catch general exception types
        OnRefreshed();
      }
    }

    /// <summary>
    /// Creates or retrieves a new instance of the 
    /// Model by invoking a static factory method.
    /// </summary>
    /// <param name="factoryMethod">Name of the static factory method.</param>
    [Obsolete("Use RefreshAsync", false)]
    protected virtual void DoRefresh(string factoryMethod)
    {
      DoRefresh(factoryMethod, Array.Empty<object>());
    }
#endif

      /// <summary>
      /// Creates or retrieves a new instance of the 
      /// Model by invoking a static factory method.
      /// </summary>
      /// <param name="factoryMethod">Static factory method action.</param>
      /// <example>BeginRefresh(BusinessList.BeginGetList)</example>
      /// <example>BeginRefresh(handler => BusinessList.BeginGetList(handler))</example>
      /// <example>BeginRefresh(handler => BusinessList.BeginGetList(id, handler))</example>
    [Obsolete("Use RefreshAsync", false)]
    protected virtual void BeginRefresh(Action<EventHandler<DataPortalResult<T>>> factoryMethod)
    {
      if (typeof(T) != null)
        try
        {
          Error = null;
          IsBusy = true;

          var handler = (EventHandler<DataPortalResult<T>>)CreateHandler(typeof(T));
          factoryMethod(handler);
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception ex)
        {
          Error = ex;
          IsBusy = false;
        }
#pragma warning restore CA1031 // Do not catch general exception types
    }

    /// <summary>
    /// Creates or retrieves a new instance of the 
    /// Model by invoking a static factory method.
    /// </summary>
    /// <param name="factoryMethod">Name of the static factory method.</param>
    /// <param name="factoryParameters">Factory method parameters.</param>
    [Obsolete("Use RefreshAsync", false)]
    protected virtual void BeginRefresh(string factoryMethod, params object[] factoryParameters)
    {
      if (typeof(T) != null)
        try
        {
          Error = null;
          IsBusy = true;
          var parameters = new List<object>(factoryParameters)
          {
            CreateHandler(typeof(T))
          };

          MethodCaller.CallFactoryMethod(typeof(T), factoryMethod, parameters.ToArray());
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception ex)
        {
          Error = ex;
          IsBusy = false;
        }
#pragma warning restore CA1031 // Do not catch general exception types
    }

    /// <summary>
    /// Creates or retrieves a new instance of the 
    /// Model by invoking a static factory method.
    /// </summary>
    /// <param name="factoryMethod">Name of the static factory method.</param>
    [Obsolete("Use RefreshAsync", false)]
    protected virtual void BeginRefresh(string factoryMethod)
    {
      BeginRefresh(factoryMethod, Array.Empty<object>());
    }

    private Delegate CreateHandler(Type objectType)
    {
      System.Reflection.MethodInfo method = MethodCaller.GetMethod(GetType(), "QueryCompleted");
      var innerType = typeof(DataPortalResult<>).MakeGenericType(objectType);
      var args = typeof(EventHandler<>).MakeGenericType(innerType);

      Delegate handler = Delegate.CreateDelegate(args, this, method);
      return handler;
    }

    [Obsolete("Use RefreshAsync", false)]
    private void QueryCompleted(object sender, EventArgs e)
    {
      try
      {
        var eventArgs = (IDataPortalResult)e;
        if (eventArgs.Error == null)
        {
          var model = (T)eventArgs.Object;
          OnRefreshing(model);
          Model = model;
        }
        else
          Error = eventArgs.Error;
        OnRefreshed();
      }
      finally
      {
        IsBusy = false;
      }
    }

    /// <summary>
    /// Method called after a refresh operation 
    /// has completed and before the model is updated.
    /// </summary>
    /// <param name="model">The model.</param>
    [Obsolete("Use RefreshAsync", false)]
    protected virtual void OnRefreshing(T model)
    { }

    /// <summary>
    /// Method called after a refresh operation 
    /// has completed.
    /// </summary>
    [Obsolete("Use RefreshAsync", false)]
    protected virtual void OnRefreshed()
    { }

    /// <summary>
    /// Saves the Model, first committing changes
    /// if ManagedObjectLifetime is true.
    /// </summary>
    [Obsolete("Use SaveAsync", false)]
    protected virtual T DoSave()
    {
      T result = (T)Model;
      Error = null;
      try
      {
        UnhookChangedEvents(Model);
        var savable = Model as Csla.Core.ISavable;
        if (ManageObjectLifetime)
        {
          // clone the object if possible
          if (Model is ICloneable clonable)
            savable = (Csla.Core.ISavable)clonable.Clone();

          //apply changes
          if (savable is Csla.Core.ISupportUndo undoable)
            undoable.ApplyEdit();
        }

        result = (T)savable.Save();

        Model = result;
        OnSaved();
      }
      catch (Exception ex)
      {
        HookChangedEvents(Model);
        Error = ex;
        OnSaved();
      }
      return result;
    }

    /// <summary>
    /// Saves the Model, first committing changes
    /// if ManagedObjectLifetime is true.
    /// </summary>
    [Obsolete("Use SaveAsync", false)]
    protected virtual void BeginSave()
    {
      try
      {
        var savable = Model as Csla.Core.ISavable;
        if (ManageObjectLifetime)
        {
          // clone the object if possible
          if (Model is ICloneable clonable)
            savable = (Csla.Core.ISavable)clonable.Clone();

          //apply changes
          if (savable is Csla.Core.ISupportUndo undoable)
            undoable.ApplyEdit();
        }

        savable.Saved += (o, e) =>
        {
          IsBusy = false;
          if (e.Error == null)
          {
            var result = e.NewObject;
            var model = (T)result;
            OnSaving(model);
            Model = model;
          }
          else
          {
            Error = e.Error;
          }
          OnSaved();
        };
        Error = null;
        IsBusy = true;
        savable.BeginSave();
      }
#pragma warning disable CA1031 // Do not catch general exception types
      catch (Exception ex)
      {
        IsBusy = false;
        Error = ex;
        OnSaved();
      }
#pragma warning restore CA1031 // Do not catch general exception types
    }

    /// <summary>
    /// Method called after a save operation 
    /// has completed and before Model is updated 
    /// (when successful).
    /// </summary>
    [Obsolete("Use SaveAsync", false)]
    protected virtual void OnSaving(T model)
    { }

    /// <summary>
    /// Method called after a save operation 
    /// has completed (whether successful or
    /// not).
    /// </summary>
    [Obsolete("Use SaveAsync", false)]
    protected virtual void OnSaved()
    { }

    #endregion

#if ANDROID || IOS || XAMARIN || WINDOWS_UWP
    private T _model;
    /// <summary>
    /// Gets or sets the Model object.
    /// </summary>
    public T Model
    {
      get { return _model; }
      set
      {
        if (!ReferenceEquals(value, _model))
        {
          var oldValue = _model;
          _model = value;
          this.OnModelChanged((T)oldValue, _model);
          OnPropertyChanged(nameof(Model));
        }
      }
    }
#else
    /// <summary>
    /// Gets or sets the Model object.
    /// </summary>
    public static readonly DependencyProperty ModelProperty =
        DependencyProperty.Register("Model", typeof(T), typeof(ViewModelBase<T>),
        new PropertyMetadata((o, e) =>
        {
          var viewmodel = (ViewModelBase<T>)o;
          viewmodel.OnModelChanged((T)e.OldValue, (T)e.NewValue);
        }));

    /// <summary>
    /// Gets or sets the Model object.
    /// </summary>
    public T Model
    {
      get { return (T)GetValue(ModelProperty); }
      set { SetValue(ModelProperty, value); }
    }
#endif

    /// <summary>
    /// Gets or sets a value indicating whether the
    /// ViewModel should automatically managed the
    /// lifetime of the Model.
    /// </summary>
#if ANDROID || IOS || XAMARIN
    public bool ManageObjectLifetimeProperty;
#else
    public static readonly DependencyProperty ManageObjectLifetimeProperty =
        DependencyProperty.Register("ManageObjectLifetime", typeof(bool),
        typeof(ViewModelBase<T>), new PropertyMetadata(true));
#endif
    /// <summary>
    /// Gets or sets a value indicating whether the
    /// ViewManageObjectLifetime should automatically managed the
    /// lifetime of the ManageObjectLifetime.
    /// </summary>
    [Browsable(false)]
    [Display(AutoGenerateField = false)]
    [ScaffoldColumn(false)]
    public bool ManageObjectLifetime
    {
#if ANDROID || IOS || XAMARIN
      get { return (bool)ManageObjectLifetimeProperty; }
      set { ManageObjectLifetimeProperty = value; }
#else
      get { return (bool)GetValue(ManageObjectLifetimeProperty); }
      set { SetValue(ManageObjectLifetimeProperty, value); }
#endif
    }

    private bool _isBusy;

    /// <summary>
    /// Gets a value indicating whether this object is
    /// executing an asynchronous process.
    /// </summary>
    public bool IsBusy
    {
      get { return _isBusy; }
      protected set
      {
        if (value != _isBusy)
        {
          _isBusy = value;
          OnPropertyChanged(nameof(IsBusy));
          OnSetProperties();
        }
      }
    }

    private bool _isDirty;

    /// <summary>
    /// Gets a value indicating whether the Model
    /// has been changed.
    /// </summary>
    public virtual bool IsDirty
    {
      get
      {
        return _isDirty;
      }
      protected set
      {
        if (_isDirty != value)
        {
          _isDirty = value;
          OnPropertyChanged(nameof(IsDirty));
        }
      }
    }

    private bool _isValid;

    /// <summary>
    /// Gets a value indicating whether the Model
    /// is currently valid (has no broken rules).
    /// </summary>
    public virtual bool IsValid
    {
      get
      {
        return _isValid;
      }
      protected set
      {
        if (_isValid != value)
        {
          _isValid = value;
          OnPropertyChanged(nameof(IsValid));
        }
      }
    }

    private bool _canSave = false;

    /// <summary>
    /// Gets a value indicating whether the Model
    /// can currently be saved.
    /// </summary>
    public virtual bool CanSave
    {
      get
      {
        return _canSave;
      }
      protected set
      {
        if (_canSave != value)
        {
          _canSave = value;
          OnPropertyChanged(nameof(CanSave));
        }
      }
    }

    private bool _canCancel = false;

    /// <summary>
    /// Gets a value indicating whether the Model
    /// can currently be canceled.
    /// </summary>
    public virtual bool CanCancel
    {
      get
      {
        return _canCancel;
      }
      protected set
      {
        if (_canCancel != value)
        {
          _canCancel = value;
          OnPropertyChanged(nameof(CanCancel));
        }
      }
    }

    private bool _canCreate = false;

    /// <summary>
    /// Gets a value indicating whether an instance
    /// of the Model
    /// can currently be created.
    /// </summary>
    public virtual bool CanCreate
    {
      get
      {
        return _canCreate;
      }
      protected set
      {
        if (_canCreate != value)
        {
          _canCreate = value;
          OnPropertyChanged(nameof(CanCreate));
        }
      }
    }

    private bool _canDelete = false;

    /// <summary>
    /// Gets a value indicating whether the Model
    /// can currently be deleted.
    /// </summary>
    public virtual bool CanDelete
    {
      get
      {
        return _canDelete;
      }
      protected set
      {
        if (_canDelete != value)
        {
          _canDelete = value;
          OnPropertyChanged(nameof(CanDelete));
        }
      }
    }

    private bool _canFetch = false;

    /// <summary>
    /// Gets a value indicating whether an instance
    /// of the Model
    /// can currently be retrieved.
    /// </summary>
    public virtual bool CanFetch
    {
      get
      {
        return _canFetch;
      }
      protected set
      {
        if (_canFetch != value)
        {
          _canFetch = value;
          OnPropertyChanged(nameof(CanFetch));
        }
      }
    }

    private bool _canRemove = false;

    /// <summary>
    /// Gets a value indicating whether the Model
    /// can currently be removed.
    /// </summary>
    public virtual bool CanRemove
    {
      get
      {
        return _canRemove;
      }
      protected set
      {
        if (_canRemove != value)
        {
          _canRemove = value;
          OnPropertyChanged(nameof(CanRemove));
        }
      }
    }

    private bool _canAddNew = false;

    /// <summary>
    /// Gets a value indicating whether the Model
    /// can currently be added.
    /// </summary>
    public virtual bool CanAddNew
    {
      get
      {
        return _canAddNew;
      }
      protected set
      {
        if (_canAddNew != value)
        {
          _canAddNew = value;
          OnPropertyChanged(nameof(CanAddNew));
        }
      }
    }

    private void SetProperties()
    {
      bool isObjectBusy = false;
      if (Model is INotifyBusy busyObject && busyObject.IsBusy)
        isObjectBusy = true;

      // Does Model instance implement ITrackStatus 
      if (Model is ITrackStatus targetObject)
      {
        var canDeleteInstance = BusinessRules.HasPermission(AuthorizationActions.DeleteObject, targetObject);

        IsDirty = targetObject.IsDirty;
        IsValid = targetObject.IsValid;
        CanSave = CanEditObject && targetObject.IsSavable && !isObjectBusy;
        CanCancel = CanEditObject && targetObject.IsDirty && !isObjectBusy;
        CanCreate = CanCreateObject && !targetObject.IsDirty && !isObjectBusy;
        CanDelete = CanDeleteObject && !isObjectBusy && canDeleteInstance;
        CanFetch = CanGetObject && !targetObject.IsDirty && !isObjectBusy;

        // Set properties for List 
        if (Model is ICollection list)
        {
          Type itemType = Utilities.GetChildItemType(Model.GetType());
          if (itemType == null)
          {
            CanAddNew = false;
            CanRemove = false;
          }
          else
          {
            CanRemove = BusinessRules.HasPermission(AuthorizationActions.DeleteObject, itemType) &&
                        list.Count > 0 && !isObjectBusy;
            CanAddNew = BusinessRules.HasPermission(AuthorizationActions.CreateObject, itemType) &&
                        !isObjectBusy;
          }
        }
        else 
        {
          CanRemove = false;
          CanAddNew = false;
        }
      }

      // Else if Model instance implement ICollection
      else if (Model is ICollection list)
      {
        Type itemType = Utilities.GetChildItemType(Model.GetType());
        if (itemType == null)
        {
          CanAddNew = false;
          CanRemove = false;
        }
        else
        {
          CanRemove = BusinessRules.HasPermission(AuthorizationActions.DeleteObject, itemType) &&
                      list.Count > 0 && !isObjectBusy;
          CanAddNew = BusinessRules.HasPermission(AuthorizationActions.CreateObject, itemType) &&
                      !isObjectBusy;
        }
      }
      else
      {
        IsDirty = false;
        IsValid = false;
        CanCancel = false;
        CanCreate = CanCreateObject;
        CanDelete = false;
        CanFetch = CanGetObject && !IsBusy;
        CanSave = false;
        CanRemove = false;
        CanAddNew = false;
      }
    }

    private bool _canCreateObject;

    /// <summary>
    /// Gets a value indicating whether the current
    /// user is authorized to create a Model.
    /// </summary>
    public virtual bool CanCreateObject
    {
      get { return _canCreateObject; }
      protected set
      {
        if (_canCreateObject != value)
        {
          _canCreateObject = value;
          OnPropertyChanged(nameof(CanCreateObject));
        }
      }
    }

    private bool _canGetObject;

    /// <summary>
    /// Gets a value indicating whether the current
    /// user is authorized to retrieve a Model.
    /// </summary>
    public virtual bool CanGetObject
    {
      get { return _canGetObject; }
      protected set
      {
        if (_canGetObject != value)
        {
          _canGetObject = value;
          OnPropertyChanged(nameof(CanGetObject));
        }
      }
    }

    private bool _canEditObject;

    /// <summary>
    /// Gets a value indicating whether the current
    /// user is authorized to save (insert or update
    /// a Model.
    /// </summary>
    public virtual bool CanEditObject
    {
      get { return _canEditObject; }
      protected set
      {
        if (_canEditObject != value)
        {
          _canEditObject = value;
          OnPropertyChanged(nameof(CanEditObject));
        }
      }
    }

    private bool _canDeleteObject;

    /// <summary>
    /// Gets a value indicating whether the current
    /// user is authorized to delete
    /// a Model.
    /// </summary>
    public virtual bool CanDeleteObject
    {
      get { return _canDeleteObject; }
      protected set
      {
        if (_canDeleteObject != value)
        {
          _canDeleteObject = value;
          OnPropertyChanged(nameof(CanDeleteObject));
        }
      }
    }

    /// <summary>
    /// This method is only called from constuctor to set default values immediately.
    /// Sets the properties at object level.
    /// </summary>
    private void SetPropertiesAtObjectLevel()
    {
      Type sourceType = typeof(T);

      CanCreateObject = BusinessRules.HasPermission(Rules.AuthorizationActions.CreateObject, sourceType);
      CanGetObject = BusinessRules.HasPermission(Rules.AuthorizationActions.GetObject, sourceType);
      CanEditObject = BusinessRules.HasPermission(Rules.AuthorizationActions.EditObject, sourceType);
      CanDeleteObject = BusinessRules.HasPermission(Rules.AuthorizationActions.DeleteObject, sourceType);

      // call SetProperties to set "instance" values 
      OnSetProperties();
    }

    /// <summary>
    /// Creates or retrieves a new instance of the 
    /// Model by invoking an action.
    /// </summary>
    /// <param name="factory">Factory method to invoke</param>
    protected virtual async Task<T> RefreshAsync<F>(Func<Task<T>> factory)
    {
      T result = default;
      try
      {
        IsBusy = true;
        result = await factory.Invoke();
        Model = result;
      }
      finally
      {
        IsBusy = false;
      }
      return result;
    }

    /// <summary>
    /// Saves the Model, first committing changes
    /// if ManagedObjectLifetime is true.
    /// </summary>
    protected virtual async Task<T> SaveAsync()
    {
      try
      {
        UnhookChangedEvents(Model);
        var savable = Model as ISavable;
        if (ManageObjectLifetime)
        {
          // clone the object if possible
          if (Model is ICloneable clonable)
            savable = (ISavable)clonable.Clone();

          //apply changes
          if (savable is ISupportUndo undoable)
            undoable.ApplyEdit();
        }

        IsBusy = true;
        Model = (T) await savable.SaveAsync();
      }
      finally
      {
        HookChangedEvents(Model);
        IsBusy = false;
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
        if (Model is ISupportUndo undo)
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
            OnSetProperties();
          }
        }
      }
    }

#if (ANDROID || IOS) || XAMARIN
    /// <summary>
    /// Adds a new item to the Model (if it
    /// is a collection).
    /// </summary>
    protected virtual void BeginAddNew()
    {
#if ANDROID || IOS
      var ibl = (Model as System.ComponentModel.IBindingList);
#else
      var ibl = (Model as IBindingList);
#endif
      if (ibl != null)
      {
        ibl.AddNew();
      }
      else
      {
        // else try to use as IObservableBindingList
        var iobl = ((IObservableBindingList)Model);
        iobl.AddNew();
      }
      OnSetProperties();
    }
#else
    /// <summary>
    /// Adds a new item to the Model (if it
    /// is a collection).
    /// </summary>
    protected virtual object DoAddNew()
    {
      object result;
      // typically use ObserableCollection 
      if (Model is IObservableBindingList iobl)
      {
        result = iobl.AddNew();
      }
      else
      {
        // else try to use as BindingList
        var ibl = ((IBindingList)Model);
        result = ibl.AddNew();
      }
      OnSetProperties();
      return result;
    }
#endif

    /// <summary>
    /// Removes an item from the Model (if it
    /// is a collection).
    /// </summary>
    protected virtual void DoRemove(object item)
    {
      ((IList)Model).Remove(item);
      OnSetProperties();
    }

    /// <summary>
    /// Marks the Model for deletion (if it is an
    /// editable root object).
    /// </summary>
    protected virtual void DoDelete()
    {
      ((IEditableBusinessObject)Model).Delete();
    }

    /// <summary>
    /// Invoked when the Model changes, allowing
    /// event handlers to be unhooked from the old
    /// object and hooked on the new object.
    /// </summary>
    /// <param name="oldValue">Previous Model reference.</param>
    /// <param name="newValue">New Model reference.</param>
    protected virtual void OnModelChanged(T oldValue, T newValue)
    {
      if (ReferenceEquals(oldValue, newValue)) return;

      if (ManageObjectLifetime && newValue is ISupportUndo undo)
        undo.BeginEdit();

      // unhook events from old value
      if (oldValue != null)
      {
        UnhookChangedEvents(oldValue);

        if (oldValue is INotifyBusy nb)
          nb.BusyChanged -= Model_BusyChanged;
      }

      // hook events on new value
      if (newValue != null)
      {
        HookChangedEvents(newValue);

        if (newValue is INotifyBusy nb)
          nb.BusyChanged += Model_BusyChanged;
      }

      OnSetProperties();
    }

    /// <summary>
    /// Unhooks changed event handlers from the model.
    /// </summary>
    /// <param name="model"></param>
    protected void UnhookChangedEvents(T model)
    {
      if (model is INotifyPropertyChanged npc)
        npc.PropertyChanged -= Model_PropertyChanged;

      if (model is INotifyChildChanged ncc)
        ncc.ChildChanged -= Model_ChildChanged;

      if (model is INotifyCollectionChanged cc)
        cc.CollectionChanged -= Model_CollectionChanged;
    }

    /// <summary>
    /// Hooks changed events on the model.
    /// </summary>
    /// <param name="model"></param>
    private void HookChangedEvents(T model)
    {
      if (model is INotifyPropertyChanged npc)
        npc.PropertyChanged += Model_PropertyChanged;

      if (model is INotifyChildChanged ncc)
        ncc.ChildChanged += Model_ChildChanged;

      if (model is INotifyCollectionChanged cc)
        cc.CollectionChanged += Model_CollectionChanged;
    }

    /// <summary>
    /// Override this method to hook into to logic of setting 
    /// properties when model is changed or edited. 
    /// </summary>
    protected virtual void OnSetProperties()
    {
      SetProperties();
    }

    private void Model_BusyChanged(object sender, BusyChangedEventArgs e)
    {
      // only set busy state for entire object.  Ignore busy state based
      // on asynch rules being active
      if (string.IsNullOrEmpty(e.PropertyName))
        IsBusy = e.Busy;
      else
        OnSetProperties();
    }

    private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      OnSetProperties();
    }

    private void Model_ChildChanged(object sender, ChildChangedEventArgs e)
    {
      OnSetProperties();
    }

    private void Model_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      OnSetProperties();
    }

    object IViewModel.Model
    {
      get { return Model; }
      set { Model = (T)value; }
    }

    /// <summary>
    /// Event raised when a property changes.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Raise the PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">Name of the changed property.</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
  }
}