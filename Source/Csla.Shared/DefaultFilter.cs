//-----------------------------------------------------------------------
// <copyright file="DefaultFilter.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla
{
  internal static class DefaultFilter
  {
    public static bool Filter(object item, object filter)
    {
      bool result = false;
      if (item != null && filter != null)
        result = item.ToString().Contains(filter.ToString());
      return result;
    }
  }
}