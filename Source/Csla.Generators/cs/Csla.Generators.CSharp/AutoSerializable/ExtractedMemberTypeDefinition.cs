//-----------------------------------------------------------------------
// <copyright file="ExtractedMemberTypeDefinition.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>The definition of a member's type, extracted from the syntax tree provided by Roslyn</summary>
//-----------------------------------------------------------------------

namespace Csla.Generators.CSharp.AutoSerialization
{

  /// <summary>
  /// The definition of a member's type, extracted from the syntax tree provided by Roslyn
  /// </summary>
  public class ExtractedMemberTypeDefinition
  {

    /// <summary>
    /// The name of the type
    /// </summary>
    public string TypeName { get; set; }

    /// <summary>
    /// The namespace in which the type is defined
    /// </summary>
    public string TypeNamespace { get; set; }

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
