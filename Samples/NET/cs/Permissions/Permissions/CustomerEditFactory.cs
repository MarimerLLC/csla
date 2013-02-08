using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestApp
{
  public class CustomerEditFactory : Csla.Server.ObjectFactory
  {
    public CustomerEdit Create()
    {
      var obj = (CustomerEdit)Activator.CreateInstance(typeof(CustomerEdit), true);
      MarkNew(obj);
      CheckRules(obj);
      return obj;
    }

    public CustomerEdit Fetch(Csla.SingleCriteria<CustomerEdit, int> criteria)
    {
      var obj = (CustomerEdit)Activator.CreateInstance(typeof(CustomerEdit), true);
      using (BypassPropertyChecks(obj))
      {
        LoadProperty(obj, CustomerEdit.IdProperty, criteria.Value);
        obj.Name = "Rocky";
        obj.City = "Eden Prairie";
      }
      MarkOld(obj);
      CheckRules(obj);
      return obj;
    }

    public CustomerEdit Update(CustomerEdit obj)
    {
      MarkOld(obj);
      return obj;
    }

    public void Delete(CustomerEdit obj)
    {
    }
  }
}
