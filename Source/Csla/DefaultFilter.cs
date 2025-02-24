//-----------------------------------------------------------------------
// <copyright file="DefaultFilter.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla
{
  internal static class DefaultFilter
  {
    public static bool Filter(object? item, object? filter)
    {
      bool result = false;
      if (item?.ToString() is string itemStr && filter?.ToString() is string filterStr)
        result = itemStr.Contains(filterStr);
      return result;
    }
  }
}