using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Security;
using Csla.Core;
using Csla.Serialization;
using Csla.Silverlight;
using Csla.Validation;

namespace Rolodex.Business.BusinessClasses
{
  [Serializable]
  [MobileFactory("Rolodex.Business.BusinessClasses.RanksFactory, Rolodex.Business", "FetchRanks")]
  public class Ranks : NameValueListBase<int, string>
  {
#if SILVERLIGHT
    public Ranks() { }

    public static void GetRanks(EventHandler<DataPortalResult<Ranks>> handler)
    {
      DataPortal<Ranks> dp = new DataPortal<Ranks>();
      dp.FetchCompleted += handler;
      dp.BeginFetch();
    }
#endif

    internal void SetReadOnlyFlag(bool flag)
    {
      IsReadOnly = flag;
    }

    public string GetRankName(int rank)
    {
      string returnValue = string.Empty;
      foreach (var oneItem in this)
      {
        if (oneItem.Key == rank)
        {
          returnValue = oneItem.Value;
          break;
        }
      }
      return returnValue;
    }
  }
}
