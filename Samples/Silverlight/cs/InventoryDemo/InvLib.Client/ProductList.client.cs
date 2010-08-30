using System;
using Csla;

namespace InvLib
{
  public partial class ProductList
  {
    #region Factory Methods

    //public static void GetProductList(EventHandler<DataPortalResult<ProductList>> callback)
    //{
    //  if (_cache == null)
    //  {
    //    var dp = new DataPortal<ProductList>();
    //    dp.FetchCompleted += (o, e1) =>
    //    {
    //      _cache = e1.Object;
    //      callback(o, e1);
    //    };
    //    dp.BeginFetch();
    //  }
    //  else
    //  {
    //    callback(null, new DataPortalResult<ProductList>(_cache, null, null));
    //  }
    //}

    #endregion
  }
}
