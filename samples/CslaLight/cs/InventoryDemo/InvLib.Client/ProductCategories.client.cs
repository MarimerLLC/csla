using System;
using Csla;

namespace InvLib
{
  public partial class ProductCategories
  {
    #region Factory Methods
    
    public static void GetProductCategories(EventHandler<DataPortalResult<ProductCategories>> callback)
    {
      if (_cache != null)
      {
        callback(null, new DataPortalResult<ProductCategories>(_cache, null, null));
      }
      else
      {
        var dp = new DataPortal<ProductCategories>();
        dp.FetchCompleted += (o, e) =>
        {
          _cache = e.Object;
          callback(dp, e);
        };
        dp.BeginFetch();
      }
    }

    #endregion
  }
}
