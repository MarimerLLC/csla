//-----------------------------------------------------------------------
// <copyright file="ExtractedMemberTypeDefinition.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>The definition of a member's type, extracted from the syntax tree provided by Roslyn</summary>
//-----------------------------------------------------------------------

namespace Csla.Generator.AutoImplementProperties.CSharp.AutoImplement
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
    /// Gets or sets a value indicating whether the type is nullable.
    /// </summary>
    public bool Nullable { get; internal set; }
  }
}
