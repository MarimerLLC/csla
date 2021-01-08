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
    /// Gets the data portal dashboard instance.
    /// </summary>
    public static Dashboard.IDashboard Dashboard { get; internal set; }

    static DataPortal()
    {
      Dashboard = Server.Dashboard.DashboardFactory.GetDashboard();
    }

    #region Constructors
    /// <summary>
    /// Default constructor
    /// </summary>
    public DataPortal()
      : this("CslaAuthorizationProvider")
    {

    }

    /// <summary>
    /// This construcor accepts the App Setting name for the Csla Authorization Provider,
    /// therefore getting the provider type from configuration file
    /// </summary>
    /// <param name="cslaAuthorizationProviderAppSettingName"></param>
    protected DataPortal(string cslaAuthorizationProviderAppSettingName)
      : this(GetAuthProviderType(cslaAuthorizationProviderAppSettingName))
    {
    }

    /// <summary>
    /// This constructor accepts the Authorization Provider Type as a parameter.
    /// </summary>
    /// <param name="authProviderType"></param>
    protected DataPortal(Type authProviderType)
    {
      if (null == authProviderType)
        throw new ArgumentNullException(nameof(authProviderType), Resources.CslaAuthenticationProviderNotSet);
      if (!typeof(IAuthorizeDataPortal).IsAssignableFrom(authProviderType))
        throw new ArgumentException(Resources.AuthenticationProviderDoesNotImplementIAuthorizeDataPortal, nameof(authProviderType));

      //only construct the type if it was not constructed already
      if (null == _authorizer)
      {
        lock (_syncRoot)
        {
          if (null == _authorizer)
            _authorizer = (IAuthorizeDataPortal)Reflection.MethodCaller.CreateInstance(authProviderType);
        }
      }

      if (InterceptorType != null)
      {
        if (_interceptor == null)
        {
          lock (_syncRoot)
          {
            if (_interceptor == null)
              _interceptor = (IInterceptDataPortal)Reflection.MethodCaller.CreateInstance(InterceptorType);
          }
        }
      }
    }

    private static Type GetAuthProviderType(string cslaAuthorizationProviderAppSettingName)
    {
      if (cslaAuthorizationProviderAppSettingName == null)
        throw new ArgumentNullException("cslaAuthorizationProviderAppSettingName", Resources.AuthorizationProviderNameNotSpecified);


      if (null == _authorizer)//not yet instantiated
      {
        var authProvider = ConfigurationManager.AppSettings[cslaAuthorizationProviderAppSettingName];
        return string.IsNullOrEmpty(authProvider) ?
          typeof(NullAuthorizer) :
          Type.GetType(authProvider, true);

      }
      else
        return _authorizer.GetType();

    }

    #endregion

    #region Data Access

#if !NETSTANDARD2_0 && !NET5_0
    private IDataPortalServer GetServicedComponentPortal(TransactionalAttribute transactionalAttribute)
    {
      switch (transactionalAttribute.TransactionIsolationLevel)
      {
        case TransactionIsolationLevel.Serializable:
          return new ServicedDataPortalSerializable();
        case TransactionIsolationLevel.RepeatableRead:
          return new ServicedDataPortalRepeatableRead();
        case TransactionIsolationLevel.ReadCommitted:
          return new ServicedDataPortalReadCommitted();
        case TransactionIsolationLevel.ReadUncommitted:
          return new ServicedDataPortalReadUncommitted();
        default:
          throw new ArgumentOutOfRangeException("transactionalAttribute");
      }
    }
#endif 

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

        Initialize(new InterceptArgs { ObjectType = objectType, Parameter = criteria, Operation = DataPortalOperations.Create, IsSync = isSync });

        AuthorizeRequest(new AuthorizeRequest(objectType, criteria, DataPortalOperations.Create));
        DataPortalResult result;
        DataPortalMethodInfo method;

        Reflection.ServiceProviderMethodInfo serviceProviderMethodInfo;
        if (criteria is Server.EmptyCriteria)
          serviceProviderMethodInfo = Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<CreateAttribute>(objectType, null);
        else
          serviceProviderMethodInfo = Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<CreateAttribute>(objectType, Server.DataPortal.GetCriteriaArray(criteria));
        serviceProviderMethodInfo.PrepForInvocation();
        method = serviceProviderMethodInfo.DataPortalMethodInfo;

        IDataPortalServer portal;
        switch (method.TransactionalAttribute.TransactionType)
        {
#if !NETSTANDARD2_0 && !NET5_0
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

            portal = new TransactionalDataPortal(method.TransactionalAttribute);
            result = await portal.Create(objectType, criteria, context, isSync).ConfigureAwait(false);

            break;
          default:
            portal = new DataPortalBroker();
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
            "DataPortal.Create " + Resources.FailedOnServer,
            new DataPortalExceptionHandler().InspectException(objectType, criteria, "DataPortal.Create", error),
            null);
        Complete(new InterceptArgs { ObjectType = objectType, Parameter = criteria, Exception = fex, Operation = DataPortalOperations.Create, IsSync = isSync });
        throw fex;
      }
      catch (Exception ex)
      {
        var fex = DataPortal.NewDataPortalException(
            "DataPortal.Create " + Resources.FailedOnServer,
            new DataPortalExceptionHandler().InspectException(objectType, criteria, "DataPortal.Create", ex),
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
      try
      {
        SetContext(context);

        Initialize(new InterceptArgs { ObjectType = objectType, Parameter = criteria, Operation = DataPortalOperations.Fetch, IsSync = isSync });

        AuthorizeRequest(new AuthorizeRequest(objectType, criteria, DataPortalOperations.Fetch));
        DataPortalResult result;
        DataPortalMethodInfo method;

        Reflection.ServiceProviderMethodInfo serviceProviderMethodInfo;
        if (criteria is EmptyCriteria)
          serviceProviderMethodInfo = Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<FetchAttribute>(objectType, null);
        else
          serviceProviderMethodInfo = Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<FetchAttribute>(objectType, Server.DataPortal.GetCriteriaArray(criteria));

        serviceProviderMethodInfo.PrepForInvocation();
        method = serviceProviderMethodInfo.DataPortalMethodInfo;

        IDataPortalServer portal;
        switch (method.TransactionalAttribute.TransactionType)
        {
#if !NETSTANDARD2_0 && !NET5_0
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
            portal = new TransactionalDataPortal(method.TransactionalAttribute);
            result = await portal.Fetch(objectType, criteria, context, isSync).ConfigureAwait(false);
            break;
          default:
            portal = new DataPortalBroker();
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
            "DataPortal.Fetch " + Resources.FailedOnServer,
            new DataPortalExceptionHandler().InspectException(objectType, criteria, "DataPortal.Fetch", error),
            null);
        Complete(new InterceptArgs { ObjectType = objectType, Parameter = criteria, Exception = fex, Operation = DataPortalOperations.Fetch, IsSync = isSync });
        throw fex;
      }
      catch (Exception ex)
      {
        var fex = DataPortal.NewDataPortalException(
            "DataPortal.Fetch " + Resources.FailedOnServer,
            new DataPortalExceptionHandler().InspectException(objectType, criteria, "DataPortal.Fetch", ex),
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
        Initialize(new InterceptArgs { ObjectType = objectType, Parameter = obj, Operation = operation, IsSync = isSync });

        AuthorizeRequest(new AuthorizeRequest(objectType, obj, operation));
        DataPortalResult result;
        DataPortalMethodInfo method;
        var factoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(objectType);
        if (factoryInfo != null)
        {
          string methodName;
          var factoryType = FactoryDataPortal.FactoryLoader.GetFactoryType(factoryInfo.FactoryTypeName);
          var bbase = obj as Core.BusinessBase;
          if (bbase != null)
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
          var bbase = obj as Core.BusinessBase;
          if (bbase != null)
          {
            if (bbase.IsDeleted)
              serviceProviderMethodInfo = Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<DeleteSelfAttribute>(objectType, null);
            else
              if (bbase.IsNew)
                serviceProviderMethodInfo = Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<InsertAttribute>(objectType, null);
              else
                serviceProviderMethodInfo = Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<UpdateAttribute>(objectType, null);
          }
          else if (obj is Core.ICommandObject)
            serviceProviderMethodInfo = Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<ExecuteAttribute>(objectType, null);
          else
            serviceProviderMethodInfo = Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<UpdateAttribute>(objectType, null);

          serviceProviderMethodInfo.PrepForInvocation();
          method = serviceProviderMethodInfo.DataPortalMethodInfo;
        }

        context.TransactionalType = method.TransactionalAttribute.TransactionType;
        IDataPortalServer portal;
        switch (method.TransactionalAttribute.TransactionType)
        {
#if !NETSTANDARD2_0 && !NET5_0
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
            portal = new TransactionalDataPortal(method.TransactionalAttribute);
            result = await portal.Update(obj, context, isSync).ConfigureAwait(false);
            break;
          default:
            portal = new DataPortalBroker();
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
            "DataPortal.Update " + Resources.FailedOnServer,
            new DataPortalExceptionHandler().InspectException(obj.GetType(), obj, null, "DataPortal.Update", error),
            obj);
        Complete(new InterceptArgs { ObjectType = objectType, Parameter = obj, Exception = fex, Operation = operation, IsSync = isSync });
        throw fex;
      }
      catch (Exception ex)
      {
        var fex = DataPortal.NewDataPortalException(
            "DataPortal.Update " + Resources.FailedOnServer,
            new DataPortalExceptionHandler().InspectException(obj.GetType(), obj, null, "DataPortal.Update", ex),
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

        Initialize(new InterceptArgs { ObjectType = objectType, Parameter = criteria, Operation = DataPortalOperations.Delete, IsSync = isSync });

        AuthorizeRequest(new AuthorizeRequest(objectType, criteria, DataPortalOperations.Delete));
        DataPortalResult result;
        DataPortalMethodInfo method;
        var factoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(objectType);
        if (factoryInfo != null)
        {
          var factoryType = FactoryDataPortal.FactoryLoader.GetFactoryType(factoryInfo.FactoryTypeName);
          string methodName = factoryInfo.DeleteMethodName;
          method = Server.DataPortalMethodCache.GetMethodInfo(factoryType, methodName, criteria);
        }
        else
        {
          Reflection.ServiceProviderMethodInfo serviceProviderMethodInfo;
          if (criteria is EmptyCriteria)
            serviceProviderMethodInfo = Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<DeleteAttribute>(objectType, null);
          else
            serviceProviderMethodInfo = Reflection.ServiceProviderMethodCaller.FindDataPortalMethod<DeleteAttribute>(objectType, Server.DataPortal.GetCriteriaArray(criteria));
          serviceProviderMethodInfo.PrepForInvocation();
          method = serviceProviderMethodInfo.DataPortalMethodInfo;
        }

        IDataPortalServer portal;
        switch (method.TransactionalAttribute.TransactionType)
        {
#if !NETSTANDARD2_0 && !NET5_0
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
            portal = new TransactionalDataPortal(method.TransactionalAttribute);
            result = await portal.Delete(objectType, criteria, context, isSync).ConfigureAwait(false);
            break;
          default:
            portal = new DataPortalBroker();
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
            "DataPortal.Delete " + Resources.FailedOnServer,
            new DataPortalExceptionHandler().InspectException(objectType, criteria, "DataPortal.Delete", error),
            null);
        Complete(new InterceptArgs { ObjectType = objectType, Parameter = criteria, Exception = fex, Operation = DataPortalOperations.Delete, IsSync = isSync });
        throw fex;
      }
      catch (Exception ex)
      {
        var fex = DataPortal.NewDataPortalException(
            "DataPortal.Delete " + Resources.FailedOnServer,
            new DataPortalExceptionHandler().InspectException(objectType, criteria, "DataPortal.Delete", ex),
            null);
        Complete(new InterceptArgs { ObjectType = objectType, Parameter = criteria, Exception = fex, Operation = DataPortalOperations.Delete, IsSync = isSync });
        throw fex;
      }
      finally
      {
        ClearContext(context);
      }
    }

    private IInterceptDataPortal _interceptor = null;
    private static Type _interceptorType = null;
    private static bool _InterceptorTypeSet = false;

    /// <summary>
    /// Gets or sets the type of interceptor invoked
    /// by the data portal for pre- and post-processing
    /// of each data portal invocation.
    /// </summary>
    public static Type InterceptorType 
    {
      get
      {
        if (!_InterceptorTypeSet)
        {
          var typeName = ConfigurationManager.AppSettings["CslaDataPortalInterceptor"];
          if (!string.IsNullOrWhiteSpace(typeName))
            InterceptorType = Type.GetType(typeName);
          _InterceptorTypeSet = true;
        }
        return _interceptorType;
      }
      set
      {
        _interceptorType = value;
        _InterceptorTypeSet = true;
      }
    }

    internal void Complete(InterceptArgs e)
    {
      var timer = ApplicationContext.ClientContext.GetValueOrNull("__dataportaltimer");
      if (timer != null)
      {
        var startTime = (DateTimeOffset)timer;
        e.Runtime = DateTimeOffset.Now - startTime;
        Dashboard.CompleteCall(e);
      }

      if (_interceptor != null)
        _interceptor.Complete(e);
    }

    internal void Initialize(InterceptArgs e)
    {
      ApplicationContext.ClientContext["__dataportaltimer"] = DateTimeOffset.Now;
      Dashboard.InitializeCall(e);

      if (_interceptor != null)
        _interceptor.Initialize(e);
    }

#endregion

#region Context

    ApplicationContext.LogicalExecutionLocations _oldLocation;

    private void SetContext(DataPortalContext context)
    {
      _oldLocation = Csla.ApplicationContext.LogicalExecutionLocation;
      ApplicationContext.SetLogicalExecutionLocation(ApplicationContext.LogicalExecutionLocations.Server);

      if (!context.IsRemotePortal && ApplicationContext.WebContextManager != null && !ApplicationContext.WebContextManager.IsValid)
        ApplicationContext.SetContext(context.ClientContext, context.GlobalContext);

      // if the dataportal is not remote then
      // do nothing
      if (!context.IsRemotePortal) return;

      // set the context value so everyone knows the
      // code is running on the server
      ApplicationContext.SetExecutionLocation(ApplicationContext.ExecutionLocations.Server);

      // set the app context to the value we got from the
      // client
      ApplicationContext.SetContext(context.ClientContext, context.GlobalContext);

      // set the thread's culture to match the client
      System.Threading.Thread.CurrentThread.CurrentCulture =
        new System.Globalization.CultureInfo(context.ClientCulture);
      System.Threading.Thread.CurrentThread.CurrentUICulture =
        new System.Globalization.CultureInfo(context.ClientUICulture);

      if (ApplicationContext.AuthenticationType == "Windows")
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

#region Authorize

    private static object _syncRoot = new object();
    private static IAuthorizeDataPortal _authorizer = null;

    /// <summary>
    /// Gets or sets a reference to the current authorizer.
    /// </summary>
    protected static IAuthorizeDataPortal Authorizer
    {
      get { return _authorizer; }
      set { _authorizer = value; }
    }

    internal void Authorize(AuthorizeRequest clientRequest)
    {
      AuthorizeRequest(clientRequest);
    }

    private static void AuthorizeRequest(AuthorizeRequest clientRequest)
    {
      _authorizer.Authorize(clientRequest);
    }

    /// <summary>
    /// Default implementation of the authorizer that
    /// allows all data portal calls to pass.
    /// </summary>
    protected class NullAuthorizer : IAuthorizeDataPortal
    {
      /// <summary>
      /// Creates an instance of the type.
      /// </summary>
      /// <param name="clientRequest">
      /// Client request information.
      /// </param>
      public void Authorize(AuthorizeRequest clientRequest)
      { /* default is to allow all requests */ }
    }

#endregion

    internal static DataPortalException NewDataPortalException(string message, Exception innerException, object businessObject)
    {
      if (!ApplicationContext.DataPortalReturnObjectOnException)
        businessObject = null;

      throw new DataPortalException(
        message,
        innerException, new DataPortalResult(businessObject));
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