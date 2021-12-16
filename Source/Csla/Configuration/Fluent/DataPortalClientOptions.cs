//-----------------------------------------------------------------------
// <copyright file="DataPortalClientOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Client-side data portal options.</summary>
//-----------------------------------------------------------------------

namespace Csla.Configuration
{
  /// <summary>
  /// Client-side data portal options.
  /// </summary>
  public class DataPortalClientOptions
  {
    /// <summary>
    /// Gets or sets a value indicating whether objects should be
    /// automatically cloned by the data portal Update()
    /// method when using a local data portal configuration.
    /// </summary>
    public bool AutoCloneOnUpdate
    {
      get
      {
        bool result = true;
        string setting = ConfigurationManager.AppSettings["CslaAutoCloneOnUpdate"];
        if (!string.IsNullOrEmpty(setting))
          result = bool.Parse(setting);
        return result;
      }
      set
      {
        ConfigurationManager.AppSettings["CslaAutoCloneOnUpdate"] = value.ToString();
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the
    /// server-side business object should be returned to
    /// the client as part of the DataPortalException.
    /// </summary>
    /// <param name="value">Value (default is false)</param>
    public bool DataPortalReturnObjectOnException
    {
      get
      {
        bool result = false;
        string setting = ConfigurationManager.AppSettings["CslaDataPortalReturnObjectOnException"];
        if (!string.IsNullOrEmpty(setting))
          result = bool.Parse(setting);
        return result;
      }
      set
      {
        ConfigurationManager.AppSettings["CslaDataPortalReturnObjectOnException"] = value.ToString();
      }
    }
  }
}
