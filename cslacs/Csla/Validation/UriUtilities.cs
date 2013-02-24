using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Validation
{
  /// <summary>
  /// This class formats the Uri string (host name) in the same manner as Csla4 does. 
  /// </summary>
  internal static class UriUtilities
  {
    /// <summary>
    /// Formats the name of the host.
    /// 
    /// Invalid characters are removed and the uri string is split into chunks of 63 characters separated by '/'
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>Formatted host string</returns>
    internal static string FormatHostName(Type type)
    {
      string typeName = type.FullName;
      //if (type.IsGenericType)
      //{
      //  var pos = typeName.IndexOf('[');
      //  if (pos > 0)
      //  {
      //    typeName = typeName.Substring(0, pos -1);
      //  }
      //}

      var hostName = EncodeString(typeName);
      if (hostName.Length > 63)
      {
        var tmp = hostName;
        hostName = null;
        for (var i = 0; i < tmp.Length - 1; i = i + 63)
          hostName = hostName + tmp.Substring(i, ((i + 63 <= tmp.Length) ? 63 : tmp.Length - i)) + "/";
        hostName = hostName.Substring(0, hostName.Length - 1);
      }

      return hostName;
    }

    /// <summary>
    /// Remove invalid charaters from the string value, calls Uri.EscapeUriString and then replaces '%' with '-'
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns>Formatted string</returns>
    internal static string EncodeString(string value)
    {
      var result = value;
      result = result.Replace("+", "-");
      result = result.Replace(" ", "");
      result = result.Replace("[", "");
      result = result.Replace("]", "");
      result = result.Replace("`", "-");
      result = result.Replace(",", "-");
      result = result.Replace("=", "-");
      result = System.Uri.EscapeUriString(result);
      result = result.Replace("%", "-");
      return result;
    }
  }
}
