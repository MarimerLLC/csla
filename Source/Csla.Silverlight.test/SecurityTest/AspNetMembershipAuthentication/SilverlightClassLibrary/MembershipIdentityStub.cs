using System;
using Csla.Security;
using Csla.Serialization;

namespace SilverlightClassLibrary
{
  [Serializable]
  public class MembershipIdentityStub : MembershipIdentity
  {
    public MembershipIdentityStub(){}

    #if !SILVERLIGHT
    protected override void LoadCustomData()
    {
      base.LoadCustomData();
      
      //Custom information to be retrieved from web server goes here
    }
    #endif
  }
}
