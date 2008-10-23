using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Reflection;

namespace Csla.DataPortalClient
{
  public class DesignTimeProxy : DataPortalClient.IDataPortalProxy
  {

    #region IDataPortalProxy Members

    public bool IsServerRemote
    {
      get { return false; }
    }

    #endregion

    #region IDataPortalServer Members

    public Csla.Server.DataPortalResult Create(Type objectType, object criteria, Csla.Server.DataPortalContext context)
    {
      return CreateDesignTimeObject(objectType, criteria, context);
    }

    public Csla.Server.DataPortalResult Fetch(Type objectType, object criteria, Csla.Server.DataPortalContext context)
    {
      return CreateDesignTimeObject(objectType, criteria, context);
    }

    public Csla.Server.DataPortalResult Update(object obj, Csla.Server.DataPortalContext context)
    {
      return new Csla.Server.DataPortalResult(obj);
    }

    public Csla.Server.DataPortalResult Delete(Type objectType, object criteria, Csla.Server.DataPortalContext context)
    {
      return CreateDesignTimeObject(objectType, criteria, context);
    }

    private Csla.Server.DataPortalResult CreateDesignTimeObject(Type objectType, object criteria, Csla.Server.DataPortalContext context)
    {
      var obj = Activator.CreateInstance(objectType, true);
      object returnValue = null;
      returnValue = MethodCaller.CallMethodIfImplemented(obj, "DesignTime_Create");
      return new Csla.Server.DataPortalResult(returnValue);
    }

    #endregion
  }
}
