using System;
using System.Collections;
using System.Messaging;
using System.Runtime.Serialization.Formatters.Binary;

namespace CSLA.BatchQueue
{
  /// <summary>
  /// Contains a list of holding, pending and active batch
  /// queue entries.
  /// </summary>
  [Serializable()]
  public class BatchEntries : CollectionBase
  {
    /// <summary>
    /// Returns a reference to an object with information about
    /// a specific batch queue entry.
    /// </summary>
    public BatchEntryInfo this [int index]
    {
      get
      {
        return (BatchEntryInfo)List[index];
      }
    }

    /// <summary>
    /// Returns a reference to an object with information about
    /// a specific batch queue entry.
    /// </summary>
    /// <param name="ID">The ID value of the entry to return.</param>
    public BatchEntryInfo this [Guid id]
    {
      get
      {
        foreach(BatchEntryInfo obj in List)
          if(obj.ID.Equals(id))
            return obj;
        return null;
      }
    }

    internal BatchEntries()
    {
      // prevent direct creation
    }

    internal void Add(BatchEntryInfo value)
    {
      List.Add(value);
    }
  }
}
