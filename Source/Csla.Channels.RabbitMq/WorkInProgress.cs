using System.Collections.Concurrent;
using Csla.Threading;

namespace Csla.Channels.RabbitMq
{
  internal static class Wip
  {
    public static readonly ConcurrentDictionary<string, WipItem> WorkInProgress = [];
  }

  internal class WipItem
  {
    public Csla.Threading.AsyncManualResetEvent ResetEvent { get; }
    public byte[]? Response { get; set; }

    public WipItem(AsyncManualResetEvent resetEvent)
    {
      ResetEvent = resetEvent;
    }
  }
}