//-----------------------------------------------------------------------
// <copyright file="DataPortalClientOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Data portal options.</summary>
//-----------------------------------------------------------------------
namespace Csla.Configuration
{
  /// <summary>
  /// Data portal options.
  /// </summary>
  public class DataPortalOptions
  {
    /// <summary>
    /// Creates an instance of the type
    /// </summary>
    /// <param name="cslaOptions">CslaOptions object</param>
    public DataPortalOptions(CslaOptions cslaOptions)
    {
      CslaOptions = cslaOptions;
      DataPortalClientOptions = new DataPortalClientOptions(this);
      DataPortalServerOptions = new DataPortalServerOptions();
    }

    internal CslaOptions CslaOptions { get; set; }

    /// <summary>
    /// Gets or sets a value representing the application version
    /// for use in server-side data portal routing.
    /// </summary>
    /// <remarks>
    /// Application version used to create data portal
    /// routing tag (can not contain '-').
    /// If this value is set then you must use the
    /// .NET Core server-side data portal endpoint
    /// as a router so the request can be routed to
    /// another app server that is running the correct
    /// version of the application's assemblies.
    /// </remarks>
    public string VersionRoutingTag { get; set; }

    /// <summary>
    /// Gets or sets the data portal client options
    /// </summary>
    internal DataPortalClientOptions DataPortalClientOptions { get; set; }
    /// <summary>
    /// Gets or sets the data portal server options
    /// </summary>
    internal DataPortalServerOptions DataPortalServerOptions { get; set; }
  }
}
