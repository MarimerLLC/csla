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
    private string _versionRoutingTag = string.Empty;

    /// <summary>
    /// Creates an instance of the type
    /// </summary>
    /// <param name="cslaOptions">CslaOptions object</param>
    /// <exception cref="ArgumentNullException"><paramref name="cslaOptions"/> is <see langword="null"/>.</exception>
    public DataPortalOptions(CslaOptions cslaOptions)
    {
      CslaOptions = cslaOptions ?? throw new ArgumentNullException(nameof(cslaOptions));
      DataPortalClientOptions = new DataPortalClientOptions(this);
      DataPortalServerOptions = new DataPortalServerOptions(CslaOptions.Services);
    }

    /// <summary>
    /// Gets or sets a value representing the application version
    /// for use in server-side data portal routing.
    /// </summary>
    /// <remarks>
    /// Application version used to create data portal
    /// routing tag (can not contain '-' or '/').
    /// If this value is set then you must use the
    /// .NET Core server-side data portal endpoint
    /// as a router so the request can be routed to
    /// another app server that is running the correct
    /// version of the application's assemblies.
    /// </remarks>
    public string VersionRoutingTag
    {
      get => _versionRoutingTag;
      set
      {
        if (!string.IsNullOrWhiteSpace(value))
          if (value!.Contains("-") || value.Contains("/"))
            throw new ArgumentException("Version routing tag value cannot contain '-' or '/' characters", nameof(value));
        _versionRoutingTag = value;
      }
    }

    /// <summary>
    /// Gets the data portal client options
    /// </summary>
    internal DataPortalClientOptions DataPortalClientOptions { get; }
    /// <summary>
    /// Gets the data portal server options
    /// </summary>
    internal DataPortalServerOptions DataPortalServerOptions { get; }

    internal CslaOptions CslaOptions { get; }
  }
}
