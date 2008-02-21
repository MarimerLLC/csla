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
