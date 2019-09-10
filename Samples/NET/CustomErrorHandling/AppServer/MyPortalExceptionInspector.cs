using System;
using System.Diagnostics;
using Csla.Server;

namespace AppServer
{
  public class MyDataPortalExceptionInspector : Csla.Server.IDataPortalExceptionInspector
  {
    public void InspectException(Type objectType, object businessObject, object criteria, string methodName, Exception ex)
    {
      // add your logging code here for exceptions on the server
      //Trace.TraceError("Server exception: {0} with {1}, stackTrace {2}", objectType.FullName, ex.ToString(), ex.StackTrace);
      Debug.Print("Server exception: {0} with {1}, stackTrace {2}", objectType.FullName, ex.ToString(), ex.StackTrace);

      // Transform to other exception to return to client
      if (!IsSerializable(ex) || ex.GetType().FullName.Contains("ServerOnlyException"))
        // transform to genereic exception to send to client
        throw new GenericBusinessException(ex);
    }

    private bool IsSerializable(Exception ex)
    {
      if (!ex.GetType().IsSerializable) return false;
      if (ex.InnerException != null)
      {
        return IsSerializable(ex.InnerException);
      }
      return true;
    }

    private GenericBusinessException ToGenericBusinessException(Exception ex)
    {
      if (ex.InnerException != null)
        return new GenericBusinessException(ex, ToGenericBusinessException(ex.InnerException));
      else
        return new GenericBusinessException(ex);
    }
  }
}