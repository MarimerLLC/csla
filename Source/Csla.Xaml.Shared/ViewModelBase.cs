﻿//-----------------------------------------------------------------------
// <copyright file="ViewModelBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Base class used to create ViewModel objects that</summary>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Windows;
using Csla.Core;
using Csla.Rules;
#if WINDOWS_UWP
using Windows.UI.Xaml;
#endif

#if ANDROID && !MAUI
namespace Csla.Axml
#elif IOS && !MAUI
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
#if ANDROID || IOS || XAMARIN || MAUI
  public abstract class ViewModelBase<T> : INotifyPropertyChanged, IViewModel
#else
  public abstract class ViewModelBase<T> : DependencyObject,
    INotifyPropertyChanged, IViewModel
#endif
  {
    private ApplicationContext ApplicationContext { get => ApplicationContextManager.GetApplicationContext(); }

#if ANDROID || IOS || XAMARIN || WINDOWS_UWP || MAUI
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
#if ANDROID || IOS || XAMARIN || MAUI
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
    [Display(AutoGenerateField = false)]
    [ScaffoldColumn(false)]
    public bool ManageObjectLifetime
    {
#if ANDROID || IOS || XAMARIN || MAUI
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
        var canDeleteInstance = BusinessRules.HasPermission(ApplicationContext, AuthorizationActions.DeleteObject, targetObject);

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
            CanRemove = BusinessRules.HasPermission(ApplicationContext, AuthorizationActions.DeleteObject, itemType) &&
                        list.Count > 0 && !isObjectBusy;
            CanAddNew = BusinessRules.HasPermission(ApplicationContext, AuthorizationActions.CreateObject, itemType) &&
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
          CanRemove = BusinessRules.HasPermission(ApplicationContext, AuthorizationActions.DeleteObject, itemType) &&
                      list.Count > 0 && !isObjectBusy;
          CanAddNew = BusinessRules.HasPermission(ApplicationContext, AuthorizationActions.CreateObject, itemType) &&
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
      get
      {
        SetPropertiesAtObjectLevel();
        return _canCreateObject;
      }
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
      get
      {
        SetPropertiesAtObjectLevel();
        return _canGetObject;
      }
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
      get
      {
        SetPropertiesAtObjectLevel();
        return _canEditObject;
      }
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
      get
      {
        SetPropertiesAtObjectLevel();
        return _canDeleteObject;
      }
      protected set
      {
        if (_canDeleteObject != value)
        {
          _canDeleteObject = value;
          OnPropertyChanged(nameof(CanDeleteObject));
        }
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

      CanCreateObject = BusinessRules.HasPermission(ApplicationContext, AuthorizationActions.CreateObject, sourceType);
      CanGetObject = BusinessRules.HasPermission(ApplicationContext, AuthorizationActions.GetObject, sourceType);
      CanEditObject = BusinessRules.HasPermission(ApplicationContext, AuthorizationActions.EditObject, sourceType);
      CanDeleteObject = BusinessRules.HasPermission(ApplicationContext, AuthorizationActions.DeleteObject, sourceType);

      // call SetProperties to set "instance" values 
      OnSetProperties();
    }

    /// <summary>
    /// Creates or retrieves a new instance of the 
    /// Model by invoking an action.
    /// </summary>
    /// <param name="factory">Factory method to invoke</param>
    public virtual async Task<T> RefreshAsync<F>(Func<Task<T>> factory)
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
    public virtual async Task<T> SaveAsync()
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
        Model = await DoSaveAsync(savable);
      }
      finally
      {
        HookChangedEvents(Model);
        IsBusy = false;
      }
      return Model;
    }

    /// <summary>
    /// Override to provide custom Model save behavior.
    /// </summary>
    protected virtual async Task<T> DoSaveAsync(ISavable cloned)
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

#if (ANDROID || IOS) || XAMARIN || MAUI
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
      ModelPropertyChanged?.Invoke(this, e);
    }

    private void Model_ChildChanged(object sender, ChildChangedEventArgs e)
    {
      OnSetProperties();
      ModelChildChanged?.Invoke(this, e);
    }

    private void Model_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      OnSetProperties();
      ModelCollectionChanged?.Invoke(this, e);
    }

    object IViewModel.Model
    {
      get { return Model; }
      set { Model = (T)value; }
    }

    /// <summary>
    /// Event raised when a property on the Model changes.
    /// </summary>
    public event PropertyChangedEventHandler ModelPropertyChanged;

    /// <summary>
    /// Event raised when a child of the Model changes.
    /// </summary>
    public event Action<object, ChildChangedEventArgs> ModelChildChanged;

    /// <summary>
    /// Event raised the Model changes and is a collection.
    /// </summary>
    public event Action<object, NotifyCollectionChangedEventArgs> ModelCollectionChanged;

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