using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Data;
using System.Reflection;
using Csla.Properties;

namespace Csla.Wpf
{
  /// <summary>
  /// Wraps and creates a CSLA .NET-style object 
  /// that you can use as a binding source.
  /// </summary>
  public class CslaDataProvider : DataSourceProvider
  {
    private Type _objectType = null;
    private string _factoryMethod = string.Empty;
    private ObservableCollection<object> _factoryParameters;
    private bool _isAsynchronous;

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    public CslaDataProvider()
    {
      _factoryParameters = new ObservableCollection<object>();
      _factoryParameters.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_factoryParameters_CollectionChanged);
    }

    void _factoryParameters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
      BeginQuery();
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
        OnPropertyChanged(new PropertyChangedEventArgs("TypeName"));
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
        OnPropertyChanged(new PropertyChangedEventArgs("GetFactoryMethod"));
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
    /// Overridden. Starts to create the requested object, 
    /// either immediately or on a background thread, 
    /// based on the value of the IsAsynchronous property.
    /// </summary>
    protected override void BeginQuery()
    {
      if (this.IsRefreshDeferred)
        return;
      QueryRequest request = new QueryRequest();
      request.ObjectType = _objectType;
      request.FactoryMethod = _factoryMethod;
      request.FactoryParameters = _factoryParameters;

      if (IsAsynchronous)
        System.Threading.ThreadPool.QueueUserWorkItem(DoQuery, request);
      else
        DoQuery(request);
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
    }

    #endregion
  }
}
