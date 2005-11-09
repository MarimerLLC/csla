using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Csla.Test.Basic
{
    [Serializable()]
#if csla20cs
    public class GrandChildren : BusinessListBase<GrandChild>
    {
        public void Add(string data)
        {
            this.Add(GrandChild.NewGrandChild(data));
        }

        internal static GrandChildren NewGrandChildren()
        {
            return new GrandChildren();
        }

        internal static GrandChildren GetGrandChildren(IDataReader dr)
        {
            //todo: load child data
            return null;
        }

        internal void Update(IDbTransaction tr)
        {
            foreach (GrandChild child in this)
            {
                child.Update(tr);
            }
        }

        private GrandChildren()
        {
            //prevent direct creation
            MarkAsChild();
        }
    }
#endif
    #if csla20vb
    public class GrandChildren : BusinessListBase<GrandChildren, GrandChild>
    {
        public void Add(string data)
        {
            this.Add(GrandChild.NewGrandChild(data));
        }

        internal static GrandChildren NewGrandChildren()
        {
            return new GrandChildren();
        }

        internal static GrandChildren GetGrandChildren(IDataReader dr)
        {
            //todo: load child data
            return null;
        }

        internal void Update(IDbTransaction tr)
        {
            foreach (GrandChild child in this)
            {
                child.Update(tr);
            }
        }

        private GrandChildren()
        {
            //prevent direct creation
            MarkAsChild();
        }
    }
#endif
}
