using System.Security.Principal;

namespace Csla.TestHelpers
{
  public class ApplicationContextManagerUnitTests : Core.ApplicationContextManagerAsyncLocal
  {
    public IPrincipal LastSetUserPrincipal { get; private set; }

    public Guid InstanceId { get; private set; } = Guid.NewGuid();

    public DateTime CreatedAt { get; private set; } = DateTime.Now;

    public override void SetUser(IPrincipal principal)
    {
      LastSetUserPrincipal = principal;
      base.SetUser(principal);
    }
  }
}
