using System;
using CSLA.BatchQueue.Server;

namespace BatchQueueTest
{
  class Class1
  {
    [STAThread]
    static void Main(string[] args)
    {
      Console.WriteLine("Server on thread {0}", AppDomain.GetCurrentThreadId());
      Console.WriteLine("Starting...");
      BatchQueueService.Start();
      Console.WriteLine("Started");

      Console.WriteLine("Press ENTER to end");
      Console.ReadLine();

      Console.WriteLine("Stopping...");
      BatchQueueService.Stop();
      Console.WriteLine("Stopped");
    }
  }
}
