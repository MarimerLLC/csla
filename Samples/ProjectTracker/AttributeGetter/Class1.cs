using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace AttributeGetter
{
  public static class DisplayGetter
  {
    public static string GetFriendlyNameFromAttributes(Type type, string name)
    {
      // If name is blank then check the DataAnnotations attribute and then the ComponentModel attribute.
      var propertyInfo = type.GetProperty(name);
      if (propertyInfo != null)
      {
        // DataAnnotations attribute.
        var display = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), true).OfType<DisplayAttribute>().FirstOrDefault();
        if (display != null)
          name = display.GetName();
      }
      return name;
    }
  }
}
