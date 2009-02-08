using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using System.Reflection;
using Csla.Reflection;
using Csla.Properties;

namespace Csla.Wpf
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
      _commandManager = new CslaDataProviderCommandManager(this);
      _factoryParameters = new ObservableCollection<object>();
      _factoryParameters.CollectionChanged += 
        new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_factoryParameters_CollectionChanged);
    }

    public event EventHandler<Csla.Core.SavedEventArgs> Saved;
    protected void OnSaved(object newObject, Exception error, object userState)
    {
      if (Saved != null)
        Saved(this, new Csla.Core.SavedEventArgs(newObject, error, userState));
    }

    void _factoryParameters_CollectionChanged(
      object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      BeginQuery();
    }

    #region Properties

    private Type _objectType = null;
    private bool _manageLifetime;
    private string _factoryMethod = string.Empty;
    private ObservableCollection<object> _factoryParameters;
    private bool _isAsynchronous;
    private CslaDataProviderCommandManager _commandManager;
    private bool _isBusy;

    /// <summary>
    /// Gets an object that can be used to execute
    /// Save and Undo commands on this CslaDataProvider 
    /// through XAML command bindings.
    /// </summary>
    public CslaDataProviderCommandManager CommandManager
    {
      get
      {
        return _commandManager;
      }
    }

    /// <summary>
    /// Gets or sets the type of object 
    /// to create an instance of.
    /// </summary>
    public Type ObjectType
    {
      get 
      { 
        return _objectType; 
      }
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
      get
      {
        return _manageLifetime;
      }
      set
      {
        _manageLifetime = value;
        OnPropertyChanged(new PropertyChangedEventArgs("ManageObjectLifetime"));
      }
    }
	
    /// <summary>
    /// Gets or sets the name of the static
    /// (Shared in Visual Basic) factory method
    /// that should be called to create the
    /// object instance.
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
        OnPropertyChanged(new PropertyChangedEventArgs("FactoryMethod"));
      }
    }

    /// <summary>
    /// Get the list of parameters to pass
    /// to the factory method.
    /// </summary>
    public IList FactoryParameters
    {
      get
      {
        return _factoryParameters;
      }
    }

    /// <summary>
    /// Gets or sets a value that indicates 
    /// whether to perform object creation in 
    /// a worker thread or in the active context.
    /// </summary>
    public bool IsAsynchronous
    {
      get { return _isAsynchronous; }
      set { _isAsynchronous = value; }
    }

    /// <summary>
    /// Gets or sets a reference to the data
    /// object.
    /// </summary>
    public object ObjectInstance
    {
      get { return Data; }
      set 
      {
        base.OnQueryFinished(value, null, null, null);
        OnPropertyChanged(new PropertyChangedEventArgs("ObjectInstance"));
      }
    }

    /// <summary>
    /// Gets a value indicating if this object is busy.
    /// </summary>
    public bool IsBusy
    {
      get { return _isBusy; }
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
      object tmp = ObjectInstance;
      ObjectInstance = null;
      ObjectInstance = tmp;
    }

    #endregion

    #region Query

    private bool _firstRun = true;

    /// <summary>
    /// Overridden. Starts to create the requested object, 
    /// either immediately or on a background thread, 
    /// based on the value of the IsAsynchronous property.
    /// </summary>
    protected override void BeginQuery()
    {
      if (this.IsRefreshDeferred)
        return;

      if (_firstRun)
      {
        _firstRun = false;
        if (!IsInitialLoadEnabled)
          return;
      }

      QueryRequest request = new QueryRequest();
      request.ObjectType = _objectType;
      request.FactoryMethod = _factoryMethod;
      request.FactoryParameters = _factoryParameters;
      request.ManageObjectLifetime = _manageLifetime;

      IsBusy = true;

      if (IsAsynchronous)
        System.Threading.ThreadPool.QueueUserWorkItem(DoQuery, request);
      else
      {
        DoQuery(request);
        IsBusy = false;
      }
    }

    private void DoQuery(object state)
    {
      QueryRequest request = (QueryRequest)state;
      object result = null;
      Exception exceptionResult = null;
      object[] parameters = new List<object>(request.FactoryParameters).ToArray();

      try
      {
        // get factory method info
        BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy;
        MethodInfo factory = request.ObjectType.GetMethod(
          request.FactoryMethod, flags, null, 
          MethodCaller.GetParameterTypes(parameters), null);

        if (factory == null)
        {
          // strongly typed factory couldn't be found
          // so find one with the correct number of
          // parameters 
          int parameterCount = parameters.Length;
          MethodInfo[] methods = request.ObjectType.GetMethods(flags);
          foreach (MethodInfo method in methods)
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
          throw new InvalidOperationException(
            string.Format(Resources.NoSuchFactoryMethod, request.FactoryMethod));
        }

        // invoke factory method
        try
        {
          result = factory.Invoke(null, parameters);
        }
        catch (Csla.DataPortalException ex)
        {
          exceptionResult = ex.BusinessException;
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
        Csla.Core.ISupportUndo undo = result as Csla.Core.ISupportUndo;
        if (undo != null)
          undo.BeginEdit();
      }

      if (!System.Windows.Application.Current.Dispatcher.CheckAccess())
        System.Windows.Application.Current.Dispatcher.Invoke(
          new Action(() => { IsBusy = false; }), 
          new object[] { });

      // return result to base class
      base.OnQueryFinished(result, exceptionResult, null, null);
    }

    #region QueryRequest Class

    private class QueryRequest
    {
      private Type _objectType;

      public Type ObjectType
      {
        get { return _objectType; }
        set { _objectType = value; }
      }

      private string _factoryMethod;

      public string FactoryMethod
      {
        get { return _factoryMethod; }
        set { _factoryMethod = value; }
      }

      private ObservableCollection<object> _factoryParameters;

      public ObservableCollection<object> FactoryParameters
      {
        get { return _factoryParameters; }
        set { _factoryParameters = 
          new ObservableCollection<object>(new List<object>(value)); }
      }
      private bool _manageLifetime;

      public bool ManageObjectLifetime
      {
        get { return _manageLifetime; }
        set { _manageLifetime = value; }
      }
	
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
      Csla.Core.ISupportUndo undo = this.Data as Csla.Core.ISupportUndo;
      if (undo != null && _manageLifetime)
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
      Csla.Core.ISavable savable = this.Data as Csla.Core.ISavable;
      if (savable != null)
      {
        object result = savable;
        Exception exceptionResult = null;
        try
        {
          IsBusy = true;

          // clone the object if possible
          ICloneable clonable = savable as ICloneable;
          if (clonable != null)
            savable = (Csla.Core.ISavable)clonable.Clone();

          // apply edits in memory
          Csla.Core.ISupportUndo undo = savable as Csla.Core.ISupportUndo;
          if (undo != null && _manageLifetime)
            undo.ApplyEdit();


          // save the clone
          result = savable.Save();

          if (!ReferenceEquals(savable, this.Data) && !Csla.ApplicationContext.AutoCloneOnUpdate)
          {
            // raise Saved event from original object
            Core.ISavable original = this.Data as Core.ISavable;
            if (original != null)
              original.SaveComplete(result);
          }

          // start editing the resulting object
          undo = result as Csla.Core.ISupportUndo;
          if (undo != null && _manageLifetime)
            undo.BeginEdit();
        }
        catch (Exception ex)
        {
          exceptionResult = ex;
        }
        // clear previous object
        base.OnQueryFinished(null, exceptionResult, null, null);
        // return result to base class
        base.OnQueryFinished(result, null, null, null);
        IsBusy = false;
        OnSaved(result, exceptionResult, null);
      }
    }


    /// <summary>
    /// Adds a new item to the object if the object
    /// implements IBindingList and AllowNew is true.
    /// </summary>
    public object AddNew()
    {
      // only do something if the object implements
      // IBindingList
      IBindingList list = this.Data as IBindingList;
      if (list != null && list.AllowNew)
        return list.AddNew();
      else
        return null;

    }

    /// <summary>
    /// Removes an item from the list if the object
    /// implements IBindingList and AllowRemove is true.
    /// </summary>
    /// <param name="item">
    /// The item to be removed from the list.
    /// </param>
    public void RemoveItem(object item)
    {
      // only do something if the object implements
      // IBindingList
      IBindingList list;
      Csla.Core.BusinessBase bb = item as Csla.Core.BusinessBase;
      if (bb != null)
        list = bb.Parent as IBindingList;
      else
        list = this.Data as IBindingList;
      if (list != null && list.AllowRemove)
        list.Remove(item);
    }

    #endregion

  }
}