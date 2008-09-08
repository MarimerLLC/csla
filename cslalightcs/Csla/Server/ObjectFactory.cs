using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Reflection;
using Csla.DataPortalClient;

namespace Csla.Server
{
  public abstract class ObjectFactory
  {
    /// <summary>
    /// Calls the MarkOld method on the specified
    /// object, if possible.
    /// </summary>
    /// <param name="obj">
    /// Object on which to call the method.
    /// </param>
    protected void MarkOld(object obj)
    {
      var target = obj as IDataPortalTarget;
      if (target != null)
        target.MarkOld();
      else
        MethodCaller.CallMethodIfImplemented(obj, "MarkOld", null);
    }

    /// <summary>
    /// Calls the MarkNew method on the specified
    /// object, if possible.
    /// </summary>
    /// <param name="obj">
    /// Object on which to call the method.
    /// </param>
    protected void MarkNew(object obj)
    {
      var target = obj as IDataPortalTarget;
      if (target != null)
        target.MarkNew();
      else
        MethodCaller.CallMethodIfImplemented(obj, "MarkNew", null);
    }

    /// <summary>
    /// Calls the MarkAsChild method on the specified
    /// object, if possible.
    /// </summary>
    /// <param name="obj">
    /// Object on which to call the method.
    /// </param>
    protected void MarkAsChild(object obj)
    {
      var target = obj as IDataPortalTarget;
      if (target != null)
        target.MarkAsChild();
      else
        MethodCaller.CallMethodIfImplemented(obj, "MarkAsChild", null);
    }
  }
}
