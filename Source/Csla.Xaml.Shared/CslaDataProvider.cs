#if !XAMARIN && !WINDOWS_UWP && !MAUI
//-----------------------------------------------------------------------
// <copyright file="CslaDataProvider.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Wraps and creates a CSLA .NET-style object </summary>
//-----------------------------------------------------------------------
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Data;
using Csla.Core;
using Csla.Properties;
using Csla.Reflection;

namespace Csla.Xaml
{
  /// <summary>
  /// Wraps and creates a CSLA .NET-style object 
  /// that you can use as a binding source.
  /// </summary>
  public class CslaDataProvider : DataSourceProvider
  {

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    public CslaDataProvider()
    {
      CommandManager = new CslaDataProviderCommandManager(this);
      _factoryParameters = new ObservableCollection<object>();
      _factoryParameters.CollectionChanged += _factoryParameters_CollectionChanged;
    }

    /// <summary>
    /// Event raised when the object has been saved.
    /// </summary>
    public event EventHandler<SavedEventArgs>? Saved;

    /// <summary>
    /// Raise the Saved event when the object has been saved.
    /// </summary>
    /// <param name="newObject">New object reference as a result
    /// of the save operation.</param>
    /// <param name="error">Reference to an exception object if
    /// an error occurred.</param>
    /// <param name="userState">Reference to a userstate object.</param>
    /// <exception cref="ArgumentNullException"><paramref name="newObject"/> is <see langword="null"/>.</exception>
    protected virtual void OnSaved(object newObject, Exception? error, object? userState)
    {
      if (newObject is null)
        throw new ArgumentNullException(nameof(newObject));

      Saved?.Invoke(this, new SavedEventArgs(newObject, error, userState));
    }

    void _factoryParameters_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      BeginQuery();
    }

    #region Properties

    private Type? _objectType = null;
    private bool _manageLifetime;
    private string _factoryMethod = string.Empty;
    private readonly ObservableCollection<object> _factoryParameters;
    private bool _isBusy;

    /// <summary>
    /// Gets an object that can be used to execute
    /// Save and Undo commands on this CslaDataProvider 
    /// through XAML command bindings.
    /// </summary>
    public CslaDataProviderCommandManager CommandManager { get; }

    /// <summary>
    /// Gets or sets the type of object 
    /// to create an instance of.
    /// </summary>
    public Type? ObjectType
    {
      get => _objectType;
      set
      {
        _objectType = value;
        OnPropertyChanged(new PropertyChangedEventArgs("ObjectType"));
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the
    /// data control should manage the lifetime of
    /// the business object, including using n-level
    /// undo.
    /// </summary>
    public bool ManageObjectLifetime
    {
      get => _manageLifetime;
      set
      {
        _manageLifetime = value;
        OnPropertyChanged(new PropertyChangedEventArgs("ManageObjectLifetime"));
      }
    }

    private object? _dataChangedHandler;

    /// <summary>
    /// Gets or sets a reference to an object that
    /// will handle the DataChanged event raised
    /// by this data provider.
    /// </summary>
    /// <remarks>
    /// This property is designed to 
    /// reference an IErrorDialog control.
    /// </remarks>
    public object? DataChangedHandler
    {
      get => _dataChangedHandler;
      set
      {
        _dataChangedHandler = value;
        if (value is IErrorDialog dialog)
          dialog.Register(this);
        OnPropertyChanged(new PropertyChangedEventArgs("DataChangedHandler"));
      }
    }

    /// <summary>
    /// Gets or sets the name of the static
    /// (Shared in Visual Basic) factory method
    /// that should be called to create the
    /// object instance.
    /// </summary>
    /// <exception cref="ArgumentNullException"><see cref="FactoryMethod"/> is <see langword="null"/>.</exception>
    public string FactoryMethod
    {
      get => _factoryMethod;
      set
      {
        _factoryMethod = value ?? throw new ArgumentNullException(nameof(FactoryMethod));
        OnPropertyChanged(new PropertyChangedEventArgs("FactoryMethod"));
      }
    }

    /// <summary>
    /// Get the list of parameters to pass
    /// to the factory method.
    /// </summary>
    public IList FactoryParameters => _factoryParameters;

    /// <summary>
    /// Gets or sets a value that indicates 
    /// whether to perform object creation in 
    /// a worker thread or in the active context.
    /// </summary>
    public bool IsAsynchronous { get; set; }

    /// <summary>
    /// Gets or sets a reference to the data
    /// object.
    /// </summary>
    public object? ObjectInstance
    {
      get => Data;
      set
      {
        OnQueryFinished(value, null, null, null);
        OnPropertyChanged(new PropertyChangedEventArgs("ObjectInstance"));
      }
    }

    /// <summary>
    /// Gets a value indicating if this object is busy.
    /// </summary>
    public bool IsBusy
    {
      get => _isBusy;
      protected set
      {
        _isBusy = value;
        OnPropertyChanged(new PropertyChangedEventArgs("IsBusy"));
      }
    }

    /// <summary>
    /// Triggers WPF data binding to rebind to the
    /// data object.
    /// </summary>
    public void Rebind()
    {
      object? tmp = ObjectInstance;
      ObjectInstance = null;
      ObjectInstance = tmp;
    }

    #endregion

    #region Query

    private bool _firstRun = true;
    private bool _init = false;
    private bool _endInitCompete = false;
    private bool _endInitError = false;

    /// <summary>
    /// Indicates that the control is about to initialize.
    /// </summary>
    protected override void BeginInit()
    {
      _init = true;
      base.BeginInit();
    }

    /// <summary>
    /// Indicates that the control has initialized.
    /// </summary>
    protected override void EndInit()
    {
      _init = false;
      base.EndInit();
      _endInitCompete = true;
    }

    /// <summary>
    /// Overridden. Starts to create the requested object, 
    /// either immediately or on a background thread, 
    /// based on the value of the IsAsynchronous property.
    /// </summary>
    protected override void BeginQuery()
    {
      if (_init)
        return;

      if (_firstRun)
      {
        _firstRun = false;
        if (!IsInitialLoadEnabled)
          return;
      }

      if (_endInitError)
      {
        // this handles a case where the WPF form initilizer
        // invokes the data provider twice when an exception
        // occurs - we really don't want to try the query twice
        // or report the error twice
        _endInitError = false;
        OnQueryFinished(null);
        return;
      }

      if (IsRefreshDeferred)
        return;

      QueryRequest request = new QueryRequest
      {
        ObjectType = _objectType ?? throw new InvalidOperationException($"{nameof(ObjectType)} == null"),
        FactoryMethod = _factoryMethod,
        FactoryParameters = _factoryParameters,
        ManageObjectLifetime = _manageLifetime
      };

      IsBusy = true;

      if (IsAsynchronous)
        ThreadPool.QueueUserWorkItem(DoQuery, request);
      else
        DoQuery(request);
    }

    /// <summary>
    /// Refresh the ObjectInstance by calling the
    /// supplied factory.
    /// </summary>
    /// <typeparam name="T">Type of ObjectInstance</typeparam>
    /// <param name="factory">Sync data portal or factory method</param>
    /// <exception cref="ArgumentNullException"><paramref name="factory"/> is <see langword="null"/>.</exception>
    public void Refresh<T>(Func<T> factory)
    {
      if (factory is null)
        throw new ArgumentNullException(nameof(factory));

      T? result = default;
      Exception? exceptionResult = null;

      // invoke factory method
      try
      {
        result = factory();
      }
      catch (DataPortalException ex)
      {
        exceptionResult = ex.BusinessException;
      }
      catch (TargetInvocationException ex)
      {
        if (ex.InnerException != null)
        {
          exceptionResult = ex.InnerException;
          if (exceptionResult is DataPortalException dpe && dpe.BusinessException != null)
            exceptionResult = dpe.BusinessException;
        }
        else
          exceptionResult = ex;
      }
      catch (Exception ex)
      {
        exceptionResult = ex;
      }

      if (ManageObjectLifetime && result is ISupportUndo undo)
      {
        undo.BeginEdit();
      }

      if (!_endInitCompete && exceptionResult != null)
        _endInitError = true;

      // return result to base class
      OnQueryFinished(result, exceptionResult, _ => { IsBusy = false; return null; }, null);
    }

    /// <summary>
    /// Refresh the ObjectInstance by calling the
    /// supplied factory.
    /// </summary>
    /// <typeparam name="T">Type of ObjectInstance</typeparam>
    /// <param name="factory">Async data portal or factory method</param>
    /// <exception cref="ArgumentNullException"><paramref name="factory"/> is <see langword="null"/>.</exception>
    public async void Refresh<T>(Func<Task<T>> factory)
    {
      if (factory is null)
        throw new ArgumentNullException(nameof(factory));
      T? result = default;
      Exception? exceptionResult = null;

      // invoke factory method
      try
      {
        result = await factory();
      }
      catch (DataPortalException ex)
      {
        exceptionResult = ex.BusinessException;
      }
      catch (TargetInvocationException ex)
      {
        if (ex.InnerException != null)
        {
          exceptionResult = ex.InnerException;
          if (exceptionResult is DataPortalException dpe && dpe.BusinessException != null)
            exceptionResult = dpe.BusinessException;
        }
        else
          exceptionResult = ex;
      }
      catch (Exception ex)
      {
        exceptionResult = ex;
      }

      if (ManageObjectLifetime && result != null)
      {
        if (result is ISupportUndo undo)
          undo.BeginEdit();
      }

      if (!_endInitCompete && exceptionResult != null)
        _endInitError = true;

      // return result to base class
      OnQueryFinished(result, exceptionResult, _ => { IsBusy = false; return null; }, null);
    }

    private void DoQuery(object? state)
    {
      if (state is not QueryRequest request)
      {
        throw new InvalidOperationException($"Internal exception: State must have been provided as an instance of type {nameof(QueryRequest)}. This should not have happened.");
      }

      object? result = null;
      Exception? exceptionResult = null;
      object[] parameters = request.FactoryParameters.ToArray();

      try
      {
        // get factory method info
        BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy;
        System.Reflection.MethodInfo? factory = request.ObjectType.GetMethod(request.FactoryMethod, flags, null, MethodCaller.GetParameterTypes(parameters), null);

        if (factory == null)
        {
          // strongly typed factory couldn't be found
          // so find one with the correct number of
          // parameters 
          int parameterCount = parameters.Length;
          System.Reflection.MethodInfo[] methods = request.ObjectType.GetMethods(flags);
          foreach (System.Reflection.MethodInfo method in methods)
            if (method.Name == request.FactoryMethod && method.GetParameters().Length == parameterCount)
            {
              factory = method;
              break;
            }
        }

        if (factory == null)
        {
          // no matching factory could be found
          // so throw exception
          throw new InvalidOperationException(string.Format(Resources.NoSuchFactoryMethod, request.FactoryMethod));
        }

        // invoke factory method
        try
        {
          result = factory.Invoke(null, parameters);
        }
        catch (DataPortalException ex)
        {
          exceptionResult = ex.BusinessException;
        }
        catch (TargetInvocationException ex)
        {
          if (ex.InnerException != null)
          {
            exceptionResult = ex.InnerException;
            if (exceptionResult is DataPortalException dpe && dpe.BusinessException != null)
              exceptionResult = dpe.BusinessException;
          }
          else
            exceptionResult = ex;
        }
        catch (Exception ex)
        {
          exceptionResult = ex;
        }
      }
      catch (Exception ex)
      {
        exceptionResult = ex;
      }

      if (request.ManageObjectLifetime && result != null)
      {
        if (result is ISupportUndo undo)
          undo.BeginEdit();
      }

      if (!_endInitCompete && exceptionResult != null)
        _endInitError = true;

      // return result to base class
      OnQueryFinished(result, exceptionResult, _ => { IsBusy = false; return null; }, null);
    }


    #region QueryRequest Class

    private class QueryRequest
    {
      public required Type ObjectType { get; set; }

      public required string FactoryMethod { get; set; }

      private ObservableCollection<object> _factoryParameters = [];

      public ObservableCollection<object> FactoryParameters
      {
        get => _factoryParameters;
        set => _factoryParameters = new ObservableCollection<object>([.. value]);
      }

      public bool ManageObjectLifetime { get; set; }

    }

    #endregion

    #endregion

    #region Cancel/Update/New/Remove  

    /// <summary>
    /// Cancels changes to the business object, returning
    /// it to its previous state.
    /// </summary>
    /// <remarks>
    /// This metod does nothing unless ManageLifetime is
    /// set to true and the object supports n-level undo.
    /// </remarks>
    public void Cancel()
    {
      if (Data is ISupportUndo undo && _manageLifetime)
      {
        IsBusy = true;
        undo.CancelEdit();
        undo.BeginEdit();
        IsBusy = false;
      }
    }

    /// <summary>
    /// Accepts changes to the business object, and
    /// commits them by calling the object's Save()
    /// method.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method does nothing unless the object
    /// implements Csla.Core.ISavable.
    /// </para><para>
    /// If the object implements IClonable, it
    /// will be cloned, and the clone will be
    /// saved.
    /// </para><para>
    /// If the object supports n-level undo and
    /// ManageLifetime is true, then this method
    /// will automatically call ApplyEdit() and
    /// BeginEdit() appropriately.
    /// </para>
    /// </remarks>
    public void Save()
    {
      // only do something if the object implements
      // ISavable
      if (Data is ISavable savable)
      {
        object result = savable;
        Exception? exceptionResult = null;
        try
        {
          IsBusy = true;

          // clone the object if possible
          if (savable is ICloneable cloneable)
            savable = (ISavable)cloneable.Clone();

          // apply edits in memory
          if (savable is ISupportUndo undo && _manageLifetime)
            undo.ApplyEdit();


          // save the clone
          result = savable.Save();

          if (!ReferenceEquals(savable, Data))
          {
            // raise Saved event from original object
            var original = Data as ISavable;
            original?.SaveComplete(result);
          }

          // start editing the resulting object
          if (result is ISupportUndo undo2 && _manageLifetime)
            undo2.BeginEdit();
        }
        catch (Exception ex)
        {
          exceptionResult = ex;
        }
        // clear previous object
        OnQueryFinished(null, exceptionResult, null, null);
        // return result to base class
        OnQueryFinished(result, null, null, null);
        IsBusy = false;
        OnSaved(result, exceptionResult, null);
      }
    }


    /// <summary>
    /// Adds a new item to the object if the object
    /// implements IBindingList and AllowNew is true.
    /// </summary>
    public object? AddNew()
    {
      // only do something if the object implements
      // IBindingList
      if (Data is IBindingList list && list.AllowNew)
        return list.AddNew();
      else
        return null;

    }

    /// <summary>
    /// Removes an item from the list if the object
    /// implements IBindingList and AllowRemove is true.
    /// </summary>
    /// <param name="sender">Object invoking this method.</param>
    /// <param name="e">
    /// ExecuteEventArgs, where MethodParameter contains 
    /// the item to be removed from the list.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="e"/> is <see langword="null"/>.</exception>
    public void RemoveItem(object? sender, ExecuteEventArgs e)
    {
      if (e is null)
        throw new ArgumentNullException(nameof(e));

      var item = e.MethodParameter;
      // only do something if the object implements
      // IBindingList
      IBindingList? list;
      if (item is BusinessBase bb)
        list = bb.Parent as IBindingList;
      else
        list = Data as IBindingList;
      if (list != null && list.AllowRemove)
        list.Remove(item);
    }

    #endregion

  }
}
#endif