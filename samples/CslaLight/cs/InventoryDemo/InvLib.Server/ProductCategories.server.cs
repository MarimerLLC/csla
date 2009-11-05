using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace InvLib
{
  [Csla.Server.ObjectFactory("InvDal.ProductCategories,InvDal")]
  public partial class ProductCategories
  {
    #region Factory Methods

    public static ProductCategories GetProductCategories()
    {
      if (_cache != null)
        return _cache;
      else
        return DataPortal.Fetch<ProductCategories>();
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public void AddItem(int id, string name)
    {
      Add(new NameValueListBase<int, string>.NameValuePair(id, name));
    }

    #endregion
  }
}
