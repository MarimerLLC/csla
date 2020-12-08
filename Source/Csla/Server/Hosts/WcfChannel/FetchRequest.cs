//-----------------------------------------------------------------------
// <copyright file="FetchRequest.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Request message for retrieving</summary>
//-----------------------------------------------------------------------
using System;
using System.Runtime.Serialization;

namespace Csla.Server.Hosts.WcfChannel
{
  /// <summary>
  /// Request message for retrieving
  /// an existing business object.
  /// </summary>
  [DataContract]
  public class FetchRequest
  {
    [DataMember]
    private Type _objectType;
    [DataMember]
    private object _criteria;
    [DataMember]
    private Csla.Server.DataPortalContext _context;

    /// <summary>
    /// Create new instance of object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">Data portal context from client.</param>
    public FetchRequest(Type objectType, object criteria, Csla.Server.DataPortalContext context)
    {
      _objectType = objectType;
      _criteria = criteria;
      _context = context;
    }

    /// <summary>
    /// The type of the business object
    /// to be retrieved.
    /// </summary>
    public Type ObjectType
    {
      get { return _objectType; }
      set { _objectType = value; }
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