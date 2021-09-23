//-----------------------------------------------------------------------
// <copyright file="IMemberDefinition.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>The contract which a member definition must fulfil</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Generators.AutoSerialization
{

  /// <summary>
  /// The contract which a member definition must fulfil
  /// </summary>
  public interface IMemberDefinition
  {

    /// <summary>
    /// The name of the member
    /// </summary>
    string MemberName { get; }

    /// <summary>
    /// The type definition of the member
    /// </summary>
    ExtractedMemberTypeDefinition TypeDefinition { get; }

  }

}
