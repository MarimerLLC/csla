using System;
using System.Collections.Generic;
using System.Text;
using Csla.Serialization;
using Csla.DataPortalClient;
using System.Diagnostics;

namespace Csla.Test.Security
{
#if TESTING
    [DebuggerNonUserCode]
#endif
  [Serializable()]
  public class PermissionsRoot : BusinessBase<PermissionsRoot>
  {
    private int _ID = 0;
    private string _firstName;

    protected override object GetIdValue()
    {
      return _ID;
    }

    public string FirstName
    {
      get
      {
        if (CanReadProperty("FirstName"))
        {
          return _firstName;
        }
        else
        {
          throw new System.Security.SecurityException("Property get not allowed");
        }
      }
      set
      {
        if (CanWriteProperty("FirstName"))
        {
          _firstName = value;
        }
        else
        {
          throw new System.Security.SecurityException("Property set not allowed");
        }
      }
    }

    public void DoWork()
    {
      CanExecuteMethod("DoWork", true);
    }

    #region Authorization

    protected override void AddAuthorizationRules()
    {
      this.AuthorizationRules.AllowRead("FirstName", "Admin");
      this.AuthorizationRules.AllowWrite("FirstName", "Admin");

      this.AuthorizationRules.AllowExecute("DoWork", "Admin");
    }

    #endregion

    #region "Constructors"
#if SILVERLIGHT
      public PermissionsRoot() { }
#else

    private PermissionsRoot()
    {
      //require use of factory methods
    }
#endif
    #endregion

    #region "factory methods"

#if SILVERLIGHT
        public static PermissionsRoot NewPermissionsRoot()
        {
            return new PermissionsRoot();
        }
#else
    public static PermissionsRoot NewPermissionsRoot()
    {
      return Csla.DataPortal.Create<PermissionsRoot>();
    }
#endif
    #endregion

    #region "Criteria"

    [Serializable()]
    private class Criteria
    {
      //implement
    }

    #endregion

#if !SILVERLIGHT

    [RunLocal()]
    protected override void DataPortal_Create()
    {
      _firstName = "default value"; //for now...
    }

#endif
  }
}
