//-----------------------------------------------------------------------
// <copyright file="DataPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements the server-side DataPortal </summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Security.Principal;
using Csla.Configuration;
using Csla.Properties;
using Csla.Reflection;
using Csla.Server;
using Csla.Server.Dashboard;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Server
{
  /// <summary>
  /// Implements the server-side DataPortal 
  /// message router as discussed
  /// in Chapter 4.
  /// </summary>
  public class DataPortal : IDataPortalServer
  {
    /// <summary>
    /// Gets or sets the current ApplicationContext object.
    /// </summary>
    private ApplicationContext _applicationContext;

    /// <summary>
    /// Gets the data portal dashboard instance.
    /// </summary>
    private IDashboard Dashboard { get; }
    private DataPortalOptions DataPortalOptions { get; }
    private IAuthorizeDataPortal Authorizer { get; }
    private InterceptorManager InterceptorManager { get; }
    private IObjectFactoryLoader FactoryLoader { get; }
    private IDataPortalActivator Activator { get; }
    private IDataPortalExceptionInspector ExceptionInspector { get; }
    private DataPortalExceptionHandler DataPortalExceptionHandler { get; }
    private SecurityOptions SecurityOptions { get; }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="applicationContext"></param>
    /// <param name="dashboard"></param>
    /// <param name="options"></param>
    /// <param name="activator"></param>
    /// <param name="authorizer"></param>
    /// <param name="exceptionInspector"></param>
    /// <param name="factoryLoader"></param>
    /// <param name="interceptors"></param>
    /// <param name="exceptionHandler"></param>
    /// <param name="securityOptions"></param>
    /// <exception cref="ArgumentNullException">Any parameter is <see langword="null"/>.</exception>
    public DataPortal(
      ApplicationContext applicationContext,
      IDashboard dashboard,
      CslaOptions options,
      IAuthorizeDataPortal authorizer,
      InterceptorManager interceptors,
      IObjectFactoryLoader factoryLoader,
      IDataPortalActivator activator,
      IDataPortalExceptionInspector exceptionInspector,
      DataPortalExceptionHandler exceptionHandler,
      SecurityOptions securityOptions)
    {
      if (options is null)
        throw new ArgumentNullException(nameof(options));
      _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
      Dashboard = dashboard ?? throw new ArgumentNullException(nameof(dashboard));
      DataPortalOptions = options.DataPortalOptions;
      Authorizer = authorizer ?? throw new ArgumentNullException(nameof(authorizer));
      InterceptorManager = interceptors ?? throw new ArgumentNullException(nameof(interceptors));
      FactoryLoader = factoryLoader ?? throw new ArgumentNullException(nameof(factoryLoader));
      Activator = activator ?? throw new ArgumentNullException(nameof(activator));
      ExceptionInspector = exceptionInspector ?? throw new ArgumentNullException(nameof(exceptionInspector));
      DataPortalExceptionHandler = exceptionHandler ?? throw new ArgumentNullException(nameof(exceptionHandler));
      SecurityOptions = securityOptions ?? throw new ArgumentNullException(nameof(securityOptions));
    }

    #region Data Access

#if !NETSTANDARD2_0 && !NET8_0_OR_GREATER
    private IDataPortalServer GetServicedComponentPortal(TransactionalAttribute transactionalAttribute)
    {
      switch (transactionalAttribute.TransactionIsolationLevel)
      {
        case TransactionIsolationLevel.Serializable:
          return _applicationContext.CreateInstanceDI<ServicedDataPortalSerializable>();
        case TransactionIsolationLevel.RepeatableRead:
          return _applicationContext.CreateInstanceDI<ServicedDataPortalRepeatableRead>();
        case TransactionIsolationLevel.ReadCommitted:
          return _applicationContext.CreateInstanceDI<ServicedDataPortalReadCommitted>();
        case TransactionIsolationLevel.ReadUncommitted:
          return _applicationContext.CreateInstanceDI<ServicedDataPortalReadUncommitted>();
        default:
          throw new ArgumentOutOfRangeException(nameof(transactionalAttribute));
      }
    }
#endif

    private Reflection.ServiceProviderMethodCaller? serviceProviderMethodCaller;
    private Reflection.ServiceProviderMethodCaller ServiceProviderMethodCaller
    {
      get
      {
        if (serviceProviderMethodCaller == null)
          serviceProviderMethodCaller = _applicationContext.CreateInstanceDI<Reflection.ServiceProviderMethodCaller>();
        return serviceProviderMethodCaller;
      }
    }

    /// <inheritdoc />
    public async Task<DataPortalResult> Create([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      if (criteria is null)
        throw new ArgumentNullException(nameof(criteria));
      if (context is null)
        throw new ArgumentNullException(nameof(context));

      try
      {
        SetContext(context);

        await AuthorizeRequestAsync(new AuthorizeRequest(objectType, criteria, DataPortalOperations.Create), CancellationToken.None);

        await InitializeAsync(new InterceptArgs(objectType, criteria, DataPortalOperations.Create, isSync));

        var method = ServiceProviderMethodCaller.GetDataPortalMethodInfoFor<CreateAttribute>(objectType, criteria);

        DataPortalResult result;
        IDataPortalServer portal;
        switch (method.TransactionalAttribute.TransactionType)
        {
#if !NETSTANDARD2_0 && !NET8_0_OR_GREATER
          case TransactionalTypes.EnterpriseServices:
            portal = GetServicedComponentPortal(method.TransactionalAttribute);
            try
            {
              result = await portal.Create(objectType, criteria, context, isSync).ConfigureAwait(false);
            }
            finally
            {
              ((System.EnterpriseServices.ServicedComponent)portal).Dispose();
            }

            break;
#endif
          case TransactionalTypes.TransactionScope:
#if NET8_0_OR_GREATER
            if (OperatingSystem.IsBrowser())
            {
              throw new PlatformNotSupportedException(Resources.TransactionScopeTransactionNotSupportedException);
            }
#endif
            var broker = _applicationContext.CreateInstanceDI<DataPortalBroker>();
            portal = new TransactionalDataPortal(broker, method.TransactionalAttribute);
            result = await portal.Create(objectType, criteria, context, isSync).ConfigureAwait(false);
            break;
          default:
            portal = _applicationContext.CreateInstanceDI<DataPortalBroker>();
            result = await portal.Create(objectType, criteria, context, isSync).ConfigureAwait(false);
            break;
        }
        Complete(new InterceptArgs(objectType, criteria, result, DataPortalOperations.Create, isSync));
        return result;
      }
      catch (DataPortalException ex)
      {
        Complete(new InterceptArgs(objectType, criteria, ex, DataPortalOperations.Create, isSync));
        throw;
      }
      catch (Exception ex)
      {
        if (ex is AggregateException aggregateEx && aggregateEx.InnerExceptions.Count > 0)
        {
            ex = aggregateEx.InnerExceptions[0].InnerException ?? aggregateEx;
        }

        var fex = NewDataPortalException(
            _applicationContext, "DataPortal.Create " + Resources.FailedOnServer,
            DataPortalExceptionHandler.InspectException(objectType, criteria, "DataPortal.Create", ex),
            null, DataPortalOptions);
        Complete(new InterceptArgs(objectType, criteria, fex, DataPortalOperations.Create, isSync));
        throw fex;
      }
      finally
      {
        ClearContext(context);
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

      if (typeof(Core.ICommandObject).IsAssignableFrom(objectType))
      {
        return await Execute(objectType, criteria, context, isSync);
      }

      try
      {
        SetContext(context);

        await AuthorizeRequestAsync(new AuthorizeRequest(objectType, criteria, DataPortalOperations.Fetch), CancellationToken.None);

        await InitializeAsync(new InterceptArgs(objectType, criteria, DataPortalOperations.Fetch, isSync));

        var method = ServiceProviderMethodCaller.GetDataPortalMethodInfoFor<FetchAttribute>(objectType, criteria);

        DataPortalResult result;
        IDataPortalServer portal;
        switch (method.TransactionalAttribute.TransactionType)
        {
#if !NETSTANDARD2_0 && !NET8_0_OR_GREATER
          case TransactionalTypes.EnterpriseServices:
            portal = GetServicedComponentPortal(method.TransactionalAttribute);
            try
            {
              result = await portal.Fetch(objectType, criteria, context, isSync).ConfigureAwait(false);
            }
            finally
            {
              ((System.EnterpriseServices.ServicedComponent)portal).Dispose();
            }
            break;
#endif
          case TransactionalTypes.TransactionScope:
#if NET8_0_OR_GREATER
            if (OperatingSystem.IsBrowser())
            {
              throw new PlatformNotSupportedException(Resources.TransactionScopeTransactionNotSupportedException);
            }
#endif
            var broker = _applicationContext.CreateInstanceDI<DataPortalBroker>();
            portal = new TransactionalDataPortal(broker, method.TransactionalAttribute);
            result = await portal.Fetch(objectType, criteria, context, isSync).ConfigureAwait(false);
            break;
          default:
            portal = _applicationContext.CreateInstanceDI<DataPortalBroker>();
            result = await portal.Fetch(objectType, criteria, context, isSync).ConfigureAwait(false);
            break;
        }
        Complete(new InterceptArgs(objectType, criteria, result, DataPortalOperations.Fetch, isSync));
        return result;
      }
      catch (DataPortalException ex)
      {
        Complete(new InterceptArgs(objectType, criteria, ex, DataPortalOperations.Fetch, isSync));
        throw;
      }
      catch (Exception ex)
      {
        if (ex is AggregateException aggregateException && aggregateException.InnerExceptions.Count > 0)
        {
          ex = aggregateException.InnerExceptions[0].InnerException ?? ex;
        }

        var fex = NewDataPortalException(
            _applicationContext, "DataPortal.Fetch " + Resources.FailedOnServer,
            DataPortalExceptionHandler.InspectException(objectType, criteria, "DataPortal.Fetch", ex),
            null, DataPortalOptions);
        Complete(new InterceptArgs(objectType, criteria, fex, DataPortalOperations.Fetch, isSync));
        throw fex;
      }
      finally
      {
        ClearContext(context);
      }
    }

    /// <summary>
    /// Execute a command.
    /// </summary>
    /// <param name="objectType">Type of command object to execute.</param>
    /// <param name="criteria">Criteria for the command.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    private async Task<DataPortalResult> Execute(
#if NET8_0_OR_GREATER
      [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
      Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      try
      {
        SetContext(context);

        await AuthorizeRequestAsync(new AuthorizeRequest(objectType, criteria, DataPortalOperations.Execute), CancellationToken.None);

        await InitializeAsync(new InterceptArgs(objectType, criteria, DataPortalOperations.Execute, isSync));

        var method = ServiceProviderMethodCaller.GetDataPortalMethodInfoFor<ExecuteAttribute>(objectType, criteria);

        DataPortalResult result;
        IDataPortalServer portal;
        switch (method.TransactionalAttribute.TransactionType)
        {
#if !NETSTANDARD2_0 && !NET8_0_OR_GREATER
          case TransactionalTypes.EnterpriseServices:
            portal = GetServicedComponentPortal(method.TransactionalAttribute);
            try
            {
              result = await portal.Fetch(objectType, criteria, context, isSync).ConfigureAwait(false);
            }
            finally
            {
              ((System.EnterpriseServices.ServicedComponent)portal).Dispose();
            }
            break;
#endif
          case TransactionalTypes.TransactionScope:
#if NET8_0_OR_GREATER
            if (OperatingSystem.IsBrowser())
            {
              throw new PlatformNotSupportedException(Resources.TransactionScopeTransactionNotSupportedException);
            }
#endif
            var broker = _applicationContext.CreateInstanceDI<DataPortalBroker>();
            portal = new TransactionalDataPortal(broker, method.TransactionalAttribute);
            result = await portal.Fetch(objectType, criteria, context, isSync).ConfigureAwait(false);
            break;
          default:
            portal = _applicationContext.CreateInstanceDI<DataPortalBroker>();
            result = await portal.Fetch(objectType, criteria, context, isSync).ConfigureAwait(false);
            break;
        }
        Complete(new InterceptArgs(objectType, criteria, result, DataPortalOperations.Execute, isSync));
        return result;
      }
      catch (DataPortalException ex)
      {
        Complete(new InterceptArgs(objectType, criteria, ex, DataPortalOperations.Execute, isSync));
        throw;
      }
      catch (Exception ex)
      {
        if (ex is AggregateException aggregateException && aggregateException.InnerExceptions.Count > 0)
        {
          ex = aggregateException.InnerExceptions[0].InnerException ?? ex;
        }

        var fex = NewDataPortalException(
            _applicationContext, "DataPortal.Execute " + Resources.FailedOnServer,
            DataPortalExceptionHandler.InspectException(objectType, criteria, "DataPortal.Execute", ex),
            null, DataPortalOptions);
        Complete(new InterceptArgs(objectType, criteria, fex, DataPortalOperations.Execute, isSync));
        throw fex;
      }
      finally
      {
        ClearContext(context);
      }
    }

    /// <inheritdoc />
    public async Task<DataPortalResult> Update(object obj, DataPortalContext context, bool isSync)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));
      if (context is null)
        throw new ArgumentNullException(nameof(context));

      var objectType = obj.GetType();
      var operation = DataPortalOperations.Update;
      try
      {
        SetContext(context);

        if (obj is Core.ICommandObject)
          operation = DataPortalOperations.Execute;

        await AuthorizeRequestAsync(new AuthorizeRequest(objectType, obj, operation), CancellationToken.None);

        await InitializeAsync(new InterceptArgs(objectType, obj, operation, isSync));

        DataPortalMethodInfo method;
        var factoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(objectType);
        if (factoryInfo != null)
        {
          string methodName;
          var factoryLoader = _applicationContext.CurrentServiceProvider.GetRequiredService<IObjectFactoryLoader>();
          var factoryType = factoryLoader.GetFactoryType(factoryInfo.FactoryTypeName);
          if (obj is Core.BusinessBase bbase)
          {
            if (bbase.IsDeleted)
              methodName = factoryInfo.DeleteMethodName;
            else
              methodName = factoryInfo.UpdateMethodName;
          }
          else if (obj is Core.ICommandObject)
            methodName = factoryInfo.ExecuteMethodName;
          else
            methodName = factoryInfo.UpdateMethodName;
          method = DataPortalMethodCache.GetMethodInfo(factoryType, methodName, [obj]);
        }
        else
        {
          if (obj is Core.BusinessBase bbase)
          {
            if (bbase.IsDeleted)
              method = ServiceProviderMethodCaller.GetDataPortalMethodInfoFor<DeleteSelfAttribute>(objectType);
            else if (bbase.IsNew)
              method = ServiceProviderMethodCaller.GetDataPortalMethodInfoFor<InsertAttribute>(objectType);
            else
              method = ServiceProviderMethodCaller.GetDataPortalMethodInfoFor<UpdateAttribute>(objectType);
          }
          else if (obj is Core.ICommandObject)
            method = ServiceProviderMethodCaller.GetDataPortalMethodInfoFor<ExecuteAttribute>(objectType);
          else
            method = ServiceProviderMethodCaller.GetDataPortalMethodInfoFor<UpdateAttribute>(objectType);
        }

        context.TransactionalType = method.TransactionalAttribute.TransactionType;
        DataPortalResult result;
        IDataPortalServer portal;
        switch (method.TransactionalAttribute.TransactionType)
        {
#if !NETSTANDARD2_0 && !NET8_0_OR_GREATER
          case TransactionalTypes.EnterpriseServices:
            portal = GetServicedComponentPortal(method.TransactionalAttribute);
            try
            {
              result = await portal.Update(obj, context, isSync).ConfigureAwait(false);
            }
            finally
            {
              ((System.EnterpriseServices.ServicedComponent)portal).Dispose();
            }
            break;
#endif
          case TransactionalTypes.TransactionScope:
#if NET8_0_OR_GREATER
            if (OperatingSystem.IsBrowser())
            {
              throw new PlatformNotSupportedException(Resources.TransactionScopeTransactionNotSupportedException);
            }
#endif
            var broker = _applicationContext.CreateInstanceDI<DataPortalBroker>();
            portal = new TransactionalDataPortal(broker, method.TransactionalAttribute);
            result = await portal.Update(obj, context, isSync).ConfigureAwait(false);
            break;
          default:
            portal = _applicationContext.CreateInstanceDI<DataPortalBroker>();
            result = await portal.Update(obj, context, isSync).ConfigureAwait(false);
            break;
        }
        Complete(new InterceptArgs(objectType, obj, result, operation, isSync));
        return result;
      }
      catch (DataPortalException ex)
      {
        Complete(new InterceptArgs(objectType, obj, ex, operation, isSync));
        throw;
      }
      catch (Exception ex)
      {
        if (ex is AggregateException aggregateException && aggregateException.InnerExceptions.Count > 0)
        {
          ex = aggregateException.InnerExceptions[0].InnerException ?? ex;
        }

        var fex = NewDataPortalException(
            _applicationContext, "DataPortal.Update " + Resources.FailedOnServer,
            DataPortalExceptionHandler.InspectException(obj.GetType(), obj, null, "DataPortal.Update", ex),
            obj, DataPortalOptions);
        Complete(new InterceptArgs(objectType, obj, fex, operation, isSync));
        throw fex;
      }
      finally
      {
        ClearContext(context);
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

      try
      {
        SetContext(context);

        await AuthorizeRequestAsync(new AuthorizeRequest(objectType, criteria, DataPortalOperations.Delete), CancellationToken.None);

        await InitializeAsync(new InterceptArgs(objectType, criteria, DataPortalOperations.Delete, isSync));

        DataPortalResult result;
        DataPortalMethodInfo method;
        var factoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(objectType);
        if (factoryInfo != null)
        {
          var factoryLoader = _applicationContext.CurrentServiceProvider.GetRequiredService<IObjectFactoryLoader>();
          var factoryType = factoryLoader.GetFactoryType(factoryInfo.FactoryTypeName);
          string methodName = factoryInfo.DeleteMethodName;
          method = DataPortalMethodCache.GetMethodInfo(factoryType, methodName, criteria);
        }
        else
        {
          method = ServiceProviderMethodCaller.GetDataPortalMethodInfoFor<DeleteAttribute>(objectType, criteria);
        }

        IDataPortalServer portal;
        switch (method.TransactionalAttribute.TransactionType)
        {
#if !NETSTANDARD2_0 && !NET8_0_OR_GREATER
          case TransactionalTypes.EnterpriseServices:
            portal = GetServicedComponentPortal(method.TransactionalAttribute);
            try
            {
              result = await portal.Delete(objectType, criteria, context, isSync).ConfigureAwait(false);
            }
            finally
            {
              ((System.EnterpriseServices.ServicedComponent)portal).Dispose();
            }
            break;
#endif
          case TransactionalTypes.TransactionScope:
#if NET8_0_OR_GREATER
            if (OperatingSystem.IsBrowser())
            {
              throw new PlatformNotSupportedException(Resources.TransactionScopeTransactionNotSupportedException);
            }
#endif
            var broker = _applicationContext.CreateInstanceDI<DataPortalBroker>();
            portal = new TransactionalDataPortal(broker, method.TransactionalAttribute);
            result = await portal.Delete(objectType, criteria, context, isSync).ConfigureAwait(false);
            break;
          default:
            portal = _applicationContext.CreateInstanceDI<DataPortalBroker>();
            result = await portal.Delete(objectType, criteria, context, isSync).ConfigureAwait(false);
            break;
        }
        Complete(new InterceptArgs(objectType, criteria, result, DataPortalOperations.Delete, isSync));
        return result;
      }
      catch (DataPortalException ex)
      {
        Complete(new InterceptArgs(objectType, criteria, ex, DataPortalOperations.Delete, isSync));
        throw;
      }
      catch (Exception ex)
      {
        if (ex is AggregateException aggregateException && aggregateException.InnerExceptions.Count > 0)
        {
          ex = aggregateException.InnerExceptions[0].InnerException ?? ex;
        }

        var fex = NewDataPortalException(
            _applicationContext, "DataPortal.Delete " + Resources.FailedOnServer,
            DataPortalExceptionHandler.InspectException(objectType, criteria, "DataPortal.Delete", ex),
            null, DataPortalOptions);
        Complete(new InterceptArgs(objectType, criteria, fex, DataPortalOperations.Delete, isSync));
        throw fex;
      }
      finally
      {
        ClearContext(context);
      }
    }

    internal void Complete(InterceptArgs e)
    {
      InterceptorManager.Complete(e);

      var timer = _applicationContext.ClientContext.GetValueOrNull("__dataportaltimer");
      if (timer == null) return;

      var startTime = (DateTimeOffset)timer;
      e.Runtime = DateTimeOffset.Now - startTime;
      Dashboard.CompleteCall(e);
    }

    internal async Task InitializeAsync(InterceptArgs e)
    {
      _applicationContext.ClientContext["__dataportaltimer"] = DateTimeOffset.Now;
      Dashboard.InitializeCall(e);

      await InterceptorManager.InitializeAsync(e);
    }

    #endregion

    #region Context

    ApplicationContext.LogicalExecutionLocations _oldLocation;

    private void SetContext(DataPortalContext context)
    {
      _oldLocation = _applicationContext.LogicalExecutionLocation;
      _applicationContext.SetLogicalExecutionLocation(ApplicationContext.LogicalExecutionLocations.Server);

      if (context.IsRemotePortal)
      {
        // indicate that the code is physically running on the server
        _applicationContext.SetExecutionLocation(ApplicationContext.ExecutionLocations.Server);
      }

      // set the app context to the value we got from the caller
      _applicationContext.SetContext(context.ClientContext);

      // set the thread's culture to match the caller
      SetCulture(context);

      // set current user principal
      SetPrincipal(context);
    }

    private void SetPrincipal(DataPortalContext context)
    {
      if (context.IsRemotePortal && !SecurityOptions.FlowSecurityPrincipalFromClient)
      {
        // When using platform-supplied security, Principal must be null
        if (context.Principal != null)
        {
          throw new Security.SecurityException(Resources.NoPrincipalAllowedException);
        }
        if (SecurityOptions.AuthenticationType == "Windows")
        {
          // Set .NET to use integrated security
          AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
        }
      }
      else
      {
        // We expect some Principal object to be available
        if (context.Principal == null)
        {
          throw new Security.SecurityException(Resources.BusinessPrincipalException + " Nothing");
        }
        _applicationContext.User = context.Principal;
      }
    }

    private static void SetCulture(DataPortalContext context)
    {
      // set the thread's culture to match the client
      Thread.CurrentThread.CurrentCulture =
        new System.Globalization.CultureInfo(context.ClientCulture);
      Thread.CurrentThread.CurrentUICulture =
        new System.Globalization.CultureInfo(context.ClientUICulture);
    }

    private void ClearContext(DataPortalContext context)
    {
      _applicationContext.SetLogicalExecutionLocation(_oldLocation);
      // if the dataportal is not remote then
      // do nothing
      if (!context.IsRemotePortal) return;
      _applicationContext.Clear();
      if (SecurityOptions.FlowSecurityPrincipalFromClient)
        _applicationContext.User = null;
    }

    #endregion

    private async Task AuthorizeRequestAsync(AuthorizeRequest clientRequest, CancellationToken ct)
    {
      await Authorizer.AuthorizeAsync(clientRequest, ct);
    }

    [DoesNotReturn]
    internal static DataPortalException NewDataPortalException(ApplicationContext applicationContext, string message, Exception innerException, object? businessObject, DataPortalOptions dataPortalOptions)
    {
      if (!dataPortalOptions.DataPortalServerOptions.DataPortalReturnObjectOnException)
        businessObject = null;

      throw new DataPortalException(message, innerException, new DataPortalResult(applicationContext, businessObject, null));
    }

    /// <summary>
    /// Converts a params array to a single 
    /// serializable criteria value.
    /// </summary>
    /// <param name="criteria">Params array</param>
    public static object GetCriteriaFromArray(params object?[]? criteria)
    {
      var clength = 0;
      if (criteria != null)
        if (criteria.GetType().Equals(typeof(object[])))
          clength = criteria.GetLength(0);
        else
          return criteria;

      if (criteria == null || (clength == 1 && criteria[0] == null))
        return NullCriteria.Instance;
      else if (clength == 0)
        return EmptyCriteria.Instance;
      else if (clength == 1)
        return criteria[0]!;
      else
        return new Core.MobileList<object?>(criteria);
    }

    /// <summary>
    /// Converts a single serializable criteria value
    /// into an array of type object.
    /// </summary>
    /// <param name="criteria">Single serializable criteria value</param>
    public static object?[]? GetCriteriaArray(object? criteria)
    {
      if (criteria == null)
        return null;
      else if (criteria is EmptyCriteria)
        return [];
      else if (criteria is NullCriteria)
        return [null];
      else if (criteria is object[] array)
      {
        var clength = array.GetLength(0);
        if (clength == 1 && array[0] is EmptyCriteria)
          return [];
        else
          return array;
      }
      else if (criteria is Core.MobileList<object> list)
        return list.ToArray();
      else
        return [criteria];
    }
  }
}
