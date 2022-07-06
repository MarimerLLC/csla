using System;
using System.Threading.Tasks;

namespace Csla.Test.Server.Scope
{
  public class GuidProvider : IAsyncDisposable
  {
    public GuidProvider() => Guid = Guid.NewGuid();

    public Guid Guid { get; set; }
    public bool Disposed { get; set; }

    private void Dispose()
    {
      Disposed = true;
      GC.SuppressFinalize(this);
    }

    public ValueTask DisposeAsync()
    {
      Dispose();
      return new ValueTask(Task.CompletedTask);
    }
  }
}
