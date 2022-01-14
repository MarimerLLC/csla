//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Collections.Specialized;
//using System.ComponentModel;
//using System.Diagnostics;
//using System.Linq;
//using System.Reflection;
//using System.Threading;
//using System.Windows.Threading;

//namespace Csla.Test.Threading
//{
//  /// <summary>
//  /// Helps ensure the proper behaviour of the BackgroundWorker.
//  /// 
//  /// Backgroundworker makes heavy use of the AsyncOperationManager & SynchronisationContext. When the default
//  /// SynchronisationContext is used, it just invokes to callbacks on the current thread, not the UI thread.
//  /// 
//  /// To ensure real world testability of items using this class, we're using a dispatcher (a WPF, but UI Agnostic
//  /// class) to provide the ability to call back into the thread that starts it.
//  /// 
//  /// Dispatchers are a message pump, so block on their RunMethod -- this means we need to spin up our own thread, an
//  /// queue items into that as tests that are synchronously invoked.
//  /// </summary>
//  public class BackgroundWorkerSyncContextHelper
//  {
//    private static Thread m_dispatcherThread;
//    private static Dispatcher m_dispatcher;
//    private static ManualResetEvent m_dispatcherSetup = new ManualResetEvent(false);
//    private static Stack<Exception> s_exceptionQueue = new Stack<Exception>();

//    public static void Init()
//    {
//      // Create the thread, and start it, and wait for it to complete init before
//      // returning and allowing people to invoke tests on the disptacher
//      m_dispatcherThread = new Thread(CreateDispatcher);
//      m_dispatcherThread.Name = "Test Disptacher Thread";
//      m_dispatcherThread.SetApartmentState(ApartmentState.STA);
//      m_dispatcherThread.Start();
//      m_dispatcherSetup.WaitOne();
//    }

//    /// <summary>
//    /// Creates thedispatcher, and hooks up the unhandled exception handler
//    /// Starts executing the dispatcher and blocks.
//    /// </summary>
//    private static void CreateDispatcher()
//    {
//      m_dispatcher = Dispatcher.CurrentDispatcher;
//      m_dispatcher.UnhandledException += new DispatcherUnhandledExceptionEventHandler(m_dispatcher_UnhandledException);
//      AsyncOperationManager.SynchronizationContext = new DispatcherSynchronizationContext(m_dispatcher);

//      m_dispatcherSetup.Set();
//      Dispatcher.Run();
//    }

//    /// <summary>
//    /// Handle all the "unhandled" exceptions on the dispatcher, and allow them to be dequeued later to show
//    /// the assertions/errors on the Test Thread
//    /// </summary>
//    private static void m_dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
//    {
//      // Don't let the dispatcher die, handle the exception always.
//      e.Handled = true;

//      // We likely want the real innerexception, not some top-level TargetInvokation exception. Get the exception
//      // and queue.
//      if (e.Exception.InnerException == null)
//      {
//        s_exceptionQueue.Push(e.Exception);
//      }
//      else
//      {
//        s_exceptionQueue.Push(e.Exception.InnerException);
//      }

//      // We want the exceptions we see to be logged rather than dumped into the ether.
//      Console.WriteLine("Exception found during execution:");
//      Console.WriteLine(s_exceptionQueue.Peek().ToString());
//    }

//    /// <summary>
//    /// clears any exceptions so as not to confuse other tests.
//    /// </summary>
//    public static void ClearException()
//    {
//      s_exceptionQueue.Clear();
//    }

//    /// <summary>
//    /// Dumps all the queued exceptions, and *throws* the last exception that was thrown
//    /// </summary>
//    public static void DumpExceptionsAndThrow()
//    {
//      // Just return if we dont have any exceptions
//      if (s_exceptionQueue.Count < 1)
//      {
//        return;
//      }

//      // Dump the exceptions as strings to the log for people to see.
//      Console.WriteLine("Dumping Exceptions seen while executing:");
//      while (s_exceptionQueue.Count > 1)
//      {
//        Exception e = s_exceptionQueue.Pop();
//        Console.WriteLine(e.ToString());
//      }

//      // Dumps the last exception that was thrown, and then throws it (breaks the callstack... sorry)
//      Exception last = s_exceptionQueue.Pop();
//      Console.WriteLine("Last Exception that was thrown:");
//      Console.WriteLine(last.ToString());

//      throw last;
//    }

//    /// <summary>
//    /// Shuts town the disptacher thread for a clean shutdown.
//    /// </summary>
//    public static void Cleanup()
//    {
//      m_dispatcher.InvokeShutdown();
//    }

//    public static Dispatcher Dispatcher
//    {
//      get { return m_dispatcher; }
//    }

//    public static event ThreadStart TestInitialize;

//    public static event ThreadStart TestCleanup;

//    public static void DoTests(Action test)
//    {
//      BackgroundWorkerSyncContextHelper.ClearException();
//      BackgroundWorkerSyncContextHelper.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new ThreadStart(delegate()
//      {
//        try
//        {
//          if (TestInitialize != null)
//          {
//            TestInitialize();
//          }

//          test();
//        }
//        finally
//        {
//          if (TestCleanup != null)
//          {
//            TestCleanup();
//          }
//        }
//      }));

//      DumpExceptionsAndThrow();
//    }

//    public static void PumpDispatcher()
//    {
//      // This is dispatcher magic. I'm not 100% sure how it works, but this is the karmic equiv. of
//      // Application.DoEvents() in the Winforms world, and allows any items in the dispatchers queue to be processed
//      DispatcherFrame f = new DispatcherFrame();
//      Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new DispatcherOperationCallback((p) =>
//      {
//        ((DispatcherFrame)p).Continue = false;
//        return null;
//      }), f);

//      Dispatcher.PushFrame(f);
//    }
//  }
//}