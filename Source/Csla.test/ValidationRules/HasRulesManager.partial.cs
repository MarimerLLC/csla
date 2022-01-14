//-----------------------------------------------------------------------
// <copyright file="HasRulesManager.partial.cs" company="Marimer LLC">
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
  public partial class HasRulesManager
  {
    [Create]
    private void DataPortal_Create(object criteria)
    {
      Criteria crit = (Criteria)(criteria);
      using (BypassPropertyChecks)
        Name = crit._name;
      TestResults.Add("HasRulesManager", "Created");
      BusinessRules.CheckRules();
    }

    [Fetch]
    protected void DataPortal_Fetch(object criteria)
    {
      Criteria crit = (Criteria)(criteria);
      using (BypassPropertyChecks)
        Name = crit._name;
      MarkOld();
      TestResults.Add("HasRulesManager", "Fetched");
    }

    [Update]
		protected void DataPortal_Update()
    {
      if (IsDeleted)
      {
        //we would delete here
        TestResults.Add("HasRulesManager", "Deleted");
        MarkNew();
      }
      else
      {
        if (this.IsNew)
        {
          //we would insert here
          TestResults.Add("HasRulesManager", "Inserted");
        }
        else
        {
          //we would update here
          TestResults.Add("HasRulesManager", "Updated");
        }
        MarkOld();
      }
    }
  }
}