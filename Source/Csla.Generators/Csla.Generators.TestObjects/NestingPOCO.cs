//-----------------------------------------------------------------------
// <copyright file="NestingPOCO.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Class that can be used for testing serialization behaviour related to nested classes</summary>
//-----------------------------------------------------------------------
using Csla.Serialization;
using System;
using System.Collections.Generic;

namespace Csla.Generators.TestObjects
{

  /// <summary>
  /// A class including a private nested class for which automatic serialization code is to be generated
  /// </summary>
  /// <remarks>The class is decorated with the AutoSerializable attribute so that it is picked up by our source generator</remarks>
  [AutoSerializable]
  public partial class NestingPOCO
  {

    [AutoSerialized]
    private NestedPOCO _poco = new NestedPOCO() { Value = "Hello" };

    [AutoSerializable]
    protected internal partial class NestedPOCO
    {

      public string Value { get; set; }

    }

    public string GetValue()
    {
      return _poco.Value;
    }

    public void SetValue(string value)
    {
      _poco.Value = value;
    }

  }
}
