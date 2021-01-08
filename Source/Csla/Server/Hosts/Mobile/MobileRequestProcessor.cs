//-----------------------------------------------------------------------
// <copyright file="MobileRequestProcessor.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Object that processes all the requests from a client</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Configuration;
using System.Globalization;
using System.Threading;
using Csla.Properties;
using System.Threading.Tasks;

namespace Csla.Server.Hosts.Mobile
{
  /// <summary>
  /// Object that processes all the requests from a client
  /// </summary>
  public class MobileRequestProcessor
  {

#region Factory Loader

    private static IMobileFactoryLoader _factoryLoader;
    /// <summary>
    /// Gets or sets a delegate reference to the method
    /// called to create instances of factory objects
    /// as requested by the MobileFactory attribute on
    /// a CSLA Light business object.
    /// </summary>
    public static IMobileFactoryLoader FactoryLoader
    {
      get
      {
        if (_factoryLoader == null)
        {
          string setting = ConfigurationManager.AppSettings["CslaMobileFactoryLoader"];
          if (!string.IsNullOrEmpty(setting))
            _factoryLoader =
              (IMobileFactoryLoader)Reflection.MethodCaller.CreateInstance(Type.GetType(setting, true, true));
          else
            _factoryLoader = new MobileFactoryLoader();
        }
        return _factoryLoader;
      }
      set
      {
        _factoryLoader = value;
      }
    }

#endregion

#region Operations

    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    /// <returns>Resulf of the create operation - an instance of a business object</returns>
    public async Task<MobileResponse> Create(MobileCriteriaRequest request)
    {
      var serverDataPortal = new Csla.Server.DataPortal();
      var result = new MobileResponse();
      Type businessObjectType = null;
      object criteria = null;
      try
      {
        criteria = request.Criteria;
        // load type for business object
        businessObjectType = Type.GetType(request.TypeName);
        if (businessObjectType == null)
          throw new InvalidOperationException(
            string.Format(Csla.Properties.Resources.ObjectTypeCouldNotBeLoaded, request.TypeName));

        SetContext(request);

        serverDataPortal.Initialize(new InterceptArgs { ObjectType = businessObjectType, Parameter = criteria, Operation = DataPortalOperations.Create });
        serverDataPortal.Authorize(new AuthorizeRequest(businessObjectType, criteria, DataPortalOperations.Create));

        object newObject = null;
        var factoryInfo = GetMobileFactoryAttribute(businessObjectType);
        if (factoryInfo == null)
        {
          if (criteria != null)
            newObject = await Csla.Reflection.MethodCaller.CallGenericStaticMethodAsync(typeof(Csla.DataPortal), "CreateAsync", new Type[] { businessObjectType }, true, criteria).ConfigureAwait(false);
          else
            newObject = await Csla.Reflection.MethodCaller.CallGenericStaticMethodAsync(typeof(Csla.DataPortal), "CreateAsync", new Type[] { businessObjectType }, false, null).ConfigureAwait(false);
        }
        else
        {
          if (string.IsNullOrEmpty(factoryInfo.CreateMethodName))
            throw new InvalidOperationException(Resources.CreateMethodNameNotSpecified);

          object f = FactoryLoader.GetFactory(factoryInfo.FactoryTypeName);
          if (criteria != null)
            newObject = await Csla.Reflection.MethodCaller.CallMethodTryAsync(f, factoryInfo.CreateMethodName, criteria).ConfigureAwait(false);
          else
            newObject = await Csla.Reflection.MethodCaller.CallMethodTryAsync(f, factoryInfo.CreateMethodName).ConfigureAwait(false);
        }
        result.Object = newObject;
#pragma warning disable CS0618 // Type or member is obsolete
        result.GlobalContext = ApplicationContext.GlobalContext;
#pragma warning restore CS0618 // Type or member is obsolete
      }
      catch (Csla.Reflection.CallMethodException ex)
      {
        var inspected = new DataPortalExceptionHandler().InspectException(businessObjectType, criteria, "DataPortal.Create", ex);
        result.Error = inspected.InnerException;
      }
      catch (Exception ex)
      {
        var inspected = new DataPortalExceptionHandler().InspectException(businessObjectType, criteria, "DataPortal.Create", ex);
        result.Error = inspected;
      }
      finally
      {
        if (result.Error != null)
          serverDataPortal.Complete(new InterceptArgs { ObjectType = businessObjectType, Parameter = criteria, Operation = DataPortalOperations.Create, Exception = result.Error, Result = new DataPortalResult(result.Object, result.Error, result.GlobalContext) });
        else
          serverDataPortal.Complete(new InterceptArgs { ObjectType = businessObjectType, Parameter = criteria, Operation = DataPortalOperations.Create, Result = new DataPortalResult(result.Object, result.GlobalContext) });
        ClearContext();
      }
      return result;
    }

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    /// <returns>Result of the fetch operation - an instance of a business object</returns>
    public async Task<MobileResponse> Fetch(MobileCriteriaRequest request)
    {
      var serverDataPortal = new Csla.Server.DataPortal();
      var result = new MobileResponse();
      Type businessObjectType = null;
      object criteria = null;
      try
      {
        // unpack criteria data into object
        criteria = request.Criteria;

        // load type for business object
        businessObjectType = Type.GetType(request.TypeName);
        if (businessObjectType == null)
          throw new InvalidOperationException(
            string.Format(Resources.ObjectTypeCouldNotBeLoaded, request.TypeName));

        SetContext(request);

        serverDataPortal.Initialize(new InterceptArgs { ObjectType = businessObjectType, Parameter = criteria, Operation = DataPortalOperations.Fetch });
        serverDataPortal.Authorize(new AuthorizeRequest(businessObjectType, criteria, DataPortalOperations.Fetch));

        object newObject = null;
        var factoryInfo = GetMobileFactoryAttribute(businessObjectType);
        if (factoryInfo == null)
        {
          if (criteria != null)
            newObject = await Csla.Reflection.MethodCaller.CallGenericStaticMethodAsync(typeof(Csla.DataPortal), "FetchAsync", new Type[] { businessObjectType }, true, criteria).ConfigureAwait(false);
          else
            newObject = await Csla.Reflection.MethodCaller.CallGenericStaticMethodAsync(typeof(Csla.DataPortal), "FetchAsync", new Type[] { businessObjectType }, false, null).ConfigureAwait(false);
        }
        else
        {
          if (string.IsNullOrEmpty(factoryInfo.FetchMethodName))
            throw new InvalidOperationException(Resources.FetchMethodNameNotSpecified);

          object f = FactoryLoader.GetFactory(factoryInfo.FactoryTypeName);
          if (criteria != null)
            newObject = await Csla.Reflection.MethodCaller.CallMethodTryAsync(f, factoryInfo.FetchMethodName, criteria).ConfigureAwait(false);
          else
            newObject = await Csla.Reflection.MethodCaller.CallMethodTryAsync(f, factoryInfo.FetchMethodName).ConfigureAwait(false);
        }
        result.Object = newObject;
#pragma warning disable CS0618 // Type or member is obsolete
        result.GlobalContext = ApplicationContext.GlobalContext;
#pragma warning restore CS0618 // Type or member is obsolete
      }
      catch (Csla.Reflection.CallMethodException ex)
      {
        var inspected = new DataPortalExceptionHandler().InspectException(businessObjectType, criteria, "DataPortal.Fetch", ex);
        result.Error = inspected.InnerException;
      }
      catch (Exception ex)
      {
        var inspected = new DataPortalExceptionHandler().InspectException(businessObjectType, criteria, "DataPortal.Fetch", ex);
        result.Error = inspected;
      }
      finally
      {
        if (result.Error != null)
          serverDataPortal.Complete(new InterceptArgs { ObjectType = businessObjectType, Parameter = criteria, Operation = DataPortalOperations.Fetch, Exception = result.Error, Result = new DataPortalResult(result.Object, result.Error, result.GlobalContext) });
        else
          serverDataPortal.Complete(new InterceptArgs { ObjectType = businessObjectType, Parameter = criteria, Operation = DataPortalOperations.Fetch, Result = new DataPortalResult(result.Object, result.GlobalContext) });
        ClearContext();
      }
      return result;
    }

    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    /// <returns>Result of the update operation - updated object</returns>
    public async Task<MobileResponse> Update(MobileUpdateRequest request)
    {
      var serverDataPortal = new Csla.Server.DataPortal();
      var result = new MobileResponse();
      Type businessObjectType = null;
      object obj = null;
      var operation = DataPortalOperations.Update;
      try
      {
        // unpack object
        obj = request.ObjectToUpdate;

        if (obj is Csla.Core.ICommandObject)
          operation = DataPortalOperations.Execute;

        // load type for business object
        businessObjectType = obj.GetType();

        SetContext(request);

        serverDataPortal.Initialize(new InterceptArgs { ObjectType = businessObjectType, Parameter = obj, Operation = operation });
        serverDataPortal.Authorize(new AuthorizeRequest(businessObjectType, obj, operation));

        object newObject = null;
        var factoryInfo = GetMobileFactoryAttribute(businessObjectType);
        if (factoryInfo == null)
        {
          newObject = await Csla.Reflection.MethodCaller.CallGenericStaticMethodAsync(typeof(Csla.DataPortal), "UpdateAsync", new Type[] { businessObjectType }, true, obj).ConfigureAwait(false);
        }
        else
        {
          if (string.IsNullOrEmpty(factoryInfo.UpdateMethodName))
            throw new InvalidOperationException(Resources.UpdateMethodNameNotSpecified);

          object f = FactoryLoader.GetFactory(factoryInfo.FactoryTypeName);
          newObject = await Csla.Reflection.MethodCaller.CallMethodTryAsync(f, factoryInfo.UpdateMethodName, obj).ConfigureAwait(false);
        }
        result.Object = newObject;
#pragma warning disable CS0618 // Type or member is obsolete
        result.GlobalContext = ApplicationContext.GlobalContext;
#pragma warning restore CS0618 // Type or member is obsolete
      }
      catch (Csla.Reflection.CallMethodException ex)
      {
        var inspected = new DataPortalExceptionHandler().InspectException(businessObjectType, obj, "DataPortal.Update", ex);
        result.Error = inspected.InnerException;
      }
      catch (Exception ex)
      {
        var inspected = new DataPortalExceptionHandler().InspectException(businessObjectType, obj, "DataPortal.Update", ex);
        result.Error = inspected;
      }
      finally
      {
        if (result.Error != null)
          serverDataPortal.Complete(new InterceptArgs { ObjectType = businessObjectType, Parameter = obj, Operation = operation, Exception = result.Error, Result = new DataPortalResult(result.Object, result.Error, result.GlobalContext) });
        else
          serverDataPortal.Complete(new InterceptArgs { ObjectType = businessObjectType, Parameter = obj, Operation = operation, Result = new DataPortalResult(result.Object, result.GlobalContext) });
        ClearContext();
      }
      return result;
    }

    /// <summary>
    /// Delete a business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    /// <returns>Result of the delete operation</returns>
    public async Task<MobileResponse> Delete(MobileCriteriaRequest request)
    {
      var serverDataPortal = new Csla.Server.DataPortal();
      var result = new MobileResponse();
      Type businessObjectType = null;
      object criteria = null;
      try
      {
        // unpack criteria data into object
        criteria = request.Criteria;

        // load type for business object
        businessObjectType = Type.GetType(request.TypeName);
        if (businessObjectType == null)
          throw new InvalidOperationException(
            string.Format(Resources.ObjectTypeCouldNotBeLoaded, request.TypeName));

        SetContext(request);

        serverDataPortal.Initialize(new InterceptArgs { ObjectType = businessObjectType, Parameter = criteria, Operation = DataPortalOperations.Delete });
        serverDataPortal.Authorize(new AuthorizeRequest(businessObjectType, criteria, DataPortalOperations.Delete));

        var factoryInfo = GetMobileFactoryAttribute(businessObjectType);
        if (factoryInfo == null)
        {
          await Csla.Reflection.MethodCaller.CallGenericStaticMethodAsync(typeof(Csla.DataPortal), "DeleteAsync", new Type[] { businessObjectType }, true, criteria).ConfigureAwait(false);
        }
        else
        {
          if (string.IsNullOrEmpty(factoryInfo.DeleteMethodName))
            throw new InvalidOperationException(Resources.DeleteMethodNameNotSpecified);

          object f = FactoryLoader.GetFactory(factoryInfo.FactoryTypeName);
          await Csla.Reflection.MethodCaller.CallMethodTryAsync(f, factoryInfo.DeleteMethodName, criteria).ConfigureAwait(false);
        }
#pragma warning disable CS0618 // Type or member is obsolete
        result.GlobalContext = ApplicationContext.GlobalContext;
#pragma warning restore CS0618 // Type or member is obsolete
      }
      catch (Csla.Reflection.CallMethodException ex)
      {
        var inspected = new DataPortalExceptionHandler().InspectException(businessObjectType, criteria, "DataPortal.Delete", ex);
        result.Error = inspected.InnerException;
      }
      catch (Exception ex)
      {
        var inspected = new DataPortalExceptionHandler().InspectException(businessObjectType, criteria, "DataPortal.Delete", ex);
        result.Error = inspected;
      }
      finally
      {
        if (result.Error != null)
          serverDataPortal.Complete(new InterceptArgs { ObjectType = businessObjectType, Parameter = criteria, Operation = DataPortalOperations.Delete, Exception = result.Error, Result = new DataPortalResult(result.Object, result.Error, result.GlobalContext) });
        else
          serverDataPortal.Complete(new InterceptArgs { ObjectType = businessObjectType, Parameter = criteria, Operation = DataPortalOperations.Delete, Result = new DataPortalResult(result.Object, result.GlobalContext) });
        ClearContext();
      }
      return result;
    }

#endregion

#region Mobile Factory

    private static MobileFactoryAttribute GetMobileFactoryAttribute(Type objectType)
    {
      var result = objectType.GetCustomAttributes(typeof(MobileFactoryAttribute), true);
      if (result != null && result.Length > 0)
        return result[0] as MobileFactoryAttribute;
      else
        return null;
    }

#endregion

#region Context and Criteria
    private void SetContext(IMobileRequest request)
    {
      ApplicationContext.SetExecutionLocation(ApplicationContext.ExecutionLocations.Server);
      ApplicationContext.SetLogicalExecutionLocation(ApplicationContext.LogicalExecutionLocations.Server);

      ApplicationContext.SetContext(request.ClientContext, request.GlobalContext);
      if (ApplicationContext.AuthenticationType != "Windows")
        ApplicationContext.User = request.Principal;
      SetClientCultures(request);
    }

    /// <summary>
    /// Clears the application context and current principal.
    /// </summary>
    public static void ClearContext()
    {
      ApplicationContext.Clear();
      if (ApplicationContext.AuthenticationType != "Windows")
        ApplicationContext.User = new System.Security.Principal.GenericPrincipal(new System.Security.Principal.GenericIdentity(string.Empty), new string[] { });
    }

#region client culture
    /// <summary>
    /// Sets the client cultures on current tread.
    /// </summary>
    /// <param name="request">The request.</param>
    private void SetClientCultures(IMobileRequest request)
    {
      CultureInfo culture = null;
      // clientCulture
      if (TryGetCulture(request.ClientCulture, ref culture))
      {
        Thread.CurrentThread.CurrentCulture = culture;
      }
      // clientUICulture
      if (TryGetCulture(request.ClientUICulture, ref culture))
      {
        Thread.CurrentThread.CurrentUICulture = culture;
      }
    }


    /// <summary>
    /// Tries the convert string culture name to culture.
    /// </summary>
    /// <param name="clientCulture">The client culture.</param>
    /// <param name="culture">The culture.</param>
    /// <returns></returns>
    private bool TryGetCulture(string clientCulture, ref CultureInfo culture)
    {
      if (string.IsNullOrWhiteSpace(clientCulture)) return false;

      try
      {
        culture = CultureInfo.GetCultureInfo(clientCulture);
        return true;
      }
      catch (CultureNotFoundException)
      {
        return false;
      }
    }

#endregion

#endregion
  }
}
