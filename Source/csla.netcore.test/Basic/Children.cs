//-----------------------------------------------------------------------
// <copyright file="Children.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
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
    public class Children : BusinessListBase<Children, Child>
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
            //todo: load child data
            return null;
        }

        internal void Update(IDbTransaction tr)
        {
            foreach (Child child in this)
            {
                child.Update(tr);
            }
        }

        private Children()
        {
            //prevent direct creation
            this.MarkAsChild();
        }

        public int DeletedCount
        {
          get { return this.DeletedList.Count; }
        }

        public List<Child> GetDeletedList()
        {
          return this.DeletedList;
        }
    }
}