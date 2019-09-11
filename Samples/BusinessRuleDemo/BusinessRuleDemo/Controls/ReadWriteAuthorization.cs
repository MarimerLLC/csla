
using System.ComponentModel;

namespace BusinessRuleDemo.Controls
{
  public class ReadWriteAuthorization : Csla.Windows.ReadWriteAuthorization
  {
    public ReadWriteAuthorization(IContainer container)
      : base(container)
    {

    }
  }
}
