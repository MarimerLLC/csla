using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Security;

namespace Csla.Validation.Test
{
 public class VPrincipal : CslaPrincipal
  {
   public VPrincipal(params string[] roles) : base(new VIdentity(roles))
   {

   }
  }
}
