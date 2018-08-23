//-----------------------------------------------------------------------
// <copyright file="BasicNameValueList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Silverlight;
using Csla.Serialization;
using Csla;

namespace cslalighttest.NameValueList
{
  [Serializable()]
  public class BasicNameValueList : NameValueListBase<Int32, string>
  {
    public static void GetBasicNameValueList(EventHandler<DataPortalResult<BasicNameValueList>> completed)
    {
      DataPortal<BasicNameValueList> dp = new DataPortal<BasicNameValueList>();
      dp.FetchCompleted += completed;
      dp.BeginFetch();
    }



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