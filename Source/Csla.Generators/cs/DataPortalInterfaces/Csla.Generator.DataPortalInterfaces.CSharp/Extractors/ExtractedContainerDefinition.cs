//-----------------------------------------------------------------------
// <copyright file="ExtractedContainerDefinition.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>The definition of a container of a type</summary>
//-----------------------------------------------------------------------

namespace Csla.Generator.DataPortalInterfaces.CSharp.Extractors
{

  /// <summary>
  /// The definition of a container of a type, extracted from the syntax tree provided by Roslyn
  /// </summary>
  public class ExtractedContainerDefinition
  {

    /// <summary>
    /// The name of the container
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The full definition of the container for use in source generation
    /// </summary>
    public string FullDefinition { get; set; } = string.Empty;
  }
}
