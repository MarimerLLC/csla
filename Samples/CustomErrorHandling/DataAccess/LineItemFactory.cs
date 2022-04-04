using Csla;
using Csla.Server;
using BusinessLibrary;

namespace DataAccess
{
  public class LineItemFactory : ObjectFactory
  {
    public LineItemFactory(ApplicationContext applicationContext)
      : base(applicationContext) { }

    public LineItem Create()
    {
      var obj = ApplicationContext.CreateInstanceDI<LineItem>();
      MarkAsChild(obj);
      MarkNew(obj);
      CheckRules(obj);
      return obj;
    }
  }
}
