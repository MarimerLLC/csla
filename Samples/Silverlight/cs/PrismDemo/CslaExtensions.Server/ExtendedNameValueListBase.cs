using System;
using Csla;
using Csla.Serialization;

namespace CslaExtensions
{
    [Serializable()]
    public class ExtendedNameValueListBase<TKey, TValue> : NameValueListBase<TKey, TValue>
    {

        protected void PreFetch()
        {
            RaiseListChangedEvents = false;
            IsReadOnly = false;
        }

        protected void PostFetch()
        {
            IsReadOnly = true;
            RaiseListChangedEvents = true;
        }
    }
}
