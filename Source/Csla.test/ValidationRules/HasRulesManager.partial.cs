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
    private void DataPortal_Create(object criteria)
    {
      Criteria crit = (Criteria)(criteria);
      using (BypassPropertyChecks)
        Name = crit._name;
      Csla.ApplicationContext.GlobalContext.Add("HasRulesManager", "Created");
      BusinessRules.CheckRules();
    }

    protected void DataPortal_Fetch(object criteria)
    {
      Criteria crit = (Criteria)(criteria);
      using (BypassPropertyChecks)
        Name = crit._name;
      MarkOld();
      Csla.ApplicationContext.GlobalContext.Add("HasRulesManager", "Fetched");
    }

    protected override void DataPortal_Update()
    {
      if (IsDeleted)
      {
        //we would delete here
        Csla.ApplicationContext.GlobalContext.Add("HasRulesManager", "Deleted");
        MarkNew();
      }
      else
      {
        if (this.IsNew)
        {
          //we would insert here
          Csla.ApplicationContext.GlobalContext.Add("HasRulesManager", "Inserted");
        }
        else
        {
          //we would update here
          Csla.ApplicationContext.GlobalContext.Add("HasRulesManager", "Updated");
        }
        MarkOld();
      }
    }
  }
}