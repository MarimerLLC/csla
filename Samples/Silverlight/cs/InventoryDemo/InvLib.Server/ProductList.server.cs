using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace InvLib
{
  [Csla.Server.ObjectFactory("InvDal.ProductList,InvDal")]
  public partial class ProductList
  {
    #region Factory Methods

    public static ProductList GetProductList()
    {
      if (_cache == null)
        return DataPortal.Fetch<ProductList>();
      else
        return _cache;
    }

    #endregion
  }
}
