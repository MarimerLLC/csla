//-----------------------------------------------------------------------
// <copyright file="GrandChildList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;
using Csla.Validation;
using System.Text.RegularExpressions;
using Csla.Security;

namespace ChildGrandChild.Business
{
    [Serializable]
    public class GrandchildList : BusinessListBase<GrandchildList, Grandchild>
    {
       // protected override object AddNewCore()
       // {
        //    Grandchild item = new Grandchild();
        //    Add(item);
        //    return item;
       // }

        public GrandchildList()
        {
            AllowNew = true;
        }

        public void DumpEditLevels(StringBuilder sb)
        {
            sb.AppendFormat("      {0} {1}: {2}\r", this.GetType().Name, "n/a", this.EditLevel);
            foreach (Grandchild item in DeletedList)
                item.DumpEditLevels(sb);
            foreach (Grandchild item in this)
                item.DumpEditLevels(sb);
        }
    }
}