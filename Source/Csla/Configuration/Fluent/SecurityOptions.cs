//-----------------------------------------------------------------------
// <copyright file="SecurityOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Use this type to configure the settings for security</summary>
//-----------------------------------------------------------------------
namespace Csla.Configuration
{
  /// <summary>
  /// Use this type to configure the settings for security.
  /// </summary>
  public class SecurityOptions
  {
    /// <summary>
    /// Sets the max size of the PrincipalCache cache.
    /// </summary>
    public int PrincipalCacheMaxCacheSize { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether we 
    /// are to flow User Principal to the server
    /// </summary>
    /// <remarks>
    /// This should generally be left at the default of false. Values on 
    /// the client can be manipulated, and therefore allowing the principal 
    /// to flow from client to server could result in an exploitable security 
    /// weakness, including impersonation or elevation of privileges.
    /// </remarks>
    public bool FlowSecurityPrincipalFromClient { get; set; } = false;

    /// <summary>
    /// Gets or sets the authentication type being used by the
    /// CSLA .NET framework.
    /// </summary>
    /// <value></value>
    public string AuthenticationType { get; set; } = "Csla";
  }
}
