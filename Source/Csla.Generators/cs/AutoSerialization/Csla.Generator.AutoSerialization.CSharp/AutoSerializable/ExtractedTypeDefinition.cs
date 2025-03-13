//-----------------------------------------------------------------------
// <copyright file="ExtractedTypeDefinition.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>The definition of a type, extracted from the syntax tree provided by Roslyn</summary>
//-----------------------------------------------------------------------

namespace Csla.Generator.AutoSerialization.CSharp.AutoSerialization
{

  /// <summary>
  /// The definition of a type, extracted from the syntax tree provided by Roslyn
  /// </summary>
  public class ExtractedTypeDefinition
  {

    /// <summary>
    /// The namespace in which the type resides
    /// </summary>
    public required string Namespace { get; init; }

    /// <summary>
    /// The scope of the class
    /// </summary>
    public string Scope { get; set; } = "public";

    /// <summary>
    /// The name of the type, excluding any namespace
    /// </summary>
    public required string TypeName { get; init; }

    /// <summary>
    /// The name of the kind of type being represented
    /// </summary>
    public required string TypeKind { get; init; }

    /// <summary>
    /// The fully qualified name of the type, including namespace
    /// </summary>
    public string FullyQualifiedName { get; set; } = string.Empty;

    /// <summary>
    /// The container definitions for this type
    /// </summary>
    public IList<ExtractedContainerDefinition> ContainerDefinitions { get; private set; } = [];

    /// <summary>
    /// The properties to be included in serialization
    /// </summary>
    public IList<ExtractedPropertyDefinition> Properties { get; private set; } = [];

    /// <summary>
    /// The fields to be included in serialization
    /// </summary>
    public IList<ExtractedFieldDefinition> Fields { get; private set; } = [];

  }

}
