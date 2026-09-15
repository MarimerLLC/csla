//-----------------------------------------------------------------------
// <copyright file="LocationInfo.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Equatable source location for operation models and diagnostics</summary>
//-----------------------------------------------------------------------

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Csla.Generator.AutoImplementProperties.CSharp.DataPortalOperations.Models
{
  /// <summary>
  /// An equatable representation of a source location, safe to keep in
  /// incremental generator models (unlike <see cref="Location"/>).
  /// </summary>
  internal sealed record LocationInfo(string FilePath, TextSpan TextSpan, LinePositionSpan LineSpan)
  {
    /// <summary>
    /// The location in the compilation's syntax tree for the file, so the
    /// diagnostic can be suppressed in source; otherwise a file location.
    /// </summary>
    public Location ToLocation(Compilation compilation)
    {
      foreach (var tree in compilation.SyntaxTrees)
      {
        if (tree.FilePath == FilePath)
          return Location.Create(tree, TextSpan);
      }
      return Location.Create(FilePath, TextSpan, LineSpan);
    }

    public static LocationInfo? From(Location? location)
    {
      if (location?.SourceTree is null)
        return null;
      return new LocationInfo(location.SourceTree.FilePath, location.SourceSpan, location.GetLineSpan().Span);
    }
  }
}
