//-----------------------------------------------------------------------
// <copyright file="ServerException.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Sanitized exception for server-side data portal operations.</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Server
{
  /// <summary>
  /// Sanitized server-side data portal exception; used to 
  /// avoid the transmission of sensitive server-side information
  /// to the client in remote data portal operations
  /// </summary>
  public class ServerException : Exception
  {

    private const string IdentifierKey = "identifier";

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="identifier">The unique identifier for this exception</param>
    public ServerException(string identifier) : base(Properties.Resources.ServerSideDataPortalException)
    {
      Data.Add(IdentifierKey, identifier);
    }

    /// <summary>
    /// The unique identifier for the request that failed
    /// Use this identifier to look for the exception details in server logs
    /// </summary>
    public string RequestIdentifier
    {
      get
      {
        return Data[IdentifierKey].ToString();
      }
    }

    /// <summary>
    /// Override of the ToString method to insert the unique identifier
    /// </summary>
    /// <returns>String representation of the custom exception, including the custom tokens</returns>
    public override string ToString()
    {
      return string.Format(Properties.Resources.SanitizedServerSideDataPortalDetailedException, RequestIdentifier);
    }
  }
}