using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Reflection;

namespace Csla.Server
{
  /// <summary>
  /// Base class to be used when creating a data portal
  /// factory object.
  /// </summary>
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

    /// <summary>
    /// By wrapping this property inside Using block
    /// you can set property values on 
    /// <paramref name="businessObject">business object</paramref>
    /// without raising PropertyChanged events
    /// and checking user rights.
    /// </summary>
    /// <param name="businessObject">
    /// Object on with you would like to set property values
    /// </param>
    /// <returns>
    /// An instance of IDisposable object that allows
    /// bypassing of normal authorization checks during
    /// property setting.
    /// </returns>
    protected IDisposable BypassPropertyChecks(Csla.Core.BusinessBase businessObject)
    {
      return businessObject.BypassPropertyChecks;
    }
  }
}
