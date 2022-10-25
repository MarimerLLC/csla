//-----------------------------------------------------------------------
// <copyright file="DataPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements the server-side DataPortal </summary>
//-----------------------------------------------------------------------
using System;
using Csla.Configuration;
using System.Security.Principal;
using System.Threading.Tasks;
using Csla.Properties;
using Csla.Server.Dashboard;
using System.Collections.Generic;

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
    private ApplicationContext ApplicationContext { get; set; }

    /// <summary>
    /// Gets the data portal dashboard instance.
    /// </summary>
    private IDashboard Dashboard { get; set; }
    private DataPortalServerOptions Options { get; set; }
    private IAuthorizeDataPortal Authorizer { get; set; }
    private InterceptorManager InterceptorManager { get; set; }
    private IObjectFactoryLoader FactoryLoader { get; set; }
    private IDataPortalActivator Activator { get; set; }
    private IDataPortalExceptionInspector ExceptionInspector { get; set; }
    private DataPortalExceptionHandler DataPortalExceptionHandler { get; set; }

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
    public DataPortal(
      ApplicationContext applicationContext, 
      IDashboard dashboard, 
      CslaOptions options,
      IAuthorizeDataPortal authorizer,
      InterceptorManager interceptors,
      IObjectFactoryLoader factoryLoader,
      IDataPortalActivator activator,
      IDataPortalExceptionInspector exceptionInspector,
      DataPortalExceptionHandler exceptionHandler)
    {
      ApplicationContext = applicationContext;
      Dashboard = dashboard;
      Options = options.DataPortalServerOptions;
      Authorizer = authorizer;
      InterceptorManager = interceptors;
      FactoryLoader = factoryLoader;
      Activator = activator;
      ExceptionInspector = exceptionInspector;
      DataPortalExceptionHandler = exceptionHandler;
    }

    #region Data Access

#if !NETSTANDARD2_0 && !NET6_0_OR_GREATER
    private IDataPortalServer GetServicedComponentPortal(TransactionalAttribute transactionalAttribute)
    {
      switch (transactionalAttribute.TransactionIsolationLevel)
      {
        case TransactionIsolationLevel.Serializable:
          return ApplicationContext.CreateInstanceDI<ServicedDataPortalSerializable>();
        case TransactionIsolationLevel.RepeatableRead:
          return ApplicationContext.CreateInstanceDI<ServicedDataPortalRepeatableRead>();
        case TransactionIsolationLevel.ReadCommitted:
          return ApplicationContext.CreateInstanceDI<ServicedDataPortalReadCommitted>();
        case TransactionIsolationLevel.ReadUncommitted:
          return ApplicationContext.CreateInstanceDI<ServicedDataPortalReadUncommitted>();
        default:
          throw new ArgumentOutOfRangeException("transactionalAttribute");
      }
    }
#endif

    private Reflection.ServiceProviderMethodCaller serviceProviderMethodCaller;
    private Reflection.ServiceProviderMethodCaller ServiceProviderMethodCaller
    {
      get
      {
        if (serviceProviderMethodCaller == null)
          serviceProviderMethodCaller = (Reflection.ServiceProviderMethodCaller)ApplicationContext.CreateInstanceDI(typeof(Reflection.ServiceProviderMethodCaller));
        return serviceProviderMethodCaller;
      }
    }

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
      Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      try
      {
        SetContext(context);

        AuthorizeRequest(new AuthorizeRequest(objectType, criteria, DataPortalOperations.Create));

        Initialize(new InterceptArgs { ObjectType = objectType, Parameter = criteria, Operation = DataPortalOperations.Create, IsSync = isSync });

        DataPortalResult result;
        DataPortalMethodInfo method;

        Reflection.ServiceProviderMethodInfo serviceProviderMethodInfo;
        if (criteria is Server.EmptyCriteria)
          serviceProviderMethodInfo = ServiceProviderMethodCaller.FindDataPortalMethod<CreateAttribute>(objectType, null);
        else
          serviceProviderMethodInfo = ServiceProviderMethodCaller.FindDataPortalMethod<CreateAttribute>(objectType, Server.DataPortal.GetCriteriaArray(criteria));
        serviceProviderMethodInfo.PrepForInvocation();
        method = serviceProviderMethodInfo.DataPortalMethodInfo;

        IDataPortalServer portal;
        switch (method.TransactionalAttribute.TransactionType)
        {
#if !NETSTANDARD2_0 && !NET6_0_OR_GREATER
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

            var broker = ApplicationContext.CreateInstanceDI<DataPortalBroker>();
            portal = new TransactionalDataPortal(broker, method.TransactionalAttribute);
            result = await portal.Create(objectType, criteria, context, isSync).ConfigureAwait(false);

            break;
          default:
            portal = ApplicationContext.CreateInstanceDI<DataPortalBroker>();
            result = await portal.Create(objectType, criteria, context, isSync).ConfigureAwait(false);
            break;
        }
        Complete(new InterceptArgs { ObjectType = objectType, Parameter = criteria, Result = result, Operation = DataPortalOperations.Create, IsSync = isSync });
        return result;
      }
      catch (Csla.Server.DataPortalException ex)
      {
        Complete(new InterceptArgs { ObjectType = objectType, Parameter = criteria, Exception = ex, Operation = DataPortalOperations.Create, IsSync = isSync });
        throw;
      }
      catch (AggregateException ex)
      {
        Exception error = null;
        if (ex.InnerExceptions.Count > 0)
          error = ex.InnerExceptions[0].InnerException;
        else
          error = ex;
        var fex = DataPortal.NewDataPortalException(
            ApplicationContext, "DataPortal.Create " + Resources.FailedOnServer,
            DataPortalExceptionHandler.InspectException(objectType, criteria, "DataPortal.Create", error),
            null);
        Complete(new InterceptArgs { ObjectType = objectType, Parameter = criteria, Exception = fex, Operation = DataPortalOperations.Create, IsSync = isSync });
        throw fex;
      }
      catch (Exception ex)
      {
        var fex = DataPortal.NewDataPortalException(
            ApplicationContext, "DataPortal.Create " + Resources.FailedOnServer,
            DataPortalExceptionHandler.InspectException(objectType, criteria, "DataPortal.Create", ex),
            null);
        Complete(new InterceptArgs { ObjectType = objectType, Parameter = criteria, Exception = fex, Operation = DataPortalOperations.Create, IsSync = isSync });
        throw fex;
      }
      finally
      {
        ClearContext(context);
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
      if (typeof(Core.ICommandObject).IsAssignableFrom(objectType))
      {
        return await Execute(objectType, criteria, context, isSync);
      }

      try
      {
        SetContext(context);

        AuthorizeRequest(new AuthorizeRequest(objectType, criteria, DataPortalOperations.Fetch));

        Initialize(new InterceptArgs { ObjectType = objectType, Parameter = criteria, Operation = DataPortalOperations.Fetch, IsSync = isSync });

        DataPortalResult result;
        DataPortalMethodInfo method;

        Reflection.ServiceProviderMethodInfo serviceProviderMethodInfo;
        if (criteria is EmptyCriteria)
          serviceProviderMethodInfo = ServiceProviderMethodCaller.FindDataPortalMethod<FetchAttribute>(objectType, null);
        else
          serviceProviderMethodInfo = ServiceProviderMethodCaller.FindDataPortalMethod<FetchAttribute>(objectType, Server.DataPortal.GetCriteriaArray(criteria));

        serviceProviderMethodInfo.PrepForInvocation();
        method = serviceProviderMethodInfo.DataPortalMethodInfo;

        IDataPortalServer portal;
        switch (method.TransactionalAttribute.TransactionType)
        {
#if !NETSTANDARD2_0 && !NET6_0_OR_GREATER
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
            var broker = ApplicationContext.CreateInstanceDI<DataPortalBroker>();
            portal = new TransactionalDataPortal(broker, method.TransactionalAttribute);
            result = await portal.Fetch(objectType, criteria, context, isSync).ConfigureAwait(false);
            break;
          default:
            portal = ApplicationContext.CreateInstanceDI<DataPortalBroker>();
            result = await portal.Fetch(objectType, criteria, context, isSync).ConfigureAwait(false);
            break;
        }
        Complete(new InterceptArgs { ObjectType = objectType, Parameter = criteria, Result = result, Operation = DataPortalOperations.Fetch, IsSync = isSync });
        return result;
      }
      catch (Csla.Server.DataPortalException ex)
      {
        Complete(new InterceptArgs { ObjectType = objectType, Parameter = criteria, Exception = ex, Operation = DataPortalOperations.Fetch, IsSync = isSync });
        throw;
      }
      catch (AggregateException ex)
      {
        Exception error = null;
        if (ex.InnerExceptions.Count > 0)
          error = ex.InnerExceptions[0].InnerException;
        else
          error = ex;
        var fex = DataPortal.NewDataPortalException(
            ApplicationContext, "DataPortal.Fetch " + Resources.FailedOnServer,
            DataPortalExceptionHandler.InspectException(objectType, criteria, "DataPortal.Fetch", error),
            null);
        Complete(new InterceptArgs { ObjectType = objectType, Parameter = criteria, Exception = fex, Operation = DataPortalOperations.Fetch, IsSync = isSync });
        throw fex;
      }
      catch (Exception ex)
      {
        var fex = DataPortal.NewDataPortalException(
            ApplicationContext, "DataPortal.Fetch " + Resources.FailedOnServer,
            DataPortalExceptionHandler.InspectException(objectType, criteria, "DataPortal.Fetch", ex),
            null);
        Complete(new InterceptArgs { ObjectType = objectType, Parameter = criteria, Exception = fex, Operation = DataPortalOperations.Fetch, IsSync = isSync });
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
    private async Task<DataPortalResult> Execute(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      try
      {
        SetContext(context);

        AuthorizeRequest(new AuthorizeRequest(objectType, criteria, DataPortalOperations.Execute));

        Initialize(new InterceptArgs { ObjectType = objectType, Parameter = criteria, Operation = DataPortalOperations.Execute, IsSync = isSync });

        DataPortalResult result;
        DataPortalMethodInfo method;

        Reflection.ServiceProviderMethodInfo serviceProviderMethodInfo;
        if (criteria is EmptyCriteria)
          serviceProviderMethodInfo = ServiceProviderMethodCaller.FindDataPortalMethod<ExecuteAttribute>(objectType, null);
        else
          serviceProviderMethodInfo = ServiceProviderMethodCaller.FindDataPortalMethod<ExecuteAttribute>(objectType, Server.DataPortal.GetCriteriaArray(criteria));

        serviceProviderMethodInfo.PrepForInvocation();
        method = serviceProviderMethodInfo.DataPortalMethodInfo;

        IDataPortalServer portal;
        switch (method.TransactionalAttribute.TransactionType)
        {
#if !NETSTANDARD2_0 && !NET6_0_OR_GREATER
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
            var broker = ApplicationContext.CreateInstanceDI<DataPortalBroker>();
            portal = new TransactionalDataPortal(broker, method.TransactionalAttribute);
            result = await portal.Fetch(objectType, criteria, context, isSync).ConfigureAwait(false);
            break;
          default:
            portal = ApplicationContext.CreateInstanceDI<DataPortalBroker>();
            result = await portal.Fetch(objectType, criteria, context, isSync).ConfigureAwait(false);
            break;
        }
        Complete(new InterceptArgs { ObjectType = objectType, Parameter = criteria, Result = result, Operation = DataPortalOperations.Execute, IsSync = isSync });
        return result;
      }
      catch (Csla.Server.DataPortalException ex)
      {
        Complete(new InterceptArgs { ObjectType = objectType, Parameter = criteria, Exception = ex, Operation = DataPortalOperations.Execute, IsSync = isSync });
        throw;
      }
      catch (AggregateException ex)
      {
        Exception error = null;
        if (ex.InnerExceptions.Count > 0)
          error = ex.InnerExceptions[0].InnerException;
        else
          error = ex;
        var fex = DataPortal.NewDataPortalException(
            ApplicationContext, "DataPortal.Execute " + Resources.FailedOnServer,
            DataPortalExceptionHandler.InspectException(objectType, criteria, "DataPortal.Execute", error),
            null);
        Complete(new InterceptArgs { ObjectType = objectType, Parameter = criteria, Exception = fex, Operation = DataPortalOperations.Execute, IsSync = isSync });
        throw fex;
      }
      catch (Exception ex)
      {
        var fex = DataPortal.NewDataPortalException(
            ApplicationContext, "DataPortal.Execute " + Resources.FailedOnServer,
            DataPortalExceptionHandler.InspectException(objectType, criteria, "DataPortal.Execute", ex),
            null);
        Complete(new InterceptArgs { ObjectType = objectType, Parameter = criteria, Exception = fex, Operation = DataPortalOperations.Execute, IsSync = isSync });
        throw fex;
      }
      finally
      {
        ClearContext(context);
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
    public async Task<DataPortalResult> Update(object obj, DataPortalContext context, bool isSync)
    {
      Type objectType = null;
      DataPortalOperations operation = DataPortalOperations.Update;
      try
      {
        SetContext(context);

        objectType = obj.GetType();

        if (obj is Core.ICommandObject)
          operation = DataPortalOperations.Execute;

        AuthorizeRequest(new AuthorizeRequest(objectType, obj, operation));

        Initialize(new InterceptArgs { ObjectType = objectType, Parameter = obj, Operation = operation, IsSync = isSync });

        DataPortalResult result;
        DataPortalMethodInfo method;
        var factoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(objectType);
        if (factoryInfo != null)
        {
          string methodName;
          var factoryLoader = ApplicationContext.CurrentServiceProvider.GetService(typeof(Server.IObjectFactoryLoader)) as Server.IObjectFactoryLoader;
          var factoryType = factoryLoader?.GetFactoryType(factoryInfo.FactoryTypeName);
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
          method = Server.DataPortalMethodCache.GetMethodInfo(factoryType, methodName, new object[] { obj });
        }
        else
        {
          Reflection.ServiceProviderMethodInfo serviceProviderMethodInfo;
          if (obj is Core.BusinessBase bbase)
          {
            if (bbase.IsDeleted)
              serviceProviderMethodInfo = ServiceProviderMethodCaller.FindDataPortalMethod<DeleteSelfAttribute>(objectType, null);
            else
              if (bbase.IsNew)
              serviceProviderMethodInfo = ServiceProviderMethodCaller.FindDataPortalMethod<InsertAttribute>(objectType, null);
            else
              serviceProviderMethodInfo = ServiceProviderMethodCaller.FindDataPortalMethod<UpdateAttribute>(objectType, null);
          }
          else if (obj is Core.ICommandObject)
            serviceProviderMethodInfo = ServiceProviderMethodCaller.FindDataPortalMethod<ExecuteAttribute>(objectType, null);
          else
            serviceProviderMethodInfo = ServiceProviderMethodCaller.FindDataPortalMethod<UpdateAttribute>(objectType, null);

          serviceProviderMethodInfo.PrepForInvocation();
          method = serviceProviderMethodInfo.DataPortalMethodInfo;
        }

        context.TransactionalType = method.TransactionalAttribute.TransactionType;
        IDataPortalServer portal;
        switch (method.TransactionalAttribute.TransactionType)
        {
#if !NETSTANDARD2_0 && !NET6_0_OR_GREATER
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
            var broker = ApplicationContext.CreateInstanceDI<DataPortalBroker>();
            portal = new TransactionalDataPortal(broker, method.TransactionalAttribute);
            result = await portal.Update(obj, context, isSync).ConfigureAwait(false);
            break;
          default:
            portal = ApplicationContext.CreateInstanceDI<DataPortalBroker>();
            result = await portal.Update(obj, context, isSync).ConfigureAwait(false);
            break;
        }
        Complete(new InterceptArgs { ObjectType = objectType, Parameter = obj, Result = result, Operation = operation, IsSync = isSync });
        return result;
      }
      catch (Csla.Server.DataPortalException ex)
      {
        Complete(new InterceptArgs { ObjectType = objectType, Parameter = obj, Exception = ex, Operation = operation, IsSync = isSync });
        throw;
      }
      catch (AggregateException ex)
      {
        Exception error = null;
        if (ex.InnerExceptions.Count > 0)
          error = ex.InnerExceptions[0].InnerException;
        else
          error = ex;
        var fex = DataPortal.NewDataPortalException(
            ApplicationContext, "DataPortal.Update " + Resources.FailedOnServer,
            DataPortalExceptionHandler.InspectException(obj.GetType(), obj, null, "DataPortal.Update", error),
            obj);
        Complete(new InterceptArgs { ObjectType = objectType, Parameter = obj, Exception = fex, Operation = operation, IsSync = isSync });
        throw fex;
      }
      catch (Exception ex)
      {
        var fex = DataPortal.NewDataPortalException(
            ApplicationContext, "DataPortal.Update " + Resources.FailedOnServer,
            DataPortalExceptionHandler.InspectException(obj.GetType(), obj, null, "DataPortal.Update", ex),
            obj);
        Complete(new InterceptArgs { ObjectType = objectType, Parameter = obj, Exception = fex, Operation = operation, IsSync = isSync });
        throw fex;
      }
      finally
      {
        ClearContext(context);
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
        SetContext(context);

        AuthorizeRequest(new AuthorizeRequest(objectType, criteria, DataPortalOperations.Delete));

        Initialize(new InterceptArgs { ObjectType = objectType, Parameter = criteria, Operation = DataPortalOperations.Delete, IsSync = isSync });

        DataPortalResult result;
        DataPortalMethodInfo method;
        var factoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(objectType);
        if (factoryInfo != null)
        {
          var factoryLoader = ApplicationContext.CurrentServiceProvider.GetService(typeof(Server.IObjectFactoryLoader)) as Server.IObjectFactoryLoader;
          var factoryType = factoryLoader?.GetFactoryType(factoryInfo.FactoryTypeName);
          string methodName = factoryInfo.DeleteMethodName;
          method = Server.DataPortalMethodCache.GetMethodInfo(factoryType, methodName, criteria);
        }
        else
        {
          Reflection.ServiceProviderMethodInfo serviceProviderMethodInfo;
          if (criteria is EmptyCriteria)
            serviceProviderMethodInfo = ServiceProviderMethodCaller.FindDataPortalMethod<DeleteAttribute>(objectType, null);
          else
            serviceProviderMethodInfo = ServiceProviderMethodCaller.FindDataPortalMethod<DeleteAttribute>(objectType, Server.DataPortal.GetCriteriaArray(criteria));
          serviceProviderMethodInfo.PrepForInvocation();
          method = serviceProviderMethodInfo.DataPortalMethodInfo;
        }

        IDataPortalServer portal;
        switch (method.TransactionalAttribute.TransactionType)
        {
#if !NETSTANDARD2_0 && !NET6_0_OR_GREATER
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
            var broker = ApplicationContext.CreateInstanceDI<DataPortalBroker>();
            portal = new TransactionalDataPortal(broker, method.TransactionalAttribute);
            result = await portal.Delete(objectType, criteria, context, isSync).ConfigureAwait(false);
            break;
          default:
            portal = ApplicationContext.CreateInstanceDI<DataPortalBroker>();
            result = await portal.Delete(objectType, criteria, context, isSync).ConfigureAwait(false);
            break;
        }
        Complete(new InterceptArgs { ObjectType = objectType, Parameter = criteria, Result = result, Operation = DataPortalOperations.Delete, IsSync = isSync });
        return result;
      }
      catch (Csla.Server.DataPortalException ex)
      {
        Complete(new InterceptArgs { ObjectType = objectType, Parameter = criteria, Exception = ex, Operation = DataPortalOperations.Delete, IsSync = isSync });
        throw;
      }
      catch (AggregateException ex)
      {
        Exception error = null;
        if (ex.InnerExceptions.Count > 0)
          error = ex.InnerExceptions[0].InnerException;
        else
          error = ex;
        var fex = DataPortal.NewDataPortalException(
            ApplicationContext, "DataPortal.Delete " + Resources.FailedOnServer,
            DataPortalExceptionHandler.InspectException(objectType, criteria, "DataPortal.Delete", error),
            null);
        Complete(new InterceptArgs { ObjectType = objectType, Parameter = criteria, Exception = fex, Operation = DataPortalOperations.Delete, IsSync = isSync });
        throw fex;
      }
      catch (Exception ex)
      {
        var fex = DataPortal.NewDataPortalException(
            ApplicationContext, "DataPortal.Delete " + Resources.FailedOnServer,
            DataPortalExceptionHandler.InspectException(objectType, criteria, "DataPortal.Delete", ex),
            null);
        Complete(new InterceptArgs { ObjectType = objectType, Parameter = criteria, Exception = fex, Operation = DataPortalOperations.Delete, IsSync = isSync });
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

      var timer = ApplicationContext.ClientContext.GetValueOrNull("__dataportaltimer");
      if (timer == null) return;

      var startTime = (DateTimeOffset)timer;
      e.Runtime = DateTimeOffset.Now - startTime;
      Dashboard.CompleteCall(e);
    }

    internal void Initialize(InterceptArgs e)
    {
      ApplicationContext.ClientContext["__dataportaltimer"] = DateTimeOffset.Now;
      Dashboard.InitializeCall(e);

      InterceptorManager.Initialize(e);
    }

#endregion

#region Context

    ApplicationContext.LogicalExecutionLocations _oldLocation;

    private void SetContext(DataPortalContext context)
    {
      _oldLocation = ApplicationContext.LogicalExecutionLocation;
      ApplicationContext.SetLogicalExecutionLocation(ApplicationContext.LogicalExecutionLocations.Server);

      if (context.IsRemotePortal)
      {
        // indicate that the code is physically running on the server
        ApplicationContext.SetExecutionLocation(ApplicationContext.ExecutionLocations.Server);
      }

      // set the app context to the value we got from the caller
      ApplicationContext.SetContext(context.ClientContext);

      // set the thread's culture to match the caller
      SetCulture(context);

      // set current user principal
      SetPrincipal(context);
    }

    private void SetPrincipal(DataPortalContext context)
    {
      if (context.IsRemotePortal && ApplicationContext.AuthenticationType == "Windows")
      {
        // When using integrated security, Principal must be null
        if (context.Principal != null)
        {
          Csla.Security.SecurityException ex =
            new Csla.Security.SecurityException(Resources.NoPrincipalAllowedException);
          //ex.Action = System.Security.Permissions.SecurityAction.Deny;
          throw ex;
        }
        // Set .NET to use integrated security
        AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
      }
      else
      {
        // We expect the some Principal object
        if (context.Principal == null)
        {
          Csla.Security.SecurityException ex =
            new Csla.Security.SecurityException(
              Resources.BusinessPrincipalException + " Nothing");
          //ex.Action = System.Security.Permissions.SecurityAction.Deny;
          throw ex;
        }
        ApplicationContext.User = context.Principal;
      }
    }

    private static void SetCulture(DataPortalContext context)
    {
      // set the thread's culture to match the client
      System.Threading.Thread.CurrentThread.CurrentCulture =
        new System.Globalization.CultureInfo(context.ClientCulture);
      System.Threading.Thread.CurrentThread.CurrentUICulture =
        new System.Globalization.CultureInfo(context.ClientUICulture);
    }

    private void ClearContext(DataPortalContext context)
    {
      ApplicationContext.SetLogicalExecutionLocation(_oldLocation);
      // if the dataportal is not remote then
      // do nothing
      if (!context.IsRemotePortal) return;
      ApplicationContext.Clear();
      if (ApplicationContext.AuthenticationType != "Windows")
        ApplicationContext.User = null;
    }

    #endregion

    private void AuthorizeRequest(AuthorizeRequest clientRequest)
    {
      Authorizer.Authorize(clientRequest);
    }

    internal static DataPortalException NewDataPortalException(
      ApplicationContext applicationContext, string message, Exception innerException, object businessObject)
    {
      if (!ApplicationContext.DataPortalReturnObjectOnException)
        businessObject = null;

      throw new DataPortalException(
        message,
        innerException, new DataPortalResult(applicationContext, businessObject));
    }

    /// <summary>
    /// Converts a params array to a single 
    /// serializable criteria value.
    /// </summary>
    /// <param name="criteria">Params array</param>
    /// <returns></returns>
    public static object GetCriteriaFromArray(params object[] criteria)
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
        return criteria[0];
      else
        return new Core.MobileList<object>(criteria);
    }

    /// <summary>
    /// Converts a single serializable criteria value
    /// into an array of type object.
    /// </summary>
    /// <param name="criteria">Single serializble criteria value</param>
    /// <returns></returns>
    public static object[] GetCriteriaArray(object criteria)
    {
      if (criteria == null)
        return null;
      else if (criteria is EmptyCriteria)
        return Array.Empty<object>();
      else if (criteria is NullCriteria)
        return new object[] { null };
      else if (criteria.GetType().Equals(typeof(object[])))
      {
        var array = (object[])criteria;
        var clength = array.GetLength(0);
        if (clength == 1 && array[0] is EmptyCriteria)
          return Array.Empty<object>();
        else
          return array;
      }
      else if (criteria is Core.MobileList<object> list)
        return list.ToArray();
      else
        return new object[] { criteria };
    }
  }
}
