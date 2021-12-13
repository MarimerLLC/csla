//-----------------------------------------------------------------------
// <copyright file="GrandChildren.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Business object type for use in tests</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Csla.Test.Server.Interceptors
{
  [Serializable()]
    public class GrandChildren : BusinessListBase<GrandChildren, GrandChild>
    {
        public void Add(string data)
        {
            var grandChild = this.AddNew();
            grandChild.Data = data;
        }

        internal void Update(IDbTransaction tr)
        {
            foreach (GrandChild child in this)
            {
                child.Update(tr);
            }
        }

        public GrandChildren()
        {
            MarkAsChild();
        }
    }
}