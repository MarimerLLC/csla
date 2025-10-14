//-----------------------------------------------------------------------
// <copyright file="RootSingleItemsList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
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


namespace Csla.Testing.Business.EditableRootListTests
{
  [Serializable]
  public class RootSingleItemsList : DynamicBindingListBase<SingleItem>
 {
    private RootSingleItemsList() { }

    [Serializable()]
    public class FetchCriteria : CriteriaBase<FetchCriteria>
    {
      public FetchCriteria() { }

      public FetchCriteria(int startId, int endId)
      {
        StartID = startId;
        EndID = endId;
      }

      public static PropertyInfo<int> StartIDProperty = RegisterProperty<int>(c => c.StartID);
      public int StartID
      {
        get { return ReadProperty(StartIDProperty); }
        private set { LoadProperty(StartIDProperty, value); }
      }

      public static PropertyInfo<int> EndIDProperty = RegisterProperty<int>(c => c.EndID);
      public int EndID
      {
        get { return ReadProperty(EndIDProperty); }
        private set { LoadProperty(EndIDProperty, value); }
      }
    }

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
  }
}