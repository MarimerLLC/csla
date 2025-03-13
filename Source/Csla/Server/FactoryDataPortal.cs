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
    private readonly ApplicationContext _applicationContext;
    private readonly IObjectFactoryLoader _factoryLoader;
    private readonly IDataPortalExceptionInspector _exceptionInspector;
    private readonly DataPortalOptions _dataPortalOptions;

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/>, <paramref name="factoryLoader"/>, <paramref name="inspector"/> or <paramref name="dataPortalOptions"/> is <see langword="null"/>.</exception>
    public FactoryDataPortal(ApplicationContext applicationContext, IObjectFactoryLoader factoryLoader, IDataPortalExceptionInspector inspector, DataPortalOptions dataPortalOptions)
    {
      _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
      _factoryLoader = factoryLoader ?? throw new ArgumentNullException(nameof(factoryLoader));
      _exceptionInspector = inspector ?? throw new ArgumentNullException(nameof(inspector));
      _dataPortalOptions = dataPortalOptions ?? throw new ArgumentNullException(nameof(dataPortalOptions));
    }

    #region Method invokes

    private async Task<DataPortalResult> InvokeMethod(string factoryTypeName, DataPortalOperations operation, string methodName, Type objectType, DataPortalContext context, bool isSync)
    {
      object factory = _factoryLoader.GetFactory(factoryTypeName);
      var eventArgs = new DataPortalEventArgs(context, objectType, null, operation);

      Reflection.MethodCaller.CallMethodIfImplemented(factory, "Invoke", eventArgs);
      object? result;
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
      return new DataPortalResult(_applicationContext, result, null);
    }

    private async Task<DataPortalResult> InvokeMethod(string factoryTypeName, DataPortalOperations operation, string methodName, Type objectType, object e, DataPortalContext context, bool isSync)
    {
      object factory = _factoryLoader.GetFactory(factoryTypeName);
      var eventArgs = new DataPortalEventArgs(context, objectType, e, operation);

      Reflection.MethodCaller.CallMethodIfImplemented(factory, "Invoke", eventArgs);
      object? result;
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
      return new DataPortalResult(_applicationContext, result, null);
    }

    #endregion

    #region IDataPortalServer Members

    /// <inheritdoc />
    public async Task<DataPortalResult> Create([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      if (criteria is null)
        throw new ArgumentNullException(nameof(criteria));
      if (context is null)
        throw new ArgumentNullException(nameof(context));
      if (context.FactoryInfo is null)
        throw new ArgumentException($"Internal: No {nameof(DataPortalContext.FactoryInfo)} provided for the {nameof(FactoryDataPortal)}. This is an internal bug.");

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
            new DataPortalExceptionHandler(_exceptionInspector).InspectException(objectType, criteria, context.FactoryInfo.CreateMethodName, ex),
            null, _dataPortalOptions);
      }
    }

    /// <inheritdoc />
    public async Task<DataPortalResult> Fetch([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      if (criteria is null)
        throw new ArgumentNullException(nameof(criteria));
      if (context is null)
        throw new ArgumentNullException(nameof(context));
      if (context.FactoryInfo is null)
        throw new ArgumentException($"Internal: No {nameof(DataPortalContext.FactoryInfo)} provided for the {nameof(FactoryDataPortal)}. This is an internal bug.");

      if (typeof(Core.ICommandObject).IsAssignableFrom(objectType))
      {
        return await Execute(objectType, criteria, context, isSync, context.FactoryInfo);
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
          new DataPortalExceptionHandler(_exceptionInspector).InspectException(objectType, criteria, context.FactoryInfo.FetchMethodName, ex),
          null, _dataPortalOptions);
      }
    }

    private async Task<DataPortalResult> Execute(Type objectType, object criteria, DataPortalContext context, bool isSync, ObjectFactoryAttribute factoryInfo)
    {
      try
      {
        DataPortalResult result;
        if (criteria is EmptyCriteria)
          result = await InvokeMethod(factoryInfo.FactoryTypeName, DataPortalOperations.Execute, factoryInfo.ExecuteMethodName, objectType, context, isSync).ConfigureAwait(false);
        else
          result = await InvokeMethod(factoryInfo.FactoryTypeName, DataPortalOperations.Execute, factoryInfo.ExecuteMethodName, objectType, criteria, context, isSync).ConfigureAwait(false);
        return result;
      }
      catch (Exception ex)
      {
        throw DataPortal.NewDataPortalException(
          _applicationContext, factoryInfo.ExecuteMethodName + " " + Resources.FailedOnServer,
          new DataPortalExceptionHandler(_exceptionInspector).InspectException(objectType, criteria, factoryInfo.ExecuteMethodName, ex),
          null, _dataPortalOptions);
      }
    }

    /// <inheritdoc />
    public async Task<DataPortalResult> Update(object obj, DataPortalContext context, bool isSync)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));
      if (context is null)
        throw new ArgumentNullException(nameof(context));
      if (context.FactoryInfo is null)
        throw new ArgumentException($"Internal: No {nameof(DataPortalContext.FactoryInfo)} provided for the {nameof(FactoryDataPortal)}. This is an internal bug.");


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
          new DataPortalExceptionHandler(_exceptionInspector).InspectException(obj.GetType(), obj, null, methodName, ex),
          obj, _dataPortalOptions);

      }
    }

    /// <inheritdoc />
    public async Task<DataPortalResult> Delete([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      if (criteria is null)
        throw new ArgumentNullException(nameof(criteria));
      if (context is null)
        throw new ArgumentNullException(nameof(context));
      if (context.FactoryInfo is null)
        throw new ArgumentException($"Internal: No {nameof(DataPortalContext.FactoryInfo)} provided for the {nameof(FactoryDataPortal)}. This is an internal bug.");

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
          new DataPortalExceptionHandler(_exceptionInspector).InspectException(objectType, criteria, context.FactoryInfo.DeleteMethodName, ex),
          null, _dataPortalOptions);
      }
    }

    #endregion
  }
}