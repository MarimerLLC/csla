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
#if !SILVERLIGHT
using RolodexEF;
using Csla.Data;
using Rolodex.Business.Data;
#endif

namespace Rolodex.Business.BusinessClasses
{
    [Serializable]
    public class Ranks : NameValueListBase<int, string>
    {
        public static void GetRanks(EventHandler<DataPortalResult<Ranks>> handler)
        {
            DataPortal<Ranks> dp = new DataPortal<Ranks>();
            dp.FetchCompleted += handler;
            dp.BeginFetch();
        }

#if SILVERLIGHT
    public Ranks() { }
#else


        public static Ranks GetRanks()
        {
            return DataPortal.Fetch<Ranks>();
        }

        private void DataPortal_Fetch()
        {
            using (ObjectContextManager<RolodexEntities> manager = ObjectContextManager<RolodexEF.RolodexEntities>.GetManager(DataConnection.EFConnectionName, true))
            {
                RaiseListChangedEvents = false;
                IsReadOnly = false;
                foreach (var item in manager.ObjectContext.Ranks)
                {
                    Add(new Ranks.NameValuePair(item.RankId, item.Rank));
                }
                IsReadOnly = true;
                RaiseListChangedEvents = true;
            }
        }


#endif


    }
}
