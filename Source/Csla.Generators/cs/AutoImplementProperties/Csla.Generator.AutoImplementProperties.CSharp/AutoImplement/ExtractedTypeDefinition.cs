﻿//-----------------------------------------------------------------------
// <copyright file="ExtractedTypeDefinition.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>The definition of a type, extracted from the syntax tree provided by Roslyn</summary>
//-----------------------------------------------------------------------

namespace Csla.Generator.AutoImplementProperties.CSharp.AutoImplement
{

  /// <summary>
  /// The definition of a type, extracted from the syntax tree provided by Roslyn
  /// </summary>
  public class ExtractedTypeDefinition
  {

    /// <summary>
    /// The namespace in which the type resides
    /// </summary>
    public string Namespace { get; set; }

    /// <summary>
    /// The scope of the class
    /// </summary>
    public string Scope { get; set; } = "public";

    /// <summary>
    /// The name of the type, excluding any namespace
    /// </summary>
    public string TypeName { get; set; }

    /// <summary>
    /// The name of the kind of type being represented
    /// </summary>
    public string TypeKind { get; set; }

    /// <summary>
    /// The fully qualified name of the type, including namespace
    /// </summary>
    public string FullyQualifiedName { get; set; }

    /// <summary>
    /// The properties to be included in serialization
    /// </summary>
    public IList<ExtractedPropertyDefinition> Properties { get; private set; } = [];
    /// <summary>
    /// The name of the base class for the type
    /// </summary>
    public string BaseClassTypeName { get; internal set; }
  }

}
