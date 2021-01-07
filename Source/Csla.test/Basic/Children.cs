//-----------------------------------------------------------------------
// <copyright file="Children.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Csla.Test.Basic
{
    [Serializable()]
    public class Children : BusinessBindingListBase<Children, Child>
    {
        public void Add(string data)
        {
            this.Add(Child.NewChild(data));
        }


        internal static Children NewChildren()
        {
            return new Children();
        }

        internal static Children GetChildren(IDataReader dr)
        {
            return null;
        }

        internal void Update(IDbTransaction tr)
        {
            foreach (Child child in this)
            {
                child.Update(tr);
            }
        }

        public Children()
        {
            this.MarkAsChild();
        }

        public int DeletedCount
        {
          get { return this.DeletedList.Count; }
        }
    }
}