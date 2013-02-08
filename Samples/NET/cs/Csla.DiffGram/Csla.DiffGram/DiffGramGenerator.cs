using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace Csla.DiffGram
{
  /// <summary>
  /// Generates and integrates diffgrams against
  /// an associated business object graph.
  /// </summary>
  public class DiffGramGenerator
  {
    private int _lastKey;

    /// <summary>
    /// Generates a diffgram object graph corresponding
    /// to the provided business object graph.
    /// </summary>
    /// <param name="root">Root of the business object graph
    /// to be exported into the diffgram.</param>
    /// <returns>
    /// A DataItem representing the root of the diffgram object graph.
    /// </returns>
    public DataItem GenerateGraph(IExportData root)
    {
      if (root != null && root.IsParticipating)
      {
        _lastKey++;
        root.Key = _lastKey;

        DataItem result = new DataItem { Key = root.Key };
        var track = root as Csla.Core.ITrackStatus;
        if (track != null)
        {
          result.IsNew = track.IsNew;
          result.IsDeleted = track.IsDeleted;
        }
        root.ExportTo(result);
        foreach (var item in root.GetChildren())
        {
          var ci = GenerateGraph(item);
          if (ci != null)
            result.Children.Add(ci);
        }
        return result;
      }
      else
        return null;
    }

    /// <summary>
    /// Integrates a diffgram object graph into an
    /// existing business object graph.
    /// </summary>
    /// <param name="root">Root of the business object graph.</param>
    /// <param name="dtoGraph">Root of the diffgram object
    /// graph containing the data used to update the business
    /// object graph.</param>
    public void IntegrateGraph(IExportData root, DataItem dtoGraph)
    {
      if (root != null && root.IsParticipating)
      {
        if (root.Key == dtoGraph.Key)
        {
          foreach (var child in root.GetChildren().Where(c => c.IsParticipating))
          {
            var dto = (from d in dtoGraph.Children
                       where d.Key == child.Key
                       select d).SingleOrDefault();
            if (dto != null)
              IntegrateGraph(child, dto);
            else
              throw new Exception("Child key not in DTO graph");
          }
          root.ImportFrom(dtoGraph);
        }
        else
          throw new Exception("Root object keys don't match");
      }
    }
  }
}
