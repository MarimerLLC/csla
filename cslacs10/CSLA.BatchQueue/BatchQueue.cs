using System;
using System.Messaging;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using CSLA;

namespace CSLA.BatchQueue
{
  /// <summary>
  /// Provides easy access to the batch queue functionality.
  /// </summary>
  /// <remarks>
  /// Client code should create an instance of this class to
  /// interact with a batch queue. A BatchQueue object can
  /// be used to interact with either the default batch queue
  /// server or with a specific batch queue server.
  /// </remarks>
  public class BatchQueue
  {
    string _queueURL;
    Server.BatchQueue _server;

    #region Constructors and Initialization

    /// <summary>
    /// Creates an instance of the object that allows interaction
    /// with the default batch queue server as specified in the
    /// application configuration file.
    /// </summary>
    public BatchQueue()
    {
      _queueURL = ConfigurationSettings.AppSettings["DefaultBatchQueueServer"];
    }

    /// <summary>
    /// Creates an instance of the object that allows interaction
    /// with a specific batch queue server as specified by
    /// the URL passed as a parameter.
    /// </summary>
    /// <param name="QueueServerURL">A URL referencing the batch queue server.</param>
    public BatchQueue(string queueServerURL)
    {
      _queueURL = queueServerURL;
    }

    Server.BatchQueue QueueServer
    {
      get
      {
        if(_server == null)
          _server = (Server.BatchQueue)Activator.GetObject(
            typeof(Server.BatchQueue), _queueURL);
        return _server;
      }
    }

    #endregion

    #region Submitting jobs

    /// <summary>
    /// Submits an entry to the batch queue.
    /// </summary>
    /// <param name="Entry">A reference to your worker object.</param>
    public void Submit(IBatchEntry entry)
    {
      QueueServer.Submit(new Server.BatchEntry(entry));
    }

    /// <summary>
    /// Submits an entry to the batch queue with extra state data.
    /// </summary>
    /// <param name="Entry">A reference to your worker object.</param>
    /// <param name="State">A reference to a serializable object containing state data.</param>
    public void Submit(IBatchEntry entry, object state)
    {
      QueueServer.Submit(new Server.BatchEntry(entry, state));
    }

    /// <summary>
    /// Submits an entry to the batch queue with a specific priority.
    /// </summary>
    /// <param name="Entry">A reference to your worker object.</param>
    /// <param name="Priority">The priority of the entry.</param>
    public void Submit(IBatchEntry entry, MessagePriority priority)
    {
      Server.BatchEntry job = new Server.BatchEntry(entry);
      job.Info.Priority = priority;
      QueueServer.Submit(job);
    }

    /// <summary>
    /// Submits an entry to the batch queue with extra state data and
    /// a specific priority.
    /// </summary>
    /// <param name="Entry">A reference to your worker object.</param>
    /// <param name="State">A reference to a serializable object containing state data.</param>
    /// <param name="Priority">The priority of the entry.</param>
    public void Submit(IBatchEntry entry, object state, MessagePriority priority)
    {
      Server.BatchEntry job = new Server.BatchEntry(entry, state);
      job.Info.Priority = priority;
      QueueServer.Submit(job);
    }

    /// <summary>
    /// Submits an entry to the batch queue to be held until a specific date/time.
    /// </summary>
    /// <param name="Entry">A reference to your worker object.</param>
    /// <param name="HoldUntil">The date/time until which the entry should be held.</param>
    public void Submit(IBatchEntry entry, DateTime holdUntil)
    {
      Server.BatchEntry job = new Server.BatchEntry(entry);
      job.Info.HoldUntil = holdUntil;
      QueueServer.Submit(job);
    }

    /// <summary>
    /// Submits an entry to the batch queue with extra state data
    /// and to be held until a specific date/time.
    /// </summary>
    /// <param name="Entry">A reference to your worker object.</param>
    /// <param name="State">A reference to a serializable object containing state data.</param>
    /// <param name="HoldUntil">The date/time until which the entry should be held.</param>
    public void Submit(IBatchEntry entry, object state, DateTime holdUntil)
    {
      Server.BatchEntry job = new Server.BatchEntry(entry, state);
      job.Info.HoldUntil = holdUntil;
      QueueServer.Submit(job);
    }

    /// <summary>
    /// Submits an entry to the batch queue to be held until 
    /// a specific date/time and at a specific priority.
    /// </summary>
    /// <param name="Entry">A reference to your worker object.</param>
    /// <param name="HoldUntil">The date/time until which the entry should be held.</param>
    /// <param name="Priority">The priority of the entry.</param>
    public void Submit(IBatchEntry entry , DateTime holdUntil, MessagePriority priority)
    {
      Server.BatchEntry job = new Server.BatchEntry(entry);
      job.Info.HoldUntil = holdUntil;
      job.Info.Priority = priority;
      QueueServer.Submit(job);
    }
    /// <summary>
    /// Submits an entry to the batch queue specifying all details.
    /// </summary>
    /// <param name="Entry">A reference to your worker object.</param>
    /// <param name="State">A reference to a serializable object containing state data.</param>
    /// <param name="HoldUntil">The date/time until which the entry should be held.</param>
    /// <param name="Priority">The priority of the entry.</param>
    public void Submit(IBatchEntry entry, 
                       object state, 
                       DateTime holdUntil, 
                       MessagePriority priority)
    {
      Server.BatchEntry job = new Server.BatchEntry(entry, state);
      job.Info.HoldUntil = holdUntil;
      job.Info.Priority = priority;
      QueueServer.Submit(job);
    }

    #endregion

    #region Public methods

    /// <summary>
    /// Removes a holding or pending entry from the
    /// batch queue.
    /// </summary>
    /// <param name="Entry">A reference to the entry to be removed.</param>
    public void Remove(BatchEntryInfo entry)
    {
      QueueServer.Remove(entry);
    }

    /// <summary>
    /// Returns the URL which identifies the batch
    /// queue server to which this object is attached.
    /// </summary>
    public string BatchQueueURL
    {
      get
      {
        return _queueURL;
      }
    }

    /// <summary>
    /// Returns a collection of the batch entries currently
    /// in the batch queue.
    /// </summary>
    public BatchEntries Entries
    {
      get
      {
        return QueueServer.GetEntries(GetPrincipal());
      }
    }

    #endregion

    #region System.Object overrides

    public override string ToString()
    {
      return _queueURL;
    }

    public bool Equals(BatchQueue queue)
    {
      return _queueURL == queue.BatchQueueURL;
    }

    public override int GetHashCode()
    {
      return _queueURL.GetHashCode();
    }

    #endregion

    #region Security

    string AUTHENTICATION
    {
      get
      {
        return ConfigurationSettings.AppSettings["Authentication"];
      }
    }

    System.Security.Principal.IPrincipal GetPrincipal()
    {
      if(AUTHENTICATION == "Windows")
      {
        // Windows integrated security 
        return null;
      }
      else
      {
        // we assume using the CSLA framework security
        return System.Threading.Thread.CurrentPrincipal;
      }
    }

    #endregion


  }
}
