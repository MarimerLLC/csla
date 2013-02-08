using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.DiffGram
{
  /// <summary>
  /// Defines the interface that must be implemented
  /// by all business objects in an object graph that
  /// is the target of diffgram import/export.
  /// </summary>
  public interface IExportData
  {
    /// <summary>
    /// Returns a list of all child objects
    /// referenced by this business object.
    /// </summary>
    List<IExportData> GetChildren();
    /// <summary>
    /// Gets a value indicating if the business
    /// object is participating in the export/import
    /// process (does it want to be part of the diffgram).
    /// </summary>
    bool IsParticipating { get; }
    /// <summary>
    /// Gets or sets an abitrary key value
    /// to link the business object to a DataItem.
    /// FOR USE BY DIFFGRAM ONLY.
    /// </summary>
    int Key { get; set; }
    /// <summary>
    /// Invoked when the object should export
    /// some or all of its state into the
    /// provided DataItem object.
    /// </summary>
    /// <param name="dto">
    /// The diffgram object that will contain
    /// this business object's changed state.
    /// </param>
    void ExportTo(DataItem dto);
    /// <summary>
    /// Invoked when the object should import
    /// some or all of its state from the
    /// provided DataItem object.
    /// </summary>
    /// <param name="dto">
    /// The diffgram object that contains this
    /// business object's changed state.
    /// </param>
    void ImportFrom(DataItem dto);
  }
}
