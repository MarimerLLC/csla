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
#if SILVERLIGHT
      public DpRoot() { }
#endif
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
#if SILVERLIGHT
        public static DpRoot NewRoot()
        {
          DpRoot returnValue = new DpRoot();
          returnValue.Data = new Criteria()._data;
          return returnValue;
        }
        public static DpRoot GetRoot(string data)
        {
          DpRoot returnValue = new DpRoot();
          returnValue.Data = new Criteria()._data;
          returnValue.MarkOld();
          return returnValue;
        }
#else
    public static DpRoot NewRoot()
    {
      Criteria crit = new Criteria();
      return (Csla.DataPortal.Create<DpRoot>(crit));
    }

    public static DpRoot GetRoot(string data)
    {
      return (Csla.DataPortal.Fetch<DpRoot>(new Criteria(data)));
    }
    private DpRoot()
    {
      //prevent direct creation
      //AddAuthRules();
    }
#endif


    #endregion

    public DpRoot CloneThis()
    {
      return this.Clone();
    }


    #region "DataPortal"

#if !SILVERLIGHT

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

#endif

    #endregion

    #region "Authorization Rules"

    protected override void AddAuthorizationRules()
    {
      string role = "Admin";

      this.AuthorizationRules.DenyRead("DenyReadOnProperty", role);
      this.AuthorizationRules.DenyWrite("DenyWriteOnProperty", role);

      this.AuthorizationRules.DenyRead("DenyReadWriteOnProperty", role);
      this.AuthorizationRules.DenyWrite("DenyReadWriteOnProperty", role);

      this.AuthorizationRules.AllowRead("AllowReadWriteOnProperty", role);
      this.AuthorizationRules.AllowWrite("AllowReadWriteOnProperty", role);
    }

    public string DenyReadOnProperty
    {
      get
      {
        if (CanReadProperty("DenyReadOnProperty"))
          throw new System.Security.SecurityException("Not allowed 1");

        else
          return "[DenyReadOnProperty] Can't read property";
      }

      set
      {
        //Not allowed
      }
    }

    public string DenyWriteOnProperty
    {
      get
      {
        return "<No Value>";
      }

      set
      {
        if (CanWriteProperty("DenyWriteOnProperty"))
          throw new System.Security.SecurityException("Not allowed 2");

        else
          _auth = "[DenyWriteOnProperty] Can't write variable";

      }
    }

    public string DenyReadWriteOnProperty
    {
      get
      {
        if (CanReadProperty("DenyReadWriteOnProperty"))
          throw new System.Security.SecurityException("Not allowed 3");

        else
          return "[DenyReadWriteOnProperty] Can't read property";
      }
      set
      {
        if (CanWriteProperty("DenyReadWriteOnProperty"))
          throw new System.Security.SecurityException("Not allowed 4");

        else
          _auth = "[DenyReadWriteOnProperty] Can't write variable";
      }
    }

    public string AllowReadWriteOnProperty
    {
      get
      {
        if (CanReadProperty("AllowReadWriteOnProperty"))
          return _auth;

        else
          throw new System.Security.SecurityException("Should be allowed 5");
      }
      set
      {
        if (CanWriteProperty("AllowReadWriteOnProperty"))
          _auth = value;

        else
          throw new System.Security.SecurityException("Should be allowed 5");
      }
    }

    #endregion
  }
}
