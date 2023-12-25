//-----------------------------------------------------------------------
// <copyright file="HttpProxyOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Options for data binding</summary>
//-----------------------------------------------------------------------

namespace Csla.Configuration
{
  /// <summary>
  /// Options for data binding
  /// </summary>
  public class BindingOptions
  {
    /// <summary>
    /// Gets or sets a value specifying how CSLA .NET should
    /// raise PropertyChanged events.
    /// </summary>
    public ApplicationContext.PropertyChangedModes PropertyChangedMode { get; set; }
  }
}
