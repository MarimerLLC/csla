Imports CSLA
Imports CSLA.BatchQueue.Server

Module Module1

  Sub Main()

    Console.WriteLine("Server on thread {0}", AppDomain.GetCurrentThreadId())
    Console.WriteLine("Starting...")
    BatchQueueService.Start()
    Console.WriteLine("Started")

    Console.WriteLine("Press ENTER to end")
    Console.ReadLine()

    Console.WriteLine("Stopping...")
    BatchQueueService.Stop()
    Console.WriteLine("Stopped")

  End Sub

End Module
