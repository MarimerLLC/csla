//-----------------------------------------------------------------------
// <copyright file="FactoryDataPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Server-side data portal implementation that</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Configuration;
using System.Threading.Tasks;
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
              (IObjectFactoryLoader)Reflection.MethodCaller.CreateInstance(Type.GetType(setting, true, true));
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

    private async Task<DataPortalResult> InvokeMethod(string factoryTypeName, DataPortalOperations operation, string methodName, Type objectType, DataPortalContext context, bool isSync)
    {
      object factory = FactoryLoader.GetFactory(factoryTypeName);
      var eventArgs = new DataPortalEventArgs(context, objectType, null, operation);

      Csla.Reflection.MethodCaller.CallMethodIfImplemented(factory, "Invoke", eventArgs);
      object result = null;
      try
      {
        Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, factory, methodName);

        result = await Csla.Reflection.MethodCaller.CallMethodTryAsync(factory, methodName).ConfigureAwait(false);
        var error = result as Exception;
        if (error != null)
          throw error;

        var busy = result as Csla.Core.ITrackStatus;
        if (busy != null && busy.IsBusy)
          throw new InvalidOperationException(string.Format("{0}.IsBusy == true", objectType.Name));

        Csla.Reflection.MethodCaller.CallMethodIfImplemented(factory, "InvokeComplete", eventArgs);
      }
      catch (Exception ex)
      {
        Csla.Reflection.MethodCaller.CallMethodIfImplemented(
          factory, "InvokeError", new DataPortalEventArgs(context, objectType, null, operation, ex));
        throw;
      }
      return new DataPortalResult(result);
    }

    private async Task<DataPortalResult> InvokeMethod(string factoryTypeName, DataPortalOperations operation, string methodName, Type objectType, object e, DataPortalContext context, bool isSync)
    {
      object factory = FactoryLoader.GetFactory(factoryTypeName);
      var eventArgs = new DataPortalEventArgs(context, objectType, e, operation);

      Csla.Reflection.MethodCaller.CallMethodIfImplemented(factory, "Invoke", eventArgs);
      object result = null;
      try
      {
        Utilities.ThrowIfAsyncMethodOnSyncClient(isSync, factory, methodName, e);

        result = await Csla.Reflection.MethodCaller.CallMethodTryAsync(factory, methodName, e).ConfigureAwait(false);
        var error = result as Exception;
        if (error != null)
          throw error;

        var busy = result as Csla.Core.ITrackStatus;
        if (busy != null && busy.IsBusy)
          throw new InvalidOperationException(string.Format("{0}.IsBusy == true", objectType.Name));

        Csla.Reflection.MethodCaller.CallMethodIfImplemented(factory, "InvokeComplete", eventArgs);
      }
      catch (Exception ex)
      {
        Csla.Reflection.MethodCaller.CallMethodIfImplemented(factory, "InvokeError", new DataPortalEventArgs(context, objectType, e, operation, ex));
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
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public async Task<DataPortalResult> Create(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      try
      {
        DataPortalResult result = null;
        if (criteria is EmptyCriteria)
          result = await InvokeMethod(context.FactoryInfo.FactoryTypeName, DataPortalOperations.Create, context.FactoryInfo.CreateMethodName, objectType, context, isSync).ConfigureAwait(false);
        else
          result = await InvokeMethod(context.FactoryInfo.FactoryTypeName, DataPortalOperations.Create, context.FactoryInfo.CreateMethodName, objectType, criteria, context, isSync).ConfigureAwait(false);
        return result;
      }
      catch (Exception ex)
      {
        throw DataPortal.NewDataPortalException(
            context.FactoryInfo.CreateMethodName + " " + Resources.FailedOnServer,
            new DataPortalExceptionHandler().InspectException(objectType, criteria, context.FactoryInfo.CreateMethodName, ex),
            null);
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
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public async Task<DataPortalResult> Fetch(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      try
      {
        DataPortalResult result = null;
        if (criteria is EmptyCriteria)
          result = await InvokeMethod(context.FactoryInfo.FactoryTypeName, DataPortalOperations.Fetch, context.FactoryInfo.FetchMethodName, objectType, context, isSync).ConfigureAwait(false);
        else
          result = await InvokeMethod(context.FactoryInfo.FactoryTypeName, DataPortalOperations.Fetch, context.FactoryInfo.FetchMethodName, objectType, criteria, context, isSync).ConfigureAwait(false);
        return result;
      }
      catch (Exception ex)
      {
        throw DataPortal.NewDataPortalException(
         context.FactoryInfo.FetchMethodName + " " + Resources.FailedOnServer,
          new DataPortalExceptionHandler().InspectException(objectType, criteria, context.FactoryInfo.FetchMethodName, ex),
          null);
      }
    }

    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public async Task<DataPortalResult> Update(object obj, DataPortalContext context, bool isSync)
    {
      string methodName = string.Empty;
      try
      {
        DataPortalResult result = null;
        if (obj is Core.ICommandObject)
          methodName = context.FactoryInfo.ExecuteMethodName;
        else
          methodName = context.FactoryInfo.UpdateMethodName;

        result = await InvokeMethod(context.FactoryInfo.FactoryTypeName, DataPortalOperations.Update, methodName, obj.GetType(), obj, context, isSync).ConfigureAwait(false);
        return result;
      }
      catch (Exception ex)
      {
        throw DataPortal.NewDataPortalException(
          methodName + " " + Resources.FailedOnServer,
          new DataPortalExceptionHandler().InspectException(obj.GetType(), obj, null, methodName, ex),
          obj);

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
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public async Task<DataPortalResult> Delete(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      try
      {
        DataPortalResult result = null;
        if (criteria is EmptyCriteria)
          result = await InvokeMethod(context.FactoryInfo.FactoryTypeName, DataPortalOperations.Delete, context.FactoryInfo.DeleteMethodName, objectType, context, isSync).ConfigureAwait(false);
        else
          result = await InvokeMethod(context.FactoryInfo.FactoryTypeName, DataPortalOperations.Delete, context.FactoryInfo.DeleteMethodName, objectType, criteria, context, isSync).ConfigureAwait(false);
        return result;
      }
      catch (Exception ex)
      {
        throw DataPortal.NewDataPortalException(
          context.FactoryInfo.DeleteMethodName + " " + Resources.FailedOnServer,
          new DataPortalExceptionHandler().InspectException(objectType, criteria, context.FactoryInfo.DeleteMethodName, ex),
          null);
      }
    }

    #endregion
  }
}