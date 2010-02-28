using System;
using System.Configuration;
using Csla.Serialization.Mobile;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Csla.Core;
using System.Security.Principal;
using Csla.Properties;

namespace Csla.Server.Hosts.Silverlight
{
  /// <summary>
  /// Exposes server-side DataPortal functionality
  /// through WCF.
  /// </summary>
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
   public class WcfPortal : IWcfPortal
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
              (IMobileFactoryLoader)Activator.CreateInstance(Type.GetType(setting, true, true));
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

    #region IWcfPortal Members

    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    public WcfResponse Create(CriteriaRequest request)
    {
      var result = new WcfResponse();
      try
      {
        request = ConvertRequest(request);
        // unpack criteria data into object
        object criteria = GetCriteria(request.CriteriaData);

        // load type for business object
        var t = Type.GetType(request.TypeName);
        if (t == null)
          throw new InvalidOperationException(
            string.Format(Resources.ObjectTypeCouldNotBeLoaded, request.TypeName));

        SetContext(request);
        object o = null;
        var factoryInfo = GetMobileFactoryAttribute(t);
        if (factoryInfo == null)
        {
          if (criteria != null)
          {
            o = Csla.DataPortal.Create(t, criteria);
          }
          else
          {
            o = Csla.DataPortal.Create(t);
          }
        }
        else
        {
          if (string.IsNullOrEmpty(factoryInfo.CreateMethodName))
            throw new InvalidOperationException(Resources.CreateMethodNameNotSpecified);

          object f = FactoryLoader.GetFactory(factoryInfo.FactoryTypeName);
          if (criteria != null)
            o = Csla.Reflection.MethodCaller.CallMethod(f, factoryInfo.CreateMethodName, criteria);
          else
            o = Csla.Reflection.MethodCaller.CallMethod(f, factoryInfo.CreateMethodName);
        }
        result.ObjectData = MobileFormatter.Serialize(o);
        result.GlobalContext = MobileFormatter.Serialize(ApplicationContext.GlobalContext);
      }
      catch (Csla.Reflection.CallMethodException ex)
      {
        result.ErrorData = new WcfErrorInfo(ex.InnerException);
      }
      catch (Exception ex)
      {
        result.ErrorData = new WcfErrorInfo(ex);
      }
      finally
      {
        ClearContext();
      }
      return ConvertResponse(result);
    }

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    public WcfResponse Fetch(CriteriaRequest request)
    {
      var result = new WcfResponse();
      try
      {
        request = ConvertRequest(request);
        // unpack criteria data into object
        object criteria = GetCriteria(request.CriteriaData);

        // load type for business object
        var t = Type.GetType(request.TypeName);
        if (t == null)
          throw new InvalidOperationException(
            string.Format(Resources.ObjectTypeCouldNotBeLoaded, request.TypeName));
        SetContext(request);
        object o = null;
        var factoryInfo = GetMobileFactoryAttribute(t);
        if (factoryInfo == null)
        {
          if (criteria == null)
            o = Csla.DataPortal.Fetch(t);
          else
            o = Csla.DataPortal.Fetch(t, criteria);
        }
        else
        {
          if (string.IsNullOrEmpty(factoryInfo.FetchMethodName))
            throw new InvalidOperationException(Resources.FetchMethodNameNotSpecified);

          object f = FactoryLoader.GetFactory(factoryInfo.FactoryTypeName);
          if (criteria != null)
            o = Csla.Reflection.MethodCaller.CallMethod(f, factoryInfo.FetchMethodName, criteria);
          else
            o = Csla.Reflection.MethodCaller.CallMethod(f, factoryInfo.FetchMethodName);
        }
        result.ObjectData = MobileFormatter.Serialize(o);
        result.GlobalContext = MobileFormatter.Serialize(ApplicationContext.GlobalContext);
      }
      catch (Csla.Reflection.CallMethodException ex)
      {
        result.ErrorData = new WcfErrorInfo(ex.InnerException);
      }
      catch (Exception ex)
      {
        result.ErrorData = new WcfErrorInfo(ex);
      }
      finally
      {
        ClearContext();
      }
      return ConvertResponse(result);
    }

    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    public WcfResponse Update(UpdateRequest request)
    {
      var result = new WcfResponse();
      try
      {
        request = ConvertRequest(request);
        // unpack object
        object obj = GetCriteria(request.ObjectData);

        // load type for business object
        var t = obj.GetType();

        object o = null;
        var factoryInfo = GetMobileFactoryAttribute(t);
        if (factoryInfo == null)
        {
          SetContext(request);
          o = Csla.DataPortal.Update(obj);
        }
        else
        {
          string methodName;
          if (obj is Core.ICommandObject)
            methodName = factoryInfo.ExecuteMethodName;
          else
          {
            methodName = factoryInfo.UpdateMethodName;
          }
          if (string.IsNullOrEmpty(methodName))
            throw new InvalidOperationException(Resources.UpdateMethodNameNotSpecified);

          SetContext(request);
          object f = FactoryLoader.GetFactory(factoryInfo.FactoryTypeName);
            o = Csla.Reflection.MethodCaller.CallMethod(f, methodName, obj);
        }
        result.ObjectData = MobileFormatter.Serialize(o);
        result.GlobalContext = MobileFormatter.Serialize(ApplicationContext.GlobalContext);
      }
      catch (Csla.Reflection.CallMethodException ex)
      {
        result.ErrorData = new WcfErrorInfo(ex.InnerException);
      }
      catch (Exception ex)
      {
        result.ErrorData = new WcfErrorInfo(ex);
      }
      finally
      {
        ClearContext();
      }
      return ConvertResponse(result);
    }

    /// <summary>
    /// Delete a business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    public WcfResponse Delete(CriteriaRequest request)
    {
      var result = new WcfResponse();
      try
      {
        request = ConvertRequest(request);
        // unpack criteria data into object
        object criteria = GetCriteria(request.CriteriaData);

        // load type for business object
        var t = Type.GetType(request.TypeName);
        if (t == null)
          throw new InvalidOperationException(
            string.Format(Resources.ObjectTypeCouldNotBeLoaded, request.TypeName));

        SetContext(request);

        var factoryInfo = GetMobileFactoryAttribute(t);
        if (factoryInfo == null)
        {
          Csla.DataPortal.Delete(criteria);
        }
        else
        {
          if (string.IsNullOrEmpty(factoryInfo.DeleteMethodName))
            throw new InvalidOperationException(Resources.DeleteMethodNameNotSpecified);

          object f = FactoryLoader.GetFactory(factoryInfo.FactoryTypeName);
          if (criteria != null)
            Csla.Reflection.MethodCaller.CallMethod(f, factoryInfo.DeleteMethodName, criteria);
          else
            Csla.Reflection.MethodCaller.CallMethod(f, factoryInfo.DeleteMethodName);
        }
        result.GlobalContext = MobileFormatter.Serialize(ApplicationContext.GlobalContext);
      }
      catch (Csla.Reflection.CallMethodException ex)
      {
        result.ErrorData = new WcfErrorInfo(ex.InnerException);
      }
      catch (Exception ex)
      {
        result.ErrorData = new WcfErrorInfo(ex);
      }
      finally
      {
        ClearContext();
      }
      return ConvertResponse(result);
    }

    #endregion

    #region Context and Criteria
    private void SetContext(CriteriaRequest request)
    {
      ApplicationContext.SetContext((ContextDictionary)MobileFormatter.Deserialize(request.ClientContext), (ContextDictionary)MobileFormatter.Deserialize(request.GlobalContext));
      ApplicationContext.User = (IPrincipal)MobileFormatter.Deserialize(request.Principal);
      // set the thread's culture to match the client
      if (!string.IsNullOrEmpty(request.ClientCulture))
        System.Threading.Thread.CurrentThread.CurrentCulture =
          new System.Globalization.CultureInfo(request.ClientCulture);
      if (!string.IsNullOrEmpty(request.ClientUICulture))
        System.Threading.Thread.CurrentThread.CurrentUICulture =
          new System.Globalization.CultureInfo(request.ClientUICulture);
    }

    private void SetContext(UpdateRequest request)
    {
      ApplicationContext.SetContext((ContextDictionary)MobileFormatter.Deserialize(request.ClientContext), (ContextDictionary)MobileFormatter.Deserialize(request.GlobalContext));
      ApplicationContext.User = (IPrincipal)MobileFormatter.Deserialize(request.Principal);
      // set the thread's culture to match the client
      if (!string.IsNullOrEmpty(request.ClientCulture))
        System.Threading.Thread.CurrentThread.CurrentCulture =
          new System.Globalization.CultureInfo(request.ClientCulture);
      if (!string.IsNullOrEmpty(request.ClientUICulture))
        System.Threading.Thread.CurrentThread.CurrentUICulture =
          new System.Globalization.CultureInfo(request.ClientUICulture);
    }

    private static void ClearContext()
    {
      ApplicationContext.Clear();
      if (ApplicationContext.AuthenticationType != "Windows")
        ApplicationContext.User = new System.Security.Principal.GenericPrincipal(new System.Security.Principal.GenericIdentity(string.Empty),new string[]{});
    }

    private static object GetCriteria(byte[] criteriaData)
    {
      object criteria = null;
      if (criteriaData != null)
        criteria = MobileFormatter.Deserialize(criteriaData);
      return criteria;
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

    #region Extention Method for Requests

    /// <summary>
    /// Override to convert the request data before it
    /// is transferred over the network.
    /// </summary>
    /// <param name="request">Request object.</param>
    protected virtual UpdateRequest ConvertRequest(UpdateRequest request)
    {
      return request;
    }

    /// <summary>
    /// Override to convert the request data before it
    /// is transferred over the network.
    /// </summary>
    /// <param name="request">Request object.</param>
    protected virtual CriteriaRequest ConvertRequest(CriteriaRequest request)
    {
      return request;
    }

    /// <summary>
    /// Override to convert the response data after it
    /// comes back from the network.
    /// </summary>
    /// <param name="response">Response object.</param>
    protected virtual WcfResponse ConvertResponse(WcfResponse response)
    {
      return response;
    }

    #endregion
  }
}