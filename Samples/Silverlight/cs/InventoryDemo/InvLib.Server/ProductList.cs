using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace InvLib
{
  [Serializable]
  public partial class ProductList : ReadOnlyListBase<ProductList, ProductInfo>
  {
    #region Factory Methods

    private static ProductList _cache = null;

    public static void ClearCache()
    {
      _cache = null;
    }

    public static void GetProductList(EventHandler<DataPortalResult<ProductList>> callback)
    {
      if (_cache == null)
      {
        var dp = new DataPortal<ProductList>();
        dp.FetchCompleted += (o, e1) =>
        {
          _cache = e1.Object;
          callback(o, e1);
        };
        dp.BeginFetch();
      }
      else
      {
        callback(null, new DataPortalResult<ProductList>(_cache, null, null));
      }
    }

    #endregion
  }
}
