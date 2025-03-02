using Csla.Threading;

namespace Csla.Channels.RabbitMq
{
  internal static class Wip
  {
    public static readonly ConcurrentDictionary<string, WipItem> WorkInProgress = [];
  }

  internal class WipItem
  {
    public Threading.AsyncManualResetEvent ResetEvent { get; }
    public byte[]? Response { get; set; }

    internal WipItem(AsyncManualResetEvent resetEvent)
    {
      ResetEvent = resetEvent;
    }
  }
}
