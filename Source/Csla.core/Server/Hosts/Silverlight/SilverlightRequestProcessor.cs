using System;
using System.Configuration;
using Csla.Properties;

namespace Csla.Server.Hosts.Silverlight
{
  /// <summary>
  /// Object taht processes all the requests from a Silverlight client
  /// </summary>
  public class SilverlightRequestProcessor
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

    #region Operations

    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    /// <returns>Resulf of the create operation - an instance of a business object</returns>
    public SilverlightResponse Create(SilverlightCriteriaRequest request)
    {
      var result = new SilverlightResponse();
      try
      {
        object criteria = request.Criteria;
        // load type for business object
        var t = Type.GetType(request.TypeName);
        if (t == null)
          throw new InvalidOperationException(
            string.Format(Csla.Properties.Resources.ObjectTypeCouldNotBeLoaded, request.TypeName));

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
        result.Object = o;
        result.GlobalContext = ApplicationContext.GlobalContext;
      }
      catch (Csla.Reflection.CallMethodException ex)
      {
        result.Error = ex.InnerException;
      }
      catch (Exception ex)
      {
        result.Error = ex;
      }
      finally
      {
        ClearContext();
      }
      return result;
    }

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    /// <returns>Result of the fetch operation - an instance of a business object</returns>
    public SilverlightResponse Fetch(SilverlightCriteriaRequest request)
    {
      var result = new SilverlightResponse();
      try
      {
        // unpack criteria data into object
        object criteria = request.Criteria;

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
        result.Object = o;
        result.GlobalContext = ApplicationContext.GlobalContext;
      }
      catch (Csla.Reflection.CallMethodException ex)
      {
        result.Error = ex.InnerException;
      }
      catch (Exception ex)
      {
        result.Error = ex;
      }
      finally
      {
        ClearContext();
      }
      return result;
    }

    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    /// <returns>Result of the update operation - updated object</returns>
    public SilverlightResponse Update(SilverlightUpdateRequest request)
    {
      var result = new SilverlightResponse();
      try
      {
        // unpack object
        object obj = request.ObjectToUpdate;

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
          if (string.IsNullOrEmpty(factoryInfo.UpdateMethodName))
            throw new InvalidOperationException(Resources.UpdateMethodNameNotSpecified);

          SetContext(request);
          object f = FactoryLoader.GetFactory(factoryInfo.FactoryTypeName);
          o = Csla.Reflection.MethodCaller.CallMethod(f, factoryInfo.UpdateMethodName, obj);
        }
        result.Object = o;
        result.GlobalContext = ApplicationContext.GlobalContext;
      }
      catch (Csla.Reflection.CallMethodException ex)
      {
        result.Error = ex.InnerException;
      }
      catch (Exception ex)
      {
        result.Error = ex;
      }
      finally
      {
        ClearContext();
      }
      return result;
    }

    /// <summary>
    /// Delete a business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    /// <returns>Result of the delete operation</returns>
    public SilverlightResponse Delete(SilverlightCriteriaRequest request)
    {
      var result = new SilverlightResponse();
      try
      {
        // unpack criteria data into object
        object criteria = request.Criteria;

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
        result.GlobalContext = ApplicationContext.GlobalContext;
      }
      catch (Csla.Reflection.CallMethodException ex)
      {
        result.Error = ex.InnerException;
      }
      catch (Exception ex)
      {
        result.Error = ex;
      }
      finally
      {
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
    private void SetContext(ISilverlightRequest request)
    {
      ApplicationContext.SetContext(request.ClientContext, request.GlobalContext);
      ApplicationContext.User = request.Principal;
    }

    public static void ClearContext()
    {
      ApplicationContext.Clear();
      if (ApplicationContext.AuthenticationType != "Windows")
        ApplicationContext.User = new System.Security.Principal.GenericPrincipal(new System.Security.Principal.GenericIdentity(string.Empty), new string[] { });
    }

    #endregion
  }
}
