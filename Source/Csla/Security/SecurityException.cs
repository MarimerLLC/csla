using System;
using System.Collections.Generic;
using System.Linq;
#if !(ANDROID || IOS) && !NETFX_CORE
using System.Security.Permissions;
#endif
using System.Text;
using System.Threading.Tasks;

namespace Csla.Security
{
  /// <summary>
  /// Security exception.
  /// </summary>
  [Serializable]
  public class SecurityException : Exception
  {
    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    public SecurityException()
    { }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="message">Exception text.</param>
    public SecurityException(string message)
      : base(message)
    { }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    /// <param name="message">Exception text.</param>
    /// <param name="innerException">Inner exception.</param>
    public SecurityException(string message, Exception innerException)
      : base(message, innerException)
    { }

#if !(ANDROID || IOS) && !NETFX_CORE
    /// <summary>
    /// Creates an instance of the object for serialization.
    /// </summary>
    /// <param name="info">Serialiation info object.</param>
    /// <param name="context">Serialization context object.</param>
    protected SecurityException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
      : base(info, context)
    {
    }

    /// <summary>
    /// Serializes the object.
    /// </summary>
    /// <param name="info">Serialiation info object.</param>
    /// <param name="context">Serialization context object.</param>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods")]
#if !NET5_0
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
    [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.SerializationFormatter)]
#endif
    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
      base.GetObjectData(info, context);
    }
#endif
  }
}

