using System;
using Csla.Core;
using System.ComponentModel;
using Csla.Serialization.Mobile;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace Csla
{
  /// <summary>
  /// Defines an object that holds a list of deleted items.
  /// </summary>
  public interface IContainsDeletedList
  {
    /// <summary>
    /// List of deleted child objects
    /// </summary>
    IEnumerable<IEditableBusinessObject> DeletedList { get; }
  }
}
