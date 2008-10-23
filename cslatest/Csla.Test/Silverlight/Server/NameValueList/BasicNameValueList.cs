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
      dp.BeginFetch(new BasicNameValueList.Criteria(typeof(BasicNameValueList)));
    }



#if !SILVERLIGHT

    public static BasicNameValueList GetBasicNameValueList()
    {
      return DataPortal.Fetch<BasicNameValueList>(new BasicNameValueList.Criteria(typeof(BasicNameValueList)));
    }

    protected void DataPortal_Fetch(object criteria)
    {
      this.IsReadOnly = false;
      for (int i = 0; i < 10; i++)
      {
        this.Add(new NameValuePair(i, "element_" + i.ToString()));
      }
      this.IsReadOnly = true;
    }

#else
    public BasicNameValueList() { }
#endif
  }
}
