//-----------------------------------------------------------------------
// <copyright file="DataPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Implements the server-side DataPortal </summary>
//-----------------------------------------------------------------------
using System;
#if !NETFX_CORE
using System.Configuration;
#endif
#if NETFX_CORE
using System.Reflection;
using Csla.Reflection;
#endif
using System.Security.Principal;
using System.Threading.Tasks;
using Csla.Properties;
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
        throw new ArgumentNullException("authProviderType", Resources.CslaAuthenticationProviderNotSet);
      if (!typeof(IAuthorizeDataPortal).IsAssignableFrom(authProviderType))
        throw new ArgumentException(Resources.AuthenticationProviderDoesNotImplementIAuthorizeDataPortal, "authProviderType");

      //only construct the type if it was not constructed already
      if (null == _authorizer)
      {
        lock (_syncRoot)
        {
          if (null == _authorizer)
            _authorizer = (IAuthorizeDataPortal)Activator.CreateInstance(authProviderType);
        }
      }

      if (InterceptorType != null)
      {
        if (_interceptor == null)
        {
          lock (_syncRoot)
          {
            if (_interceptor == null)
              _interceptor = (IInterceptDataPortal)Activator.CreateInstance(InterceptorType);
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
#if (ANDROID || IOS) || NETFX_CORE || NETSTANDARD2_0
        string authProvider = string.Empty;
#else
        var authProvider = ConfigurationManager.AppSettings[cslaAuthorizationProviderAppSettingName];
#endif
        return string.IsNullOrEmpty(authProvider) ?
          typeof(NullAuthorizer) :
          Type.GetType(authProvider, true);

      }
      else
        return _authorizer.GetType();

    }

    #endregion

    #region Data Access

#if !(ANDROID || IOS) && !NETFX_CORE && !MONO && !NETSTANDARD2_0
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

        DataPortalMethodInfo method = DataPortalMethodCache.GetCreateMethod(objectType, criteria);

        IDataPortalServer portal;
#if !(ANDROID || IOS) && !NETFX_CORE && !NETSTANDARD2_0
        switch (method.TransactionalAttribute.TransactionType)
        {
#if !MONO
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
#else
        portal = new DataPortalBroker();
        result = await portal.Create(objectType, criteria, context, isSync).ConfigureAwait(false);
#endif
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

        DataPortalMethodInfo method = DataPortalMethodCache.GetFetchMethod(objectType, criteria);

        IDataPortalServer portal;
#if !(ANDROID || IOS) && !NETFX_CORE && !NETSTANDARD2_0
        switch (method.TransactionalAttribute.TransactionType)
        {
#if !MONO
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
#else
        portal = new DataPortalBroker();
        result = await portal.Fetch(objectType, criteria, context, isSync).ConfigureAwait(false);
#endif
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
          string methodName;
          var bbase = obj as Core.BusinessBase;
          if (bbase != null)
          {
            if (bbase.IsDeleted)
              methodName = "DataPortal_DeleteSelf";
            else
              if (bbase.IsNew)
                methodName = "DataPortal_Insert";
              else
                methodName = "DataPortal_Update";
          }
          else if (obj is Core.ICommandObject)
            methodName = "DataPortal_Execute";
          else
            methodName = "DataPortal_Update";
          method = DataPortalMethodCache.GetMethodInfo(obj.GetType(), methodName);
        }
#if !(ANDROID || IOS) && !NETFX_CORE && !NETSTANDARD2_0
        context.TransactionalType = method.TransactionalAttribute.TransactionType;
#else
        context.TransactionalType = method.TransactionalType;
#endif
        IDataPortalServer portal;
#if !(ANDROID || IOS) && !NETFX_CORE && !NETSTANDARD2_0
        switch (method.TransactionalAttribute.TransactionType)
        {
#if !MONO
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
#else
        portal = new DataPortalBroker();
        result = await portal.Update(obj, context, isSync).ConfigureAwait(false);
#endif
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
          method = DataPortalMethodCache.GetMethodInfo(objectType, "DataPortal_Delete", criteria);
        }

        IDataPortalServer portal;
#if !(ANDROID || IOS) && !NETFX_CORE && !NETSTANDARD2_0
        switch (method.TransactionalAttribute.TransactionType)
        {
#if !MONO
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
#else
        portal = new DataPortalBroker();
        result = await portal.Delete(objectType, criteria, context, isSync).ConfigureAwait(false);
#endif
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
#if !(ANDROID || IOS) && !NETFX_CORE && !NETSTANDARD2_0
          var typeName = ConfigurationManager.AppSettings["CslaDataPortalInterceptor"];
          if (!string.IsNullOrWhiteSpace(typeName))
            InterceptorType = Type.GetType(typeName);
#endif
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
      if (_interceptor != null)
        _interceptor.Complete(e);
    }

    internal void Initialize(InterceptArgs e)
    {
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
#if !PCL46 // rely on NuGet bait-and-switch for actual implementation
#if NETCORE
      System.Globalization.CultureInfo.CurrentCulture =
        new System.Globalization.CultureInfo(context.ClientCulture); 
      System.Globalization.CultureInfo.CurrentUICulture = 
        new System.Globalization.CultureInfo(context.ClientUICulture);
#elif NETFX_CORE
      var list = new System.Collections.ObjectModel.ReadOnlyCollection<string>(new List<string> { context.ClientUICulture });
      Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().Languages = list;
      list = new System.Collections.ObjectModel.ReadOnlyCollection<string>(new List<string> { context.ClientCulture });
      Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().Languages = list;
#else
      System.Threading.Thread.CurrentThread.CurrentCulture =
        new System.Globalization.CultureInfo(context.ClientCulture);
      System.Threading.Thread.CurrentThread.CurrentUICulture =
        new System.Globalization.CultureInfo(context.ClientUICulture);
#endif
#endif

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
#if !(ANDROID || IOS) && !NETFX_CORE
        // Set .NET to use integrated security
        AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
#endif
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
  }
}