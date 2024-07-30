//-----------------------------------------------------------------------
// <copyright file="NestingPOCO.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Class that can be used for testing auto implementation of properties</summary>
//-----------------------------------------------------------------------
using Csla.Serialization;

namespace Csla.Generator.AutoImplementProperties.CSharp.TestObjects
{


  /// <summary>
  /// Class that can be used for testing auto implementation of properties
  /// </summary>
  [CslaImplementProperties]
  public partial class ReadOnlyPOCO : ReadOnlyBase<ReadOnlyPOCO>
  {

    /// <summary>
    /// Gets the name.
    /// </summary>
    public partial string Name { get; private set; }

  }
}
