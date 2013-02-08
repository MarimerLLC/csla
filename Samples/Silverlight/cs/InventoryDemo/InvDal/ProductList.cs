using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Server;
using Csla.Data;

namespace InvDal
{
  public class ProductList : ObjectFactory
  {
    public InvLib.ProductList Fetch()
    {
      var item = new InvLib.ProductList();
      base.SetIsReadOnly(item, false);
      foreach (var p in MockDb.ProductData)
      {
        var qoh = (from b in MockDb.BinData 
                   where b.ProductId == p.Id 
                   select b.Quantity).Sum();
        item.Add(MakeChild(p.Id, p.Name, p.Price, qoh));
      }
      base.SetIsReadOnly(item, true);
      return item;
    }

    private InvLib.ProductInfo MakeChild(int id, string name, float price, int qoh)
    {
      var child = new InvLib.ProductInfo();
      LoadProperty(child, InvLib.ProductInfo.IdProperty, id);
      LoadProperty(child, InvLib.ProductInfo.NameProperty, name);
      LoadProperty(child, InvLib.ProductInfo.PriceProperty, price);
      LoadProperty(child, InvLib.ProductInfo.QohProperty, qoh);
      return child;
    }
  }
}
