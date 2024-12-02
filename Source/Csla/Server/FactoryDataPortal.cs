//-----------------------------------------------------------------------
// <copyright file="FactoryDataPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Server-side data portal implementation that</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Csla.Configuration;
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
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="applicationContext"></param>
    /// <param name="factoryLoader"></param>
    /// <param name="inspector"></param>
    /// <param name="dataPortalOptions"></param>
    public FactoryDataPortal(ApplicationContext applicationContext, IObjectFactoryLoader factoryLoader, IDataPortalExceptionInspector inspector, DataPortalOptions dataPortalOptions)
    {
      _applicationContext = applicationContext;
      FactoryLoader = factoryLoader;
      ExceptionInspector = inspector;
      DataPortalOptions = dataPortalOptions;
    }

    private ApplicationContext _applicationContext;
    private IObjectFactoryLoader FactoryLoader { get; set; }
    private IDataPortalExceptionInspector ExceptionInspector { get; set; }
    private DataPortalOptions DataPortalOptions { get; set; }

    #region Method invokes

    private async Task<DataPortalResult> InvokeMethod(string factoryTypeName, DataPortalOperations operation, string methodName, Type objectType, DataPortalContext context, bool isSync)
    {
      object factory = FactoryLoader.GetFactory(factoryTypeName);
      var eventArgs = new DataPortalEventArgs(context, objectType, null, operation);

      Reflection.MethodCaller.CallMethodIfImplemented(factory, "Invoke", eventArgs);
      object result;
      try
      {
        Utilities.ThrowIfAsyncMethodOnSyncClient(_applicationContext, isSync, factory, methodName);

        result = await Reflection.MethodCaller.CallMethodTryAsync(factory, methodName).ConfigureAwait(false);
        if (result is Exception error)
          throw error;

        if (result is Core.ITrackStatus busy && busy.IsBusy)
          throw new InvalidOperationException($"{objectType.Name}.IsBusy == true");

        Reflection.MethodCaller.CallMethodIfImplemented(factory, "InvokeComplete", eventArgs);
      }
      catch (Exception ex)
      {
        Reflection.MethodCaller.CallMethodIfImplemented(
          factory, "InvokeError", new DataPortalEventArgs(context, objectType, null, operation, ex));
        throw;
      }
      return new DataPortalResult(_applicationContext, result);
    }

    private async Task<DataPortalResult> InvokeMethod(string factoryTypeName, DataPortalOperations operation, string methodName, Type objectType, object e, DataPortalContext context, bool isSync)
    {
      object factory = FactoryLoader.GetFactory(factoryTypeName);
      var eventArgs = new DataPortalEventArgs(context, objectType, e, operation);

      Reflection.MethodCaller.CallMethodIfImplemented(factory, "Invoke", eventArgs);
      object result;
      try
      {
        Utilities.ThrowIfAsyncMethodOnSyncClient(_applicationContext, isSync, factory, methodName, e);

        result = await Reflection.MethodCaller.CallMethodTryAsync(factory, methodName, e).ConfigureAwait(false);
        if (result is Exception error)
          throw error;

        if (result is Core.ITrackStatus busy && busy.IsBusy)
          throw new InvalidOperationException($"{objectType.Name}.IsBusy == true");

        Reflection.MethodCaller.CallMethodIfImplemented(factory, "InvokeComplete", eventArgs);
      }
      catch (Exception ex)
      {
        Reflection.MethodCaller.CallMethodIfImplemented(factory, "InvokeError", new DataPortalEventArgs(context, objectType, e, operation, ex));
        throw;
      }
      return new DataPortalResult(_applicationContext, result);
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
    public async Task<DataPortalResult> Create(
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
      Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      try
      {
        DataPortalResult result;
        if (criteria is EmptyCriteria)
          result = await InvokeMethod(context.FactoryInfo.FactoryTypeName, DataPortalOperations.Create, context.FactoryInfo.CreateMethodName, objectType, context, isSync).ConfigureAwait(false);
        else
          result = await InvokeMethod(context.FactoryInfo.FactoryTypeName, DataPortalOperations.Create, context.FactoryInfo.CreateMethodName, objectType, criteria, context, isSync).ConfigureAwait(false);
        return result;
      }
      catch (Exception ex)
      {
        throw DataPortal.NewDataPortalException(
            _applicationContext, context.FactoryInfo.CreateMethodName + " " + Resources.FailedOnServer,
            new DataPortalExceptionHandler(ExceptionInspector).InspectException(objectType, criteria, context.FactoryInfo.CreateMethodName, ex),
            null, DataPortalOptions);
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
    public async Task<DataPortalResult> Fetch(
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
      Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (typeof(Core.ICommandObject).IsAssignableFrom(objectType))
      {
        return await Execute(objectType, criteria, context, isSync);
      }

      try
      {
        DataPortalResult result;
        if (criteria is EmptyCriteria)
          result = await InvokeMethod(context.FactoryInfo.FactoryTypeName, DataPortalOperations.Fetch, context.FactoryInfo.FetchMethodName, objectType, context, isSync).ConfigureAwait(false);
        else
          result = await InvokeMethod(context.FactoryInfo.FactoryTypeName, DataPortalOperations.Fetch, context.FactoryInfo.FetchMethodName, objectType, criteria, context, isSync).ConfigureAwait(false);
        return result;
      }
      catch (Exception ex)
      {
        throw DataPortal.NewDataPortalException(
          _applicationContext, context.FactoryInfo.FetchMethodName + " " + Resources.FailedOnServer,
          new DataPortalExceptionHandler(ExceptionInspector).InspectException(objectType, criteria, context.FactoryInfo.FetchMethodName, ex),
          null, DataPortalOptions);
      }
    }

    private async Task<DataPortalResult> Execute(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      try
      {
        DataPortalResult result;
        if (criteria is EmptyCriteria)
          result = await InvokeMethod(context.FactoryInfo.FactoryTypeName, DataPortalOperations.Execute, context.FactoryInfo.ExecuteMethodName, objectType, context, isSync).ConfigureAwait(false);
        else
          result = await InvokeMethod(context.FactoryInfo.FactoryTypeName, DataPortalOperations.Execute, context.FactoryInfo.ExecuteMethodName, objectType, criteria, context, isSync).ConfigureAwait(false);
        return result;
      }
      catch (Exception ex)
      {
        throw DataPortal.NewDataPortalException(
          _applicationContext, context.FactoryInfo.ExecuteMethodName + " " + Resources.FailedOnServer,
          new DataPortalExceptionHandler(ExceptionInspector).InspectException(objectType, criteria, context.FactoryInfo.ExecuteMethodName, ex),
          null, DataPortalOptions);
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
        DataPortalResult result;
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
          _applicationContext, methodName + " " + Resources.FailedOnServer,
          new DataPortalExceptionHandler(ExceptionInspector).InspectException(obj.GetType(), obj, null, methodName, ex),
          obj, DataPortalOptions);

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
    public async Task<DataPortalResult> Delete(
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
      Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      try
      {
        DataPortalResult result;
        if (criteria is EmptyCriteria)
          result = await InvokeMethod(context.FactoryInfo.FactoryTypeName, DataPortalOperations.Delete, context.FactoryInfo.DeleteMethodName, objectType, context, isSync).ConfigureAwait(false);
        else
          result = await InvokeMethod(context.FactoryInfo.FactoryTypeName, DataPortalOperations.Delete, context.FactoryInfo.DeleteMethodName, objectType, criteria, context, isSync).ConfigureAwait(false);
        return result;
      }
      catch (Exception ex)
      {
        throw DataPortal.NewDataPortalException(
          _applicationContext, context.FactoryInfo.DeleteMethodName + " " + Resources.FailedOnServer,
          new DataPortalExceptionHandler(ExceptionInspector).InspectException(objectType, criteria, context.FactoryInfo.DeleteMethodName, ex),
          null, DataPortalOptions);
      }
    }

    #endregion
  }
}