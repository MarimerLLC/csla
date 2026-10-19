//-----------------------------------------------------------------------
// <copyright file="ViewModel.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Base type for creating a viewmodel that exposes a CSLA Model</summary>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Csla.Core;
using Csla.Rules;

namespace Csla.Mvvm
{
  /// <summary>
  /// Base type for creating a viewmodel that exposes a CSLA
  /// business object graph through its <see cref="Model"/> property.
  /// Provides behaviors common to all UI frameworks: model lifetime
  /// management, save/refresh/cancel, error capture, and
  /// authorization-based status properties.
  /// </summary>
  /// <typeparam name="T">Type of the Model object.</typeparam>
  public class ViewModel<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T> : INotifyPropertyChanged
  {
    private ApplicationContext? _applicationContext;

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="applicationContext">The current ApplicationContext.</param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> is <see langword="null"/>.</exception>
    public ViewModel(ApplicationContext applicationContext)
      : this(applicationContext, false)
    { }

    /// <summary>
    /// Creates an instance of the type with an initial
    /// <see cref="ManageObjectLifetime"/> value. No
    /// PropertyChanged event is raised for that value.
    /// </summary>
    /// <param name="applicationContext">The current ApplicationContext.</param>
    /// <param name="manageObjectLifetime">Initial value of <see cref="ManageObjectLifetime"/>.</param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> is <see langword="null"/>.</exception>
    protected ViewModel(ApplicationContext applicationContext, bool manageObjectLifetime)
    {
      _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
      _manageObjectLifetime = manageObjectLifetime;
    }

    /// <summary>
    /// Creates an instance of the type that resolves its
    /// ApplicationContext on first use by calling
    /// <see cref="ResolveApplicationContext"/>.
    /// </summary>
    protected ViewModel()
    { }

    /// <summary>
    /// Creates an instance of the type with an initial
    /// <see cref="ManageObjectLifetime"/> value, resolving its
    /// ApplicationContext on first use by calling
    /// <see cref="ResolveApplicationContext"/>. No
    /// PropertyChanged event is raised for that value.
    /// </summary>
    /// <param name="manageObjectLifetime">Initial value of <see cref="ManageObjectLifetime"/>.</param>
    protected ViewModel(bool manageObjectLifetime)
    {
      _manageObjectLifetime = manageObjectLifetime;
    }

    /// <summary>
    /// Gets the current ApplicationContext instance.
    /// </summary>
    protected ApplicationContext ApplicationContext => _applicationContext ??= ResolveApplicationContext();

    /// <summary>
    /// Override to provide the ApplicationContext when the
    /// viewmodel was created with the parameterless constructor.
    /// </summary>
    /// <exception cref="InvalidOperationException">Always thrown by the default implementation.</exception>
    protected virtual ApplicationContext ResolveApplicationContext()
    {
      throw new InvalidOperationException(
        $"{GetType().Name} was created without an {nameof(Csla.ApplicationContext)}; use the constructor that accepts one, or override {nameof(ResolveApplicationContext)}.");
    }

    #region Events

    /// <summary>
    /// Event raised when a property changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Event raised after Model has been saved.
    /// </summary>
    public event Action? Saved;

    /// <summary>
    /// Event raised when a refresh or save operation fails,
    /// or when an authorization rule used to calculate the
    /// status properties throws an exception.
    /// </summary>
    public event EventHandler<Core.ErrorEventArgs>? Error;

    /// <summary>
    /// Event raised when Model is changing.
    /// </summary>
    public event Action<T?, T?>? ModelChanging;

    /// <summary>
    /// Event raised when Model has changed.
    /// </summary>
    public event Action? ModelChanged;

    /// <summary>
    /// Event raised when the Model object
    /// raises its PropertyChanged event.
    /// </summary>
    public event PropertyChangedEventHandler? ModelPropertyChanged;

    /// <summary>
    /// Event raised when the Model object
    /// raises its ChildChanged event.
    /// </summary>
    public event Action<object, ChildChangedEventArgs>? ModelChildChanged;

    /// <summary>
    /// Event raised when the Model object
    /// raises its CollectionChanged event.
    /// </summary>
    public event Action<object, NotifyCollectionChangedEventArgs>? ModelCollectionChanged;

    /// <summary>
    /// Raises the PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">Name of the changed property.</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Invoked before the Model changes. Unhooks event handlers
    /// from the old object, hooks them on the new object, and
    /// raises the ModelChanging event.
    /// </summary>
    /// <param name="oldValue">Previous Model reference.</param>
    /// <param name="newValue">New Model reference.</param>
    protected virtual void OnModelChanging(T? oldValue, T? newValue)
    {
      if (ReferenceEquals(oldValue, newValue))
        return;

      if (ManageObjectLifetime && newValue is ISupportUndo undo)
        undo.BeginEdit();

      // unhook events from old value
      if (oldValue != null)
      {
        UnhookChangedEvents(oldValue);

        if (oldValue is INotifyBusy nb)
          nb.BusyChanged -= OnModelBusyChanged;
      }

      // hook events on new value
      if (newValue != null)
      {
        HookChangedEvents(newValue);

        if (newValue is INotifyBusy nb)
          nb.BusyChanged += OnModelBusyChanged;
      }

      ModelChanging?.Invoke(oldValue, newValue);
    }

    /// <summary>
    /// Invoked after the Model has changed. Raises the
    /// ModelChanged event.
    /// </summary>
    /// <param name="oldValue">Previous Model reference.</param>
    /// <param name="newValue">New Model reference.</param>
    protected virtual void OnModelChanged(T? oldValue, T? newValue)
    {
      ModelChanged?.Invoke();
    }

    /// <summary>
    /// Unhooks changed event handlers from the model.
    /// </summary>
    /// <param name="model">Model object.</param>
    protected void UnhookChangedEvents(T? model)
    {
      if (model is INotifyPropertyChanged npc)
        npc.PropertyChanged -= OnModelPropertyChanged;

      if (model is INotifyChildChanged ncc)
        ncc.ChildChanged -= OnModelChildChanged;

      if (model is INotifyCollectionChanged cc)
        cc.CollectionChanged -= OnModelCollectionChanged;
    }

    /// <summary>
    /// Hooks changed event handlers on the model.
    /// </summary>
    /// <param name="model">Model object.</param>
    protected void HookChangedEvents(T? model)
    {
      if (model is INotifyPropertyChanged npc)
        npc.PropertyChanged += OnModelPropertyChanged;

      if (model is INotifyChildChanged ncc)
        ncc.ChildChanged += OnModelChildChanged;

      if (model is INotifyCollectionChanged cc)
        cc.CollectionChanged += OnModelCollectionChanged;
    }

    private void OnModelBusyChanged(object sender, BusyChangedEventArgs e)
    {
      // only set busy state for entire object. Ignore busy state based
      // on async rules being active
      if (string.IsNullOrEmpty(e.PropertyName))
        SetBusy(ref _isModelBusy, e.Busy);
      else
        OnSetProperties();
    }

    /// <summary>
    /// Invoked when the Model raises PropertyChanged;
    /// raises the ModelPropertyChanged event.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    protected virtual void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
      OnSetProperties();
      ModelPropertyChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Invoked when the Model raises ChildChanged;
    /// raises the ModelChildChanged event.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    protected virtual void OnModelChildChanged(object? sender, ChildChangedEventArgs e)
    {
      OnSetProperties();
      ModelChildChanged?.Invoke(this, e);
    }

    /// <summary>
    /// Invoked when the Model raises CollectionChanged;
    /// raises the ModelCollectionChanged event.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    protected virtual void OnModelCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
      OnSetProperties();
      ModelCollectionChanged?.Invoke(this, e);
    }

    #endregion

    #region Model and status properties

    private T? _model;
    /// <summary>
    /// Gets or sets the Model object.
    /// </summary>
    public T? Model
    {
      get => _model;
      set
      {
        if (!ReferenceEquals(_model, value))
        {
          var oldValue = _model;
          OnModelChanging(oldValue, value);
          _model = value;
          OnModelChanged(oldValue, value);
          OnPropertyChanged(nameof(Model));
          // the busy state reported by the old model no longer applies
          if (!SetBusy(ref _isModelBusy, false))
            OnSetProperties();
        }
      }
    }

    private bool _manageObjectLifetime;
    /// <summary>
    /// Gets or sets a value indicating whether the
    /// viewmodel should manage the lifetime of
    /// the business object, including using n-level
    /// undo.
    /// </summary>
    [Display(AutoGenerateField = false)]
    [ScaffoldColumn(false)]
    public bool ManageObjectLifetime
    {
      get => _manageObjectLifetime;
      set
      {
        if (_manageObjectLifetime != value)
        {
          _manageObjectLifetime = value;
          OnPropertyChanged(nameof(ManageObjectLifetime));
        }
      }
    }

    /// <summary>
    /// Gets or sets a value specifying the timeout
    /// when calling SaveAsync and the Model is
    /// currently busy with async rules.
    /// </summary>
    public TimeSpan BusyTimeout { get; set; } = TimeSpan.FromSeconds(30);

    private bool _isBusy;
    private bool _isModelBusy;
    /// <summary>
    /// Gets a value indicating whether this object is
    /// executing an asynchronous process, or the Model
    /// reports that it is busy.
    /// </summary>
    public bool IsBusy
    {
      get => _isBusy || _isModelBusy;
      protected set => SetBusy(ref _isBusy, value);
    }

    // the viewmodel's own operations and the Model's BusyChanged event
    // have separate flags, so the Model becoming idle part way through
    // a save does not clear the viewmodel's busy state
    private bool SetBusy(ref bool field, bool value)
    {
      if (field == value)
        return false;
      var wasBusy = IsBusy;
      field = value;
      if (wasBusy == IsBusy)
        return false;
      OnPropertyChanged(nameof(IsBusy));
      OnSetProperties();
      return true;
    }

    private bool _isDirty;
    /// <summary>
    /// Gets a value indicating whether the Model
    /// has been changed.
    /// </summary>
    public virtual bool IsDirty
    {
      get => _isDirty;
      protected set => SetStatus(ref _isDirty, value, nameof(IsDirty));
    }

    private bool _isValid;
    /// <summary>
    /// Gets a value indicating whether the Model
    /// is currently valid (has no broken rules).
    /// </summary>
    public virtual bool IsValid
    {
      get => _isValid;
      protected set => SetStatus(ref _isValid, value, nameof(IsValid));
    }

    private bool _canSave;
    /// <summary>
    /// Gets a value indicating whether the Model
    /// can currently be saved.
    /// </summary>
    public virtual bool CanSave
    {
      get => _canSave;
      protected set => SetStatus(ref _canSave, value, nameof(CanSave));
    }

    private bool _canCancel;
    /// <summary>
    /// Gets a value indicating whether the Model
    /// can currently be canceled.
    /// </summary>
    public virtual bool CanCancel
    {
      get => _canCancel;
      protected set => SetStatus(ref _canCancel, value, nameof(CanCancel));
    }

    private bool _canCreate;
    /// <summary>
    /// Gets a value indicating whether an instance
    /// of the Model can currently be created.
    /// </summary>
    public virtual bool CanCreate
    {
      get => _canCreate;
      protected set => SetStatus(ref _canCreate, value, nameof(CanCreate));
    }

    private bool _canDelete;
    /// <summary>
    /// Gets a value indicating whether the Model
    /// can currently be deleted.
    /// </summary>
    public virtual bool CanDelete
    {
      get => _canDelete;
      protected set => SetStatus(ref _canDelete, value, nameof(CanDelete));
    }

    private bool _canFetch;
    /// <summary>
    /// Gets a value indicating whether an instance
    /// of the Model can currently be retrieved.
    /// </summary>
    public virtual bool CanFetch
    {
      get => _canFetch;
      protected set => SetStatus(ref _canFetch, value, nameof(CanFetch));
    }

    private bool _canRemove;
    /// <summary>
    /// Gets a value indicating whether an item
    /// can currently be removed from the Model.
    /// </summary>
    public virtual bool CanRemove
    {
      get => _canRemove;
      protected set => SetStatus(ref _canRemove, value, nameof(CanRemove));
    }

    private bool _canAddNew;
    /// <summary>
    /// Gets a value indicating whether an item
    /// can currently be added to the Model.
    /// </summary>
    public virtual bool CanAddNew
    {
      get => _canAddNew;
      protected set => SetStatus(ref _canAddNew, value, nameof(CanAddNew));
    }

    private void SetStatus(ref bool field, bool value, string propertyName)
    {
      if (field != value)
      {
        field = value;
        OnPropertyChanged(propertyName);
      }
    }

    #endregion

    #region Status calculation

    private int _setPropertiesVersion;

    /// <summary>
    /// Override this method to hook into to logic of setting
    /// properties when model is changed or edited.
    /// </summary>
    /// <remarks>
    /// Authorization rules are evaluated with
    /// <see cref="BusinessRules.HasPermissionAsync(ApplicationContext, AuthorizationActions, object, CancellationToken)"/>.
    /// When a rule is asynchronous, the permission-based properties
    /// (such as CanDelete or CanAddNew) may be updated after this
    /// method returns; PropertyChanged is raised when they change.
    /// If a rule throws, the exception is reported through
    /// <see cref="Exception"/>, <see cref="ViewModelErrorText"/> and
    /// <see cref="Error"/>, and the status properties keep their
    /// previous values.
    /// </remarks>
    protected virtual void OnSetProperties()
    {
      SetProperties();
    }

    private void SetProperties()
    {
      // resolve the context first, so a missing ApplicationContext
      // throws to the caller instead of being reported as a rule failure
      _ = ApplicationContext;
      ObserveTask(SetPropertiesAsync());
    }

    private bool _statusCalculationFailed;

    private async Task SetPropertiesAsync()
    {
      try
      {
        if (await CalculateStatusAsync())
          _statusCalculationFailed = false;
      }
      catch (Exception ex)
      {
        // the calculation runs on every model change, so a rule that keeps
        // failing is reported when it starts failing, not on every change
        if (!_statusCalculationFailed)
        {
          _statusCalculationFailed = true;
          ReportError(ex);
        }
      }
    }

    // returns false if a newer calculation superseded this one
    private async Task<bool> CalculateStatusAsync()
    {
      var version = ++_setPropertiesVersion;
      var model = Model;

      // evaluate authorization rules first; when all rules are synchronous
      // these tasks are already complete and no thread switch occurs
      var canDeleteInstance = false;
      var canRemoveItem = false;
      var canAddNewItem = false;
      Type? itemType = null;
      if (model is ITrackStatus targetObject)
        canDeleteInstance = await BusinessRules.HasPermissionAsync(ApplicationContext, AuthorizationActions.DeleteObject, targetObject, CancellationToken.None);
      if (model is ICollection)
      {
        itemType = GetChildItemType(model.GetType());
        if (itemType != null)
        {
          canRemoveItem = await BusinessRules.HasPermissionAsync(ApplicationContext, AuthorizationActions.DeleteObject, itemType, CancellationToken.None);
          canAddNewItem = await BusinessRules.HasPermissionAsync(ApplicationContext, AuthorizationActions.CreateObject, itemType, CancellationToken.None);
        }
      }

      // a newer call has started (or the model changed) while awaiting
      if (version != _setPropertiesVersion || !ReferenceEquals(model, Model))
        return false;

      // the viewmodel is busy while it saves a clone of the model,
      // even though the model itself is idle
      var isObjectBusy = IsBusy || (Model is INotifyBusy busyObject && busyObject.IsBusy);

      if (Model is ITrackStatus trackStatus)
      {
        IsDirty = trackStatus.IsDirty;
        IsValid = trackStatus.IsValid;
        CanSave = CanEditObject && trackStatus.IsSavable && !isObjectBusy;
        CanCancel = CanEditObject && trackStatus.IsDirty && !isObjectBusy;
        CanCreate = CanCreateObject && !trackStatus.IsDirty && !isObjectBusy;
        CanDelete = CanDeleteObject && !isObjectBusy && canDeleteInstance;
        CanFetch = CanGetObject && !trackStatus.IsDirty && !isObjectBusy;
        SetListProperties(itemType, canRemoveItem, canAddNewItem, isObjectBusy);
      }
      else if (Model is ICollection)
      {
        SetListProperties(itemType, canRemoveItem, canAddNewItem, isObjectBusy);
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
      return true;
    }

    private Type? _childItemTypeSource;
    private Type? _childItemType;

    // the status calculation runs on every model change, so cache
    // the reflection-based lookup for the current Model type
    private Type? GetChildItemType(Type modelType)
    {
      if (modelType != _childItemTypeSource)
      {
        _childItemType = Utilities.GetChildItemType(modelType);
        _childItemTypeSource = modelType;
      }
      return _childItemType;
    }

    private void SetListProperties(Type? itemType, bool canRemoveItem, bool canAddNewItem, bool isObjectBusy)
    {
      if (Model is ICollection list && itemType != null)
      {
        CanRemove = canRemoveItem && list.Count > 0 && !isObjectBusy;
        CanAddNew = canAddNewItem && !isObjectBusy;
      }
      else
      {
        CanRemove = false;
        CanAddNew = false;
      }
    }

    #endregion

    #region Object-level permissions

    private bool _canCreateObject;
    /// <summary>
    /// Gets a value indicating whether the current user
    /// is authorized to create an instance of the
    /// business domain type.
    /// </summary>
    public virtual bool CanCreateObject
    {
      get
      {
        SetPropertiesAtObjectLevel();
        return _canCreateObject;
      }
      protected set => SetStatus(ref _canCreateObject, value, nameof(CanCreateObject));
    }

    private bool _canGetObject;
    /// <summary>
    /// Gets a value indicating whether the current user
    /// is authorized to retrieve an instance of the
    /// business domain type.
    /// </summary>
    public virtual bool CanGetObject
    {
      get
      {
        SetPropertiesAtObjectLevel();
        return _canGetObject;
      }
      protected set => SetStatus(ref _canGetObject, value, nameof(CanGetObject));
    }

    private bool _canEditObject;
    /// <summary>
    /// Gets a value indicating whether the current user
    /// is authorized to edit/save an instance of the
    /// business domain type.
    /// </summary>
    public virtual bool CanEditObject
    {
      get
      {
        SetPropertiesAtObjectLevel();
        return _canEditObject;
      }
      protected set => SetStatus(ref _canEditObject, value, nameof(CanEditObject));
    }

    private bool _canDeleteObject;
    /// <summary>
    /// Gets a value indicating whether the current user
    /// is authorized to delete an instance of the
    /// business domain type.
    /// </summary>
    public virtual bool CanDeleteObject
    {
      get
      {
        SetPropertiesAtObjectLevel();
        return _canDeleteObject;
      }
      protected set => SetStatus(ref _canDeleteObject, value, nameof(CanDeleteObject));
    }

    private bool _objectLevelPermissionsRequested;
    private Task? _objectLevelPermissionsTask;

    /// <summary>
    /// Sets the properties at object level, evaluating
    /// authorization rules only once.
    /// </summary>
    /// <remarks>
    /// When all authorization rules are synchronous the values are
    /// set before this method returns. If a rule is asynchronous the
    /// values are set when it completes, and PropertyChanged is raised.
    /// </remarks>
    private void SetPropertiesAtObjectLevel()
    {
      if (_objectLevelPermissionsRequested)
        return;
      ObserveTask(EnsureObjectLevelPermissionsAsync(retryFailed: false));
    }

    private async void ObserveTask(Task task)
    {
      // a failure is reported through Exception and Error rather than
      // posted to the SynchronizationContext, so it behaves the same on
      // every host. When the task has already completed (all rules are
      // synchronous) this runs before the caller continues.
      try
      {
        await task;
      }
      catch (Exception ex)
      {
        ReportError(ex);
      }
    }

    // retryFailed starts a new evaluation if the previous one failed. Only
    // explicit operations retry: property reads do not, so a rule that keeps
    // failing is not re-evaluated (and reported) on every read.
    private Task EnsureObjectLevelPermissionsAsync(bool retryFailed)
    {
      if (!_objectLevelPermissionsRequested || (retryFailed && _objectLevelPermissionsTask is { IsFaulted: true }))
      {
        // resolve the context first, so a missing ApplicationContext
        // throws to the caller instead of being reported as a rule failure
        var applicationContext = ApplicationContext;
        // set the flag first: the property setters raise PropertyChanged,
        // and handlers may read the permission properties re-entrantly
        _objectLevelPermissionsRequested = true;
        _objectLevelPermissionsTask = LoadObjectLevelPermissionsAsync(applicationContext);
      }
      return _objectLevelPermissionsTask ?? Task.CompletedTask;
    }

    private async Task LoadObjectLevelPermissionsAsync(ApplicationContext applicationContext)
    {
      var sourceType = typeof(T);

      CanCreateObject = await BusinessRules.HasPermissionAsync(applicationContext, AuthorizationActions.CreateObject, sourceType, CancellationToken.None);
      CanGetObject = await BusinessRules.HasPermissionAsync(applicationContext, AuthorizationActions.GetObject, sourceType, CancellationToken.None);
      CanEditObject = await BusinessRules.HasPermissionAsync(applicationContext, AuthorizationActions.EditObject, sourceType, CancellationToken.None);
      CanDeleteObject = await BusinessRules.HasPermissionAsync(applicationContext, AuthorizationActions.DeleteObject, sourceType, CancellationToken.None);

      // set "instance" values now that object-level values are known
      OnSetProperties();
    }

    #endregion

    #region Errors

    /// <summary>
    /// Gets any error text generated by refresh or save operations,
    /// or by the authorization rules used to calculate the status
    /// properties.
    /// </summary>
    public string ViewModelErrorText { get; protected set; } = string.Empty;

    /// <summary>
    /// Gets the first validation error
    /// message from the Model.
    /// </summary>
    protected virtual string ModelErrorText
    {
      get
      {
        if (Model is IDataErrorInfo obj)
          return obj.Error;
        return string.Empty;
      }
    }

    /// <summary>
    /// Gets the last exception caught by
    /// the viewmodel during refresh or save
    /// operations, or while evaluating the
    /// authorization rules used to calculate
    /// the status properties.
    /// </summary>
    public Exception? Exception { get; private set; }

    private void SetError(Exception? exception, string errorText)
    {
      Exception = exception;
      ViewModelErrorText = errorText;
      OnPropertyChanged(nameof(Exception));
      OnPropertyChanged(nameof(ViewModelErrorText));
    }

    private void ReportError(Exception exception)
    {
      // a failed permission load can be observed by both a property
      // read and RefreshAsync; report it once
      if (ReferenceEquals(Exception, exception))
        return;

      var errorText = exception is DataPortalException dpe ? dpe.BusinessExceptionMessage : exception.Message;
      SetError(exception, errorText);
      Error?.Invoke(this, new Core.ErrorEventArgs(this, exception));
    }

    #endregion

    #region Operations

    /// <summary>
    /// Creates or retrieves an instance of the Model by
    /// invoking a factory. Exceptions are captured in
    /// <see cref="Exception"/> and <see cref="ViewModelErrorText"/>,
    /// and the <see cref="Error"/> event is raised.
    /// </summary>
    /// <param name="factory">Async data portal or factory method.</param>
    /// <exception cref="ArgumentNullException"><paramref name="factory"/> is <see langword="null"/>.</exception>
    public virtual async Task<T?> RefreshAsync(Func<Task<T?>> factory)
    {
      if (factory is null)
        throw new ArgumentNullException(nameof(factory));

      SetError(null, string.Empty);
      try
      {
        // start (or retry) the permission load before IsBusy triggers
        // the status calculation, which reads the permissions
        var permissions = EnsureObjectLevelPermissionsAsync(retryFailed: true);
        IsBusy = true;
        await permissions;
        Model = await factory();
      }
      catch (Exception ex)
      {
        Model = default;
        ReportError(ex);
      }
      finally
      {
        IsBusy = false;
      }

      return Model;
    }

    private CancellationToken _busyTimeoutToken;

    /// <summary>
    /// Saves the Model, waiting up to <see cref="BusyTimeout"/>
    /// for the Model to become idle. Exceptions are captured in
    /// <see cref="Exception"/> and <see cref="ViewModelErrorText"/>,
    /// and the <see cref="Error"/> event is raised. If the Model
    /// is still busy after <see cref="BusyTimeout"/>, the exception
    /// is a <see cref="TimeoutException"/>.
    /// </summary>
    public virtual async Task<T?> SaveAsync()
    {
      using var cts = BusyTimeout.ToCancellationTokenSource();
      // lets SaveAsync(CancellationToken) tell the timeout apart from
      // a caller's cancellation, without bypassing an override
      _busyTimeoutToken = cts.Token;
      try
      {
        return await SaveAsync(cts.Token);
      }
      finally
      {
        _busyTimeoutToken = default;
      }
    }

    /// <summary>
    /// Saves the Model. If ManageObjectLifetime is true, a clone
    /// of the Model with its changes committed is saved, and the
    /// Model's changes are committed only if the save succeeds, so
    /// <see cref="DoCancel"/> can still restore the original values
    /// after a failed save. Exceptions are captured in
    /// <see cref="Exception"/> and <see cref="ViewModelErrorText"/>,
    /// and the <see cref="Error"/> event is raised.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    public virtual async Task<T?> SaveAsync(CancellationToken ct)
    {
      SetError(null, string.Empty);
      var model = Model;
      if (model is null)
        return model;

      Exception? error = null;
      try
      {
        if (model is ITrackStatus obj && !obj.IsSavable)
        {
          await WaitForIdleAsync(obj, ct);
          if (!obj.IsValid)
          {
            SetError(null, ModelErrorText);
            return Model;
          }
          else if (!obj.IsSavable)
            throw new InvalidOperationException(
                $"{obj.GetType().Name} IsBusy: {obj.IsBusy}, IsValid: {obj.IsValid}, IsSavable: {obj.IsSavable}");
        }

        UnhookChangedEvents(model);

        var savable = model as ISavable;
        if (ManageObjectLifetime)
        {
          if (savable is ICloneable clonable)
          {
            // save a clone with its changes committed; the Model keeps
            // its snapshot until the save succeeds
            savable = (ISavable)clonable.Clone();
            if (savable is ISupportUndo clonedUndo)
            {
              clonedUndo.ApplyEdit();
              _pendingApplyEdit = model as ISupportUndo;
            }
          }
          else if (model is ISupportUndo undoable)
          {
            undoable.ApplyEdit();
          }
        }

        IsBusy = true;
        Model = await DoSaveAsync(savable);
        // an override may have kept the Model without committing it
        if (ReferenceEquals(Model, model))
          CommitPendingEdit();
        Saved?.Invoke();
      }
      catch (Exception ex)
      {
        error = ex;
      }
      finally
      {
        // after a failed save the Model keeps its snapshot, so Cancel
        // restores the values from before the edit
        _pendingApplyEdit = null;
        if (ManageObjectLifetime && Model is IUndoableObject udbl && udbl.EditLevel == 0 && Model is ISupportUndo undo)
          undo.BeginEdit();

        // handlers may already be attached: the save may have returned
        // before unhooking, or replaced Model (which hooks the new model)
        UnhookChangedEvents(Model);
        HookChangedEvents(Model);
        IsBusy = false;
        OnSetProperties();
        if (error != null)
          ReportError(error);
      }
      return Model;
    }

    private async Task WaitForIdleAsync(ITrackStatus obj, CancellationToken ct)
    {
      try
      {
        if (obj is INotifyBusy notifyBusy)
        {
          await BusyHelper.WaitForIdle(notifyBusy, ct);
        }
        else
        {
          while (obj.IsBusy)
            await Task.Delay(1, ct);
        }
      }
      catch (OperationCanceledException ex) when (ct.IsCancellationRequested && ct == _busyTimeoutToken)
      {
        throw new TimeoutException($"{obj.GetType().FullName}.{nameof(SaveAsync)} - {BusyTimeout}.", ex);
      }
    }

    private ISupportUndo? _pendingApplyEdit;

    // commits the Model's changes once the save of its clone has succeeded
    private void CommitPendingEdit()
    {
      var pending = _pendingApplyEdit;
      _pendingApplyEdit = null;
      pending?.ApplyEdit();
    }

    /// <summary>
    /// Override to provide custom Model save behavior.
    /// </summary>
    /// <param name="cloned">The object to save (a clone of Model if ManageObjectLifetime is true).</param>
    /// <exception cref="InvalidOperationException"><paramref name="cloned"/> is <see langword="null"/> because the Model does not implement <see cref="ISavable"/>.</exception>
    protected virtual async Task<T?> DoSaveAsync(ISavable? cloned)
    {
      if (cloned is null)
        throw new InvalidOperationException($"{typeof(T).Name} does not implement {nameof(ISavable)}.");

      var saved = (T)await cloned.SaveAsync();
      // the save succeeded: commit the Model's changes before merging
      // so it doesn't keep the pre-save snapshot
      CommitPendingEdit();
      if (Model is IEditableBusinessObject editable)
        await new GraphMerger(ApplicationContext).MergeGraphAsync(editable, (IEditableBusinessObject)saved);
      else
        Model = saved;
      return Model;
    }

    /// <summary>
    /// Cancels changes made to the model
    /// if ManageObjectLifetime is true.
    /// </summary>
    protected virtual void DoCancel()
    {
      if (ManageObjectLifetime && Model is ISupportUndo undo)
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

    /// <summary>
    /// Adds a new item to the Model (if it
    /// is a collection).
    /// </summary>
    /// <returns>The new item.</returns>
    protected virtual object DoAddNew()
    {
      object result;
      if (Model is IObservableBindingList iobl)
        result = iobl.AddNew();
      else if (Model is IBindingList ibl)
        result = ibl.AddNew()!;
      else
        throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Properties.Resources.UnsupportedXamlListTypeForModel, nameof(IObservableBindingList), nameof(IBindingList)));

      OnSetProperties();
      return result;
    }

    /// <summary>
    /// Removes an item from the Model (if it
    /// is a collection).
    /// </summary>
    /// <param name="item">Item to remove.</param>
    protected virtual void DoRemove(object? item)
    {
      ThrowIfModelIsNull();
      ((IList)Model).Remove(item);
      OnSetProperties();
    }

    /// <summary>
    /// Marks the Model for deletion (if it is an
    /// editable root object).
    /// </summary>
    protected virtual void DoDelete()
    {
      ThrowIfModelIsNull();
      ((IEditableBusinessObject)Model).Delete();
    }

    [StackTraceHidden, MemberNotNull(nameof(Model))]
    private void ThrowIfModelIsNull()
    {
      _ = Model ?? throw new InvalidOperationException($"{nameof(Model)} == null");
    }

    #endregion
  }
}
