using System.Collections.Concurrent;

namespace Csla.Channels.RabbitMq
{
  internal static class Wip
  {
    public static readonly ConcurrentDictionary<string, WipItem> WorkInProgress = 
      new ConcurrentDictionary<string, WipItem>();
  }

  internal class WipItem
  {
    public Csla.Reflection.AsyncManualResetEvent ResetEvent { get; set; }
    public byte[] Response { get; set; }
  }
}
