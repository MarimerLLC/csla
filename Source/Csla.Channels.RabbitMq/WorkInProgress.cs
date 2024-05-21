using System.Collections.Concurrent;

namespace Csla.Channels.RabbitMq
{
  internal static class Wip
  {
    public static readonly ConcurrentDictionary<string, WipItem> WorkInProgress = [];
  }

  internal class WipItem
  {
    public Threading.AsyncManualResetEvent ResetEvent { get; set; }
    public byte[] Response { get; set; }
  }
}
