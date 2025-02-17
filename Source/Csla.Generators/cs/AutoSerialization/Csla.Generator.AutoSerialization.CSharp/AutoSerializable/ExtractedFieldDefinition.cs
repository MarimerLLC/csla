//-----------------------------------------------------------------------
// <copyright file="ExtractedFieldDefinition.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>The definition of a field, extracted from the syntax tree provided by Roslyn</summary>
//-----------------------------------------------------------------------

namespace Csla.Generator.AutoSerialization.CSharp.AutoSerialization
{

  /// <summary>
  /// The definition of a field, extracted from the syntax tree provided by Roslyn
  /// </summary>
  public class ExtractedFieldDefinition : IMemberDefinition
  {

    /// <summary>
    /// The name of the field
    /// </summary>
    public string FieldName { get; set; }

    /// <summary>
    /// The definition of the type of this field
    /// </summary>
    public ExtractedMemberTypeDefinition TypeDefinition { get; } = new();

    /// <summary>
    /// The member name for the field
    /// </summary>
    string IMemberDefinition.MemberName => FieldName;

  }

}
