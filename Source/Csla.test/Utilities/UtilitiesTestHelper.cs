//-----------------------------------------------------------------------
// <copyright file="UtilitiesTestHelper.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Test.Utilities
{
  public class UtilitiesTestHelper
  {
    public const string ToStringValue = "UtilitiesTestHelper.ToString";

    public string StringProperty { get; set; }

    public string NullableStringProperty { get; set; }

    public int IntProperty { get; set; }

    public override string ToString()
    {
      return ToStringValue;
    }
  }
}