using System;

namespace Csla
{
  internal static class PortedExtensions
  {
    public static bool IsNullOrWhiteSpace(this string value)
    {
      return string.IsNullOrEmpty(value.Trim());
    }
  }
}
