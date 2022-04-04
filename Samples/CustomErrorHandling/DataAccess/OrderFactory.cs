using Csla;
using Csla.Server;
using BusinessLibrary;

namespace DataAccess
{
  public class OrderFactory : ObjectFactory
  {
    public OrderFactory(ApplicationContext applicationContext)
      : base(applicationContext) { }

    private static int _lastId;

    public Order Create()
    {
      var obj = ApplicationContext.CreateInstanceDI<Order>();
      var id = System.Threading.Interlocked.Decrement(ref _lastId);
      LoadProperty(obj, Order.IdProperty, id);
      MarkNew(obj);
      CheckRules(obj);
      return obj;
    }

    public Order Update(Order obj)
    {
      if (obj.IsDeleted)
      {
        if (!obj.IsNew)
        {
          // delete data
        }
        MarkNew(obj);
      }
      else
      {
        if (obj.IsNew)
        {
          // insert data
          LoadProperty(obj, Order.IdProperty, System.Math.Abs(obj.Id));
        }
        else
        {
          // update data
        }
        MarkOld(obj);
      }
      return obj;
    }

    public void Delete(int criteria)
    {
      if (criteria == 202)
        throw new ServerOnlyException("This Exception is only available on the Server side");
      else
        throw new MyNonSerializableException("This exception is not serializable");
    }
  }
}
