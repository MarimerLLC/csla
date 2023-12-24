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
    /// Gets or sets the data portal client options
    /// </summary>
    internal DataPortalClientOptions DataPortalClientOptions { get; set; }
    /// <summary>
    /// Gets or sets the data portal server options
    /// </summary>
    internal DataPortalServerOptions DataPortalServerOptions { get; set; }
  }
}
