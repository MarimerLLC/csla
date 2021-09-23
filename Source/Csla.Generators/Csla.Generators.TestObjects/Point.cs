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
  internal partial struct Point
  {

    public int X { get; set; }

    public int Y { get; set; }

  }
}
