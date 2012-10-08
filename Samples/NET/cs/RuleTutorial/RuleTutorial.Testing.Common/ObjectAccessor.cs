using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Core;
using Csla.Server;

namespace RuleTutorial.Testing.Common
{
  public class ObjectAccessor : ObjectFactory
  {
    public ObjectAccessor()
    { }

    public object ReadProperty(object businessObject, IPropertyInfo property)
    {
      return base.ReadProperty(businessObject, property);
    }
  }
}
