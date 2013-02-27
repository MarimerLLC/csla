using System;
using Csla;
using Csla.Serialization;

namespace CslaExtensions
{
    [Serializable()]
    public class ExtendedBusinessListBase<TClass, TItem> : BusinessListBase<TClass, TItem>
        where TClass : ExtendedBusinessListBase<TClass, TItem>
        where TItem : ExtendedBusinessBase<TItem>
    {


        protected void PreFetch()
        {
            RaiseListChangedEvents = false;
        }

        protected void PostFetch()
        {
            RaiseListChangedEvents = true;
        }

    }
}
