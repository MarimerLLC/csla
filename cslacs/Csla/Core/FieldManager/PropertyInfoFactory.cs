using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Core.FieldManager
{
  /// <summary>
  /// Creates the factory object that
  /// creates PropertyInfo objects.
  /// </summary>
  public static class PropertyInfoFactory
  {
    private static Csla.Core.IPropertyInfoFactory _factory;

    /// <summary>
    /// Gets or sets the factory object that
    /// creates PropertyInfo objects.
    /// </summary>
    public static Csla.Core.IPropertyInfoFactory Factory
    {
      get
      {
        if (_factory == null)
        {
#if SILVERLIGHT
          _factory = new DefaultPropertyInfoFactory();
#else
          var typeName = System.Configuration.ConfigurationManager.AppSettings["CslaPropertyInfoFactory"];
          if (string.IsNullOrEmpty(typeName))
          {
            _factory = new DefaultPropertyInfoFactory();
          }
          else
          {
            var type = Type.GetType(typeName);
            _factory = (Csla.Core.IPropertyInfoFactory)Activator.CreateInstance(type);
          }
#endif
        }
        return _factory;
      }
      set
      {
        _factory = value;
      }
    }
  }
}
