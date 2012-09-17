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
#if !SILVERLIGHT && !NETFX_CORE
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
#endif

    #endregion

    #region Data Access

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

#if !SILVERLIGHT && !NETFX_CORE
        AuthorizeRequest(new AuthorizeRequest(objectType, criteria, DataPortalOperations.Create));
#endif
        DataPortalResult result;

        DataPortalMethodInfo method = DataPortalMethodCache.GetCreateMethod(objectType, criteria);

        IDataPortalServer portal;
#if !SILVERLIGHT && !NETFX_CORE
        switch (method.TransactionalType)
        {
#if !MONO
          case TransactionalTypes.EnterpriseServices:
            portal = new ServicedDataPortal();
            try
            {
              result = await portal.Create(objectType, criteria, context, isSync);
            }
            finally
            {
              ((ServicedDataPortal)portal).Dispose();
            }

            break;
#endif
          case TransactionalTypes.TransactionScope:

            portal = new TransactionalDataPortal();
            result = await portal.Create(objectType, criteria, context, isSync);

            break;
          default:
            portal = new DataPortalSelector();
            result = await portal.Create(objectType, criteria, context, isSync);
            break;
        }
#else
        portal = new DataPortalSelector();
        result = await portal.Create(objectType, criteria, context, isSync);
#endif
        return result;
      }
      catch (Csla.Server.DataPortalException)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new DataPortalException(
            "DataPortal.Create " + Resources.FailedOnServer,
            new DataPortalExceptionHandler().InspectException(objectType, criteria, "DataPortal.Create", ex),
            new DataPortalResult());
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

#if !SILVERLIGHT && !NETFX_CORE
        AuthorizeRequest(new AuthorizeRequest(objectType, criteria, DataPortalOperations.Fetch));
#endif
        DataPortalResult result;

        DataPortalMethodInfo method = DataPortalMethodCache.GetFetchMethod(objectType, criteria);

        IDataPortalServer portal;
#if !SILVERLIGHT && !NETFX_CORE
        switch (method.TransactionalType)
        {
#if !MONO
          case TransactionalTypes.EnterpriseServices:
            portal = new ServicedDataPortal();
            try
            {
              result = await portal.Fetch(objectType, criteria, context, isSync);
            }
            finally
            {
              ((ServicedDataPortal)portal).Dispose();
            }
            break;
#endif
          case TransactionalTypes.TransactionScope:
            portal = new TransactionalDataPortal();
            result = await portal.Fetch(objectType, criteria, context, isSync);
            break;
          default:
            portal = new DataPortalSelector();
            result = await portal.Fetch(objectType, criteria, context, isSync);
            break;
        }
#else
        portal = new DataPortalSelector();
        result = await portal.Fetch(objectType, criteria, context, isSync);
#endif
        return result;
      }
      catch (Csla.Server.DataPortalException)
      {
        throw;
      }
      catch (AggregateException ex)
      {
        Exception error = null;
        if (ex.InnerExceptions.Count > 0)
          error = ex.InnerExceptions[0].InnerException;
        else
          error = ex;
        throw new DataPortalException(
            "DataPortal.Fetch " + Resources.FailedOnServer,
            new DataPortalExceptionHandler().InspectException(objectType, criteria, "DataPortal.Fetch", error),
            new DataPortalResult());
      }
      catch (Exception ex)
      {
        throw new DataPortalException(
            "DataPortal.Fetch " + Resources.FailedOnServer,
            new DataPortalExceptionHandler().InspectException(objectType, criteria, "DataPortal.Fetch", ex),
            new DataPortalResult());
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
      try
      {
        SetContext(context);

#if !SILVERLIGHT && !NETFX_CORE
        AuthorizeRequest(new AuthorizeRequest(obj.GetType(), obj, DataPortalOperations.Update));
#endif
        DataPortalResult result;
        DataPortalMethodInfo method;
        var factoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(obj.GetType());
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

        context.TransactionalType = method.TransactionalType;
        IDataPortalServer portal;
#if !SILVERLIGHT && !NETFX_CORE
        switch (method.TransactionalType)
        {
#if !MONO
          case TransactionalTypes.EnterpriseServices:
            portal = new ServicedDataPortal();
            try
            {
              result = await portal.Update(obj, context, isSync);
            }
            finally
            {
              ((ServicedDataPortal)portal).Dispose();
            }
            break;
#endif
          case TransactionalTypes.TransactionScope:
            portal = new TransactionalDataPortal();
            result = await portal.Update(obj, context, isSync);
            break;
          default:
            portal = new DataPortalSelector();
            result = await portal.Update(obj, context, isSync);
            break;
        }
#else
        portal = new DataPortalSelector();
        result = await portal.Update(obj, context, isSync);
#endif
        return result;
      }
      catch (Csla.Server.DataPortalException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new DataPortalException(
            "DataPortal.Update " + Resources.FailedOnServer,
            new DataPortalExceptionHandler().InspectException(obj.GetType(), obj, null, "DataPortal.Update", ex),
            new DataPortalResult());
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

#if !SILVERLIGHT && !NETFX_CORE
        AuthorizeRequest(new AuthorizeRequest(objectType, criteria, DataPortalOperations.Delete));
#endif
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
#if !SILVERLIGHT && !NETFX_CORE
        switch (method.TransactionalType)
        {
#if !MONO
          case TransactionalTypes.EnterpriseServices:
            portal = new ServicedDataPortal();
            try
            {
              result = await portal.Delete(objectType, criteria, context, isSync);
            }
            finally
            {
              ((ServicedDataPortal)portal).Dispose();
            }
            break;
#endif
          case TransactionalTypes.TransactionScope:
            portal = new TransactionalDataPortal();
            result = await portal.Delete(objectType, criteria, context, isSync);
            break;
          default:
            portal = new DataPortalSelector();
            result = await portal.Delete(objectType, criteria, context, isSync);
            break;
        }
#else
        portal = new DataPortalSelector();
        result = await portal.Delete(objectType, criteria, context, isSync);
#endif
        return result;
      }
      catch (Csla.Server.DataPortalException ex)
      {
        Exception tmp = ex;
        throw;
      }
      catch (Exception ex)
      {
        throw new DataPortalException(
            "DataPortal.Delete " + Resources.FailedOnServer,
            new DataPortalExceptionHandler().InspectException(objectType, criteria, "DataPortal.Delete", ex),
            new DataPortalResult());
      }
      finally
      {
        ClearContext(context);
      }
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
#if NETFX_CORE
      var list = new System.Collections.ObjectModel.ReadOnlyCollection<string>(new List<string> { context.ClientUICulture });
      Windows.ApplicationModel.Resources.Core.ResourceManager.Current.DefaultContext.Languages = list;
      list = new System.Collections.ObjectModel.ReadOnlyCollection<string>(new List<string> { context.ClientCulture });
      Windows.ApplicationModel.Resources.Core.ResourceManager.Current.DefaultContext.Languages = list;
#else
      System.Threading.Thread.CurrentThread.CurrentCulture =
        new System.Globalization.CultureInfo(context.ClientCulture);
      System.Threading.Thread.CurrentThread.CurrentUICulture =
        new System.Globalization.CultureInfo(context.ClientUICulture);
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
#if !SILVERLIGHT && !NETFX_CORE
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

#if !SILVERLIGHT && !NETFX_CORE
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
#endif

    #endregion
  }
}