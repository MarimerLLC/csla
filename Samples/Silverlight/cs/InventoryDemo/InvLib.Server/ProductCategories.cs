using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace InvLib
{
  [Serializable]
  public partial class ProductCategories : NameValueListBase<int, string>
  {
    #region Factory Methods

    private static ProductCategories _cache = null;

    public void ClearCache()
    {
      _cache = null;
    }

    #endregion
  }
}
