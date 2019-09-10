using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Server;
using BusinessLibrary;

namespace DataAccess
{
  public class LineItemFactory : ObjectFactory
  {
    public LineItem Create()
    {
      var obj = (LineItem)Activator.CreateInstance(typeof(LineItem), true);
      MarkAsChild(obj);
      MarkNew(obj);
      CheckRules(obj);
      return obj;
    }
  }
}
