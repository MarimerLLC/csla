using System;

namespace CSLA.BatchQueue
{
  /// <summary>
  /// A helper object used to execute a specified class
  /// from a specified DLL on the server.
  /// </summary>
  /// <remarks>
  /// <para>
  /// A worker object can be provided directly by the client
  /// workstation. In that case, the worker object is passed
  /// by value to the server where it is executed. The drawback
  /// to such an approach is that the worker assembly must be
  /// installed on both client and server.
  /// </para><para>
  /// BatchJobRequest is a worker object that specifies the
  /// type and assembly name of a class on the server. When
  /// this job is run, it dynamically creates an instance of
  /// the specified class and executes it on the server. This
  /// means that the actual worker assembly needs to be installed
  /// only on the server, not on the client.
  /// </para>
  /// </remarks>
  [Serializable()]
  public class BatchJobRequest : IBatchEntry
  {
    string _assembly = string.Empty;
    string _type = string.Empty;

    /// <summary>
    /// Creates a new object, specifying the type and assembly
    /// of the actual worker object.
    /// </summary>
    /// <param name="Type">The class name of the actual worker object.</param>
    /// <param name="Assembly">The name of the assembly containing the actual worker class.</param>
    public BatchJobRequest(string type, string assembly)
    {
      _assembly = assembly;
      _type = type;
    }

    /// <summary>
    /// The class name of the worker object.
    /// </summary>
    public string Type
    {
      get
      {
        return _type;
      }
      set
      {
        _type = value;
      }
    }

    /// <summary>
    /// The name of the assembly containing the actual worker class.
    /// </summary>
    public string Assembly
    {
      get
      {
        return _assembly;
      }
      set
      {
        _assembly = value;
      }
    }

    /// <summary>
    /// Executes the batch job on the server.
    /// </summary>
    /// <remarks>
    /// This method runs on the server - it is called
    /// by <see cref="T:CSLA.BatchQueue.Server.BatchEntry" />,
    /// which is called by 
    /// <see cref="T:CSLA.BatchQueue.Server.BatchQueueService" />.
    /// </remarks>
    /// <param name="State"></param>
    void IBatchEntry.Execute(object state)
    {
      // create an instance of the specified object
      IBatchEntry job = 
        (IBatchEntry)AppDomain.CurrentDomain.CreateInstanceAndUnwrap(
        _assembly, _type);

      // execute the job
      job.Execute(state);
    }

    #region System.Object overrides

    public override string ToString()
    {
      return "BatchJobRequest: " + _type + "," + _assembly;
    }

    #endregion
  }
}
