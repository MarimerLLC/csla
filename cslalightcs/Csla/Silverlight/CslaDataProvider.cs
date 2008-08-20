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

namespace Csla.Silverlight
{
  /// <summary>
  /// Creates, retrieves and manages business objects
  /// from XAML in a form.
  /// </summary>
  public class CslaDataProvider : INotifyPropertyChanged
  {

    #region Properties
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
        _dataObject = value;
        if (_manageObjectLifetime)
        {
          var undoable = _dataObject as Csla.Core.ISupportUndo;
          if (undoable != null)
            undoable.BeginEdit();
        }

        try
        {
          OnPropertyChanged(new PropertyChangedEventArgs("Data"));
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

    public void FetchCompleted(object sender, CslaDataProviderQueryCompletedEventArgs e)
    {
      if (_manageObjectLifetime && e.Data != null && e.Error == null)
      {
        this.Data = e.Data;
        this.Error = e.Error;
        _isInitialLoadCompleted = true;
      }
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
      if (_objectType != null && _fetchFactoryMethod != null)
        try
        {
          BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy;
          List<object> parameters = new List<object>(FactoryParameters);
          parameters.Add(new EventHandler<CslaDataProviderQueryCompletedEventArgs>(FetchCompleted));
          Type objectType = Type.GetType(_objectType);
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
            factory.Invoke(null,parameters.ToArray());
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
      if (_objectType != null && _fetchFactoryMethod != null)
        try
        {
          BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy;
          List<object> parameters = new List<object>(FactoryParameters);
          parameters.Add(new EventHandler<CslaDataProviderQueryCompletedEventArgs>(FetchCompleted));
          Type objectType = Type.GetType(_objectType);
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

    /// <summary>
    /// Begins an async add new operation to add a 
    /// new item to an editable list business object.
    /// </summary>
    public void AddNewItem()
    {
      var obj = _dataObject as Csla.Core.IBindingList;
      if (obj != null)
        obj.AddNew();
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
