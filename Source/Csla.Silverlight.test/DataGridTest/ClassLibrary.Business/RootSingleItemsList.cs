//-----------------------------------------------------------------------
// <copyright file="RootSingleItemsList.cs" company="Marimer LLC">
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
  public class RootSingleItemsList : EditableRootListBase<SingleItem>
 {
#if SILVERLIGHT
    public RootSingleItemsList() { }
#else
    private RootSingleItemsList() { }
#endif

    [Serializable()]
    public class FetchCriteria : CriteriaBase
    {
      public FetchCriteria() { }

      public FetchCriteria(int startId, int endId)
        : base(typeof(RootSingleItemsList))
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
    public static void GetRootSingleItemsList(int startID, int endID, EventHandler<DataPortalResult<RootSingleItemsList>> handler)
    {
      DataPortal<RootSingleItemsList> dp = new DataPortal<RootSingleItemsList>();
      dp.FetchCompleted += handler;
      dp.BeginFetch(new FetchCriteria(startID, endID));
    }
#else
    protected void DataPortal_Fetch(FetchCriteria criteria )
    {
      RaiseListChangedEvents = false;
      for (int i = criteria.StartID; i <= criteria.EndID; i++)
      {
        DateTime aDate = new DateTime(2000,1, (i%30)+1);
        this.Add(SingleItem.GetSingleItem(i, "Item # " + i.ToString(),aDate));
      }
      RaiseListChangedEvents = true;
    }
#endif

  }
}