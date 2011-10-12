using Csla;
using Csla.Core;
using Csla.Serialization;
using System;

namespace CslaExtensions
{
    [Serializable()]
    public partial class ExtendedBusinessBase<TClass> : BusinessBase<TClass>
            where TClass : ExtendedBusinessBase<TClass>
    {

        public void DeleteInList()
        {
            (this.Parent as IParent).RemoveChild(this);
        }

    }

}
