//-----------------------------------------------------------------------
// <copyright file="DpRoot.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Csla.DataPortalClient;
using Csla.Serialization;

namespace Csla.Test.DataPortal
{
  [Serializable()]
  public class DpRoot : BusinessBase<DpRoot>
  {
    private string _auth = "No value";

    #region "Get/Set Private Variables"

    public static PropertyInfo<string> DataProperty = RegisterProperty<string>(c => c.Data);
    public string Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }

    public string Auth
    {
      get
      {
        return _auth;
      }

      set
      {
        //Not allowed
      }
    }

    #endregion

    #region "Criteria class"

    [Serializable()]
    private class Criteria
    {
      public string _data;

      public Criteria()
      {
        _data = "<new>";
      }

      public Criteria(string data)
      {
        this._data = data;
      }
    }

    #endregion

    #region "New root + constructor"

    public static DpRoot NewRoot()
    {
      Criteria crit = new Criteria();
      return (Csla.DataPortal.Create<DpRoot>(crit));
    }

    public static DpRoot GetRoot(string data)
    {
      return (Csla.DataPortal.Fetch<DpRoot>(new Criteria(data)));
    }


    #endregion

    public DpRoot CloneThis()
    {
      return this.Clone();
    }


    #region "DataPortal"

    private void DataPortal_Create(object criteria)
    {
      Criteria crit = (Criteria)(criteria);
      using (BypassPropertyChecks)
        Data = crit._data;
    }

    protected void DataPortal_Fetch(object criteria)
    {
      Criteria crit = (Criteria)(criteria);
      using (BypassPropertyChecks)
        Data = crit._data;
      MarkOld();
    }

    protected override void DataPortal_Insert()
    {
      //we would insert here
    }

    protected override void DataPortal_Update()
    {
      //we would update here
    }

    protected override void DataPortal_DeleteSelf()
    {
      //we would delete here
    }

    protected void DataPortal_Delete(object criteria)
    {
      //we would delete here
    }

    protected override void DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {
      Csla.ApplicationContext.GlobalContext["serverinvoke"] = true;
    }

    protected override void DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      Csla.ApplicationContext.GlobalContext["serverinvokecomplete"] = true;
    }

    #endregion

    #region "Authorization Rules"

    protected override void AddBusinessRules()
    {
      string role = "Admin";

      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsNotInRole(Rules.AuthorizationActions.ReadProperty, DenyReadOnPropertyProperty, new List<string> { role }));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsNotInRole(Rules.AuthorizationActions.WriteProperty, DenyWriteOnPropertyProperty, new List<string> { role }));

      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsNotInRole(Rules.AuthorizationActions.ReadProperty, DenyReadWriteOnPropertyProperty, new List<string> { role }));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsNotInRole(Rules.AuthorizationActions.WriteProperty, DenyReadWriteOnPropertyProperty, new List<string> { role }));

      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Rules.AuthorizationActions.ReadProperty, AllowReadWriteOnPropertyProperty, new List<string> { role }));
      BusinessRules.AddRule(new Csla.Rules.CommonRules.IsInRole(Rules.AuthorizationActions.WriteProperty, AllowReadWriteOnPropertyProperty, new List<string> { role }));
    }

    public static PropertyInfo<string> DenyReadOnPropertyProperty = RegisterProperty<string>(c => c.DenyReadOnProperty);
    public string DenyReadOnProperty
    {
      get
      {
        if (CanReadProperty("DenyReadOnProperty"))
          throw new Csla.Security.SecurityException("Not allowed 1");

        else
          return "[DenyReadOnProperty] Can't read property";
      }

      set
      {
        //Not allowed
      }
    }

    public static PropertyInfo<string> DenyWriteOnPropertyProperty = RegisterProperty<string>(c => c.DenyWriteOnProperty);
    public string DenyWriteOnProperty
    {
      get
      {
        return "<No Value>";
      }

      set
      {
        if (CanWriteProperty("DenyWriteOnProperty"))
          throw new Csla.Security.SecurityException("Not allowed 2");

        else
          _auth = "[DenyWriteOnProperty] Can't write variable";

      }
    }

    public static PropertyInfo<string> DenyReadWriteOnPropertyProperty = RegisterProperty<string>(c => c.DenyReadWriteOnProperty);
    public string DenyReadWriteOnProperty
    {
      get
      {
        if (CanReadProperty("DenyReadWriteOnProperty"))
          throw new Csla.Security.SecurityException("Not allowed 3");

        else
          return "[DenyReadWriteOnProperty] Can't read property";
      }
      set
      {
        if (CanWriteProperty("DenyReadWriteOnProperty"))
          throw new Csla.Security.SecurityException("Not allowed 4");

        else
          _auth = "[DenyReadWriteOnProperty] Can't write variable";
      }
    }

    public static PropertyInfo<string> AllowReadWriteOnPropertyProperty = RegisterProperty<string>(c => c.AllowReadWriteOnProperty);
    public string AllowReadWriteOnProperty
    {
      get
      {
        if (CanReadProperty("AllowReadWriteOnProperty"))
          return _auth;

        else
          throw new Csla.Security.SecurityException("Should be allowed 5");
      }
      set
      {
        if (CanWriteProperty("AllowReadWriteOnProperty"))
          _auth = value;

        else
          throw new Csla.Security.SecurityException("Should be allowed 5");
      }
    }

    #endregion
  }
}