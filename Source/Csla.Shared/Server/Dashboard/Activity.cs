using System;

namespace Csla.Server.Dashboard
{
  public class Activity
  {
    public Activity(InterceptArgs result)
    {
      ObjectType = result.ObjectType;
      Operation = result.Operation;
      Runtime = result.Runtime;
      Exception = result.Exception;
    }

    public Type ObjectType { get; private set; }
    public DataPortalOperations Operation { get; private set; }
    public TimeSpan Runtime { get; private set; }
    public Exception Exception { get; private set; }
  }
}
