using System;
using System.Runtime.Serialization;

namespace Csla.Server.Hosts.WcfChannel
{
  /// <summary>
  /// Request message for deleting
  /// a business object.
  /// </summary>
  [DataContract]
  public class DeleteRequest
  {
    [DataMember]
    private object _criteria;
    [DataMember]
    private Csla.Server.DataPortalContext _context;

    /// <summary>
    /// Create new instance of object.
    /// </summary>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">Data portal context from client.</param>
    public DeleteRequest(object criteria, Csla.Server.DataPortalContext context)
    {
      _criteria = criteria;
      _context = context;
    }

    /// <summary>
    /// Criteria object describing business object.
    /// </summary>
    public object Criteria
    {
      get { return _criteria; }
      set { _criteria = value; }
    }

    /// <summary>
    /// Data portal context from client.
    /// </summary>
    public Csla.Server.DataPortalContext Context
    {
      get { return _context; }
      set { _context = value; }
    }
  }
}