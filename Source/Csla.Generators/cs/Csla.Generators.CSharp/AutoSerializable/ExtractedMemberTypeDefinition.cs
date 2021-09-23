//-----------------------------------------------------------------------
// <copyright file="ExtractedMemberTypeDefinition.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>The definition of a member's type, extracted from the syntax tree provided by Roslyn</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

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
    /// Whether the type is marked as AutoSerialiable
    /// </summary>
    public bool IsAutoSerializable { get; set; } = false;

    /// <summary>
    /// Whether the type implements the IMobileObject interface (directly or indirectly)
    /// </summary>
    public bool ImplementsIMobileObject { get; set; } = false;

  }
}
