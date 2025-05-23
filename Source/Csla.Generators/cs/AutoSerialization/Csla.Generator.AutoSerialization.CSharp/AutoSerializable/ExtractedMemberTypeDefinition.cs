﻿//-----------------------------------------------------------------------
// <copyright file="ExtractedMemberTypeDefinition.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>The definition of a member's type, extracted from the syntax tree provided by Roslyn</summary>
//-----------------------------------------------------------------------

namespace Csla.Generator.AutoSerialization.CSharp.AutoSerialization
{

  /// <summary>
  /// The definition of a member's type, extracted from the syntax tree provided by Roslyn
  /// </summary>
  public class ExtractedMemberTypeDefinition
  {

    /// <summary>
    /// The name of the type
    /// </summary>
    public required string TypeName { get; init; }

    /// <summary>
    /// The namespace in which the type is defined
    /// </summary>
    public required string TypeNamespace { get; init; }

    /// <summary>
    /// Whether the type is marked as AutoSerializable
    /// </summary>
    public bool IsAutoSerializable { get; set; } = false;

    /// <summary>
    /// Whether the type implements the IMobileObject interface (directly or indirectly)
    /// </summary>
    public bool ImplementsIMobileObject { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether the type is nullable.
    /// </summary>
    public bool Nullable { get; internal set; }
  }
}
