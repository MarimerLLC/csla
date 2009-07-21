using System;
using System.Configuration;
using System.Security.Principal;
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
      if(null==authProviderType)
        throw new ArgumentNullException("authProviderType", Resources.CslaAuthenticationProviderNotSet);
      if(!typeof(IAuthorizeDataPortal).IsAssignableFrom(authProviderType))
        throw new ArgumentException(Resources.AuthenticationProviderDoesNotImplementIAuthorizeDataPortal,"authProviderType");

      //only construct the type if it was not constructed already
      if (null == _authorizer )
      {
        lock(_syncRoot)
        {
          if (null == _authorizer)
            _authorizer = (IAuthorizeDataPortal) Activator.CreateInstance(authProviderType);
        }
      }
    }

    private static Type GetAuthProviderType(string cslaAuthorizationProviderAppSettingName)
    {
      if(cslaAuthorizationProviderAppSettingName==null)
        throw new ArgumentNullException("cslaAuthorizationProviderAppSettingName", Resources.AuthorizationProviderNameNotSpecified);


      if (null == _authorizer)//not yet instantiated
      {
        var authProvider = ConfigurationManager.AppSettings[cslaAuthorizationProviderAppSettingName];

        return string.IsNullOrEmpty(authProvider) ?
          typeof(NullAuthorizer) :
          Type.GetType(authProvider, true);
        
      }else
        return _authorizer.GetType();

    }

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
    public DataPortalResult Create(
      Type objectType, object criteria, DataPortalContext context)
    {
      try
      {
        SetContext(context);

        Authorize(new AuthorizeRequest(objectType, criteria, DataPortalOperations.Create));

        DataPortalResult result;

        DataPortalMethodInfo method = DataPortalMethodCache.GetCreateMethod(objectType, criteria);

        IDataPortalServer portal;
        switch (method.TransactionalType)
        {
          case TransactionalTypes.EnterpriseServices:
            portal = new ServicedDataPortal();
            try
            {
              result = portal.Create(objectType, criteria, context);
            }
            finally
            {
              ((ServicedDataPortal)portal).Dispose();
            }

            break;
          case TransactionalTypes.TransactionScope:

            portal = new TransactionalDataPortal();
            result = portal.Create(objectType, criteria, context);

            break;
          default:
            portal = new DataPortalSelector();
            result = portal.Create(objectType, criteria, context);
            break;
        }
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
          "DataPortal.Create " + Resources.FailedOnServer,
          ex, new DataPortalResult());
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
    public DataPortalResult Fetch(Type objectType, object criteria, DataPortalContext context)
    {
      try
      {
        SetContext(context);

        Authorize(new AuthorizeRequest(objectType, criteria, DataPortalOperations.Fetch));

        DataPortalResult result;

        DataPortalMethodInfo method = DataPortalMethodCache.GetFetchMethod(objectType, criteria);

        IDataPortalServer portal;
        switch (method.TransactionalType)
        {
          case TransactionalTypes.EnterpriseServices:
            portal = new ServicedDataPortal();
            try
            {
              result = portal.Fetch(objectType, criteria, context);
            }
            finally
            {
              ((ServicedDataPortal)portal).Dispose();
            }
            break;
          case TransactionalTypes.TransactionScope:
            portal = new TransactionalDataPortal();
            result = portal.Fetch(objectType, criteria, context);
            break;
          default:
            portal = new DataPortalSelector();
            result = portal.Fetch(objectType, criteria, context);
            break;
        }
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
          "DataPortal.Fetch " + Resources.FailedOnServer,
          ex, new DataPortalResult());
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
    public DataPortalResult Update(object obj, DataPortalContext context)
    {
      try
      {
        SetContext(context);

        Authorize(new AuthorizeRequest(obj.GetType(), obj, DataPortalOperations.Update));

        DataPortalResult result;
        DataPortalMethodInfo method;
        var factoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(obj.GetType());
        if (factoryInfo != null)
        {
          var factoryType = FactoryDataPortal.FactoryLoader.GetFactoryType(factoryInfo.FactoryTypeName);
          var bbase = obj as Core.BusinessBase;
          if (bbase != null && bbase.IsDeleted)
            method = Server.DataPortalMethodCache.GetMethodInfo(factoryType, factoryInfo.DeleteMethodName, new object[] { obj });
          else
            method = Server.DataPortalMethodCache.GetMethodInfo(factoryType, factoryInfo.UpdateMethodName, new object[] { obj });
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
          else if (obj is CommandBase)
            methodName = "DataPortal_Execute";
          else
            methodName = "DataPortal_Update";
          method = DataPortalMethodCache.GetMethodInfo(obj.GetType(), methodName);
        }

        context.TransactionalType = method.TransactionalType;
        IDataPortalServer portal;
        switch (method.TransactionalType)
        {
          case TransactionalTypes.EnterpriseServices:
            portal = new ServicedDataPortal();
            try
            {
              result = portal.Update(obj, context);
            }
            finally
            {
              ((ServicedDataPortal)portal).Dispose();
            }
            break;
          case TransactionalTypes.TransactionScope:
            portal = new TransactionalDataPortal();
            result = portal.Update(obj, context);
            break;
          default:
            portal = new DataPortalSelector();
            result = portal.Update(obj, context);
            break;
        }
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
          "DataPortal.Update " + Resources.FailedOnServer,
          ex, new DataPortalResult());
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
    public DataPortalResult Delete(Type objectType, object criteria, DataPortalContext context)
    {
      try
      {
        SetContext(context);

        Authorize(new AuthorizeRequest(objectType, criteria, DataPortalOperations.Delete));

        DataPortalResult result;

        var method = DataPortalMethodCache.GetMethodInfo(objectType, "DataPortal_Delete", criteria);

        IDataPortalServer portal;
        switch (method.TransactionalType)
        {
          case TransactionalTypes.EnterpriseServices:
            portal = new ServicedDataPortal();
            try
            {
              result = portal.Delete(objectType, criteria, context);
            }
            finally
            {
              ((ServicedDataPortal)portal).Dispose();
            }
            break;
          case TransactionalTypes.TransactionScope:
            portal = new TransactionalDataPortal();
            result = portal.Delete(objectType, criteria, context);
            break;
          default:
            portal = new DataPortalSelector();
            result = portal.Delete(objectType, criteria, context);
            break;
        }
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
          ex, new DataPortalResult());
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
      System.Threading.Thread.CurrentThread.CurrentCulture =
        new System.Globalization.CultureInfo(context.ClientCulture);
      System.Threading.Thread.CurrentThread.CurrentUICulture =
        new System.Globalization.CultureInfo(context.ClientUICulture);

      if (ApplicationContext.AuthenticationType == "Windows")
      {
        // When using integrated security, Principal must be null
        if (context.Principal != null)
        {
          System.Security.SecurityException ex =
            new System.Security.SecurityException(Resources.NoPrincipalAllowedException);
          ex.Action = System.Security.Permissions.SecurityAction.Deny;
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
          System.Security.SecurityException ex =
            new System.Security.SecurityException(
              Resources.BusinessPrincipalException + " Nothing");
          ex.Action = System.Security.Permissions.SecurityAction.Deny;
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

    private static void Authorize(AuthorizeRequest clientRequest)
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
  }
}