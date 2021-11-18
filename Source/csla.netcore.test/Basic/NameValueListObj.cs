//-----------------------------------------------------------------------
// <copyright file="NameValueListObj.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.Basic
{
    [Serializable()]
    public class NameValueListObj : NameValueListBase<int, string>
    {
        [Fetch]
        protected void DataPortal_Fetch()
        {
            TestResults.Add("NameValueListObj", "Fetched");

            this.IsReadOnly = false;
            for (int i = 0; i < 10; i++)
            {
                this.Add(new NameValuePair(i, "element_" + i.ToString()));
            }
            this.IsReadOnly = true;
        }
    }
}