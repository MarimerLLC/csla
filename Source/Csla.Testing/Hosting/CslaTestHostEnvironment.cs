//-----------------------------------------------------------------------
// <copyright file="CslaTestHostEnvironment.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Minimal IHostEnvironment implementation for use in unit tests</summary>
//-----------------------------------------------------------------------

using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace Csla.Testing
{
  /// <summary>
  /// Minimal <see cref="IHostEnvironment"/> implementation so types that take a
  /// dependency on the hosting environment can be resolved in a unit test.
  /// </summary>
  /// <remarks>
  /// Several CSLA .NET configuration paths expect an <see cref="IHostEnvironment"/>
  /// to be available. Registering this type satisfies them without requiring a
  /// test to stand up a real host.
  /// </remarks>
  public sealed class CslaTestHostEnvironment : IHostEnvironment
  {
    /// <inheritdoc />
    public string EnvironmentName { get; set; } = "Production";

    /// <inheritdoc />
    public string ApplicationName { get; set; } = "Csla.Testing";

    /// <inheritdoc />
    public string ContentRootPath { get; set; } = Path.GetTempPath();

    /// <inheritdoc />
    public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
  }
}
