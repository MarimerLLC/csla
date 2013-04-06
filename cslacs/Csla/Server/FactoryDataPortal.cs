using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Properties;

namespace Csla.Server
{
  /// <summary>
  /// Server-side data portal implementation that
  /// invokes an object factory rather than directly
  /// interacting with the business object.
  /// </summary>
  public class FactoryDataPortal : IDataPortalServer
  {
    #region Factory Loader

    private static IObjectFactoryLoader _factoryLoader;

    /// <summary>
    /// Gets or sets a delegate reference to the method
    /// called to create instances of factory objects
    /// as requested by the ObjectFactory attribute on
    /// a CSLA .NET business object.
    /// </summary>
    public static IObjectFactoryLoader FactoryLoader
    {
      get
      {
        if (_factoryLoader == null)
        {
          string setting = ConfigurationManager.AppSettings["CslaObjectFactoryLoader"];
          if (!string.IsNullOrEmpty(setting))
            _factoryLoader =
              (IObjectFactoryLoader)Activator.CreateInstance(Type.GetType(setting, true, true));
          else
            _factoryLoader = new ObjectFactoryLoader();
        }
        return _factoryLoader;
      }
      set
      {
        _factoryLoader = value;
      }
    }

    #endregion

    #region Method invokes

    private DataPortalResult InvokeMethod(string factoryTypeName, string methodName, DataPortalContext context)
    {
      object factory = FactoryLoader.GetFactory(factoryTypeName);
      Csla.Reflection.MethodCaller.CallMethodIfImplemented(factory, "Invoke", context);
      object result = null;
      try
      {
        result = Csla.Reflection.MethodCaller.CallMethod(factory, methodName);
        Csla.Reflection.MethodCaller.CallMethodIfImplemented(factory, "InvokeComplete", context);
      }
      catch (Exception ex)
      {
        Csla.Reflection.MethodCaller.CallMethodIfImplemented(factory, "InvokeError", ex);
        throw;
      }
      return new DataPortalResult(result);
    }

    private DataPortalResult InvokeMethod(string factoryTypeName, string methodName, object e, DataPortalContext context)
    {
      object factory = FactoryLoader.GetFactory(factoryTypeName);
      Csla.Reflection.MethodCaller.CallMethodIfImplemented(factory, "Invoke", context);
      object result = null;
      try
      {
        result = Csla.Reflection.MethodCaller.CallMethod(factory, methodName, e);
        Csla.Reflection.MethodCaller.CallMethodIfImplemented(factory, "InvokeComplete", context);
      }
      catch (Exception ex)
      {
        Csla.Reflection.MethodCaller.CallMethodIfImplemented(factory, "InvokeError", ex);
        throw;
      }
      return new DataPortalResult(result);
    }

    #endregion

    #region IDataPortalServer Members

    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    public DataPortalResult Create(Type objectType, object criteria, DataPortalContext context)
    {
      try
      {
        DataPortalResult result = null;
        if (criteria is int)
          result = InvokeMethod(context.FactoryInfo.FactoryTypeName, context.FactoryInfo.CreateMethodName, context);
        else
          result = InvokeMethod(context.FactoryInfo.FactoryTypeName, context.FactoryInfo.CreateMethodName, criteria, context);
        return result;
      }
      catch (Exception ex)
      {
        throw new DataPortalException(
          context.FactoryInfo.CreateMethodName + " " + Resources.FailedOnServer,
          ex, new DataPortalResult());
      }
    }

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="objectType">Type of business object to retrieve.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    public DataPortalResult Fetch(Type objectType, object criteria, DataPortalContext context)
    {
      try
      {
        DataPortalResult result = null;
        if (criteria is int)
          result = InvokeMethod(context.FactoryInfo.FactoryTypeName, context.FactoryInfo.FetchMethodName, context);
        else
          result = InvokeMethod(context.FactoryInfo.FactoryTypeName, context.FactoryInfo.FetchMethodName, criteria, context);
        return result;
      }
      catch (Exception ex)
      {
        throw new DataPortalException(
          context.FactoryInfo.FetchMethodName + " " + Resources.FailedOnServer,
          ex, new DataPortalResult());
      }
    }

    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    public DataPortalResult Update(object obj, DataPortalContext context)
    {
      try
      {
        DataPortalResult result = null;
        result = InvokeMethod(context.FactoryInfo.FactoryTypeName, context.FactoryInfo.UpdateMethodName, obj, context);
        return result;
      }
      catch (Exception ex)
      {
        throw new DataPortalException(
          context.FactoryInfo.UpdateMethodName + " " + Resources.FailedOnServer,
          ex, new DataPortalResult());
      }
    }

    /// <summary>
    /// Delete a business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    public DataPortalResult Delete(Type objectType, object criteria, DataPortalContext context)
    {
      try
      {
        DataPortalResult result = null;
        if (criteria is int)
          result = InvokeMethod(context.FactoryInfo.FactoryTypeName, context.FactoryInfo.DeleteMethodName, context);
        else
          result = InvokeMethod(context.FactoryInfo.FactoryTypeName, context.FactoryInfo.DeleteMethodName, criteria, context);
        return result;
      }
      catch (Exception ex)
      {
        throw new DataPortalException(
          context.FactoryInfo.DeleteMethodName + " " + Resources.FailedOnServer,
          ex, new DataPortalResult());
      }
    }

    #endregion

  }
}
