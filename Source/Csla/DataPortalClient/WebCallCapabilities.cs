//-----------------------------------------------------------------------
// <copyright file="HttpProxy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Exposes platform-specific web call capabilities</summary>
//-----------------------------------------------------------------------
using System;
using System.Runtime.InteropServices;

namespace Csla.DataPortalClient
{

  /// <summary>
  /// Determines capabilities of web calls in the current runtime environment
  /// </summary>
  internal static class WebCallCapabilities
  {

    /// <summary>
    /// Method to determine if the runtime supports synchronous WebClient usage
    /// WebAssembly specifically disallows use of this synchronous class
    /// </summary>
    /// <returns>Boolean true if synchronous data access is enabled, otherwise false</returns>
    public static bool AreSyncWebClientMethodsSupported()
    {
      bool isWebAssembly;

#if NETFRAMEWORK
      isWebAssembly = false;
#elif NETSTANDARD
      // Use textual OSDescription to identify WebAssembly on .NET Standard
      string osDescription = RuntimeInformation.OSDescription;
      isWebAssembly = osDescription.Equals("web", StringComparison.InvariantCultureIgnoreCase);
#else
      isWebAssembly = RuntimeInformation.OSArchitecture == Architecture.Wasm;
#endif

      return !isWebAssembly;
    }

  }
}
