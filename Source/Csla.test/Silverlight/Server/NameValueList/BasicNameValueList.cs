//-----------------------------------------------------------------------
// <copyright file="BasicNameValueList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using Csla;

namespace cslalighttest.NameValueList
{
  [Serializable()]
  public class BasicNameValueList : NameValueListBase<Int32, string>
  {
    public static BasicNameValueList GetBasicNameValueList()
    {
      return DataPortal.Fetch<BasicNameValueList>();
    }

    protected void DataPortal_Fetch()
    {
      this.IsReadOnly = false;
      for (int i = 0; i < 10; i++)
      {
        this.Add(new NameValuePair(i, "element_" + i.ToString()));
      }
      this.IsReadOnly = true;
    }
  }
}