//-----------------------------------------------------------------------
// <copyright file="ValueComparer.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Base class for an object that is serializable</summary>
//-----------------------------------------------------------------------

namespace Csla.Core
{
  internal static class ValueComparer
  {
    internal static bool AreNotEqual(object? value1, object? value2)
    {
      bool valuesDiffer;
      
      if (value1 == null)
        valuesDiffer = value2 != null;
      else
        valuesDiffer = !value1.Equals(value2);

      return valuesDiffer;
    }
  }
}