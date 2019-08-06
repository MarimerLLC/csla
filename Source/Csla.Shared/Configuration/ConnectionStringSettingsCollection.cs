#if NETSTANDARD2_0
//-----------------------------------------------------------------------
// <copyright file="ConnectionStringSettingsCollection.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Collection of connection strings.</summary>
//-----------------------------------------------------------------------
using System.Collections.Generic;

namespace Csla.Configuration
{
  /// <summary>
  /// Collection of connection strings.
  /// </summary>
  public class ConnectionStringSettingsCollection : Dictionary<string, ConnectionStringSettings>
  {
  }
}
#endif