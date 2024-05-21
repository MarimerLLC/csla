//-----------------------------------------------------------------------
// <copyright file="DpRoot.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.DataPortal
{
  [Serializable]
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

    [Serializable]
    internal class Criteria : CriteriaBase<Criteria>
    {
      public static PropertyInfo<string> DataProperty = RegisterProperty<string>(c => c.Data);
      public string Data
      {
        get { return ReadProperty(DataProperty); }
        set { LoadProperty(DataProperty, value); }
      }

      public Criteria()
      {
        Data = "<new>";
      }

      public Criteria(string data)
      {
        Data = data;
      }
    }

    #endregion

    public DpRoot CloneThis()
    {
      return Clone();
    }


    #region "DataPortal"

    private void DataPortal_Create(object criteria)
    {
      Criteria crit = (Criteria)(criteria);
      using (BypassPropertyChecks)
        Data = crit.Data;
    }

    protected void DataPortal_Fetch(object criteria)
    {
      Criteria crit = (Criteria)(criteria);
      using (BypassPropertyChecks)
        Data = crit.Data;
      MarkOld();
    }

    [Insert]
    protected void DataPortal_Insert()
    {
      //we would insert here
    }

    [Update]
    protected void DataPortal_Update()
    {
      //we would update here
    }

    [DeleteSelf]
    protected void DataPortal_DeleteSelf()
    {
      //we would delete here
    }

    [Delete]
    protected void DataPortal_Delete(object criteria)
    {
      //we would delete here
    }

    protected override void DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {
      TestResults.Add("serverinvoke", "true");
    }

    protected override void DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      TestResults.Add("serverinvokecomplete", "true");
    }

    #endregion

    #region "Authorization Rules"

    protected override void AddBusinessRules()
    {
      string role = "Admin";

      BusinessRules.AddRule(new Rules.CommonRules.IsNotInRole(Rules.AuthorizationActions.ReadProperty, DenyReadOnPropertyProperty, new List<string> { role }));
      BusinessRules.AddRule(new Rules.CommonRules.IsNotInRole(Rules.AuthorizationActions.WriteProperty, DenyWriteOnPropertyProperty, new List<string> { role }));

      BusinessRules.AddRule(new Rules.CommonRules.IsNotInRole(Rules.AuthorizationActions.ReadProperty, DenyReadWriteOnPropertyProperty, new List<string> { role }));
      BusinessRules.AddRule(new Rules.CommonRules.IsNotInRole(Rules.AuthorizationActions.WriteProperty, DenyReadWriteOnPropertyProperty, new List<string> { role }));

      BusinessRules.AddRule(new Rules.CommonRules.IsInRole(Rules.AuthorizationActions.ReadProperty, AllowReadWriteOnPropertyProperty, new List<string> { role }));
      BusinessRules.AddRule(new Rules.CommonRules.IsInRole(Rules.AuthorizationActions.WriteProperty, AllowReadWriteOnPropertyProperty, new List<string> { role }));
    }

    public static PropertyInfo<string> DenyReadOnPropertyProperty = RegisterProperty<string>(c => c.DenyReadOnProperty);
    public string DenyReadOnProperty
    {
      get
      {
        if (CanReadProperty("DenyReadOnProperty"))
          //return "Not allowed 1";
          return ((TestHelpers.ApplicationContextManagerUnitTests)ApplicationContext.ContextManager).InstanceId.ToString();

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
          return "Not allowed 3";

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
          return "Should be allowed 5";
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