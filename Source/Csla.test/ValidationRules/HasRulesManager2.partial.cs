//-----------------------------------------------------------------------
// <copyright file="HasRulesManager2.partial.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Test.ValidationRules
{
  public partial class HasRulesManager2
  {
    [Create]
    private void DataPortal_Create()
    {
      BusinessRules.CheckRules();
    }

    [Create]
    private void DataPortal_Create(object criteria)
    {
      Criteria crit = (Criteria)(criteria);
      using (BypassPropertyChecks)
        Name = crit._name;
      TestResults.Add("HasRulesManager2", "Created");
      BusinessRules.CheckRules();
    }

    protected void DataPortal_Fetch(object criteria)
    {
      Criteria crit = (Criteria)(criteria);
      using (BypassPropertyChecks)
        Name = crit._name;
      MarkOld();
      TestResults.Add("HasRulesManager2", "Fetched");
    }

    [Update]
		protected void DataPortal_Update()
    {
      if (IsDeleted)
      {
        //we would delete here
        TestResults.Add("HasRulesManager2", "Deleted");
        MarkNew();
      }
      else
      {
        if (this.IsNew)
        {
          //we would insert here
          TestResults.Add("HasRulesManager2", "Inserted");
        }
        else
        {
          //we would update here
          TestResults.Add("HasRulesManager2", "Updated");
        }
        MarkOld();
      }
    }

    [Delete]
		protected void DataPortal_Delete(object criteria)
    {
      //we would delete here
      TestResults.Add("HasRulesManager2", "Deleted");
    }


  }
}