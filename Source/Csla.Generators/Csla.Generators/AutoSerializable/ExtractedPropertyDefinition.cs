//-----------------------------------------------------------------------
// <copyright file="ExtractedPropertyDefinition.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>The definition of a property, extracted from the syntax tree provided by Roslyn</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Generators.AutoSerialization
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

  }

}
