//-----------------------------------------------------------------------
// <copyright file="SanitizedServerSideDataPortalException.cs" company="Marimer LLC">
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
  public class SanitizedServerSideDataPortalException : ApplicationException
  {

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="identifier">The unique identifier for this exception</param>
    public SanitizedServerSideDataPortalException(string identifier) : base(Properties.Resources.SanitizedServerSideDataPortalException)
    {
      this.Data.Add("identifier", identifier);
    }
  }
}
