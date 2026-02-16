
using System.ComponentModel;
using System.Runtime.Versioning;

namespace BusinessRuleDemo.Controls
{
  [SupportedOSPlatform("windows")]
  public class ReadWriteAuthorization : Csla.Windows.ReadWriteAuthorization
  {
    public ReadWriteAuthorization(IContainer container)
      : base(container)
    {

    }
  }
}
