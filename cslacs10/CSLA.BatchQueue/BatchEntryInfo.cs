using System;
using System.Messaging;

namespace CSLA.BatchQueue
{
  /// <summary>
  /// Values indicating the status of a batch queue entry.
  /// </summary>
  public enum BatchEntryStatus
  {
    Pending,
    Holding,
    Active
  }

  /// <summary>
  /// Contains information about a batch queue entry.
  /// </summary>
  /// <remarks>
  /// This object contains information about batch entry including
  /// when it was submitted, the user that submitted the job and
  /// which machine the user was using at the time. It also contains
  /// the job's priority, status and the optional date/time until
  /// which the job should be held until it is run.
  /// </remarks>
  [Serializable()]
  public class BatchEntryInfo
  {
    Guid _id = Guid.NewGuid();
    DateTime _submitted = DateTime.Now;
    string _user = System.Environment.UserName;
    string _machine = System.Environment.MachineName;
    MessagePriority _priority = MessagePriority.Normal;
    string _msgID;
    DateTime _holdUntil = DateTime.MinValue;
    BatchEntryStatus _status = BatchEntryStatus.Pending;

    /// <summary>
    /// Returns the unique ID value for this batch entry.
    /// </summary>
    public Guid ID
    {
      get
      {
        return _id;
      }
    }

    /// <summary>
    /// Returns the date and time that the batch entry
    /// was submitted.
    /// </summary>
    public DateTime Submitted
    {
      get
      {
        return _submitted;
      }
    }

    /// <summary>
    /// Returns the Windows user id of the user that
    /// was logged into the workstation when the job
    /// was submitted.
    /// </summary>
    public string User
    {
      get
      {
        return _user;
      }
    }

    /// <summary>
    /// Returns the name of the workstation from
    /// which this job was submitted.
    /// </summary>
    public string Machine
    {
      get
      {
        return _machine;
      }
    }

    /// <summary>
    /// Returns the priority of this batch entry.
    /// </summary>
    /// <remarks>
    /// The priority values flow from System.Messaging and
    /// the priority is used by MSMQ to order the entries
    /// in the queue.
    /// </remarks>
    public MessagePriority Priority
    {
      get
      {
        return _priority;
      }
      set
      {
        _priority = value;
      }
    }

    /// <summary>
    /// Returns the MSMQ message ID of the batch entry.
    /// </summary>
    /// <remarks>
    /// This value is only valid after the batch entry
    /// has been submitted to the queue.
    /// </remarks>
    public string MessageID
    {
      get
      {
        return _msgID;
      }
    }

    internal void SetMessageID(string id)
    {
      _msgID = id;
    }

    /// <summary>
    /// Returns the date and time until which the
    /// batch entry will be held before it can be run.
    /// </summary>
    /// <remarks>
    /// This value is optional. If it was provided, the batch
    /// entry will be held until this date/time. At this date/time,
    /// the entry will switch from Holding status to Pending
    /// status and will be queued based on its priority along
    /// with all other Pending entries.
    /// </remarks>
    public DateTime HoldUntil
    {
      get
      {
        return _holdUntil;
      }
      set
      {
        _holdUntil = value;
      }
    }

    /// <summary>
    /// Returns the status of the batch entry.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the job is Holding, it means that the job
    /// won't run until the data/time specified by
    /// <see cref="P:CSLA.BatchQueue.BatchEntryInfo.HoldUntil" />.
    /// </para><para>
    /// If the job is Pending, it means that the job
    /// will run as soon as possible, but that the queue
    /// is busy. Pending entries are run in priority order based
    /// on <see cref="P:CSLA.BatchQueue.BatchEntryInfo.Priority" />.
    /// </para><para>
    /// If the job is Active, it means that the job is
    /// currently being executed on the server.
    /// </para>
    /// </remarks>
    public BatchEntryStatus Status
    {
      get
      {
        if(_holdUntil > DateTime.Now && _status == BatchEntryStatus.Pending)
          return BatchEntryStatus.Holding;
        else
          return _status;
      }
    }

    internal void SetStatus(BatchEntryStatus status)
    {
      _status = status;
    }

    #region System.Object overrides

    public override string ToString()
    {
      return _user + "@" + _machine + ":" + _id.ToString();
    }

    public bool Equals(BatchEntryInfo info)
    {
      return _id.Equals(info.ID);
    }

    public override int GetHashCode()
    {
      return _id.GetHashCode();
    }

    #endregion
  }
}
