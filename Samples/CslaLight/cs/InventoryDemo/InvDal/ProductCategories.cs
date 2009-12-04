using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Server;
using Csla.Data;

namespace InvDal
{
  public class ProductCategories : ObjectFactory
  {
    public InvLib.ProductCategories Fetch()
    {
      var item = new InvLib.ProductCategories();
      base.SetIsReadOnly(item, false);
      foreach (var p in MockDb.ProductCategoryData)
        item.AddItem(p.Id, p.Name);
      base.SetIsReadOnly(item, true);
      return item;
    }
  }
}
