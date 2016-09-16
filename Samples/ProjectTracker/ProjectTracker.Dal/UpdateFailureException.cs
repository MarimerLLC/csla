using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Serialization;
using System.Security.Permissions;

namespace ProjectTracker.Dal
{
  [Serializable]
  public class UpdateFailureException : Exception
  {
    public UpdateFailureException(string message)
      : base(message)
    { }

    public UpdateFailureException(string message, Exception innerException)
      : base(message, innerException)
    { }

    protected UpdateFailureException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context)
      : base(info, context)
    { }

    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
    public override void GetObjectData(
      System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      base.GetObjectData(info, context);
    }
  }
}
