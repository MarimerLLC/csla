using System;
using Csla;
using Csla.Serialization;

namespace CslaExtensions
{
    [Serializable()]
    public class ExtendedReadOnlyListBase<TClass, TItem> : ReadOnlyListBase<TClass, TItem>
        where TClass : ExtendedReadOnlyListBase<TClass, TItem>
        where TItem : ExtendedReadOnlyBase<TItem>
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
