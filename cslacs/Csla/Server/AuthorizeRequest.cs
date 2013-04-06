using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Server
{
  /// <summary>
  /// Object containing information about the
  /// client request to the data portal.
  /// </summary>
  public class AuthorizeRequest
  {
    /// <summary>
    /// Gets the type of business object affected by
    /// the client request.
    /// </summary>
    public Type ObjectType { get; private set; }
    /// <summary>
    /// Gets a reference to the criteria or 
    /// business object passed from
    /// the client to the server.
    /// </summary>
    public object RequestObject { get; private set; }
    /// <summary>
    /// Gets the data portal operation requested
    /// by the client.
    /// </summary>
    public DataPortalOperations Operation { get; private set; }

    internal AuthorizeRequest(Type objectType, object requestObject, DataPortalOperations operation)
    {
      this.ObjectType = objectType;
      this.RequestObject = requestObject;
      this.Operation = operation;
    }
  }
}
