using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Server;
using Csla.Data;

namespace InvDal
{
  public class ProductEdit : ObjectFactory
  {
    public InvLib.ProductEdit Create()
    {
      var item = new InvLib.ProductEdit();
      item.CategoryId = 3;
      CheckRules(item);
      return item;
    }
    
    public InvLib.ProductEdit Fetch(SingleCriteria<InvLib.ProductEdit, int> criteria)
    {
      var item = new InvLib.ProductEdit();
      using (BypassPropertyChecks(item))
      {
        var p = (from r in MockDb.ProductData where r.Id == criteria.Value select r).Single();
        LoadProperty(item, InvLib.ProductEdit.IdProperty, p.Id);
        item.Name = p.Name;
        item.Price = p.Price;
        item.CategoryId = p.CategoryId;
      }
      MarkOld(item);
      return item;
    }

    public InvLib.ProductEdit Update(InvLib.ProductEdit obj)
    {
      if (obj.IsNew)
      {
        var newId = (from r in MockDb.ProductData select r.Id).Max() + 1;
        LoadProperty(obj, InvLib.ProductEdit.IdProperty, newId);
        var p = new ProductData { Id = newId, Name = obj.Name, Price = obj.Price, CategoryId = obj.CategoryId };
        MockDb.ProductData.Add(p);
        MarkOld(obj);
      }
      else if (obj.IsDeleted)
      {
        Delete(new SingleCriteria<InvLib.ProductEdit,int>(obj.Id));
        MarkNew(obj);
      }
      else
      {
        var p = (from r in MockDb.ProductData where r.Id == obj.Id select r).Single();
        p.Name = obj.Name;
        p.Price = obj.Price;
        p.CategoryId = obj.CategoryId;
        MarkOld(obj);
      }
      return obj;
    }

    public void Delete(SingleCriteria<InvLib.ProductEdit, int> criteria)
    {
      var p = (from r in MockDb.ProductData where r.Id == criteria.Value select r).Single();
      MockDb.ProductData.Remove(p);

      var b = (from r in MockDb.BinData where r.ProductId == criteria.Value select r);
      foreach (var bin in b)
        MockDb.BinData.Remove(bin);

      var l = (from r in MockDb.OrderLineItemData where r.ProductId == criteria.Value select r);
      foreach (var line in l)
        MockDb.OrderLineItemData.Remove(line);
    }
  }
}
