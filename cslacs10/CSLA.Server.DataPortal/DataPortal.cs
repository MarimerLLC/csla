using System;
using System.Reflection;
using System.Security.Principal;
using System.Configuration;

namespace CSLA.Server
{
  /// <summary>
  /// Implements the server-side DataPortal as discussed
  /// in Chapter 5.
  /// </summary>
  public class DataPortal : MarshalByRefObject
  {

    #region Data Access

    /// <summary>
    /// Called by the client-side DataPortal to create a new object.
    /// </summary>
    /// <param name="Criteria">Object-specific criteria.</param>
    /// <param name="Principal">The user's principal object (if using CSLA .NET security).
    /// </param>
    /// <returns>A populated business object.</returns>
    public object Create(object criteria, object principal)
    {
      SetPrincipal(principal);

      // create an instance of the business object
      object obj = CreateBusinessObject(criteria);

      // tell the business object to fetch its data
      CallMethod(obj, "DataPortal_Create", criteria);
      // return the populated business object as a result
      return obj;
    }

    /// <summary>
    /// Called by the client-side DataProtal to retrieve an object.
    /// </summary>
    /// <param name="Criteria">Object-specific criteria.</param>
    /// <param name="Principal">The user's principal object (if using CSLA .NET security).
    /// </param>
    /// <returns>A populated business object.</returns>
    public object Fetch(object criteria, object principal)
    {
      SetPrincipal(principal);

      // create an instance of the business object
      object obj = CreateBusinessObject(criteria);

      // tell the business object to fetch its data
      CallMethod(obj, "DataPortal_Fetch", criteria);

      // return the populated business object as a result
      return obj;
    }

    /// <summary>
    /// Called by the client-side DataPortal to update an object.
    /// </summary>
    /// <param name="obj">A reference to the object being updated.</param>
    /// <param name="Principal">The user's principal object (if using CSLA .NET security).
    /// </param>
    /// <returns>A reference to the newly updated object.</returns>
    public object Update(object obj, object principal)
    {
      SetPrincipal(principal);

      // tell the business object to update itself
      CallMethod(obj, "DataPortal_Update");
      return obj;
    }

    /// <summary>
    /// Called by the client-side DataPortal to delete an object.
    /// </summary>
    /// <param name="Criteria">Object-specific criteria.</param>
    /// <param name="Principal">The user's principal object (if using CSLA .NET security).
    /// </param>
    public void Delete(object criteria, object principal)
    {
      SetPrincipal(principal);

      // create an instance of the business object
      object obj = CreateBusinessObject(criteria);

      // tell the business object to delete itself
      CallMethod(obj, "DataPortal_Delete", criteria);
    }

    #endregion

    #region Security

    private string AUTHENTICATION()
    {
      string val = ConfigurationSettings.AppSettings["Authentication"];
      if(val == null)
        return string.Empty;
      else
        return val;
    }

    private void SetPrincipal(object principal)
    {
      if(AUTHENTICATION() == "Windows")
      {
        // when using integrated security, Principal must be Nothing
        // and we need to set our policy to use the Windows principal
        if(principal == null)
        {
          AppDomain.CurrentDomain.SetPrincipalPolicy(
            PrincipalPolicy.WindowsPrincipal);
          return;
        }
        else
          throw new System.Security.SecurityException(
            "No principal object should be passed to DataPortal " + 
            "when using Windows integrated security");
      }

      // we expect Principal to be of type BusinessPrincipal, but
      // we can't enforce that since it causes a circular reference
      // with the business library so instead we must use type object
      // for the parameter, so here we do a check on the type of the
      // parameter
      if(principal.ToString() == "CSLA.Security.BusinessPrincipal")
      {
        // see if our current principal is
        // different from the caller's principal
        if(!ReferenceEquals(principal, System.Threading.Thread.CurrentPrincipal))
        {                                                                                                // the caller had a different principal, so change ours to
          // match the caller's so all our objects use the caller's
          // security
          System.Threading.Thread.CurrentPrincipal = (IPrincipal)principal;
        }
      }
      else
        throw new System.Security.SecurityException(
          "Principal must be of type BusinessPrincipal, not " + 
          principal.ToString());
    }

    #endregion

    #region Creating the business object

    private object CreateBusinessObject(object criteria)
    {
      // get the type of the actual business object
      Type businessType = criteria.GetType().DeclaringType;
      // create an instance of the business object
      return Activator.CreateInstance(businessType, true);
    }

    #endregion

    #region Calling a method

    object CallMethod(object obj, string method, params object[] p)
    {
      // call a private method on the object
      MethodInfo info = GetMethod(obj.GetType(), method);
      object result;

      try
      {
        result = info.Invoke(obj, p);
      }
      catch(System.Exception e)
      {
        throw e.InnerException();
      }
      return result;
    }

    MethodInfo GetMethod(Type objectType, string method)
    {
      return objectType.GetMethod(method,                                                              BindingFlags.FlattenHierarchy |
        BindingFlags.Instance |
        BindingFlags.Public |
        BindingFlags.NonPublic);
    }

    #endregion

  }
}
