using System;
using Csla;
using Csla.Serialization;

namespace CslaExtensions
{
    [Serializable()]
    public class ExtendedCommandBase<T> : CommandBase<T>
        where T: ExtendedCommandBase<T>
    {

    }


}
