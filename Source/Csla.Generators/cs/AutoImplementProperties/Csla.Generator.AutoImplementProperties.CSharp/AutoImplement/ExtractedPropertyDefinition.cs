﻿//-----------------------------------------------------------------------
// <copyright file="ExtractedPropertyDefinition.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>The definition of a property, extracted from the syntax tree provided by Roslyn</summary>
//-----------------------------------------------------------------------

namespace Csla.Generator.AutoImplementProperties.CSharp.AutoImplement
{

  /// <summary>
  /// The definition of a property, extracted from the syntax tree provided by Roslyn
  /// </summary>
  public class ExtractedPropertyDefinition : IMemberDefinition
  {

    /// <summary>
    /// The name of the property
    /// </summary>
    public string PropertyName { get; set; }

    /// <summary>
    /// The definition of the type of this property
    /// </summary>
    public ExtractedMemberTypeDefinition TypeDefinition { get; } = new ExtractedMemberTypeDefinition();

    /// <summary>
    /// The member name for the field
    /// </summary>
    string IMemberDefinition.MemberName => PropertyName;

    /// <summary>
    /// The attribute definitions for this property
    /// </summary>
    public List<ExtractedAttributeDefinition> AttributeDefinitions { get; } = [];

    /// <summary>
    /// Gets or sets a value indicating whether this property has a getter.
    /// </summary>
    public bool Getter { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this property has a setter.
    /// </summary>
    public bool Setter { get; set; }

    /// <summary>
    /// The modifiers for this property
    /// </summary>
    public string[] Modifiers { get; internal set; }

    /// <summary>
    /// Gets or sets a value indicating whether this property is partial.
    /// </summary>
    public bool Partial { get; internal set; }

    /// <summary>
    /// The modifiers for the setter of this property
    /// </summary>
    public string[] SetterModifiers { get; internal set; }
  }

}
