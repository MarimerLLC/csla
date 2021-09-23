//-----------------------------------------------------------------------
// <copyright file="ExtractedContainerDefinition.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>The definition of a container of a type, extracted from the syntax tree provided by Roslyn</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Generators.AutoSerialization
{

  /// <summary>
  /// The definition of a container of a type, extracted from the syntax tree provided by Roslyn
  /// </summary>
  public class ExtractedContainerDefinition
  {

    /// <summary>
    /// The name of the container, such as the class name or namespace name
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The full definition of the container for use in source generation
    /// </summary>
    public string FullDefinition { get; set; }

  }
}
