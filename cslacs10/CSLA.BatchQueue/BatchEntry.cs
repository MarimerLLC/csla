using System;
using System.Security.Principal;
using System.Configuration;
using System.Diagnostics;

namespace CSLA.BatchQueue.Server
{
  /// <summary>
  /// A batch queue entry.
  /// </summary>
  /// <remarks>
  /// Each batch queue entry consists of basic information about
  /// the entry, a Principal object (if you are using CSLA .NET
  /// security), the actual worker object containing the code
  /// to be run and an optional state object containing arbitrary
  /// state data.
  /// </remarks>
  [Serializable()]
  public sealed class BatchEntry
  {
    BatchEntryInfo _info = new BatchEntryInfo();
    IPrincipal _principal;
    IBatchEntry _worker;
    object _state;

    /// <summary>
    /// Returns a reference to the object containing information
    /// about this batch entry.
    /// </summary>
    public BatchEntryInfo Info
    {
      get
      {
        return _info;
      }
    }

    /// <summary>
    /// Returns a reference to the 
    /// <see cref="T:CSLA.Security.BusinessPrincipal" />
    /// object for the user that submitted this entry.
    /// </summary>
    public IPrincipal Principal
    {
      get
      {
        return _principal;
      }
    }

    /// <summary>
    /// Returns a reference to the worker object that
    /// contains the code which is to be executed as
    /// a batch process.
    /// </summary>
    public IBatchEntry Entry
    {
      get
      {
        return _worker;
      }
      set
      {
        _worker = value;
      }
    }

    /// <summary>
    /// Returns a reference to the optional state object.
    /// </summary>
    /// <returns></returns>
    public object State
    {
      get
      {
        return _state;
      }
      set
      {
        _state = value;
      }
    }

    #region Batch execution

    // this will run in a background thread in the
    // thread pool
    internal void Execute(object state)
    {
      IPrincipal oldPrincipal = System.Threading.Thread.CurrentPrincipal;;

      try
      {
        // set this thread's principal to our user
        SetPrincipal(_principal);

        try
        {
          // now run the user's code
          _worker.Execute(_state);

          System.Text.StringBuilder sb = new System.Text.StringBuilder();
          sb.AppendFormat("Batch job completed\n");
          sb.AppendFormat("Batch job: {0}\n", this.ToString());
          sb.AppendFormat("Job object: {0}\n", (object)_worker.ToString());

          System.Diagnostics.EventLog.WriteEntry(
            BatchQueueService.Name, sb.ToString(), EventLogEntryType.Information);
        }
        catch(Exception ex)
        {
          System.Text.StringBuilder sb = new System.Text.StringBuilder();
          sb.AppendFormat("Batch job failed due to execution error\n");
          sb.AppendFormat("Batch job: {0}\n", this.ToString());
          sb.AppendFormat("Job object: {0}\n", (object)_worker.ToString());
          sb.Append(ex.ToString());

          System.Diagnostics.EventLog.WriteEntry(
            BatchQueueService.Name, sb.ToString(), EventLogEntryType.Warning);
        }
      }
      finally
      {
        BatchQueueService.Deactivate(this);
        // reset the thread's principal object
        System.Threading.Thread.CurrentPrincipal = oldPrincipal;
      }
    }

    #endregion

    #region System.Object overrides

    public override string ToString()
    {
      return _info.ToString();
    }

    public bool Equals(BatchEntry entry)
    {
      return _info.Equals(entry.Info);
    }

    public override int GetHashCode()
    {
      return _info.GetHashCode();
    }

    #endregion

    #region Constructors

    internal BatchEntry(IBatchEntry entry)
    {
      _principal = GetPrincipal();
      _worker = entry;
    }

    internal BatchEntry(IBatchEntry entry, object state)
    {
      _principal = GetPrincipal();
      _worker = entry;
      _state = state;
    }

    #endregion

    #region Security

    string AUTHENTICATION()
    {
      string val = ConfigurationSettings.AppSettings["Authentication"];
      if(val == null)
        return string.Empty;
      else
        return val;
    }

    IPrincipal GetPrincipal() 
    {
      if(AUTHENTICATION() == "Windows")
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

    void SetPrincipal(object principal)
    {
      if(AUTHENTICATION() == "Windows")
      {
        // when using integrated security, Principal must be Nothing
        // and we need to set our policy to use the Windows principal
        if(principal == null)
        {
          AppDomain.CurrentDomain.SetPrincipalPolicy(PrincipalPolicy.WindowsPrincipal);
          return;
        }
        else
        {
          throw new System.Security.SecurityException("No principal object should be passed to DataPortal when using Windows integrated security");
        }
      }

      // we expect Principal to be of type BusinessPrincipal, but
      // we can't enforce that since it causes a circular reference
      // with the business library so instead we must use type Object
      // for the parameter, so here we do a check on the type of the
      // parameter
      if(principal.ToString() == "CSLA.Security.BusinessPrincipal")
      {
        // see if our current principal is
        // different from the caller's principal
        if(!ReferenceEquals(principal, System.Threading.Thread.CurrentPrincipal))
        {
          // the caller had a different principal, so change ours to
          // match the caller's so all our objects use the caller's
          // security
          System.Threading.Thread.CurrentPrincipal = (IPrincipal)principal;
        }
      }
      else
      {
        throw new System.Security.SecurityException("Principal must be of type BusinessPrincipal, not " + Principal.ToString());
      }
    }

    #endregion
  }
}
