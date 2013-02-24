using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Core;
using System.ServiceModel;

namespace ClassLibrary
{
  public partial class SLIdentity
  {
    public void DataPortal_Fetch(UsernameCriteria criteria)
    {
      switch (criteria.Username)
      {
        case "TestUser":
          if (criteria.Password == "1234")
          {
            base.Name = criteria.Username;
            base.IsAuthenticated = true;
            this.Roles = new MobileList<string> { "User", "Admin" };
          }
          break;
       
        case "TestUserA":
          if (criteria.Password == "1234")
          {
            base.Name = criteria.Username;
            base.IsAuthenticated = true;
            this.Roles = new MobileList<string> { "ClassARole", "PropertyARole" };
          }
          break;

        case "TestUserD":
          if (criteria.Password == "1234")
          {
            base.Name = criteria.Username;
            base.IsAuthenticated = true;
            this.Roles = new MobileList<string>();
            this.Extra = "Extra data";
            this.MoreData = "Even more data";
          }
          break;

        default:
          this.Roles = new MobileList<string>();
          break;
      }
    }
  }
}
