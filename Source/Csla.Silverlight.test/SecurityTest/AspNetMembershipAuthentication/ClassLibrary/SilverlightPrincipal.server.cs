using System.Security.Principal;
using Csla.Security;

namespace SilverlightClassLibrary
{
  public partial class SilverlightPrincipal : Csla.Security.BusinessPrincipalBase
  {
    public static void LoginUsingMembershipProviderDataPortal(string name, string password)
    {
      var identity = MembershipIdentity.GetMembershipIdentity<MembershipIdentityStub>(name, password, false);
      SetPrincipal(identity);
    }

    public static void LoginUsingMembershipProviderWebServer(string name, string password)
    {
      var identity = MembershipIdentity.GetMembershipIdentity<MembershipIdentityStub>(name, password, true);
      SetPrincipal(identity);
    }
  }
}