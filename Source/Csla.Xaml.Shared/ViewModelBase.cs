//-----------------------------------------------------------------------
// <copyright file="ViewModelBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Base class used to create ViewModel objects that</summary>
//-----------------------------------------------------------------------

using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
#if !MAUI && !AVALONIA
using System.Windows;
#endif
using Csla.Core;

namespace Csla.Xaml
{
#if MAUI || AVALONIA
  /// <summary>
  /// Base class used to create ViewModel objects that
  /// implement their own commands/verbs/actions.
  /// </summary>
  /// <typeparam name="T">Type of the Model object.</typeparam>
  public abstract class ViewModelBase<T> : Csla.Mvvm.ViewModel<T>, IViewModel
  {
    /// <summary>
    /// Creates an instance of the type that resolves
    /// its ApplicationContext through
    /// <see cref="ApplicationContextManager"/>.
    /// </summary>
    protected ViewModelBase()
    {
      ManageObjectLifetime = true;
    }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="applicationContext">The current ApplicationContext.</param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> is <see langword="null"/>.</exception>
    protected ViewModelBase(ApplicationContext applicationContext)
      : base(applicationContext)
    {
      ManageObjectLifetime = true;
    }

    /// <inheritdoc />
    protected override ApplicationContext ResolveApplicationContext() => ApplicationContextManager.GetApplicationContext();

    /// <summary>
    /// Creates or retrieves a new instance of the
    /// Model by invoking an action.
    /// </summary>
    /// <param name="factory">Factory method to invoke</param>
    [Obsolete("Use RefreshAsync(Func<Task<T?>>)")]
    public virtual Task<T?> RefreshAsync<F>(Func<Task<T?>> factory) => RefreshAsync(factory);

#if MAUI
    /// <summary>
    /// Adds a new item to the Model (if it
    /// is a collection).
    /// </summary>
    [Obsolete("Use DoAddNew")]
    protected virtual void BeginAddNew() => DoAddNew();
#endif

    object? IViewModel.Model
    {
      get => Model;
      set => Model = (T?)value;
    }
  }
#else
  /// <summary>
  /// Base class used to create ViewModel objects that
  /// implement their own commands/verbs/actions.
  /// </summary>
  /// <typeparam name="T">Type of the Model object.</typeparam>
  /// <remarks>
  /// This type is a <see cref="DependencyObject"/>, so it
  /// delegates its behavior to a <see cref="Csla.Mvvm.ViewModel{T}"/>
  /// instance rather than inheriting from it.
  /// </remarks>
  public abstract class ViewModelBase<T> : DependencyObject,
    INotifyPropertyChanged, IViewModel
  {
    private readonly InnerViewModel _inner;

    /// <summary>
    /// Creates an instance of the type that resolves
    /// its ApplicationContext through
    /// <see cref="ApplicationContextManager"/>.
    /// </summary>
    protected ViewModelBase()
    {
      _inner = new InnerViewModel(this);
      Initialize();
    }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="applicationContext">The current ApplicationContext.</param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> is <see langword="null"/>.</exception>
    protected ViewModelBase(ApplicationContext applicationContext)
    {
      _inner = new InnerViewModel(this, applicationContext);
      Initialize();
    }

    private void Initialize()
    {
      _inner.ManageObjectLifetime = ManageObjectLifetime;
      _inner.Error += (_, e) => Error?.Invoke(this, new Core.ErrorEventArgs(this, e.Error));
    }

    /// <summary>
    /// Gets the current ApplicationContext instance.
    /// </summary>
    protected ApplicationContext ApplicationContext => _inner.Context;

    #region Model and ManageObjectLifetime

    /// <summary>
    /// Gets or sets the Model object.
    /// </summary>
    public static readonly DependencyProperty ModelProperty =
        DependencyProperty.Register(nameof(Model), typeof(T), typeof(ViewModelBase<T>),
        new PropertyMetadata((o, e) => ((ViewModelBase<T>)o)._inner.Model = (T?)e.NewValue));

    /// <summary>
    /// Gets or sets the Model object.
    /// </summary>
    public T? Model
    {
      get => (T?)GetValue(ModelProperty);
      set => SetValue(ModelProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether the
    /// ViewModel should automatically managed the
    /// lifetime of the Model.
    /// </summary>
    public static readonly DependencyProperty ManageObjectLifetimeProperty =
        DependencyProperty.Register(nameof(ManageObjectLifetime), typeof(bool),
        typeof(ViewModelBase<T>), new PropertyMetadata(true,
          (o, e) => ((ViewModelBase<T>)o)._inner.ManageObjectLifetime = (bool)e.NewValue));

    /// <summary>
    /// Gets or sets a value indicating whether the
    /// ViewModel should automatically manage the
    /// lifetime of the Model.
    /// </summary>
    [Display(AutoGenerateField = false)]
    [ScaffoldColumn(false)]
    public bool ManageObjectLifetime
    {
      get => (bool)GetValue(ManageObjectLifetimeProperty);
      set => SetValue(ManageObjectLifetimeProperty, value);
    }

    object? IViewModel.Model
    {
      get => Model;
      set => Model = (T?)value;
    }

    #endregion

    #region Status properties

    /// <inheritdoc cref="Csla.Mvvm.ViewModel{T}.BusyTimeout"/>
    public TimeSpan BusyTimeout
    {
      get => _inner.BusyTimeout;
      set => _inner.BusyTimeout = value;
    }

    /// <inheritdoc cref="Csla.Mvvm.ViewModel{T}.IsBusy"/>
    public bool IsBusy
    {
      get => _inner.IsBusy;
      protected set => _inner.SetIsBusy(value);
    }

    /// <inheritdoc cref="Csla.Mvvm.ViewModel{T}.IsDirty"/>
    public virtual bool IsDirty
    {
      get => _inner.BaseIsDirty;
      protected set => _inner.BaseIsDirty = value;
    }

    /// <inheritdoc cref="Csla.Mvvm.ViewModel{T}.IsValid"/>
    public virtual bool IsValid
    {
      get => _inner.BaseIsValid;
      protected set => _inner.BaseIsValid = value;
    }

    /// <inheritdoc cref="Csla.Mvvm.ViewModel{T}.CanSave"/>
    public virtual bool CanSave
    {
      get => _inner.BaseCanSave;
      protected set => _inner.BaseCanSave = value;
    }

    /// <inheritdoc cref="Csla.Mvvm.ViewModel{T}.CanCancel"/>
    public virtual bool CanCancel
    {
      get => _inner.BaseCanCancel;
      protected set => _inner.BaseCanCancel = value;
    }

    /// <inheritdoc cref="Csla.Mvvm.ViewModel{T}.CanCreate"/>
    public virtual bool CanCreate
    {
      get => _inner.BaseCanCreate;
      protected set => _inner.BaseCanCreate = value;
    }

    /// <inheritdoc cref="Csla.Mvvm.ViewModel{T}.CanDelete"/>
    public virtual bool CanDelete
    {
      get => _inner.BaseCanDelete;
      protected set => _inner.BaseCanDelete = value;
    }

    /// <inheritdoc cref="Csla.Mvvm.ViewModel{T}.CanFetch"/>
    public virtual bool CanFetch
    {
      get => _inner.BaseCanFetch;
      protected set => _inner.BaseCanFetch = value;
    }

    /// <inheritdoc cref="Csla.Mvvm.ViewModel{T}.CanRemove"/>
    public virtual bool CanRemove
    {
      get => _inner.BaseCanRemove;
      protected set => _inner.BaseCanRemove = value;
    }

    /// <inheritdoc cref="Csla.Mvvm.ViewModel{T}.CanAddNew"/>
    public virtual bool CanAddNew
    {
      get => _inner.BaseCanAddNew;
      protected set => _inner.BaseCanAddNew = value;
    }

    /// <inheritdoc cref="Csla.Mvvm.ViewModel{T}.CanCreateObject"/>
    public virtual bool CanCreateObject
    {
      get => _inner.BaseCanCreateObject;
      protected set => _inner.BaseCanCreateObject = value;
    }

    /// <inheritdoc cref="Csla.Mvvm.ViewModel{T}.CanGetObject"/>
    public virtual bool CanGetObject
    {
      get => _inner.BaseCanGetObject;
      protected set => _inner.BaseCanGetObject = value;
    }

    /// <inheritdoc cref="Csla.Mvvm.ViewModel{T}.CanEditObject"/>
    public virtual bool CanEditObject
    {
      get => _inner.BaseCanEditObject;
      protected set => _inner.BaseCanEditObject = value;
    }

    /// <inheritdoc cref="Csla.Mvvm.ViewModel{T}.CanDeleteObject"/>
    public virtual bool CanDeleteObject
    {
      get => _inner.BaseCanDeleteObject;
      protected set => _inner.BaseCanDeleteObject = value;
    }

    #endregion

    #region Errors

    /// <inheritdoc cref="Csla.Mvvm.ViewModel{T}.ViewModelErrorText"/>
    public string ViewModelErrorText
    {
      get => _inner.ViewModelErrorText;
      protected set => _inner.SetViewModelErrorText(value);
    }

    /// <inheritdoc cref="Csla.Mvvm.ViewModel{T}.Exception"/>
    public Exception? Exception => _inner.Exception;

    /// <summary>
    /// Gets the first validation error
    /// message from the Model.
    /// </summary>
    protected virtual string ModelErrorText => _inner.BaseModelErrorText;

    #endregion

    #region Operations

    /// <inheritdoc cref="Csla.Mvvm.ViewModel{T}.RefreshAsync(Func{Task{T}})"/>
    public virtual Task<T?> RefreshAsync(Func<Task<T?>> factory) => _inner.RefreshAsync(factory);

    /// <summary>
    /// Creates or retrieves a new instance of the
    /// Model by invoking an action.
    /// </summary>
    /// <param name="factory">Factory method to invoke</param>
    [Obsolete("Use RefreshAsync(Func<Task<T?>>)")]
    public virtual Task<T?> RefreshAsync<F>(Func<Task<T?>> factory) => RefreshAsync(factory);

    /// <inheritdoc cref="Csla.Mvvm.ViewModel{T}.SaveAsync()"/>
    public virtual Task<T?> SaveAsync() => _inner.SaveAsync();

    /// <inheritdoc cref="Csla.Mvvm.ViewModel{T}.SaveAsync(CancellationToken)"/>
    public virtual Task<T?> SaveAsync(CancellationToken ct) => _inner.BaseSaveAsync(ct);

    /// <summary>
    /// Override to provide custom Model save behavior.
    /// </summary>
    /// <param name="cloned">The object to save (a clone of Model if ManageObjectLifetime is true).</param>
    protected virtual Task<T?> DoSaveAsync(ISavable? cloned) => _inner.BaseDoSaveAsync(cloned);

    /// <summary>
    /// Cancels changes made to the model
    /// if ManagedObjectLifetime is true.
    /// </summary>
    protected virtual void DoCancel() => _inner.BaseDoCancel();

    /// <summary>
    /// Adds a new item to the Model (if it
    /// is a collection).
    /// </summary>
    protected virtual object DoAddNew() => _inner.BaseDoAddNew();

    /// <summary>
    /// Removes an item from the Model (if it
    /// is a collection).
    /// </summary>
    protected virtual void DoRemove(object? item) => _inner.BaseDoRemove(item);

    /// <summary>
    /// Marks the Model for deletion (if it is an
    /// editable root object).
    /// </summary>
    protected virtual void DoDelete() => _inner.BaseDoDelete();

    #endregion

    #region Events and hooks

    /// <summary>
    /// Invoked before the Model changes. The default implementation
    /// moves event handlers from the old object to the new object.
    /// </summary>
    /// <param name="oldValue">Previous Model reference.</param>
    /// <param name="newValue">New Model reference.</param>
    protected virtual void OnModelChanging(T? oldValue, T? newValue) => _inner.BaseOnModelChanging(oldValue, newValue);

    /// <summary>
    /// Invoked after the Model has changed.
    /// </summary>
    /// <param name="oldValue">Previous Model reference.</param>
    /// <param name="newValue">New Model reference.</param>
    protected virtual void OnModelChanged(T? oldValue, T? newValue) => _inner.BaseOnModelChanged(oldValue, newValue);

    /// <summary>
    /// Unhooks changed event handlers from the model.
    /// </summary>
    /// <param name="model">Model object.</param>
    protected void UnhookChangedEvents(T? model) => _inner.Unhook(model);

    /// <summary>
    /// Hooks changed event handlers on the model.
    /// </summary>
    /// <param name="model">Model object.</param>
    protected void HookChangedEvents(T? model) => _inner.Hook(model);

    /// <summary>
    /// Override this method to hook into to logic of setting
    /// properties when model is changed or edited.
    /// </summary>
    /// <remarks>
    /// Authorization rules are evaluated asynchronously. When a rule
    /// is asynchronous, the permission-based properties
    /// (such as CanDelete or CanAddNew) may be updated after this
    /// method returns; PropertyChanged is raised when they change.
    /// </remarks>
    protected virtual void OnSetProperties() => _inner.BaseOnSetProperties();

    /// <summary>
    /// Event raised after Model has been saved.
    /// </summary>
    public event Action? Saved
    {
      add => _inner.Saved += value;
      remove => _inner.Saved -= value;
    }

    /// <summary>
    /// Event raised when a save operation fails.
    /// </summary>
    public event EventHandler<Core.ErrorEventArgs>? Error;

    /// <summary>
    /// Event raised when Model is changing.
    /// </summary>
    public event Action<T?, T?>? ModelChanging
    {
      add => _inner.ModelChanging += value;
      remove => _inner.ModelChanging -= value;
    }

    /// <summary>
    /// Event raised when Model has changed.
    /// </summary>
    public event Action? ModelChanged
    {
      add => _inner.ModelChanged += value;
      remove => _inner.ModelChanged -= value;
    }

    /// <summary>
    /// Event raised when a property on the Model changes.
    /// </summary>
    public event PropertyChangedEventHandler? ModelPropertyChanged;

    /// <summary>
    /// Event raised when a child of the Model changes.
    /// </summary>
    public event Action<object, ChildChangedEventArgs>? ModelChildChanged;

    /// <summary>
    /// Event raised the Model changes and is a collection.
    /// </summary>
    public event Action<object, NotifyCollectionChangedEventArgs>? ModelCollectionChanged;

    /// <summary>
    /// Event raised when a property changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Raise the PropertyChanged event.
    /// </summary>
    /// <param name="propertyName">Name of the changed property.</param>
    protected virtual void OnPropertyChanged(string propertyName)
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion

    /// <summary>
    /// The <see cref="Csla.Mvvm.ViewModel{T}"/> that implements
    /// the behavior, routing its virtual members back to the
    /// owning viewmodel so overrides there take effect.
    /// </summary>
    private sealed class InnerViewModel : Csla.Mvvm.ViewModel<T>
    {
      private readonly ViewModelBase<T> _owner;

      public InnerViewModel(ViewModelBase<T> owner)
      {
        _owner = owner;
      }

      public InnerViewModel(ViewModelBase<T> owner, ApplicationContext applicationContext)
        : base(applicationContext)
      {
        _owner = owner;
      }

      public ApplicationContext Context => ApplicationContext;

      protected override ApplicationContext ResolveApplicationContext() => ApplicationContextManager.GetApplicationContext();

      // status properties: the owner reads and writes through these

      public bool BaseIsDirty { get => base.IsDirty; set => base.IsDirty = value; }
      public bool BaseIsValid { get => base.IsValid; set => base.IsValid = value; }
      public bool BaseCanSave { get => base.CanSave; set => base.CanSave = value; }
      public bool BaseCanCancel { get => base.CanCancel; set => base.CanCancel = value; }
      public bool BaseCanCreate { get => base.CanCreate; set => base.CanCreate = value; }
      public bool BaseCanDelete { get => base.CanDelete; set => base.CanDelete = value; }
      public bool BaseCanFetch { get => base.CanFetch; set => base.CanFetch = value; }
      public bool BaseCanRemove { get => base.CanRemove; set => base.CanRemove = value; }
      public bool BaseCanAddNew { get => base.CanAddNew; set => base.CanAddNew = value; }
      public bool BaseCanCreateObject { get => base.CanCreateObject; set => base.CanCreateObject = value; }
      public bool BaseCanGetObject { get => base.CanGetObject; set => base.CanGetObject = value; }
      public bool BaseCanEditObject { get => base.CanEditObject; set => base.CanEditObject = value; }
      public bool BaseCanDeleteObject { get => base.CanDeleteObject; set => base.CanDeleteObject = value; }

      // status and permission reads and writes go through the owner,
      // so overrides of its getters and protected setters take effect
      public override bool IsDirty { get => _owner.IsDirty; protected set => _owner.IsDirty = value; }
      public override bool IsValid { get => _owner.IsValid; protected set => _owner.IsValid = value; }
      public override bool CanSave { get => _owner.CanSave; protected set => _owner.CanSave = value; }
      public override bool CanCancel { get => _owner.CanCancel; protected set => _owner.CanCancel = value; }
      public override bool CanCreate { get => _owner.CanCreate; protected set => _owner.CanCreate = value; }
      public override bool CanDelete { get => _owner.CanDelete; protected set => _owner.CanDelete = value; }
      public override bool CanFetch { get => _owner.CanFetch; protected set => _owner.CanFetch = value; }
      public override bool CanRemove { get => _owner.CanRemove; protected set => _owner.CanRemove = value; }
      public override bool CanAddNew { get => _owner.CanAddNew; protected set => _owner.CanAddNew = value; }
      public override bool CanCreateObject { get => _owner.CanCreateObject; protected set => _owner.CanCreateObject = value; }
      public override bool CanGetObject { get => _owner.CanGetObject; protected set => _owner.CanGetObject = value; }
      public override bool CanEditObject { get => _owner.CanEditObject; protected set => _owner.CanEditObject = value; }
      public override bool CanDeleteObject { get => _owner.CanDeleteObject; protected set => _owner.CanDeleteObject = value; }

      public void SetIsBusy(bool value) => IsBusy = value;
      public void SetViewModelErrorText(string value) => ViewModelErrorText = value;
      public string BaseModelErrorText => base.ModelErrorText;
      protected override string ModelErrorText => _owner.ModelErrorText;

      public void Hook(T? model) => HookChangedEvents(model);
      public void Unhook(T? model) => UnhookChangedEvents(model);

      protected override void OnPropertyChanged(string propertyName)
      {
        base.OnPropertyChanged(propertyName);
        _owner.OnPropertyChanged(propertyName);
      }

      protected override void OnModelChanging(T? oldValue, T? newValue) => _owner.OnModelChanging(oldValue, newValue);
      public void BaseOnModelChanging(T? oldValue, T? newValue) => base.OnModelChanging(oldValue, newValue);

      protected override void OnModelChanged(T? oldValue, T? newValue)
      {
        // keep the dependency property in sync when the model
        // is replaced by a refresh or save operation
        if (!ReferenceEquals(_owner.GetValue(ModelProperty), newValue))
          _owner.SetValue(ModelProperty, newValue);
        _owner.OnModelChanged(oldValue, newValue);
      }
      public void BaseOnModelChanged(T? oldValue, T? newValue) => base.OnModelChanged(oldValue, newValue);

      protected override void OnSetProperties() => _owner.OnSetProperties();
      public void BaseOnSetProperties() => base.OnSetProperties();

      protected override void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
      {
        OnSetProperties();
        _owner.ModelPropertyChanged?.Invoke(_owner, e);
      }

      protected override void OnModelChildChanged(object? sender, ChildChangedEventArgs e)
      {
        OnSetProperties();
        _owner.ModelChildChanged?.Invoke(_owner, e);
      }

      protected override void OnModelCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
      {
        OnSetProperties();
        _owner.ModelCollectionChanged?.Invoke(_owner, e);
      }

      public override Task<T?> SaveAsync(CancellationToken ct) => _owner.SaveAsync(ct);
      public Task<T?> BaseSaveAsync(CancellationToken ct) => base.SaveAsync(ct);

      protected override Task<T?> DoSaveAsync(ISavable? cloned) => _owner.DoSaveAsync(cloned);
      public Task<T?> BaseDoSaveAsync(ISavable? cloned) => base.DoSaveAsync(cloned);

      public void BaseDoCancel() => base.DoCancel();
      public object BaseDoAddNew() => base.DoAddNew();
      public void BaseDoRemove(object? item) => base.DoRemove(item);
      public void BaseDoDelete() => base.DoDelete();
    }
  }
#endif
}
