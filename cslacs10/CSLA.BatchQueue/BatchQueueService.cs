using System;
using System.Collections;
using System.Threading;
using System.Messaging;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using CSLA;

namespace CSLA.BatchQueue.Server
{
  /// <summary>
  /// Implements the batch queue service.
  /// </summary>
  /// <remarks>
  /// This class implements the server-side batch queue functionality.
  /// It must be hosted within some process, typically a Windows Service.
  /// It may also be hosted within a console application, which is
  /// useful for testing and debugging.
  /// </remarks>
  public class BatchQueueService
  {
    static TcpServerChannel _channel;
    static MessageQueue _queue;
    static Thread _monitor;
    static System.Timers.Timer _timer;
    static volatile bool _running;
    static Hashtable _activeEntries = Hashtable.Synchronized(new Hashtable());

    static AutoResetEvent _sync = new AutoResetEvent(false);
    static ManualResetEvent _waitToEnd = new ManualResetEvent(false);

    static BatchQueueService()
    {
      _timer = new System.Timers.Timer();
      _timer.Elapsed += new System.Timers.ElapsedEventHandler(Timer_Elapsed);
    }

    /// <summary>
    /// Returns the name of the batch queue server.
    /// </summary>
    public static string Name
    {
      get
      {
        return LISTENER_NAME;
      }
    }

    #region Start/Stop

    /// <summary>
    /// Starts the batch queue.
    /// </summary>
    /// <remarks>
    /// Call this as your Windows Service starts up to
    /// start the batch queue itself. This will cause
    /// the queue to start listening for user requests
    /// via remoting and to the MSMQ for queued jobs.
    /// </remarks>
    public static void Start()
    {
      _timer.AutoReset = false;

      // open and/or create queue
      if(MessageQueue.Exists(QUEUE_NAME))
        _queue = new MessageQueue(QUEUE_NAME);
      else
        _queue = MessageQueue.Create(QUEUE_NAME);
      _queue.MessageReadPropertyFilter.Extension = true;

      // start reading from queue
      _running = true;
      _monitor = new Thread(new ThreadStart(MonitorQueue));
      _monitor.Name = "MonitorQueue";
      _monitor.Start();

      // start remoting for Server.BatchQueue
      if(_channel == null)
      {
        // set application name (virtual root name)
        RemotingConfiguration.ApplicationName = LISTENER_NAME;

        // set up channel
        Hashtable properties = new Hashtable();
        properties["name"] = "TcpBinary";
        properties["port"] = PORT.ToString();
        BinaryServerFormatterSinkProvider svFormatter = 
          new BinaryServerFormatterSinkProvider();

        //TODO: uncomment the following line for .NET 1.1
        //svFormatter.TypeFilterLevel = 
        //  System.Runtime.Serialization.Formatters.TypeFilterLevel.Full;

        _channel = new TcpServerChannel(properties, svFormatter);
        ChannelServices.RegisterChannel(_channel);

        // register our class
        RemotingConfiguration.RegisterWellKnownServiceType( 
          typeof(Server.BatchQueue), "BatchQueue.rem", 
          WellKnownObjectMode.SingleCall);
      }
      else
        _channel.StartListening(null);

      System.Text.StringBuilder sb = new System.Text.StringBuilder();

      sb.AppendFormat("Batch queue processor started\n");
      sb.AppendFormat("Name: {0}\n", Name);
      sb.AppendFormat("Port: {0}\n", PORT);
      sb.AppendFormat("Queue: {0}\n", QUEUE_NAME);
      sb.AppendFormat("Max jobs: {0}\n", MAX_ENTRIES);
      System.Diagnostics.EventLog.WriteEntry(Name, sb.ToString(), 
        System.Diagnostics.EventLogEntryType.Information);
    }

    /// <summary>
    /// Stops the batch queue.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Call this as your Windows Service is stopping. It
    /// stops the batch queue, causing it to stop listening
    /// for user requests via remoting and to stop looking for
    /// jobs in the MSMQ queue.
    /// </para><para>
    /// NOTE that this method will not return until any
    /// currently active (executing) batch jobs have completed.
    /// </para>
    /// </remarks>
    public static void Stop()
    {
      // stop remoting for Server.BatchQueue
      _channel.StopListening(null);

      // signal to stop working 
      _running = false;
      _sync.Set();
      _monitor.Join();
      // close the queue
      _queue.Close();

      if(_activeEntries.Count > 0)
      {
        // wait for work to end
        _waitToEnd.WaitOne();
      }
    }

    #endregion

    #region Process messages

    // this will be running on a background thread
    static void MonitorQueue()
    {
      while(_running)
      {
        ScanQueue();
        _sync.WaitOne();
      }
    }

    // this runs on a threadpool thread
    static void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
      _timer.Stop();
      _sync.Set();
    }

    // this is called by MonitorQueue
    static void ScanQueue()
    {
      Message msg;
      DateTime holdUntil;
      DateTime nextWake = DateTime.MaxValue;

      MessageEnumerator en = _queue.GetMessageEnumerator();
      while(en.MoveNext())
      {
        msg = en.Current;
        holdUntil = 
          DateTime.Parse(System.Text.Encoding.ASCII.GetString(msg.Extension));
        if(holdUntil <= DateTime.Now)
        {
          if(_activeEntries.Count < MAX_ENTRIES)
          {
            ProcessEntry(_queue.ReceiveById(msg.Id));
          }
          else
          {
            // the queue is busy, go to sleep
            return;
          }
        }
        else
        {
          if(holdUntil < nextWake)
          {
            // find the minimum holduntil value
            nextWake = holdUntil;
          }
        }
      }

      if(nextWake < DateTime.MaxValue && nextWake > DateTime.Now)
      {
        // we have at least one entry holding, so set the
        // timer to wake us when it should be run
        _timer.Interval = nextWake.Subtract(DateTime.Now).TotalMilliseconds;
        _timer.Start();
      }
    }

    static void ProcessEntry(Message msg)
    {
      // get entry from queue
      BatchEntry entry;
      BinaryFormatter formatter = new BinaryFormatter();
      entry = (BatchEntry)formatter.Deserialize(msg.BodyStream);

      // make active
      entry.Info.SetStatus(BatchEntryStatus.Active);
      _activeEntries.Add(entry.Info.ID, entry.Info);

      // start processing entry on background thread
      ThreadPool.QueueUserWorkItem(new WaitCallback(entry.Execute));
    }

    // called by BatchEntry when it is done processing so
    // we know that it is complete and we can start another
    // job if needed
    internal static void Deactivate(BatchEntry entry)
    {
      _activeEntries.Remove(entry.Info.ID);
      _sync.Set();
      if(!_running && _activeEntries.Count == 0)
      {
        // indicate that there are no active workers
        _waitToEnd.Set();
      }
    }

    #endregion

    #region Enqueue/Dequeue/LoadEntries

    internal static void Enqueue(BatchEntry entry)
    {
      Message msg = new Message();
      BinaryFormatter f = new BinaryFormatter();

      msg.Label = entry.ToString();
      msg.Priority = entry.Info.Priority;
      msg.Extension = 
        System.Text.Encoding.ASCII.GetBytes(
        entry.Info.HoldUntil.ToString());
      entry.Info.SetMessageID(msg.Id);
      f.Serialize(msg.BodyStream, entry);

        _queue.Send(msg);

      _sync.Set();
    }

    internal static void Dequeue(BatchEntryInfo entry)
    {
      string msgID;

      string label = entry.ToString();
      MessageEnumerator en = _queue.GetMessageEnumerator();
      _queue.MessageReadPropertyFilter.Label = true;

      while(en.MoveNext())
      {
        if(en.Current.Label == label)
        {
          // we found a match
          msgID = en.Current.Id;
        }
      }

      if(msgID != null)
        _queue.ReceiveById(msgID);
    }

    internal static void LoadEntries(BatchEntries list)
    {
      // load our list of BatchEntry objects
      BinaryFormatter formatter = new BinaryFormatter();
      Server.BatchEntry entry;

      // get all active entries
      lock(_activeEntries.SyncRoot)
      {
        foreach(DictionaryEntry de in _activeEntries)
        {
          list.Add((BatchEntryInfo)de.Value);
        }
      }

      // get all queued entries
      Message [] msgs = _queue.GetAllMessages();
      foreach(Message msg in msgs)
      {
        entry = (Server.BatchEntry)formatter.Deserialize(msg.BodyStream);
        entry.Info.SetMessageID(msg.Id);
        list.Add(entry.Info);
      }
    }

    #endregion

    #region Utility functions

    static string QUEUE_NAME
    {
      get
      {
        return @".\private$\" + ConfigurationSettings.AppSettings["QueueName"];
      }
    }

    static string LISTENER_NAME
    {
      get
      {
        return ConfigurationSettings.AppSettings["ListenerName"];
      }
    }

    static int PORT
    {
      get
      {
        return Convert.ToInt32(ConfigurationSettings.AppSettings["ListenerPort"]);
      }
    }

    static int MAX_ENTRIES
    {
      get
      {
        return Convert.ToInt32(
          ConfigurationSettings.AppSettings["MaxActiveEntries"]);
      }
    }

    #endregion


  }
}
