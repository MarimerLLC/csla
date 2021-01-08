//-----------------------------------------------------------------------
// <copyright file="MobileResponse.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Object that encompasses the resut of the request from </summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Core;

namespace Csla.Server.Hosts.Mobile
{
  /// <summary>
  /// Object that encompasses the resut of the request from 
  /// a client
  /// </summary>
  public class MobileResponse
  {
    /// <summary>
    /// Exception that occurred during portal execution
    /// Null if no exception occurred
    /// </summary>
    public Exception Error { get; set; }
    /// <summary>
    /// Global context object.
    /// </summary>
    public ContextDictionary GlobalContext { get; set; }
    /// <summary>
    /// Result of the request
    /// </summary>
    public object Object { get; set; }
  }
}