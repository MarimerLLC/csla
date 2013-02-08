//-----------------------------------------------------------------------
// <copyright file="HasRulesManager.partial.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Csla.DataPortalClient;
using Csla.Serialization.Mobile;
using Csla.Core;

namespace Csla.Test.ValidationRules
{
  public partial class HasRulesManager
  {
    public HasRulesManager()
      : base()
    { }

    public void DataPortal_Create(object criteria)
    {
      Criteria crit = (Criteria)(criteria);
      using (BypassPropertyChecks)
        Name = crit._name;
      Csla.ApplicationContext.GlobalContext.Add("HasRulesManager", "Created");
      BusinessRules.CheckRules();
    }

    #if !SILVERLIGHT

    protected override void DataPortal_Fetch(object criteria)
    {
      Criteria crit = (Criteria)(criteria);
      _name = crit._name;
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

    protected override void DataPortal_Delete(object criteria)
    {
      //we would delete here
      Csla.ApplicationContext.GlobalContext.Add("HasRulesManager", "Deleted");
    }

    #endif
  }
}