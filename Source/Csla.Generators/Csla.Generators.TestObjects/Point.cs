//-----------------------------------------------------------------------
// <copyright file="Point.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Struct that can be used for testing serialization behaviour</summary>
//-----------------------------------------------------------------------
using Csla.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Generators.TestObjects
{

  /// <summary>
  /// Struct that can be used for testing serialization behaviour
  /// </summary>
  [AutoSerializable]
  public partial struct Point
  {

    public int X { get; set; }

    public int Y { get; set; }

  }
}
