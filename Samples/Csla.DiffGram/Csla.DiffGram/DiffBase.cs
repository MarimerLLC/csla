using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace Csla.DiffGram
{
  /// <summary>
  /// Custom BusinessBase base class that adds
  /// support for the DiffGramGenerator.
  /// </summary>
  /// <typeparam name="T">Type of business object</typeparam>
  [Serializable]
  public abstract class DiffBase<T> : BusinessBase<T>,
    IExportData
    where T : DiffBase<T>
  {
    List<Csla.DiffGram.IExportData> Csla.DiffGram.IExportData.GetChildren()
    {
      var result = new List<IExportData>();
      foreach (var item in FieldManager.GetChildren())
        result.Add((IExportData)item);
      return result;
    }

    bool Csla.DiffGram.IExportData.IsParticipating
    {
      get { return IsParticipating; }
    }

    /// <summary>
    /// Gets a value indicating whether this object
    /// instance wants to participate in the diffgram
    /// export/import process.
    /// </summary>
    protected virtual bool IsParticipating
    {
      get { return IsDirty; }
    }

    int Csla.DiffGram.IExportData.Key { get; set; }

    void Csla.DiffGram.IExportData.ExportTo(Csla.DiffGram.DataItem dto)
    {
      ExportTo(dto);
    }

    /// <summary>
    /// Invoked when the business object should export
    /// its data into the supplied DataItem object so
    /// the data is included in the diffgram.
    /// </summary>
    /// <param name="dto">
    /// DataItem object used to store the business object's
    /// state data.
    /// </param>
    protected virtual void ExportTo(DataItem dto)
    { }

    void Csla.DiffGram.IExportData.ImportFrom(Csla.DiffGram.DataItem dto)
    {
      ImportFrom(dto);
      if (IsDeleted)
        MarkNew();
      else
        MarkOld();
    }

    /// <summary>
    /// Invoked when the business object should import
    /// data from the supplied DataItem object as the
    /// diffgram is reintegrated.
    /// </summary>
    /// <param name="dto">
    /// DataItem object containing the business object's
    /// state data.
    /// </param>
    protected virtual void ImportFrom(DataItem dto)
    { }
  }
}
