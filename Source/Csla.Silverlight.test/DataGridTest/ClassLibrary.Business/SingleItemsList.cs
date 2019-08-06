//-----------------------------------------------------------------------
// <copyright file="SingleItemsList.cs" company="Marimer LLC">
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
using Csla.Security;
using Csla.Core;
using Csla.Serialization;

namespace ClassLibrary.Business
{
  [Serializable]
  public class SingleItemsList : BusinessListBase<SingleItemsList,SingleItem>
  {
#if SILVERLIGHT
    public SingleItemsList() { }
#else
    private SingleItemsList() { }
#endif

    [Serializable()]
    public class FetchCriteria : CriteriaBase
    {
      public FetchCriteria() { }

      public FetchCriteria(int startId, int endId) : base(typeof(SingleItemsList))
      {
        _startId = startId;
        _endId = endId;
      }

      private int _startId;
      private int _endId;

      public int StartID
      {
        get
        {
          return _startId;
        }
      }
      public int EndID
      {
        get
        {
          return _endId;
        }
      }
      protected override void OnGetState(Csla.Serialization.Mobile.SerializationInfo info, StateMode mode)
      {
        base.OnGetState(info, mode);
        info.AddValue("_startId", _startId);
        info.AddValue("_endId", _endId);
      }
      protected override void OnSetState(Csla.Serialization.Mobile.SerializationInfo info, StateMode mode)
      {
        base.OnSetState(info, mode);
        _endId = info.GetValue<int>("_endId");
        _startId = info.GetValue<int>("_startId");
      }
    }

#if SILVERLIGHT
    public static void GetSingleItemsList(int startID, int endID, EventHandler<DataPortalResult<SingleItemsList>> handler)
    {
      DataPortal<SingleItemsList> dp = new DataPortal<SingleItemsList>();
      dp.FetchCompleted += handler;
      dp.BeginFetch(new FetchCriteria(startID, endID));
    }
#else
    protected void DataPortal_Fetch(FetchCriteria criteria )
    {
      for (int i = criteria.StartID; i <= criteria.EndID; i++)
      {
        DateTime aDate = new DateTime(2000,1, (i%30)+1);
        this.Add(SingleItem.GetSingleItem(i, "Item # " + i.ToString(),aDate));
      }
    }
#endif

  }
}