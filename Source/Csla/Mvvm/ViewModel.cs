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
    {
      _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
    }

    /// <summary>
    /// Creates an instance of the type that resolves its
    /// ApplicationContext on first use by calling
    /// <see cref="ResolveApplicationContext"/>.
    /// </summary>
    protected ViewModel()
    { }

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
    /// Event raised when a save operation fails.
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
        IsBusy = e.Busy;
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
    /// <summary>
    /// Gets a value indicating whether this object is
    /// executing an asynchronous process.
    /// </summary>
    public bool IsBusy
    {
      get => _isBusy;
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
    /// </remarks>
    protected virtual void OnSetProperties()
    {
      SetProperties();
    }

    private void SetProperties()
    {
      ObserveTask(SetPropertiesAsync());
    }

    private async Task SetPropertiesAsync()
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
        itemType = Utilities.GetChildItemType(model.GetType());
        if (itemType != null)
        {
          canRemoveItem = await BusinessRules.HasPermissionAsync(ApplicationContext, AuthorizationActions.DeleteObject, itemType, CancellationToken.None);
          canAddNewItem = await BusinessRules.HasPermissionAsync(ApplicationContext, AuthorizationActions.CreateObject, itemType, CancellationToken.None);
        }
      }

      // a newer call has started (or the model changed) while awaiting
      if (version != _setPropertiesVersion || !ReferenceEquals(model, Model))
        return;

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
      var task = EnsureObjectLevelPermissionsAsync();
      if (task.IsCompleted)
        task.GetAwaiter().GetResult(); // surface exceptions from synchronous rules
      else
        ObserveTask(task);
    }

    private static async void ObserveTask(Task task)
    {
      // async void so exceptions surface on the SynchronizationContext
      // (such as a UI thread), just as a synchronous exception would.
      // Without a context an async void exception would crash the
      // process, so it is not rethrown; the status properties are
      // simply left unchanged.
      var context = SynchronizationContext.Current;
      try
      {
        await task;
      }
      catch (Exception) when (context is null)
      { }
    }

    private Task EnsureObjectLevelPermissionsAsync()
    {
      if (!_objectLevelPermissionsRequested)
      {
        // set the flag first: the property setters raise PropertyChanged,
        // and handlers may read the permission properties re-entrantly
        _objectLevelPermissionsRequested = true;
        _objectLevelPermissionsTask = LoadObjectLevelPermissionsAsync();
      }
      return _objectLevelPermissionsTask ?? Task.CompletedTask;
    }

    private async Task LoadObjectLevelPermissionsAsync()
    {
      var sourceType = typeof(T);

      CanCreateObject = await BusinessRules.HasPermissionAsync(ApplicationContext, AuthorizationActions.CreateObject, sourceType, CancellationToken.None);
      CanGetObject = await BusinessRules.HasPermissionAsync(ApplicationContext, AuthorizationActions.GetObject, sourceType, CancellationToken.None);
      CanEditObject = await BusinessRules.HasPermissionAsync(ApplicationContext, AuthorizationActions.EditObject, sourceType, CancellationToken.None);
      CanDeleteObject = await BusinessRules.HasPermissionAsync(ApplicationContext, AuthorizationActions.DeleteObject, sourceType, CancellationToken.None);

      // set "instance" values now that object-level values are known
      OnSetProperties();
    }

    #endregion

    #region Errors

    /// <summary>
    /// Gets any error text generated by refresh or save operations.
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
    /// operations.
    /// </summary>
    public Exception? Exception { get; private set; }

    private void SetError(Exception? exception, string errorText)
    {
      Exception = exception;
      ViewModelErrorText = errorText;
      OnPropertyChanged(nameof(Exception));
      OnPropertyChanged(nameof(ViewModelErrorText));
    }

    #endregion

    #region Operations

    /// <summary>
    /// Creates or retrieves an instance of the Model by
    /// invoking a factory. Exceptions are captured in
    /// <see cref="Exception"/> and <see cref="ViewModelErrorText"/>.
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
        IsBusy = true;
        await EnsureObjectLevelPermissionsAsync();
        Model = await factory();
      }
      catch (DataPortalException ex)
      {
        Model = default;
        SetError(ex, ex.BusinessExceptionMessage);
      }
      catch (Exception ex)
      {
        Model = default;
        SetError(ex, ex.Message);
      }
      finally
      {
        IsBusy = false;
      }

      return Model;
    }

    /// <summary>
    /// Saves the Model, waiting up to <see cref="BusyTimeout"/>
    /// for the Model to become idle. Exceptions are captured in
    /// <see cref="Exception"/> and <see cref="ViewModelErrorText"/>,
    /// and the <see cref="Error"/> event is raised.
    /// </summary>
    public virtual async Task<T?> SaveAsync()
    {
      try
      {
        using var cts = BusyTimeout.ToCancellationTokenSource();
        await SaveAsync(cts.Token);
      }
      catch (TaskCanceledException tcex)
      {
        var timeout = new TimeoutException(nameof(SaveAsync), tcex);
        SetError(timeout, timeout.Message);
      }
      return Model;
    }

    /// <summary>
    /// Saves the Model, first committing changes
    /// if ManageObjectLifetime is true. Exceptions are captured in
    /// <see cref="Exception"/> and <see cref="ViewModelErrorText"/>,
    /// and the <see cref="Error"/> event is raised.
    /// </summary>
    /// <param name="ct">The cancellation token.</param>
    public virtual async Task<T?> SaveAsync(CancellationToken ct)
    {
      SetError(null, string.Empty);
      try
      {
        if (Model is ITrackStatus obj && !obj.IsSavable)
        {
          while (obj.IsBusy)
          {
            ct.ThrowIfCancellationRequested();
            await Task.Delay(1, ct);
          }
          if (!obj.IsValid)
          {
            SetError(null, ModelErrorText);
            return Model;
          }
          else if (!obj.IsSavable)
            throw new InvalidOperationException(
                $"{obj.GetType().Name} IsBusy: {obj.IsBusy}, IsValid: {obj.IsValid}, IsSavable: {obj.IsSavable}");
        }

        UnhookChangedEvents(Model);

        var savable = Model as ISavable;
        if (ManageObjectLifetime)
        {
          // apply changes - must apply edit to Model not clone
          if (Model is ISupportUndo undoable)
            undoable.ApplyEdit();

          // clone the object if possible
          if (savable is ICloneable clonable)
            savable = (ISavable)clonable.Clone();
        }

        IsBusy = true;
        Model = await DoSaveAsync(savable);
        Saved?.Invoke();
      }
      catch (DataPortalException ex)
      {
        SetError(ex, ex.BusinessExceptionMessage);
      }
      catch (Exception ex)
      {
        SetError(ex, ex.Message);
      }
      finally
      {
        if (ManageObjectLifetime && Model is IUndoableObject udbl && udbl.EditLevel == 0 && Model is ISupportUndo undo)
          undo.BeginEdit();

        // handlers may already be attached: the save may have returned
        // before unhooking, or replaced Model (which hooks the new model)
        UnhookChangedEvents(Model);
        HookChangedEvents(Model);
        IsBusy = false;
        OnSetProperties();
        if (Exception != null)
          Error?.Invoke(this, new Core.ErrorEventArgs(this, Exception));
      }
      return Model;
    }

    /// <summary>
    /// Override to provide custom Model save behavior.
    /// </summary>
    /// <param name="cloned">The object to save (a clone of Model if ManageObjectLifetime is true).</param>
    protected virtual async Task<T?> DoSaveAsync(ISavable? cloned)
    {
      if (cloned != null)
      {
        var saved = (T)await cloned.SaveAsync();
        if (Model is IEditableBusinessObject editable)
          await new GraphMerger(ApplicationContext).MergeGraphAsync(editable, (IEditableBusinessObject)saved);
        else
          Model = saved;
      }
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
