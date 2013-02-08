using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Objects;
using Csla.Core;
using Csla;

namespace Rolodex.Business.Data
{
  public static class EntityExtensions
  {
    public static EntityKey GetEntityKey(this ObjectContext context, Type entityType, string propertyName, object value)
    {
      return new EntityKey(context.GetType().Name + "." + entityType.Name, propertyName, value);
    }

    public static EntityKey GetEntityKey(this ObjectContext context, Type entityType, IPropertyInfo propertyInfo, object value)
    {
      return new EntityKey(context.GetType().Name + "." + entityType.Name, propertyInfo.Name, value);
    }
  }
}
