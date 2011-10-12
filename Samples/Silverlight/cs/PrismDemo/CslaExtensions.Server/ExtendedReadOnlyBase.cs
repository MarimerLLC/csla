using System;
using Csla;
using Csla.Serialization;

namespace CslaExtensions
{
    [Serializable()]
    public class ExtendedReadOnlyBase<TClass> : ReadOnlyBase<TClass>
        where TClass : ExtendedReadOnlyBase<TClass>
    {



    }
}
