//-----------------------------------------------------------------------
// <copyright file="NestingPOCO.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Class that can be used for testing serialization behaviour related to nested classes</summary>
//-----------------------------------------------------------------------
using Csla.Serialization;

namespace Csla.Generator.AutoImplementProperties.CSharp.TestObjects
{

  /// <summary>
  /// A class including a private nested class for which automatic serialization code is to be generated
  /// </summary>
  /// <remarks>The class is decorated with the AutoSerializable attribute so that it is picked up by our source generator</remarks>
  [CslaImplementProperties]
  public partial class ReadOnlyPOCO : ReadOnlyBase<ReadOnlyPOCO>
  {

    public partial string Name { get; private set; }

  }
}
